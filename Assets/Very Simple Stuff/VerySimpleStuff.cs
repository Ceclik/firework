#pragma warning disable 0649
using System;
using System.Collections.Generic;
using UnityEngine;

public class VerySimpleStuff : MonoBehaviour
{
    [SerializeField] private Vector2 tileSize;
    [SerializeField] private Row[] rows;

// Whatever.  Just to test the Tile class.	
    [SerializeField] private float rotationSpeed;

    private Mesh mesh;
    private Color[] meshColors;
    private Vector3[] meshPositions;
    private Tile[][] tiles;

    private void Awake()
    {
        tiles = new Tile[rows.Length][];
        var halfTileSize = tileSize * .5F;
        // Adjust accordingly if you want the center other than at top-left...
        Vector3 tileCenter = new Vector2(halfTileSize.x, -halfTileSize.y);
        var tileVertIndeces = new List<int>(new[] { 0, 1, 2, 3 });

        var meshPositions = new List<Vector3>();
        var meshTriangles = new List<int>();
        var meshColors = new List<Color>();

        for (var rowIndex = 0; rowIndex < rows.Length; ++rowIndex)
        {
            tiles[rowIndex] = new Tile[rows[rowIndex].colors.Length];
            for (var columnIndex = 0; columnIndex < tiles[rowIndex].Length; ++columnIndex)
            {
                var tileColor = rows[rowIndex].colors[columnIndex];

                tiles[rowIndex][columnIndex] = new Tile(
                    tileCenter, tileSize, tileColor, tileVertIndeces.ToArray(),
                    ChangeMeshPositions, ChangeMeshColors
                );

                Vector3[] tileVertPositions =
                {
                    -halfTileSize, new Vector2(-halfTileSize.x, halfTileSize.y),
                    halfTileSize, new Vector2(halfTileSize.x, -halfTileSize.y)
                };
                for (var i = 0; i < 4; ++i) tileVertPositions[i] += tileCenter;
                meshPositions.AddRange(tileVertPositions);
                tileCenter.x += tileSize.x;

                int[] tileTriangles = { 0, 1, 2, 0, 2, 3 };
                for (var i = 0; i < 6; ++i) tileTriangles[i] += tileVertIndeces[0];
                meshTriangles.AddRange(tileTriangles);

                for (var i = 0; i < 4; ++i)
                {
                    meshColors.Add(tileColor);
                    tileVertIndeces[i] += 4;
                }
            }

            tileCenter = new Vector2(halfTileSize.x, tileCenter.y - tileSize.y);
        }

        rows = null;

        mesh = new Mesh();
        this.meshPositions = meshPositions.ToArray();
        mesh.vertices = this.meshPositions;
        this.meshColors = meshColors.ToArray();
        mesh.colors = this.meshColors;
        mesh.triangles = meshTriangles.ToArray();
        ;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        foreach (var row in tiles)
        foreach (var tile in row)
            tile.Rotation = Time.time * rotationSpeed;
        UpdateMeshPositions();

        if (Input.GetMouseButtonDown(0))
        {
            for (var rowIndex = 0; rowIndex < tiles.Length; ++rowIndex)
            {
                var leftColor = tiles[rowIndex][0].Color;
                for (var columnIndex = 0; columnIndex < tiles[rowIndex].Length - 1; ++columnIndex)
                    tiles[rowIndex][columnIndex].Color = tiles[rowIndex][columnIndex + 1].Color;
                tiles[rowIndex][tiles[rowIndex].Length - 1].Color = leftColor;
            }

            UpdateMeshColors();
        }
    }

    private void ChangeMeshPositions(int[] indeces, Vector3[] positions)
    {
        for (var i = 0; i < indeces.Length; ++i) meshPositions[indeces[i]] = positions[i];
    }

    private void ChangeMeshColors(int[] indeces, Color color)
    {
        foreach (var index in indeces) meshColors[index] = color;
    }

// Too expensive to upload the arrays to the GPU willy-nilly.
    public void UpdateMeshPositions()
    {
        mesh.vertices = meshPositions;
    }

    public void UpdateMeshColors()
    {
        mesh.colors = meshColors;
    }

    [Serializable]
    private class Tile
    {
        private Vector2 center, size;
        private Action<int[], Color> changeColor;
        private Color color;
        private int[] meshIndeces;
        private Action<int[], Vector3[]> rotate;

        public Tile(Vector2 center, Vector2 size, Color color, int[] meshIndeces,
            Action<int[], Vector3[]> rotate, Action<int[], Color> changeColor)
        {
            this.center = center;
            this.size = size;
            this.color = color;
            this.meshIndeces = meshIndeces;
            this.rotate = rotate;
            this.changeColor = changeColor;
        }

        public float Rotation
        {
            set
            {
                var halfSize = size * .5F;
                Vector3[] vertPositions =
                {
                    -halfSize, new Vector2(-halfSize.x, halfSize.y),
                    halfSize, new Vector2(halfSize.x, -halfSize.y)
                };
                var matrix = new RotationMatrix(value);
                for (var i = 0; i < 4; ++i)
                    vertPositions[i] = matrix * vertPositions[i] + center;
                rotate(meshIndeces, vertPositions);
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                changeColor(meshIndeces, value);
            }
        }

        private struct RotationMatrix
        {
            private readonly float cosine;
            private readonly float sine;

            public RotationMatrix(float rotation)
            {
                cosine = Mathf.Cos(rotation);
                sine = Mathf.Sin(rotation);
            }

            public static Vector2 operator *(RotationMatrix matrix, Vector2 vector)
            {
                return new Vector2(
                    matrix.cosine * vector.x - matrix.sine * vector.y,
                    matrix.sine * vector.x + matrix.cosine * vector.y
                );
            }
        }
    }

// Unity won't serialize jagged or multidimensional arrays.
    [Serializable]
    private class Row
    {
        public Color[] colors;
    }
}