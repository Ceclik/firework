using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.XR;

namespace UnityEngine.Rendering.PostProcessing
{
#if UNITY_2017_2_OR_NEWER
    using XRSettings = XRSettings;

#elif UNITY_5_6_OR_NEWER
    using XRSettings = UnityEngine.VR.VRSettings;
#endif

    // TODO: XMLDoc everything (?)
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [ImageEffectAllowedInSceneView]
    [AddComponentMenu("Rendering/Post-process Layer", 1000)]
    [RequireComponent(typeof(Camera))]
    public sealed class PostProcessLayer : MonoBehaviour
    {
        public enum Antialiasing
        {
            None,
            FastApproximateAntialiasing,
            SubpixelMorphologicalAntialiasing,
            TemporalAntialiasing
        }

        // Settings
        public Transform volumeTrigger;
        public LayerMask volumeLayer;
        public bool stopNaNPropagation = true;

        // Builtins / hardcoded effects that don't benefit from volume blending
        public Antialiasing antialiasingMode = Antialiasing.None;
        public TemporalAntialiasing temporalAntialiasing;
        public SubpixelMorphologicalAntialiasing subpixelMorphologicalAntialiasing;
        public FastApproximateAntialiasing fastApproximateAntialiasing;
        public Fog fog;
        public Dithering dithering;

        public PostProcessDebugLayer debugLayer;

        [SerializeField] private PostProcessResources m_Resources;

        // UI states
        [SerializeField] private bool m_ShowToolkit;
        [SerializeField] private bool m_ShowCustomSorter;

        // Will stop applying post-processing effects just before color grading is applied
        // Currently used to export to exr without color grading
        public bool breakBeforeColorGrading;

        [SerializeField] private List<SerializedBundleRef> m_BeforeTransparentBundles;

        [SerializeField] private List<SerializedBundleRef> m_BeforeStackBundles;

        [SerializeField] private List<SerializedBundleRef> m_AfterStackBundles;

        // Recycled list - used to reduce GC stress when gathering active effects in a bundle list
        // on each frame
        private readonly List<PostProcessEffectRenderer> m_ActiveEffects = new();
        private readonly List<RenderTargetIdentifier> m_Targets = new();

        // Settings/Renderer bundles mapped to settings types
        private Dictionary<Type, PostProcessBundle> m_Bundles;
        private Camera m_Camera;
        private PostProcessRenderContext m_CurrentContext;
        private bool m_IsRenderingInSceneView;
        private CommandBuffer m_LegacyCmdBuffer;
        private CommandBuffer m_LegacyCmdBufferBeforeLighting;
        private CommandBuffer m_LegacyCmdBufferBeforeReflections;
        private CommandBuffer m_LegacyCmdBufferOpaque;
        private LogHistogram m_LogHistogram;

        private bool m_NaNKilled;

        private PropertySheetFactory m_PropertySheetFactory;

        private bool m_SettingsUpdateNeeded = true;

        private TargetPool m_TargetPool;

        public Dictionary<PostProcessEvent, List<SerializedBundleRef>> sortedBundles { get; private set; }

        // We need to keep track of bundle initialization because for some obscure reason, on
        // assembly reload a MonoBehavior's Editor OnEnable will be called BEFORE the MonoBehavior's
        // own OnEnable... So we'll use it to pre-init bundles if the layer inspector is opened and
        // the component hasn't been enabled yet.
        public bool haveBundlesBeenInited { get; private set; }

        // Called everytime the user resets the component from the inspector and more importantly
        // the first time it's added to a GameObject. As we don't have added/removed event for
        // components, this will do fine
        private void Reset()
        {
            volumeTrigger = transform;
        }

        private void OnEnable()
        {
            Init(null);

            if (!haveBundlesBeenInited)
                InitBundles();

            m_LogHistogram = new LogHistogram();
            m_PropertySheetFactory = new PropertySheetFactory();
            m_TargetPool = new TargetPool();

            debugLayer.OnEnable();

            if (RuntimeUtilities.scriptableRenderPipelineActive)
                return;

            InitLegacy();
        }

        private void OnDisable()
        {
            if (!RuntimeUtilities.scriptableRenderPipelineActive)
            {
                m_Camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, m_LegacyCmdBufferBeforeReflections);
                m_Camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, m_LegacyCmdBufferBeforeLighting);
                m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_LegacyCmdBufferOpaque);
                m_Camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, m_LegacyCmdBuffer);
            }

            temporalAntialiasing.Release();
            m_LogHistogram.Release();

            foreach (var bundle in m_Bundles.Values)
                bundle.Release();

            m_Bundles.Clear();
            m_PropertySheetFactory.Release();

            if (debugLayer != null)
                debugLayer.OnDisable();

            // Might be an issue if several layers are blending in the same frame...
            TextureLerper.instance.Clear();

            haveBundlesBeenInited = false;
        }

        private void OnPostRender()
        {
            // Unused in scriptable render pipelines
            if (RuntimeUtilities.scriptableRenderPipelineActive)
                return;

            if (m_CurrentContext.IsTemporalAntialiasingActive())
            {
                m_Camera.ResetProjectionMatrix();

                if (m_CurrentContext.stereoActive)
                    if (RuntimeUtilities.isSinglePassStereoEnabled ||
                        m_Camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
                        m_Camera.ResetStereoProjectionMatrices();
            }
        }

        private void OnPreCull()
        {
            // Unused in scriptable render pipelines
            if (RuntimeUtilities.scriptableRenderPipelineActive)
                return;

            if (m_Camera == null || m_CurrentContext == null)
                InitLegacy();

            // Resets the projection matrix from previous frame in case TAA was enabled.
            // We also need to force reset the non-jittered projection matrix here as it's not done
            // when ResetProjectionMatrix() is called and will break transparent rendering if TAA
            // is switched off and the FOV or any other camera property changes.
            m_Camera.ResetProjectionMatrix();
            m_Camera.nonJitteredProjectionMatrix = m_Camera.projectionMatrix;

            if (m_Camera.stereoEnabled)
            {
                m_Camera.ResetStereoProjectionMatrices();
                Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, XRSettings.renderViewportScale);
            }
            else
            {
                Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1.0f);
            }

            BuildCommandBuffers();
        }

        private void OnPreRender()
        {
            // Unused in scriptable render pipelines
            // Only needed for multi-pass stereo right eye
            if (RuntimeUtilities.scriptableRenderPipelineActive ||
                m_Camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
                return;

            BuildCommandBuffers();
        }

        private void InitLegacy()
        {
            m_LegacyCmdBufferBeforeReflections = new CommandBuffer { name = "Deferred Ambient Occlusion" };
            m_LegacyCmdBufferBeforeLighting = new CommandBuffer { name = "Deferred Ambient Occlusion" };
            m_LegacyCmdBufferOpaque = new CommandBuffer { name = "Opaque Only Post-processing" };
            m_LegacyCmdBuffer = new CommandBuffer { name = "Post-processing" };

            m_Camera = GetComponent<Camera>();
            m_Camera.forceIntoRenderTexture = true; // Needed when running Forward / LDR / No MSAA
            m_Camera.AddCommandBuffer(CameraEvent.BeforeReflections, m_LegacyCmdBufferBeforeReflections);
            m_Camera.AddCommandBuffer(CameraEvent.BeforeLighting, m_LegacyCmdBufferBeforeLighting);
            m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_LegacyCmdBufferOpaque);
            m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffects, m_LegacyCmdBuffer);

            // Internal context used if no SRP is set
            m_CurrentContext = new PostProcessRenderContext();
        }

        public void Init(PostProcessResources resources)
        {
            if (resources != null) m_Resources = resources;

            RuntimeUtilities.CreateIfNull(ref temporalAntialiasing);
            RuntimeUtilities.CreateIfNull(ref subpixelMorphologicalAntialiasing);
            RuntimeUtilities.CreateIfNull(ref fastApproximateAntialiasing);
            RuntimeUtilities.CreateIfNull(ref dithering);
            RuntimeUtilities.CreateIfNull(ref fog);
            RuntimeUtilities.CreateIfNull(ref debugLayer);
        }

        public void InitBundles()
        {
            if (haveBundlesBeenInited)
                return;

            // Create these lists only once, the serialization system will take over after that
            RuntimeUtilities.CreateIfNull(ref m_BeforeTransparentBundles);
            RuntimeUtilities.CreateIfNull(ref m_BeforeStackBundles);
            RuntimeUtilities.CreateIfNull(ref m_AfterStackBundles);

            // Create a bundle for each effect type
            m_Bundles = new Dictionary<Type, PostProcessBundle>();

            foreach (var type in PostProcessManager.instance.settingsTypes.Keys)
            {
                var settings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(type);
                var bundle = new PostProcessBundle(settings);
                m_Bundles.Add(type, bundle);
            }

            // Update sorted lists with newly added or removed effects in the assemblies
            UpdateBundleSortList(m_BeforeTransparentBundles, PostProcessEvent.BeforeTransparent);
            UpdateBundleSortList(m_BeforeStackBundles, PostProcessEvent.BeforeStack);
            UpdateBundleSortList(m_AfterStackBundles, PostProcessEvent.AfterStack);

            // Push all sorted lists in a dictionary for easier access
            sortedBundles = new Dictionary<PostProcessEvent, List<SerializedBundleRef>>(new PostProcessEventComparer())
            {
                { PostProcessEvent.BeforeTransparent, m_BeforeTransparentBundles },
                { PostProcessEvent.BeforeStack, m_BeforeStackBundles },
                { PostProcessEvent.AfterStack, m_AfterStackBundles }
            };

            // Done
            haveBundlesBeenInited = true;
        }

        private void UpdateBundleSortList(List<SerializedBundleRef> sortedList, PostProcessEvent evt)
        {
            // First get all effects associated with the injection point
            var effects = m_Bundles
                .Where(kvp => kvp.Value.attribute.eventType == evt && !kvp.Value.attribute.builtinEffect)
                .Select(kvp => kvp.Value)
                .ToList();

            // Remove types that don't exist anymore
            sortedList.RemoveAll(x =>
            {
                var searchStr = x.assemblyQualifiedName;
                return !effects.Exists(b => b.settings.GetType().AssemblyQualifiedName == searchStr);
            });

            // Add new ones
            foreach (var effect in effects)
            {
                var typeName = effect.settings.GetType().AssemblyQualifiedName;

                if (!sortedList.Exists(b => b.assemblyQualifiedName == typeName))
                {
                    var sbr = new SerializedBundleRef { assemblyQualifiedName = typeName };
                    sortedList.Add(sbr);
                }
            }

            // Link internal references
            foreach (var effect in sortedList)
            {
                var typeName = effect.assemblyQualifiedName;
                var bundle = effects.Find(b => b.settings.GetType().AssemblyQualifiedName == typeName);
                effect.bundle = bundle;
            }
        }

        private void BuildCommandBuffers()
        {
            var context = m_CurrentContext;
            var sourceFormat = m_Camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

            context.Reset();
            context.camera = m_Camera;
            context.sourceFormat = sourceFormat;

            // TODO: Investigate retaining command buffers on XR multi-pass right eye
            m_LegacyCmdBufferBeforeReflections.Clear();
            m_LegacyCmdBufferBeforeLighting.Clear();
            m_LegacyCmdBufferOpaque.Clear();
            m_LegacyCmdBuffer.Clear();

            SetupContext(context);

            context.command = m_LegacyCmdBufferOpaque;
            UpdateSettingsIfNeeded(context);

            // Lighting & opaque-only effects
            var aoBundle = GetBundle<AmbientOcclusion>();
            var aoSettings = aoBundle.CastSettings<AmbientOcclusion>();
            var aoRenderer = aoBundle.CastRenderer<AmbientOcclusionRenderer>();

            var aoSupported = aoSettings.IsEnabledAndSupported(context);
            var aoAmbientOnly = aoRenderer.IsAmbientOnly(context);
            var isAmbientOcclusionDeferred = aoSupported && aoAmbientOnly;
            var isAmbientOcclusionOpaque = aoSupported && !aoAmbientOnly;

            var ssrBundle = GetBundle<ScreenSpaceReflections>();
            var ssrSettings = ssrBundle.settings;
            var ssrRenderer = ssrBundle.renderer;
            var isScreenSpaceReflectionsActive = ssrSettings.IsEnabledAndSupported(context);

            // Ambient-only AO is a special case and has to be done in separate command buffers
            if (isAmbientOcclusionDeferred)
            {
                var ao = aoRenderer.Get();

                // Render as soon as possible - should be done async in SRPs when available
                context.command = m_LegacyCmdBufferBeforeReflections;
                ao.RenderAmbientOnly(context);

                // Composite with GBuffer right before the lighting pass
                context.command = m_LegacyCmdBufferBeforeLighting;
                ao.CompositeAmbientOnly(context);
            }
            else if (isAmbientOcclusionOpaque)
            {
                context.command = m_LegacyCmdBufferOpaque;
                aoRenderer.Get().RenderAfterOpaque(context);
            }

            var isFogActive = fog.IsEnabledAndSupported(context);
            var hasCustomOpaqueOnlyEffects = HasOpaqueOnlyEffects(context);
            var opaqueOnlyEffects = 0;
            opaqueOnlyEffects += isScreenSpaceReflectionsActive ? 1 : 0;
            opaqueOnlyEffects += isFogActive ? 1 : 0;
            opaqueOnlyEffects += hasCustomOpaqueOnlyEffects ? 1 : 0;

            // This works on right eye because it is resolved/populated at runtime
            var cameraTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);

            if (opaqueOnlyEffects > 0)
            {
                var cmd = m_LegacyCmdBufferOpaque;
                context.command = cmd;

                // We need to use the internal Blit method to copy the camera target or it'll fail
                // on tiled GPU as it won't be able to resolve
                var tempTarget0 = m_TargetPool.Get();
                context.GetScreenSpaceTemporaryRT(cmd, tempTarget0, 24, sourceFormat);
                cmd.Blit(cameraTarget, tempTarget0);
                context.source = tempTarget0;

                var tempTarget1 = -1;

                if (opaqueOnlyEffects > 1)
                {
                    tempTarget1 = m_TargetPool.Get();
                    context.GetScreenSpaceTemporaryRT(cmd, tempTarget1, 24, sourceFormat);
                    context.destination = tempTarget1;
                }
                else
                {
                    context.destination = cameraTarget;
                }

                if (isScreenSpaceReflectionsActive)
                {
                    ssrRenderer.Render(context);
                    opaqueOnlyEffects--;
                    var prevSource = context.source;
                    context.source = context.destination;
                    context.destination = opaqueOnlyEffects == 1 ? cameraTarget : prevSource;
                }

                if (isFogActive)
                {
                    fog.Render(context);
                    opaqueOnlyEffects--;
                    var prevSource = context.source;
                    context.source = context.destination;
                    context.destination = opaqueOnlyEffects == 1 ? cameraTarget : prevSource;
                }

                if (hasCustomOpaqueOnlyEffects)
                    RenderOpaqueOnly(context);

                if (opaqueOnlyEffects > 1)
                    cmd.ReleaseTemporaryRT(tempTarget1);

                cmd.ReleaseTemporaryRT(tempTarget0);
            }

            // Post-transparency stack
            // Same as before, first blit needs to use the builtin Blit command to properly handle
            // tiled GPUs
            var tempRt = m_TargetPool.Get();
            context.GetScreenSpaceTemporaryRT(m_LegacyCmdBuffer, tempRt, 24, sourceFormat, RenderTextureReadWrite.sRGB);
            m_LegacyCmdBuffer.Blit(cameraTarget, tempRt, RuntimeUtilities.copyStdMaterial, stopNaNPropagation ? 1 : 0);
            m_NaNKilled = stopNaNPropagation;

            context.command = m_LegacyCmdBuffer;
            context.source = tempRt;
            context.destination = cameraTarget;
            Render(context);
            m_LegacyCmdBuffer.ReleaseTemporaryRT(tempRt);
        }

        public PostProcessBundle GetBundle<T>()
            where T : PostProcessEffectSettings
        {
            return GetBundle(typeof(T));
        }

        public PostProcessBundle GetBundle(Type settingsType)
        {
            Assert.IsTrue(m_Bundles.ContainsKey(settingsType), "Invalid type");
            return m_Bundles[settingsType];
        }

        public T GetSettings<T>()
            where T : PostProcessEffectSettings
        {
            return GetBundle<T>().CastSettings<T>();
        }

        public void BakeMSVOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination,
            RenderTargetIdentifier? depthMap, bool invert)
        {
            var bundle = GetBundle<AmbientOcclusion>();
            var renderer = bundle.CastRenderer<AmbientOcclusionRenderer>().GetMultiScaleVO();
            renderer.SetResources(m_Resources);
            renderer.GenerateAOMap(cmd, camera, destination, depthMap, invert);
        }

        internal void OverrideSettings(List<PostProcessEffectSettings> baseSettings, float interpFactor)
        {
            // Go through all settings & overriden parameters for the given volume and lerp values
            foreach (var settings in baseSettings)
            {
                if (!settings.active)
                    continue;

                var target = GetBundle(settings.GetType()).settings;
                var count = settings.parameters.Count;

                for (var i = 0; i < count; i++)
                {
                    var toParam = settings.parameters[i];
                    if (toParam.overrideState)
                    {
                        var fromParam = target.parameters[i];
                        fromParam.Interp(fromParam, toParam, interpFactor);
                    }
                }
            }
        }

        // In the legacy render loop you have to explicitely set flags on camera to tell that you
        // need depth, depth+normals or motion vectors... This won't have any effect with most
        // scriptable render pipelines.
        private void SetLegacyCameraFlags(PostProcessRenderContext context)
        {
            var flags = context.camera.depthTextureMode;

            foreach (var bundle in m_Bundles)
                if (bundle.Value.settings.IsEnabledAndSupported(context))
                    flags |= bundle.Value.renderer.GetCameraFlags();

            // Special case for AA & lighting effects
            if (context.IsTemporalAntialiasingActive())
                flags |= temporalAntialiasing.GetCameraFlags();

            if (fog.IsEnabledAndSupported(context))
                flags |= fog.GetCameraFlags();

            if (debugLayer.debugOverlay != DebugOverlay.None)
                flags |= debugLayer.GetCameraFlags();

            context.camera.depthTextureMode = flags;
        }

        // Call this function whenever you need to reset any temporal effect (TAA, Motion Blur etc).
        // Mainly used when doing camera cuts.
        public void ResetHistory()
        {
            foreach (var bundle in m_Bundles)
                bundle.Value.ResetHistory();

            temporalAntialiasing.ResetHistory();
        }

        public bool HasOpaqueOnlyEffects(PostProcessRenderContext context)
        {
            return HasActiveEffects(PostProcessEvent.BeforeTransparent, context);
        }

        public bool HasActiveEffects(PostProcessEvent evt, PostProcessRenderContext context)
        {
            var list = sortedBundles[evt];

            foreach (var item in list)
                if (item.bundle.settings.IsEnabledAndSupported(context))
                    return true;

            return false;
        }

        private void SetupContext(PostProcessRenderContext context)
        {
            m_IsRenderingInSceneView = context.camera.cameraType == CameraType.SceneView;
            context.isSceneView = m_IsRenderingInSceneView;
            context.resources = m_Resources;
            context.propertySheets = m_PropertySheetFactory;
            context.debugLayer = debugLayer;
            context.antialiasing = antialiasingMode;
            context.temporalAntialiasing = temporalAntialiasing;
            context.logHistogram = m_LogHistogram;
            SetLegacyCameraFlags(context);

            // Prepare debug overlay
            debugLayer.SetFrameSize(context.width, context.height);

            // Unsafe to keep this around but we need it for OnGUI events for debug views
            // Will be removed eventually
            m_CurrentContext = context;
        }

        private void UpdateSettingsIfNeeded(PostProcessRenderContext context)
        {
            if (m_SettingsUpdateNeeded)
            {
                context.command.BeginSample("VolumeBlending");
                PostProcessManager.instance.UpdateSettings(this);
                context.command.EndSample("VolumeBlending");
                m_TargetPool.Reset();

                // TODO: fix me once VR support is in SRP
                // Needed in SRP so that _RenderViewportScaleFactor isn't 0
                if (RuntimeUtilities.scriptableRenderPipelineActive)
                    Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
            }

            m_SettingsUpdateNeeded = false;
        }

        // Renders before-transparent effects.
        // Make sure you check `HasOpaqueOnlyEffects()` before calling this method as it won't
        // automatically blit source into destination if no opaque effects are active.
        public void RenderOpaqueOnly(PostProcessRenderContext context)
        {
            if (RuntimeUtilities.scriptableRenderPipelineActive)
                SetupContext(context);

            TextureLerper.instance.BeginFrame(context);

            // Update & override layer settings first (volume blending), will only be done once per
            // frame, either here or in Render() if there isn't any opaque-only effect to render.
            UpdateSettingsIfNeeded(context);

            RenderList(sortedBundles[PostProcessEvent.BeforeTransparent], context, "OpaqueOnly");
        }

        // Renders everything not opaque-only
        //
        // Current order of operation is as following:
        //     1. Pre-stack
        //     2. Built-in stack
        //     3. Post-stack
        //     4. Built-in final pass
        //
        // Final pass should be skipped when outputting to a HDR display.
        public void Render(PostProcessRenderContext context)
        {
            if (RuntimeUtilities.scriptableRenderPipelineActive)
                SetupContext(context);

            TextureLerper.instance.BeginFrame(context);
            var cmd = context.command;

            // Update & override layer settings first (volume blending) if the opaque only pass
            // hasn't been called this frame.
            UpdateSettingsIfNeeded(context);

            // Do a NaN killing pass if needed
            var lastTarget = -1;
            if (stopNaNPropagation && !m_NaNKilled)
            {
                lastTarget = m_TargetPool.Get();
                context.GetScreenSpaceTemporaryRT(cmd, lastTarget, 24, context.sourceFormat);
                cmd.BlitFullscreenTriangle(context.source, lastTarget, RuntimeUtilities.copySheet, 1);
                context.source = lastTarget;
                m_NaNKilled = true;
            }

            // Do temporal anti-aliasing first
            if (context.IsTemporalAntialiasingActive())
            {
                if (!RuntimeUtilities.scriptableRenderPipelineActive)
                {
                    if (context.stereoActive)
                    {
                        // We only need to configure all of this once for stereo, during OnPreCull
                        if (context.camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Right)
                            temporalAntialiasing.ConfigureStereoJitteredProjectionMatrices(context);
                    }
                    else
                    {
                        temporalAntialiasing.ConfigureJitteredProjectionMatrix(context);
                    }
                }

                var taaTarget = m_TargetPool.Get();
                var finalDestination = context.destination;
                context.GetScreenSpaceTemporaryRT(cmd, taaTarget, 24, context.sourceFormat);
                context.destination = taaTarget;
                temporalAntialiasing.Render(context);
                context.source = taaTarget;
                context.destination = finalDestination;

                if (lastTarget > -1)
                    cmd.ReleaseTemporaryRT(lastTarget);

                lastTarget = taaTarget;
            }

            var hasBeforeStackEffects = HasActiveEffects(PostProcessEvent.BeforeStack, context);
            var hasAfterStackEffects =
                HasActiveEffects(PostProcessEvent.AfterStack, context) && !breakBeforeColorGrading;
            var needsFinalPass = (hasAfterStackEffects
                                  || antialiasingMode == Antialiasing.FastApproximateAntialiasing ||
                                  (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing &&
                                   subpixelMorphologicalAntialiasing.IsSupported()))
                                 && !breakBeforeColorGrading;

            // Right before the builtin stack
            if (hasBeforeStackEffects)
                lastTarget = RenderInjectionPoint(PostProcessEvent.BeforeStack, context, "BeforeStack", lastTarget);

            // Builtin stack
            lastTarget = RenderBuiltins(context, !needsFinalPass, lastTarget);

            // After the builtin stack but before the final pass (before FXAA & Dithering)
            if (hasAfterStackEffects)
                lastTarget = RenderInjectionPoint(PostProcessEvent.AfterStack, context, "AfterStack", lastTarget);

            // And close with the final pass
            if (needsFinalPass)
                RenderFinalPass(context, lastTarget);

            // Render debug monitors & overlay if requested
            debugLayer.RenderSpecialOverlays(context);
            debugLayer.RenderMonitors(context);

            // End frame cleanup
            TextureLerper.instance.EndFrame();
            debugLayer.EndFrame();
            m_SettingsUpdateNeeded = true;
            m_NaNKilled = false;
        }

        private int RenderInjectionPoint(PostProcessEvent evt, PostProcessRenderContext context, string marker,
            int releaseTargetAfterUse = -1)
        {
            var tempTarget = m_TargetPool.Get();
            var finalDestination = context.destination;

            var cmd = context.command;
            context.GetScreenSpaceTemporaryRT(cmd, tempTarget, 24, context.sourceFormat);
            context.destination = tempTarget;
            RenderList(sortedBundles[evt], context, marker);
            context.source = tempTarget;
            context.destination = finalDestination;

            if (releaseTargetAfterUse > -1)
                cmd.ReleaseTemporaryRT(releaseTargetAfterUse);

            return tempTarget;
        }

        private void RenderList(List<SerializedBundleRef> list, PostProcessRenderContext context, string marker)
        {
            var cmd = context.command;
            cmd.BeginSample(marker);

            // First gather active effects - we need this to manage render targets more efficiently
            m_ActiveEffects.Clear();
            for (var i = 0; i < list.Count; i++)
            {
                var effect = list[i].bundle;
                if (effect.settings.IsEnabledAndSupported(context))
                    if (!context.isSceneView || (context.isSceneView && effect.attribute.allowInSceneView))
                        m_ActiveEffects.Add(effect.renderer);
            }

            var count = m_ActiveEffects.Count;

            // If there's only one active effect, we can simply execute it and skip the rest
            if (count == 1)
            {
                m_ActiveEffects[0].Render(context);
            }
            else
            {
                // Else create the target chain
                m_Targets.Clear();
                m_Targets.Add(context.source); // First target is always source

                var tempTarget1 = m_TargetPool.Get();
                var tempTarget2 = m_TargetPool.Get();

                for (var i = 0; i < count - 1; i++)
                    m_Targets.Add(i % 2 == 0 ? tempTarget1 : tempTarget2);

                m_Targets.Add(context.destination); // Last target is always destination

                // Render
                context.GetScreenSpaceTemporaryRT(cmd, tempTarget1, 24, context.sourceFormat);
                if (count > 2)
                    context.GetScreenSpaceTemporaryRT(cmd, tempTarget2, 24, context.sourceFormat);

                for (var i = 0; i < count; i++)
                {
                    context.source = m_Targets[i];
                    context.destination = m_Targets[i + 1];
                    m_ActiveEffects[i].Render(context);
                }

                cmd.ReleaseTemporaryRT(tempTarget1);
                if (count > 2)
                    cmd.ReleaseTemporaryRT(tempTarget2);
            }

            cmd.EndSample(marker);
        }

        private int RenderBuiltins(PostProcessRenderContext context, bool isFinalPass, int releaseTargetAfterUse = -1)
        {
            var uberSheet = context.propertySheets.Get(context.resources.shaders.uber);
            uberSheet.ClearKeywords();
            uberSheet.properties.Clear();
            context.uberSheet = uberSheet;
            context.autoExposureTexture = RuntimeUtilities.whiteTexture;
            context.bloomBufferNameID = -1;

            var cmd = context.command;
            cmd.BeginSample("BuiltinStack");

            var tempTarget = -1;
            var finalDestination = context.destination;

            if (!isFinalPass)
            {
                // Render to an intermediate target as this won't be the final pass
                tempTarget = m_TargetPool.Get();
                context.GetScreenSpaceTemporaryRT(cmd, tempTarget, 24, context.sourceFormat);
                context.destination = tempTarget;

                // Handle FXAA's keep alpha mode
                if (antialiasingMode == Antialiasing.FastApproximateAntialiasing &&
                    !fastApproximateAntialiasing.keepAlpha)
                    uberSheet.properties.SetFloat(ShaderIDs.LumaInAlpha, 1f);
            }

            // Depth of field final combination pass used to be done in Uber which led to artifacts
            // when used at the same time as Bloom (because both effects used the same source, so
            // the stronger bloom was, the more DoF was eaten away in out of focus areas)
            var depthOfFieldTarget = RenderEffect<DepthOfField>(context, true);

            // Motion blur is a separate pass - could potentially be done after DoF depending on the
            // kind of results you're looking for...
            var motionBlurTarget = RenderEffect<MotionBlur>(context, true);

            // Prepare exposure histogram if needed
            if (ShouldGenerateLogHistogram(context))
                m_LogHistogram.Generate(context);

            // Uber effects
            RenderEffect<AutoExposure>(context);
            uberSheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);

            RenderEffect<ChromaticAberration>(context);
            RenderEffect<Bloom>(context);
            RenderEffect<Vignette>(context);
            RenderEffect<Grain>(context);

            if (!breakBeforeColorGrading)
                RenderEffect<ColorGrading>(context);

            var pass = 0;

            if (isFinalPass)
            {
                uberSheet.EnableKeyword("FINALPASS");
                dithering.Render(context);

                if (context.flip && !context.isSceneView)
                    pass = 1;
            }

            cmd.BlitFullscreenTriangle(context.source, context.destination, uberSheet, pass);

            context.source = context.destination;
            context.destination = finalDestination;

            if (releaseTargetAfterUse > -1) cmd.ReleaseTemporaryRT(releaseTargetAfterUse);
            if (motionBlurTarget > -1) cmd.ReleaseTemporaryRT(motionBlurTarget);
            if (depthOfFieldTarget > -1) cmd.ReleaseTemporaryRT(motionBlurTarget);
            if (context.bloomBufferNameID > -1) cmd.ReleaseTemporaryRT(context.bloomBufferNameID);

            cmd.EndSample("BuiltinStack");

            return tempTarget;
        }

        // This pass will have to be disabled for HDR screen output as it's an LDR pass
        private void RenderFinalPass(PostProcessRenderContext context, int releaseTargetAfterUse = -1)
        {
            var cmd = context.command;
            cmd.BeginSample("FinalPass");

            if (breakBeforeColorGrading)
            {
                var sheet = context.propertySheets.Get(context.resources.shaders.discardAlpha);
                cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            }
            else
            {
                var uberSheet = context.propertySheets.Get(context.resources.shaders.finalPass);
                uberSheet.ClearKeywords();
                uberSheet.properties.Clear();
                context.uberSheet = uberSheet;
                var tempTarget = -1;

                if (antialiasingMode == Antialiasing.FastApproximateAntialiasing)
                {
                    uberSheet.EnableKeyword(fastApproximateAntialiasing.fastMode
                        ? "FXAA_LOW"
                        : "FXAA"
                    );

                    if (fastApproximateAntialiasing.keepAlpha)
                        uberSheet.EnableKeyword("FXAA_KEEP_ALPHA");
                }
                else if (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing &&
                         subpixelMorphologicalAntialiasing.IsSupported())
                {
                    tempTarget = m_TargetPool.Get();
                    var finalDestination = context.destination;
                    context.GetScreenSpaceTemporaryRT(context.command, tempTarget, 24, context.sourceFormat);
                    context.destination = tempTarget;
                    subpixelMorphologicalAntialiasing.Render(context);
                    context.source = tempTarget;
                    context.destination = finalDestination;
                }

                dithering.Render(context);

                cmd.BlitFullscreenTriangle(context.source, context.destination, uberSheet,
                    context.flip && !context.isSceneView ? 1 : 0);

                if (tempTarget > -1)
                    cmd.ReleaseTemporaryRT(tempTarget);
            }

            if (releaseTargetAfterUse > -1)
                cmd.ReleaseTemporaryRT(releaseTargetAfterUse);

            cmd.EndSample("FinalPass");
        }

        private int RenderEffect<T>(PostProcessRenderContext context, bool useTempTarget = false)
            where T : PostProcessEffectSettings
        {
            var effect = GetBundle<T>();

            if (!effect.settings.IsEnabledAndSupported(context))
                return -1;

            if (m_IsRenderingInSceneView && !effect.attribute.allowInSceneView)
                return -1;

            if (!useTempTarget)
            {
                effect.renderer.Render(context);
                return -1;
            }

            var finalDestination = context.destination;
            var tempTarget = m_TargetPool.Get();
            context.GetScreenSpaceTemporaryRT(context.command, tempTarget, 24, context.sourceFormat);
            context.destination = tempTarget;
            effect.renderer.Render(context);
            context.source = tempTarget;
            context.destination = finalDestination;
            return tempTarget;
        }

        private bool ShouldGenerateLogHistogram(PostProcessRenderContext context)
        {
            var autoExpo = GetBundle<AutoExposure>().settings.IsEnabledAndSupported(context);
            var lightMeter = debugLayer.lightMeter.IsRequestedAndSupported();
            return autoExpo || lightMeter;
        }

        // Pre-ordered custom user effects
        // These are automatically populated and made to work properly with the serialization
        // system AND the editor. Modify at your own risk.
        [Serializable]
        public sealed class SerializedBundleRef
        {
            // We can't serialize Type so use assemblyQualifiedName instead, we only need this at
            // init time anyway so it's fine
            public string assemblyQualifiedName;

            // Not serialized, is set/reset when deserialization kicks in
            public PostProcessBundle bundle;
        }
    }
}