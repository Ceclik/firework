﻿//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using Random = System.Random;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningGenerator
    {
        public static readonly LightningGenerator GeneratorInstance = new();

        private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
        {
            if (directionNormalized == Vector3.zero)
            {
                side = Vector3.right;
            }
            else
            {
                // use cross product to find any perpendicular vector around directionNormalized:
                // 0 = x * px + y * py + z * pz
                // => pz = -(x * px + y * py) / z
                // for computational stability use the component farthest from 0 to divide by
                var x = directionNormalized.x;
                var y = directionNormalized.y;
                var z = directionNormalized.z;
                float px, py, pz;
                float ax = Mathf.Abs(x), ay = Mathf.Abs(y), az = Mathf.Abs(z);
                if (ax >= ay && ay >= az)
                {
                    // x is the max, so we can pick (py, pz) arbitrarily at (1, 1):
                    py = 1.0f;
                    pz = 1.0f;
                    px = -(y * py + z * pz) / x;
                }
                else if (ay >= az)
                {
                    // y is the max, so we can pick (px, pz) arbitrarily at (1, 1):
                    px = 1.0f;
                    pz = 1.0f;
                    py = -(x * px + z * pz) / y;
                }
                else
                {
                    // z is the max, so we can pick (px, py) arbitrarily at (1, 1):
                    px = 1.0f;
                    py = 1.0f;
                    pz = -(x * px + y * py) / z;
                }

                side = new Vector3(px, py, pz).normalized;
            }
        }

        protected virtual void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end,
            LightningBoltParameters p)
        {
            GenerateLightningBoltStandard(bolt, start, end, p.Generations, p.Generations, 0.0f, p);
        }

        public bool ShouldCreateFork(LightningBoltParameters p, int generation, int totalGenerations)
        {
            return generation > p.generationWhereForksStop && generation >= totalGenerations - p.forkednessCalculated &&
                   (float)p.Random.NextDouble() < p.Forkedness;
        }

        public void CreateFork(LightningBolt bolt, LightningBoltParameters p, int generation, int totalGenerations,
            Vector3 start, Vector3 midPoint)
        {
            if (ShouldCreateFork(p, generation, totalGenerations))
            {
                var branchVector = (midPoint - start) * p.ForkMultiplier();
                var splitEnd = midPoint + branchVector;
                GenerateLightningBoltStandard(bolt, midPoint, splitEnd, generation, totalGenerations, 0.0f, p);
            }
        }

        public void GenerateLightningBoltStandard(LightningBolt bolt, Vector3 start, Vector3 end, int generation,
            int totalGenerations, float offsetAmount, LightningBoltParameters p)
        {
            if (generation < 1) return;

            var group = bolt.AddGroup();
            group.Segments.Add(new LightningBoltSegment { Start = start, End = end });

            // every generation, get the percentage we have gone down and square it, this makes lines thinner
            var widthMultiplier = generation / (float)totalGenerations;
            widthMultiplier *= widthMultiplier;

            Vector3 randomVector;
            group.LineWidth = p.TrunkWidth * widthMultiplier;
            group.Generation = generation;
            group.Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(255.0f * widthMultiplier));
            group.EndWidthMultiplier = p.EndWidthMultiplier * p.ForkEndWidthMultiplier;
            if (offsetAmount <= 0.0f) offsetAmount = (end - start).magnitude * p.ChaosFactor;

            while (generation-- > 0)
            {
                var previousStartIndex = group.StartIndex;
                group.StartIndex = group.Segments.Count;
                for (var i = previousStartIndex; i < group.StartIndex; i++)
                {
                    start = group.Segments[i].Start;
                    end = group.Segments[i].End;

                    // determine a new direction for the split
                    var midPoint = (start + end) * 0.5f;

                    // adjust the mid point to be the new location
                    RandomVector(bolt, ref start, ref end, offsetAmount, p.Random, out randomVector);
                    midPoint += randomVector;

                    // add two new segments
                    group.Segments.Add(new LightningBoltSegment { Start = start, End = midPoint });
                    group.Segments.Add(new LightningBoltSegment { Start = midPoint, End = end });

                    CreateFork(bolt, p, generation, totalGenerations, start, midPoint);
                }

                // halve the distance the lightning can deviate for each generation down
                offsetAmount *= 0.5f;
            }
        }

        public Vector3 RandomDirection3D(Random r)
        {
            var z = 2.0f * (float)r.NextDouble() - 1.0f; // z is in the range [-1,1]
            var planar = RandomDirection2D(r) * Mathf.Sqrt(1.0f - z * z);
            planar.z = z;

            return planar;
        }

        public Vector3 RandomDirection2D(Random r)
        {
            var azimuth = (float)r.NextDouble() * 2.0f * Mathf.PI;
            return new Vector3(Mathf.Cos(azimuth), Mathf.Sin(azimuth), 0.0f);
        }

        public Vector3 RandomDirection2DXZ(Random r)
        {
            var azimuth = (float)r.NextDouble() * 2.0f * Mathf.PI;
            return new Vector3(Mathf.Cos(azimuth), 0.0f, Mathf.Sin(azimuth));
        }

        public void RandomVector(LightningBolt bolt, ref Vector3 start, ref Vector3 end, float offsetAmount,
            Random random, out Vector3 result)
        {
            if (bolt.CameraMode == CameraMode.Perspective)
            {
                var direction = (end - start).normalized;
                var side = Vector3.Cross(start, end);
                if (side == Vector3.zero)
                    // slow path, rarely hit unless cross product is zero
                    GetPerpendicularVector(ref direction, out side);
                else
                    side.Normalize();

                // generate random distance and angle
                var distance = ((float)random.NextDouble() + 0.1f) * offsetAmount;

#if DEBUG

                var rotationAngle = (float)random.NextDouble() * 360.0f;
                result = Quaternion.AngleAxis(rotationAngle, direction) * side * distance;

#else
                // optimized path for RELEASE mode, skips two normalize and two multiplies in Quaternion.AngleAxis
                float rotationAngle = ((float)random.NextDouble() * Mathf.PI);
                direction *= (float)System.Math.Sin(rotationAngle);
                Quaternion rotation;
                rotation.x = direction.x;
                rotation.y = direction.y;
                rotation.z = direction.z;
                rotation.w = (float)System.Math.Cos(rotationAngle);
                result = rotation * side * distance;

#endif
            }
            else if (bolt.CameraMode == CameraMode.OrthographicXY)
            {
                // XY plane
                end.z = start.z;
                var directionNormalized = (end - start).normalized;
                var side = new Vector3(-directionNormalized.y, directionNormalized.x, 0.0f);
                var distance = (float)random.NextDouble() * offsetAmount * 2.0f - offsetAmount;
                result = side * distance;
            }
            else
            {
                // XZ plane
                end.y = start.y;
                var directionNormalized = (end - start).normalized;
                var side = new Vector3(-directionNormalized.z, 0.0f, directionNormalized.x);
                var distance = (float)random.NextDouble() * offsetAmount * 2.0f - offsetAmount;
                result = side * distance;
            }
        }

        public void GenerateLightningBolt(LightningBolt bolt, LightningBoltParameters p)
        {
            var start = p.ApplyVariance(p.Start, p.StartVariance);
            var end = p.ApplyVariance(p.End, p.EndVariance);
            OnGenerateLightningBolt(bolt, start, end, p);
        }
    }

    /// <summary>
    ///     Generates lightning that follows a path
    /// </summary>
    public class LightningGeneratorPath : LightningGenerator
    {
        public static readonly LightningGeneratorPath PathGeneratorInstance = new();

        public void GenerateLightningBoltPath(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters p)
        {
            if (p.Points.Count < 2)
            {
                UnityEngine.Debug.LogError("Lightning path should have at least two points");
                return;
            }

            var generation = p.Generations;
            var totalGenerations = generation;
            float offsetAmount, d;
            var smoothingFactor = p.SmoothingFactor - 1;
            Vector3 distance, randomVector;
            var group = bolt.AddGroup();
            group.LineWidth = p.TrunkWidth;
            group.Generation = generation--;
            group.EndWidthMultiplier = p.EndWidthMultiplier;
            group.Color = Color.white;

            p.Start = p.Points[0] + start;
            p.End = p.Points[p.Points.Count - 1] + end;
            end = p.Start;

            for (var i = 1; i < p.Points.Count; i++)
            {
                start = end;
                end = p.Points[i];
                distance = end - start;
                d = PathGenerator.SquareRoot(distance.sqrMagnitude);
                if (p.ChaosFactor > 0.0f)
                {
                    if (bolt.CameraMode == CameraMode.Perspective)
                        end += d * p.ChaosFactor * RandomDirection3D(p.Random);
                    else if (bolt.CameraMode == CameraMode.OrthographicXY)
                        end += d * p.ChaosFactor * RandomDirection2D(p.Random);
                    else
                        end += d * p.ChaosFactor * RandomDirection2DXZ(p.Random);
                    distance = end - start;
                }

                group.Segments.Add(new LightningBoltSegment { Start = start, End = end });

                offsetAmount = d * p.ChaosFactor;
                RandomVector(bolt, ref start, ref end, offsetAmount, p.Random, out randomVector);

                if (ShouldCreateFork(p, generation, totalGenerations))
                {
                    var branchVector = distance * p.ForkMultiplier() * smoothingFactor * 0.5f;
                    var forkEnd = end + branchVector + randomVector;
                    GenerateLightningBoltStandard(bolt, start, forkEnd, generation, totalGenerations, 0.0f, p);
                }

                if (--smoothingFactor == 0) smoothingFactor = p.SmoothingFactor - 1;
            }
        }

        protected override void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end,
            LightningBoltParameters p)
        {
            GenerateLightningBoltPath(bolt, start, end, p);
        }
    }
}