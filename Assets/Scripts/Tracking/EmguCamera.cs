using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Debog
{
    static Debog _instance;
    public static Debog Instance { get { return _instance; } }

    static string path = @"F:\Fireworks\Fireworks\Assets\error.txt";
    static public void Write(string line)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        using(StreamWriter sw = new StreamWriter(File.Open(path, FileMode.Append)))
        {
            sw.WriteLine("[" + DateTime.Now.ToString() + "] " + line);
        }
    }
}

namespace Assets.Scripts.Tracking
{
    public class EmguCamera : Singleton<EmguCamera>
    {
        public EventHandler<Mat> ProcessFrame;

        private VideoCapture _capture;
        private Mat _webcamMat;
        private CamSettings _settings;
        private bool _runned;//runned camera or not, for prevent onEnable execution after scene change

        public int Width
        {
            get
            {
                return _capture.Width;
            }
        }
        public int Height
        {
            get
            {
                return _capture.Height;
            }
        }

        #region CameraSettings

        public bool IsCalibrated
        {
            get { return _settings.isCalibrated; }
        }

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
            get { return _settings.Hmin; }
            set { _settings.Hmin = value; }
        }
        public double HMax
        {
            get { return _settings.Hmax; }
            set { _settings.Hmax = value; }
        }
        public double SMin
        {
            get { return _settings.Smin; }
            set { _settings.Smin = value; }
        }
        public double SMax
        {
            get { return _settings.Smax; }
            set { _settings.Smax = value; }
        }
        public double VMin
        {
            get { return _settings.Vmin; }
            set { _settings.Vmin = value; }
        }
        public double VMax
        {
            get { return _settings.Vmax; }
            set { _settings.Vmax = value; }
        }


        #endregion

        #region Crop Rect Settings

        public Vector2Int LeftTop
        {
            get { return new Vector2Int((int)_settings.LeftTopX, (int)_settings.LeftTopY); }
            set { _settings.LeftTopX = value.x; _settings.LeftTopY = value.y; }
        }

        public Vector2Int LeftBottom
        {
            get { return new Vector2Int((int)_settings.LeftBottomX, (int)_settings.LeftBottomY); }
            set { _settings.LeftBottomX = value.x; _settings.LeftBottomY = value.y; }
        }

        public Vector2Int RightTop
        {
            get { return new Vector2Int((int)_settings.RightTopX, (int)_settings.RightTopY); }
            set { _settings.RightTopX = value.x; _settings.RightTopY = value.y; }
        }

        public Vector2Int RightBottom
        {
            get { return new Vector2Int((int)_settings.RightBottomX, (int)_settings.RightBottomY); }
            set { _settings.RightBottomX = value.x; _settings.RightBottomY = value.y; }
        }

        #endregion
        public override void Awake()
        {
            base.Awake();
            Initialize(CamSettings.Default());
        }

        /// <summary>
        /// Initialize webcamera input
        /// </summary>
        /// <param name="settings"></param>
        public void Initialize(CamSettings settings)
        {
            _capture = new VideoCapture();
            settings.LoadSettings();            


            settings.ApplySettings(_capture);
            _settings = settings;
            _webcamMat = new Mat();
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
                Debug.Log("Stopping camera capture system");
            }
        }

        private void OnApplicationQuit()
        {
            OnDisable();
            _capture.Dispose();
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
            catch(Exception exc)
            {
                Debog.Write("ERROR" + exc.Message);
            }
        }

        public Vector2Int ScreenToCamera(Vector2 screenCoords, Canvas canvas)
        {
            Vector2Int screen = Vector2Int.RoundToInt(screenCoords);
            return new Vector2Int
            {
                x = (int)((screenCoords.x / canvas.pixelRect.width) * (float)Width),
                y = (int)((screenCoords.y / canvas.pixelRect.height) * (float)Height)
            };            
        }

        public Vector2 CameraToScreen(Vector2Int camCoords, Canvas canvas)
        {
            return new Vector2
            {
                x = (float)((camCoords.x / (float)Width) * canvas.pixelRect.width),
                y = (float)((camCoords.y / (float)Height) * canvas.pixelRect.height)
            };            
        }
        public Vector2 CameraToScreen(Vector2 camCoords, Canvas canvas)
        {
            return new Vector2
            {
                x = (float)((camCoords.x / (float)Width) * canvas.pixelRect.width),
                y = (float)((camCoords.y / (float)Height) * canvas.pixelRect.height)
            };
        }
    }
}
