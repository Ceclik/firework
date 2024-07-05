using System;
using System.Globalization;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using UnityEngine;
using Utils;

public class Debug
{
    private static readonly string _path = @"F:\Fireworks\Fireworks\Assets\error.txt";
    public static Debug Instance { get; }

    public static void Write(string line)
    {
        if (!File.Exists(_path)) File.Create(_path).Close();

        using var sw = new StreamWriter(File.Open(_path, FileMode.Append));
        sw.WriteLine("[" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "] " + line);
    }
}

namespace Assets.Scripts.Tracking
{
    public class EmguCamera : Singleton<EmguCamera>
    {
        private VideoCapture _capture;
        private bool _runned; //runned camera or not, for prevent onEnable execution after scene change
        private CamSettings _settings;
        private Mat _webcamMat;
        public EventHandler<Mat> ProcessFrame;

        public int Width => _capture.Width;
        public int Height => _capture.Height;

        public override void Awake()
        {
            base.Awake();
            Initialize(CamSettings.Default());
        }

        private void OnEnable()
        {
            if (!_runned)
            {
                _capture.Start();
                _capture.ImageGrabbed += OnProcessFrame;
                _runned = true;
            }
        }

        private void OnDisable()
        {
            if (_runned)
            {
                _capture.ImageGrabbed -= OnProcessFrame;
                _capture.Stop();
                UnityEngine.Debug.Log("Stopping camera capture system");
            }
        }

        private void OnApplicationQuit()
        {
            _capture.Dispose();
            OnDisable();
        }


        private void Initialize(CamSettings settings)
        {
            _capture = new VideoCapture();
            settings.LoadSettings();


            settings.ApplySettings(_capture);
            _settings = settings;
            _webcamMat = new Mat();
        }

        private void OnProcessFrame(object sender, EventArgs arg)
        {
            try
            {
                if (_capture != null && _capture.Ptr != IntPtr.Zero)
                {
                    _capture.Retrieve(_webcamMat);
                    if (ProcessFrame != null) ProcessFrame.Invoke(this, _webcamMat);
                }
            }
            catch (Exception exc)
            {
                Debug.Write("ERROR" + exc.Message);
            }
        }

        public Vector2Int ScreenToCamera(Vector2 screenCoords, Canvas canvas)
        {
            var screen = Vector2Int.RoundToInt(screenCoords);
            return new Vector2Int
            {
                x = (int)(screenCoords.x / canvas.pixelRect.width * Width),
                y = (int)(screenCoords.y / canvas.pixelRect.height * Height)
            };
        }

        public Vector2 CameraToScreen(Vector2Int camCoords, Canvas canvas)
        {
            return new Vector2
            {
                x = camCoords.x / (float)Width * canvas.pixelRect.width,
                y = camCoords.y / (float)Height * canvas.pixelRect.height
            };
        }

        public Vector2 CameraToScreen(Vector2 camCoords, Canvas canvas)
        {
            return new Vector2
            {
                x = camCoords.x / Width * canvas.pixelRect.width,
                y = camCoords.y / Height * canvas.pixelRect.height
            };
        }

        #region CameraSettings

        public bool IsCalibrated => _settings.isCalibrated;

        public void SaveCurrentSettings()
        {
            _settings.SaveSettings();
        }

        public void CancelSettings()
        {
            //OnDisable();         
            _capture.ImageGrabbed -= OnProcessFrame;
            _capture.Stop();
            _settings.LoadSettings();
            _settings.ApplySettings(_capture);
            _capture.Start();
            _capture.ImageGrabbed += OnProcessFrame;
            //OnEnable();            
        }

        public void SetExposure(double exposure)
        {
            _settings.exposure = exposure;
            _capture.SetCaptureProperty(CapProp.Exposure, exposure);
        }

        public void SetSaturation(double saturation)
        {
            _settings.saturation = saturation;
            _capture.SetCaptureProperty(CapProp.Saturation, saturation);
        }

        public void SetFocus(double focus)
        {
            _settings.focus = focus;
            _capture.SetCaptureProperty(CapProp.Focus, focus);
        }

        public double GetExposure()
        {
            return _settings.exposure;
        }

        public double GetSaturation()
        {
            return _settings.saturation;
        }

        public double GetFocus()
        {
            return _settings.focus;
        }

        #endregion

        #region HueSetting

        public double HMin
        {
            get => _settings.Hmin;
            set => _settings.Hmin = value;
        }

        public double HMax
        {
            get => _settings.Hmax;
            set => _settings.Hmax = value;
        }

        public double SMin
        {
            get => _settings.Smin;
            set => _settings.Smin = value;
        }

        public double SMax
        {
            get => _settings.Smax;
            set => _settings.Smax = value;
        }

        public double VMin
        {
            get => _settings.Vmin;
            set => _settings.Vmin = value;
        }

        public double VMax
        {
            get => _settings.Vmax;
            set => _settings.Vmax = value;
        }

        #endregion

        #region Crop Rect Settings

        public Vector2Int LeftTop
        {
            get => new((int)_settings.LeftTopX, (int)_settings.LeftTopY);
            set
            {
                _settings.LeftTopX = value.x;
                _settings.LeftTopY = value.y;
            }
        }

        public Vector2Int LeftBottom
        {
            get => new((int)_settings.LeftBottomX, (int)_settings.LeftBottomY);
            set
            {
                _settings.LeftBottomX = value.x;
                _settings.LeftBottomY = value.y;
            }
        }

        public Vector2Int RightTop
        {
            get => new((int)_settings.RightTopX, (int)_settings.RightTopY);
            set
            {
                _settings.RightTopX = value.x;
                _settings.RightTopY = value.y;
            }
        }

        public Vector2Int RightBottom
        {
            get => new((int)_settings.RightBottomX, (int)_settings.RightBottomY);
            set
            {
                _settings.RightBottomX = value.x;
                _settings.RightBottomY = value.y;
            }
        }

        #endregion
    }
}