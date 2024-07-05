//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

// comment out to disable lightning path lines

#define SHOW_LIGHTNING_PATH

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DigitalRuby.ThunderAndLightning
{
    public abstract class LightningBoltPathScriptBase : LightningBoltPrefabScriptBase
    {
        [Header("Lightning Path Properties")]
        [ReorderableListAttribute("The game objects to follow for the lightning path")]
        public ReorderableList_GameObject LightningPath;

        private readonly List<GameObject> currentPathObjects = new();

        /// <summary>
        ///     Get the game objects in the path currently - null or inactive objects are not returned
        /// </summary>
        /// <returns>List of game objects in the path</returns>
        protected List<GameObject> GetCurrentPathObjects()
        {
            currentPathObjects.Clear();
            if (LightningPath != null && LightningPath.List != null)
                foreach (var obj in LightningPath.List)
                    if (obj != null && obj.activeInHierarchy)
                        currentPathObjects.Add(obj);
            return currentPathObjects;
        }

        /// <summary>
        ///     Create lightning bolt path parameters
        /// </summary>
        /// <returns>Lightning bolt path parameters</returns>
        protected override LightningBoltParameters OnCreateParameters()
        {
            var p = base.OnCreateParameters();
            p.Generator = LightningGenerator.GeneratorInstance;
            return p;
        }

#if UNITY_EDITOR

        [NonSerialized] private bool gizmosCleanedUp;

        [NonSerialized] private readonly List<GameObject> lastGizmos = new();

        private void DoGizmoCleanup()
        {
            if (gizmosCleanedUp) return;

            gizmosCleanedUp = true;

            foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(LightningGizmoScript)))
            {
                LightningGizmoScript s;
                s = obj as LightningGizmoScript;
                if (s == null)
                {
                    var gObj = obj as GameObject;
                    if (gObj != null) s = gObj.GetComponent<LightningGizmoScript>();
                }

                if (s != null) DestroyImmediate(s, true);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DoGizmoCleanup();

#if SHOW_LIGHTNING_PATH

            var noLightningPath = LightningPath == null || LightningPath.List == null || LightningPath.List.Count == 0;

#else
            bool noLightningPath = true;

#endif

            // remove any objects that were taken out of the list and cleanup the gizmo script
            for (var i = lastGizmos.Count - 1; i >= 0; i--)
                if (noLightningPath || !LightningPath.List.Contains(lastGizmos[i]))
                {
                    if (lastGizmos[i] != null)
                    {
                        var s = lastGizmos[i].GetComponent<LightningGizmoScript>();
                        if (s != null) DestroyImmediate(s, true);
                    }

                    lastGizmos.RemoveAt(i);
                }

            // no objects, we are done
            if (noLightningPath) return;

            // add gizmo scripts and draw lines as needed
            Vector3 gizmoPosition;
            var gizmos = new HashSet<GameObject>();
            Vector3? previousPoint = null;
            LightningGizmoScript gizmoScript;
            lastGizmos.Clear();

            for (var index = 0; index < LightningPath.List.Count; index++)
            {
                var o = LightningPath.List[index];
                if (o == null || !o.activeInHierarchy)
                    continue;
                if ((gizmoScript = o.GetComponent<LightningGizmoScript>()) == null)
                    // we need to add the gizmo script so that this object can be selectable by tapping on the lightning bolt in the scene view
                    gizmoScript = o.AddComponent<LightningGizmoScript>();
                gizmoScript.hideFlags = HideFlags.HideInInspector;

                // setup label based on whether we've seen this one before
                if (gizmos.Add(o))
                    gizmoScript.Label = index.ToString();
                else
                    gizmoScript.Label += ", " + index;

                gizmoPosition = o.transform.position;
                if (previousPoint != null && previousPoint.Value != gizmoPosition)
                {
                    // draw a line and arrow in the proper direction
                    Gizmos.DrawLine(previousPoint.Value, gizmoPosition);
                    var direction = gizmoPosition - previousPoint.Value;
                    var center = (previousPoint.Value + gizmoPosition) * 0.5f;
                    var arrowSize = Mathf.Min(1.0f, direction.magnitude);
                    //UnityEditor.Handles.ArrowCap(0, center, Quaternion.LookRotation(direction), arrowSize);
                }

                previousPoint = gizmoPosition;
                lastGizmos.Add(o);
            }
        }

#endif
    }

    public class LightningBoltPathScript : LightningBoltPathScriptBase
    {
        [Tooltip("How fast the lightning moves through the points or objects. 1 is normal speed, " +
                 "0.01 is slower, so the lightning will move slowly between the points or objects.")]
        [Range(0.01f, 1.0f)]
        public float Speed = 1.0f;

        [SingleLineClamp("When each new point is moved to, this can provide a random value to make the movement to " +
                         "the next point appear more staggered or random. Leave as 1 and 1 to have constant speed. Use a higher " +
                         "maximum to create more randomness.", 1.0, 500.0)]
        public RangeOfFloats SpeedIntervalRange = new() { Minimum = 1.0f, Maximum = 1.0f };

        [Tooltip("Repeat when the path completes?")]
        public bool Repeat = true;

        private Vector3? lastPoint;
        private int nextIndex;

        private float nextInterval = 1.0f;

        public void Reset()
        {
            lastPoint = null;
            nextIndex = 0;
            nextInterval = 1.0f;
        }

        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            Vector3? currentPoint = null;
            var lightningPath = GetCurrentPathObjects();

            if (lightningPath.Count < 2) return;

            if (nextIndex >= lightningPath.Count)
            {
                if (!Repeat)
                {
                    return;
                }

                if (lightningPath[lightningPath.Count - 1] == lightningPath[0])
                {
                    nextIndex = 1;
                }
                else
                {
                    nextIndex = 0;
                    lastPoint = null;
                }
            }

            try
            {
                if (lastPoint == null) lastPoint = lightningPath[nextIndex++].transform.position;
                currentPoint = lightningPath[nextIndex].transform.position;
                if (lastPoint != null && currentPoint != null)
                {
                    parameters.Start = lastPoint.Value;
                    parameters.End = currentPoint.Value;
                    base.CreateLightningBolt(parameters);

                    if ((nextInterval -= Speed) <= 0.0f)
                    {
                        var speedValue = Random.Range(SpeedIntervalRange.Minimum, SpeedIntervalRange.Maximum);
                        nextInterval = speedValue + nextInterval;
                        lastPoint = currentPoint;
                        nextIndex++;
                    }
                }
            }
            catch (NullReferenceException)
            {
                // null reference exception can happen here, in which case we bail as the list is broken until the null object gets taken out
            }
        }
    }
}