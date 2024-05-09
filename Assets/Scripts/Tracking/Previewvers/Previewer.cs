using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tracking.Previewvers
{
    [RequireComponent (typeof (RawImage))]
    public abstract class Previewer:MonoBehaviour
    {
        private RawImage _previewImage;

        private Texture2D _texture;
        protected ICamFilter[] _filters;
        private EmguCamera _camera;
        private Byte[] _data;

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
            if (_texture==null)
            {
                _texture = new Texture2D(_camera.Width, _camera.Height, TextureFormat.RGB24, false, true);
            }
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
            foreach (var filter in _filters)
            {
                currentFrame = filter.ProcessFrame(currentFrame);
            }
            _data = currentFrame.GetData();
            _updated = true;
        }

        
    }
}
