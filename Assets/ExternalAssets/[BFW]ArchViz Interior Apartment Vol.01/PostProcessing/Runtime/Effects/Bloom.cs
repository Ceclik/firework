using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
    // For now and by popular request, this bloom effect is geared toward artists so they have full
    // control over how it looks at the expense of physical correctness.
    // Eventually we will need a "true" natural bloom effect with proper energy conservation.

    [Serializable]
    [PostProcess(typeof(BloomRenderer), "Unity/Bloom")]
    public sealed class Bloom : PostProcessEffectSettings
    {
        [Min(0f)]
        [Tooltip(
            "Strength of the bloom filter. Values higher than 1 will make bloom contribute more energy to the final render. Keep this under or equal to 1 if you want energy conservation.")]
        public FloatParameter intensity = new() { value = 0f };

        [Min(0f)] [Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
        public FloatParameter threshold = new() { value = 1f };

        [Range(0f, 1f)]
        [Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
        public FloatParameter softKnee = new() { value = 0.5f };

        [Range(1f, 10f)]
        [Tooltip(
            "Changes the extent of veiling effects. For maximum quality stick to integer values. Because this value changes the internal iteration count, animating it isn't recommended as it may introduce small hiccups in the perceived radius.")]
        public FloatParameter diffusion = new() { value = 7f };

        [Range(-1f, 1f)]
        [Tooltip(
            "Distorts the bloom to give an anamorphic look. Negative values distort vertically, positive values distort horizontally.")]
        public FloatParameter anamorphicRatio = new() { value = 0f };

#if UNITY_2018_1_OR_NEWER
        [ColorUsage(false, true)] [Tooltip("Global tint of the bloom filter.")]
#else
        [ColorUsage(false, true, 0f, 8f, 0.125f, 3f), Tooltip("Global tint of the bloom filter.")]
#endif
        public ColorParameter color = new() { value = Color.white };

        [FormerlySerializedAs("mobileOptimized")]
        [Tooltip(
            "Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
        public BoolParameter fastMode = new() { value = false };

        [Tooltip("Dirtiness texture to add smudges or dust to the bloom effect.")] [DisplayName("Texture")]
        public TextureParameter dirtTexture = new() { value = null };

        [Min(0f)] [Tooltip("Amount of dirtiness.")] [DisplayName("Intensity")]
        public FloatParameter dirtIntensity = new() { value = 0f };

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value
                   && intensity.value > 0f;
        }
    }

    public sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
    {
        private const int k_MaxPyramidSize = 16; // Just to make sure we handle 64k screens... Future-proof!

        // [down,up]
        private Level[] m_Pyramid;

        public override void Init()
        {
            m_Pyramid = new Level[k_MaxPyramidSize];

            for (var i = 0; i < k_MaxPyramidSize; i++)
                m_Pyramid[i] = new Level
                {
                    down = Shader.PropertyToID("_BloomMipDown" + i),
                    up = Shader.PropertyToID("_BloomMipUp" + i)
                };
        }

        public override void Render(PostProcessRenderContext context)
        {
            var cmd = context.command;
            cmd.BeginSample("BloomPyramid");

            var sheet = context.propertySheets.Get(context.resources.shaders.bloom);

            // Apply auto exposure adjustment in the prefiltering pass
            sheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);

            // Negative anamorphic ratio values distort vertically - positive is horizontal
            var ratio = Mathf.Clamp(settings.anamorphicRatio, -1, 1);
            var rw = ratio < 0 ? -ratio : 0f;
            var rh = ratio > 0 ? ratio : 0f;

            // Do bloom on a half-res buffer, full-res doesn't bring much and kills performances on
            // fillrate limited platforms
            var tw = Mathf.FloorToInt(context.screenWidth / (2f - rw));
            var th = Mathf.FloorToInt(context.screenHeight / (2f - rh));

            // Determine the iteration count
            var s = Mathf.Max(tw, th);
            var logs = Mathf.Log(s, 2f) + Mathf.Min(settings.diffusion.value, 10f) - 10f;
            var logs_i = Mathf.FloorToInt(logs);
            var iterations = Mathf.Clamp(logs_i, 1, k_MaxPyramidSize);
            var sampleScale = 0.5f + logs - logs_i;
            sheet.properties.SetFloat(ShaderIDs.SampleScale, sampleScale);

            // Prefiltering parameters
            var lthresh = Mathf.GammaToLinearSpace(settings.threshold.value);
            var knee = lthresh * settings.softKnee.value + 1e-5f;
            var threshold = new Vector4(lthresh, lthresh - knee, knee * 2f, 0.25f / knee);
            sheet.properties.SetVector(ShaderIDs.Threshold, threshold);

            var qualityOffset = settings.fastMode ? 1 : 0;

            // Downsample
            var lastDown = context.source;
            for (var i = 0; i < iterations; i++)
            {
                var mipDown = m_Pyramid[i].down;
                var mipUp = m_Pyramid[i].up;
                var pass = i == 0
                    ? (int)Pass.Prefilter13 + qualityOffset
                    : (int)Pass.Downsample13 + qualityOffset;

                context.GetScreenSpaceTemporaryRT(cmd, mipDown, 0, context.sourceFormat, RenderTextureReadWrite.Default,
                    FilterMode.Bilinear, tw, th);
                context.GetScreenSpaceTemporaryRT(cmd, mipUp, 0, context.sourceFormat, RenderTextureReadWrite.Default,
                    FilterMode.Bilinear, tw, th);
                cmd.BlitFullscreenTriangle(lastDown, mipDown, sheet, pass);

                lastDown = mipDown;
                tw = Mathf.Max(tw / 2, 1);
                th = Mathf.Max(th / 2, 1);
            }

            // Upsample
            var lastUp = m_Pyramid[iterations - 1].down;
            for (var i = iterations - 2; i >= 0; i--)
            {
                var mipDown = m_Pyramid[i].down;
                var mipUp = m_Pyramid[i].up;
                cmd.SetGlobalTexture(ShaderIDs.BloomTex, mipDown);
                cmd.BlitFullscreenTriangle(lastUp, mipUp, sheet, (int)Pass.UpsampleTent + qualityOffset);
                lastUp = mipUp;
            }

            var linearColor = settings.color.value.linear;
            var intensity = RuntimeUtilities.Exp2(settings.intensity.value / 10f) - 1f;
            var shaderSettings = new Vector4(sampleScale, intensity, settings.dirtIntensity.value, iterations);

            // Debug overlays
            if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
            {
                context.PushDebugOverlay(cmd, context.source, sheet, (int)Pass.DebugOverlayThreshold);
            }
            else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
            {
                sheet.properties.SetVector(ShaderIDs.ColorIntensity,
                    new Vector4(linearColor.r, linearColor.g, linearColor.b, intensity));
                context.PushDebugOverlay(cmd, m_Pyramid[0].up, sheet, (int)Pass.DebugOverlayTent + qualityOffset);
            }

            // Lens dirtiness
            // Keep the aspect ratio correct & center the dirt texture, we don't want it to be
            // stretched or squashed
            var dirtTexture = settings.dirtTexture.value == null
                ? RuntimeUtilities.blackTexture
                : settings.dirtTexture.value;

            var dirtRatio = dirtTexture.width / (float)dirtTexture.height;
            var screenRatio = context.screenWidth / (float)context.screenHeight;
            var dirtTileOffset = new Vector4(1f, 1f, 0f, 0f);

            if (dirtRatio > screenRatio)
            {
                dirtTileOffset.x = screenRatio / dirtRatio;
                dirtTileOffset.z = (1f - dirtTileOffset.x) * 0.5f;
            }
            else if (screenRatio > dirtRatio)
            {
                dirtTileOffset.y = dirtRatio / screenRatio;
                dirtTileOffset.w = (1f - dirtTileOffset.y) * 0.5f;
            }

            // Shader properties
            var uberSheet = context.uberSheet;
            uberSheet.EnableKeyword("BLOOM");
            uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, dirtTileOffset);
            uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, shaderSettings);
            uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linearColor);
            uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, dirtTexture);
            cmd.SetGlobalTexture(ShaderIDs.BloomTex, lastUp);

            // Cleanup
            for (var i = 0; i < iterations; i++)
            {
                if (m_Pyramid[i].down != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].down);
                if (m_Pyramid[i].up != lastUp)
                    cmd.ReleaseTemporaryRT(m_Pyramid[i].up);
            }

            cmd.EndSample("BloomPyramid");

            context.bloomBufferNameID = lastUp;
        }

        private enum Pass
        {
            Prefilter13,
            Prefilter4,
            Downsample13,
            Downsample4,
            UpsampleTent,
            UpsampleBox,
            DebugOverlayThreshold,
            DebugOverlayTent,
            DebugOverlayBox
        }

        private struct Level
        {
            internal int down;
            internal int up;
        }
    }
}