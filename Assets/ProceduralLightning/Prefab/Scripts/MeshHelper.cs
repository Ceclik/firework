//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class MeshHelper
    {
        private float[] normalizedAreaWeights;

        public MeshHelper(Mesh mesh)
        {
            Mesh = mesh;
            Triangles = mesh.triangles;
            Vertices = mesh.vertices;
            Normals = mesh.normals;
            CalculateNormalizedAreaWeights();
        }

        public Mesh Mesh { get; }

        public int[] Triangles { get; }

        public Vector3[] Vertices { get; }

        public Vector3[] Normals { get; }

        public void GenerateRandomPoint(ref RaycastHit hit, out int triangleIndex)
        {
            triangleIndex = SelectRandomTriangle();
            GetRaycastFromTriangleIndex(triangleIndex, ref hit);
        }

        public void GetRaycastFromTriangleIndex(int triangleIndex, ref RaycastHit hit)
        {
            var bc = GenerateRandomBarycentricCoordinates();
            var p1 = Vertices[Triangles[triangleIndex]];
            var p2 = Vertices[Triangles[triangleIndex + 1]];
            var p3 = Vertices[Triangles[triangleIndex + 2]];

            hit.barycentricCoordinate = bc;
            hit.point = p1 * bc.x + p2 * bc.y + p3 * bc.z;

            if (Normals == null)
            {
                // face normal
                hit.normal = Vector3.Cross(p3 - p2, p1 - p2).normalized;
            }
            else
            {
                // interpolated vertex normal
                p1 = Normals[Triangles[triangleIndex]];
                p2 = Normals[Triangles[triangleIndex + 1]];
                p3 = Normals[Triangles[triangleIndex + 2]];
                hit.normal = p1 * bc.x + p2 * bc.y + p3 * bc.z;
            }
        }

        private float[] CalculateSurfaceAreas(out float totalSurfaceArea)
        {
            var idx = 0;
            totalSurfaceArea = 0.0f;
            var surfaceAreas = new float[Triangles.Length / 3];
            for (var triangleIndex = 0; triangleIndex < Triangles.Length; triangleIndex += 3)
            {
                var p1 = Vertices[Triangles[triangleIndex]];
                var p2 = Vertices[Triangles[triangleIndex + 1]];
                var p3 = Vertices[Triangles[triangleIndex + 2]];

                // http://www.wikihow.com/Sample/Area-of-a-Triangle-Side-Length
                var a = (p1 - p2).sqrMagnitude;
                var b = (p1 - p3).sqrMagnitude;
                var c = (p2 - p3).sqrMagnitude;

                // faster with only 1 square root: http://www.iquilezles.org/blog/?p=1579
                // A² = (2ab + 2bc + 2ca – a² – b² – c²)/16
                var areaSquared = (2.0f * a * b + 2.0f * b * c + 2.0f * c * a - a * a - b * b - c * c) / 16.0f;
                var area = PathGenerator.SquareRoot(areaSquared);
                surfaceAreas[idx++] = area;
                totalSurfaceArea += area;
            }

            return surfaceAreas;
        }

        private void CalculateNormalizedAreaWeights()
        {
            // create a sorted array of normalized area weights - this is an aggregate and is easily binary searched with a random value between 0 and 1 to find
            // a random triangle. Larger triangles have bigger gaps in the array.
            float totalSurfaceArea;
            normalizedAreaWeights = CalculateSurfaceAreas(out totalSurfaceArea);
            if (normalizedAreaWeights.Length == 0) return;
            float normalizedArea;
            var normalizedAggregate = 0.0f;
            for (var i = 0; i < normalizedAreaWeights.Length; i++)
            {
                normalizedArea = normalizedAreaWeights[i] / totalSurfaceArea;
                normalizedAreaWeights[i] = normalizedAggregate;
                normalizedAggregate += normalizedArea;
            }
        }

        private int SelectRandomTriangle()
        {
            var randomValue = Random.value;
            var imin = 0;
            var imax = normalizedAreaWeights.Length - 1;
            while (imin < imax)
            {
                var imid = (imin + imax) / 2;
                if (normalizedAreaWeights[imid] < randomValue)
                    imin = imid + 1;
                else
                    imax = imid;
            }

            return imin * 3;
        }

        private Vector3 GenerateRandomBarycentricCoordinates()
        {
            var barycentric = new Vector3(Random.Range(Mathf.Epsilon, 1.0f), Random.Range(Mathf.Epsilon, 1.0f),
                Random.Range(Mathf.Epsilon, 1.0f));

            // normalize the barycentric coordinates. These are normalized such that x + y + z = 1, as opposed to
            // normal vectors which are normalized such that Sqrt(x^2 + y^2 + z^2) = 1. See:
            // http://en.wikipedia.org/wiki/Barycentric_coordinate_system

            return barycentric / (barycentric.x + barycentric.y + barycentric.z);
        }
    }
}