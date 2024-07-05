using System;
using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Tracking.CamFilters;
using UnityEngine;

namespace Tracking
{
    public class CamSettings
    {
        public double autofocus;
        public double exposure;
        public bool flipVertical;
        public double focus;
        public double fps;
        public double frameHeight;
        public double frameWidth;
        public double Hmax;

        //hsv threshold settings
        public double Hmin;
        public bool isCalibrated;
        public double LeftBottomX;
        public double LeftBottomY;

        //Crop Rectangle Settings
        public double LeftTopX;
        public double LeftTopY;
        public double RightBottomX;
        public double RightBottomY;
        public double RightTopX;
        public double RightTopY;
        public double saturation;
        public double Smax;
        public double Smin;
        public double Vmax;
        public double Vmin;
        public double whitebalance;

        public static CamSettings Default()
        {
            return new CamSettings
            {
                isCalibrated = false,
                flipVertical = true,
                exposure = -6,
                frameWidth = 640,
                frameHeight = 480,
                fps = 60,
                autofocus = 5,
                whitebalance = -1,
                focus = 1,
                saturation = 128,
                Hmin = 0,
                Hmax = 255,
                Smin = 0,
                Smax = 255,
                Vmin = 0,
                Vmax = 255,
                LeftTopX = 50,
                LeftTopY = 300,
                LeftBottomX = 50,
                LeftBottomY = 50,
                RightTopX = 500,
                RightTopY = 300,
                RightBottomX = 500,
                RightBottomY = 50
            };
        }

        public void SaveSettings()
        {
            isCalibrated = true;
            var structType = typeof(CamSettings);
            var fields = structType.GetFields();
            //Debug.Log("Saving camera settings:");
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
                {
                    var value = (float)(double)field.GetValue(this);
                    //Debug.Log("Field name:" + field.Name+ ", Field value:" + value.ToString());                
                    PlayerPrefs.SetFloat(field.Name, value);
                }

                if (field.FieldType == typeof(bool))
                    //Debug.Log("Field name:" + field.Name + ", Field value:" + ((bool)field.GetValue(this)).ToString());                
                    PlayerPrefs.SetString(field.Name, ((bool)field.GetValue(this)).ToString());
            }
        }

        public void LoadSettings()
        {
            var structType = typeof(CamSettings);
            var fields = structType.GetFields();
            //Debug.Log("Loading camera settings:");

            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (PlayerPrefs.HasKey(field.Name))
                {
                    if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
                    {
                        var value = PlayerPrefs.GetFloat(field.Name);
                        //Debug.Log("Field name:" + field.Name + ", Field value:" + value.ToString());
                        field.SetValue(this, (double)value);
                    }

                    if (field.FieldType == typeof(bool))
                    {
                        var value = Convert.ToBoolean(PlayerPrefs.GetString(field.Name));
                        //Debug.Log("Field name:" + field.Name + ", Field value:" + value.ToString());
                        field.SetValue(this, value);
                    }
                }
            }
        }

        public void ApplySettings(VideoCapture capture)
        {
            capture.FlipVertical = flipVertical;
            capture.SetCaptureProperty(CapProp.FrameWidth, frameWidth);
            capture.SetCaptureProperty(CapProp.FrameHeight, frameHeight);
            capture.SetCaptureProperty(CapProp.Exposure, exposure);
            capture.SetCaptureProperty(CapProp.Fps, fps);
            capture.SetCaptureProperty(CapProp.Autofocus, autofocus);
            capture.SetCaptureProperty(CapProp.Saturation, saturation);
            capture.SetCaptureProperty(CapProp.WhiteBalanceBlueU, whitebalance);
            if (autofocus != -1) capture.SetCaptureProperty(CapProp.Focus, focus);
        }

        public void ApplySettings(HsvThresholdRgb filter)
        {
            filter.HMin = (float)Hmin;
            filter.HMax = (float)Hmax;
            filter.SMin = (float)Smin;
            filter.SMax = (float)Smax;
            filter.VMin = (float)Vmin;
            filter.VMax = (float)Vmax;
        }
    }
}