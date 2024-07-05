//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningSplineScript : LightningBoltPathScriptBase
    {
        /// <summary>
        ///     For performance, cap generations
        /// </summary>
        public const int MaxSplineGenerations = 5;

        [Header("Lightning Spline Properties")]
        [Tooltip(
            "The distance hint for each spline segment. Set to <= 0 to use the generations to determine how many spline segments to use. " +
            "If > 0, it will be divided by Generations before being applied. This value is a guideline and is approximate, and not uniform on the spline.")]
        public float DistancePerSegmentHint;

        private readonly List<Vector3> prevSourcePoints = new(new[] { Vector3.zero });
        private readonly List<Vector3> sourcePoints = new();
        private float previousDistancePerSegment = -1.0f;

        private int previousGenerations = -1;
        private List<Vector3> savedSplinePoints;

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        private bool SourceChanged()
        {
            if (sourcePoints.Count != prevSourcePoints.Count) return true;
            for (var i = 0; i < sourcePoints.Count; i++)
                if (sourcePoints[i] != prevSourcePoints[i])
                    return true;

            return false;
        }

        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            if (LightningPath == null || LightningPath.List == null) return;

            sourcePoints.Clear();
            try
            {
                foreach (var obj in LightningPath.List)
                    if (obj != null)
                        sourcePoints.Add(obj.transform.position);
            }
            catch (NullReferenceException)
            {
                return;
            }

            if (sourcePoints.Count < PathGenerator.MinPointsForSpline)
                UnityEngine.Debug.LogError("To create spline lightning, you need a lightning path with at least " +
                                           PathGenerator.MinPointsForSpline + " points.");

            Generations = parameters.Generations = Mathf.Clamp(Generations, 1, MaxSplineGenerations);
            if (previousGenerations != Generations || previousDistancePerSegment != DistancePerSegmentHint ||
                SourceChanged())
            {
                previousGenerations = Generations;
                previousDistancePerSegment = DistancePerSegmentHint;
                parameters.Points = new List<Vector3>(sourcePoints.Count * Generations);
                PopulateSpline(parameters.Points, sourcePoints, Generations, DistancePerSegmentHint, Camera);
                prevSourcePoints.Clear();
                prevSourcePoints.AddRange(sourcePoints);
                savedSplinePoints = parameters.Points;
            }
            else
            {
                parameters.Points = savedSplinePoints;
            }

            parameters.SmoothingFactor = (parameters.Points.Count - 1) / sourcePoints.Count;

            base.CreateLightningBolt(parameters);
        }

        protected override LightningBoltParameters OnCreateParameters()
        {
            var p = LightningBoltParameters.GetOrCreateParameters();
            p.Generator = LightningGeneratorPath.PathGeneratorInstance;
            return p;
        }

        /// <summary>
        ///     Triggers lightning that follows a set of points, rather than the standard lightning bolt that goes between two
        ///     points.
        /// </summary>
        /// <param name="points">Points to follow</param>
        /// <param name="spline">Whether to spline the lightning through the points or not</param>
        public void Trigger(List<Vector3> points, bool spline)
        {
            if (points.Count < 2) return;
            Generations = Mathf.Clamp(Generations, 1, MaxSplineGenerations);
            var parameters = CreateParameters();
            if (spline && points.Count > 3)
            {
                parameters.Points = new List<Vector3>(points.Count * Generations);
                PopulateSpline(parameters.Points, points, Generations, DistancePerSegmentHint, Camera);
                parameters.SmoothingFactor = (parameters.Points.Count - 1) / points.Count;
            }
            else
            {
                parameters.Points = new List<Vector3>(points);
                parameters.SmoothingFactor = 1;
            }

            base.CreateLightningBolt(parameters);
            CreateLightningBoltsNow();
        }

        /// <summary>
        ///     Populate a list of spline points from source points
        /// </summary>
        /// <param name="splinePoints">List to fill with spline points</param>
        /// <param name="sourcePoints">Source points</param>
        /// <param name="generations">Generations</param>
        /// <param name="distancePerSegmentHit">
        ///     Distance per segment hint - if non-zero, attempts to maintain this distance between
        ///     spline points.
        /// </param>
        /// <param name="camera">Optional camera</param>
        public static void PopulateSpline(List<Vector3> splinePoints, List<Vector3> sourcePoints, int generations,
            float distancePerSegmentHit, Camera camera)
        {
            splinePoints.Clear();
            PathGenerator.Is2D = camera != null && camera.orthographic;
            if (distancePerSegmentHit > 0.0f)
                PathGenerator.CreateSplineWithSegmentDistance(splinePoints, sourcePoints,
                    distancePerSegmentHit / generations, false);
            else
                PathGenerator.CreateSpline(splinePoints, sourcePoints, sourcePoints.Count * generations * generations,
                    false);
        }
    }
}