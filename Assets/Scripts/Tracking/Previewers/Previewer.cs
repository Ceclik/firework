using Assets.Scripts.Tracking;
using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using UnityEngine;
using UnityEngine.UI;

namespace Tracking.Previewers
{
    [RequireComponent(typeof(RawImage))]
    public abstract class Previewer : MonoBehaviour
    {
        private EmguCamera _camera;
        private byte[] _data;
        protected ICamFilter[] Filters;
        private RawImage _previewImage;

        private Texture2D _texture;

        private bool _updated; //need to update preview texture;        

        private void Update()
        {
            if (_updated)
            {
                _texture.LoadRawTextureData(_data);
                _texture.Apply();
                _updated = false;
            }
        }

        private void OnEnable()
        {
            _previewImage = GetComponent<RawImage>();
            _camera = EmguCamera.Instance;
            if (_texture == null)
                _texture = new Texture2D(_camera.Width, _camera.Height, TextureFormat.RGB24, false, true);
            _previewImage.texture = _texture;
            _camera.ProcessFrame += ProcessFrame;
        }

        private void OnDisable()
        {
            _camera.ProcessFrame -= ProcessFrame;
        }

        private void ProcessFrame(object sender, Mat frame)
        {
            var currentFrame = frame;
            foreach (var filter in Filters) currentFrame = filter.ProcessFrame(currentFrame);
            _data = currentFrame.GetData();
            _updated = true;
        }
    }
}