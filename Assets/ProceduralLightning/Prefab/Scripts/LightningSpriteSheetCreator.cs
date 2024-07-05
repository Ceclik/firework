//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
#if UNITY_EDITOR && !UNITY_WEBPLAYER

    public class LightningSpriteSheetCreator : MonoBehaviour
    {
        [Tooltip("The width of the spritesheet in pixels")]
        public int Width = 512;

        [Tooltip("The height of the spritesheet in pixels")]
        public int Height = 512;

        [Tooltip("The number of rows in the spritesheet")]
        public int Rows = 5;

        [Tooltip("The number of columns in the spritesheet")]
        public int Columns = 5;

        [Tooltip("Delay in between frames to export. Gives time for animations and effects to happen.")]
        public float FrameDelay = 0.1f;

        [Tooltip("Background color for the sprite sheet. Usually black or transparent.")]
        public Color BackgroundColor = Color.black;

        [Tooltip("The full path and file name to save the saved sprite sheet to")]
        public string SaveFileName;

        [Tooltip("The label to notify that the export is working and then completed")]
        public Text ExportCompleteLabel;

        private bool exporting;
        private RenderTexture renderTexture;

        private void Start()
        {
        }

        private void Update()
        {
        }

        private IEnumerator ExportFrame(int row, int column, float delay)
        {
            yield return new WaitForSeconds(delay);

            var cellWidth = Width / (float)Columns;
            var cellHeight = Height / (float)Rows;
            var x = column * cellWidth / Width;
            var y = row * cellHeight / Height;
            var w = cellWidth / Width;
            var h = cellHeight / Height;

            Camera.main.clearFlags = CameraClearFlags.Nothing;
            Camera.main.targetTexture = renderTexture;
            var existingViewportRect = Camera.main.rect;
            Camera.main.rect = new Rect(x, 1.0f - y - h, w, h);
            Camera.main.Render();
            Camera.main.rect = existingViewportRect;
            Camera.main.targetTexture = null;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        private IEnumerator PerformExport(float delay)
        {
            yield return new WaitForSeconds(delay);

            RenderTexture.active = renderTexture;
            var png = new Texture2D(Width, Height, TextureFormat.ARGB32, false, false);
            png.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            RenderTexture.active = null;
            var pngBytes = png.EncodeToPNG();
            if (string.IsNullOrEmpty(SaveFileName))
                SaveFileName = Path.Combine(Application.persistentDataPath, "LightningSpriteSheet.png");
            File.WriteAllBytes(SaveFileName, pngBytes);
            ExportCompleteLabel.text = "Done!";
            exporting = false;
            renderTexture = null;
        }

        public void ExportTapped()
        {
            if (exporting) return;

            exporting = true;
            ExportCompleteLabel.text = "Processing...";
            renderTexture = new RenderTexture(Width, Height, 0, RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Default);
            renderTexture.anisoLevel = 4;
            renderTexture.antiAliasing = 4;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, BackgroundColor, 0.0f);
            RenderTexture.active = null;

            var delay = FrameDelay * 0.5f;
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
            {
                StartCoroutine(ExportFrame(i, j, delay));
                delay += FrameDelay;
            }

            StartCoroutine(PerformExport(delay));
        }
    }

#endif
}