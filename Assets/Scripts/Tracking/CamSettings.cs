using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CamSettings
{
    public bool flipVertical;
    public bool isCalibrated;
    public double exposure;
    public double frameWidth;
    public double frameHeight;
    public double fps;
    public double autofocus;
    public double focus;
    public double whitebalance;
    public double saturation;

    //hsv threshold settings
    public double Hmin;
    public double Hmax;
    public double Smin;
    public double Smax;
    public double Vmin;
    public double Vmax;

    //Crop Rectangle Settings
    public double LeftTopX;
    public double LeftTopY;
    public double LeftBottomX;
    public double LeftBottomY;
    public double RightTopX;
    public double RightTopY;
    public double RightBottomX;
    public double RightBottomY;

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
        Type structType = typeof(CamSettings);
        FieldInfo[] fields = structType.GetFields();
        //Debug.Log("Saving camera settings:");
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
            {                
                var value = (float)((double)field.GetValue(this));
                //Debug.Log("Field name:" + field.Name+ ", Field value:" + value.ToString());                
                PlayerPrefs.SetFloat(field.Name, value);
            }
            if (field.FieldType == typeof(bool))
            {
                //Debug.Log("Field name:" + field.Name + ", Field value:" + ((bool)field.GetValue(this)).ToString());                
                PlayerPrefs.SetString(field.Name,((bool)field.GetValue(this)).ToString());
            }
        }
    }

    public void LoadSettings()
    {
        Type structType = typeof(CamSettings);
        FieldInfo[] fields = structType.GetFields();
        //Debug.Log("Loading camera settings:");
        
        for (int i=0;i<fields.Length;i++)
        {
            var field = fields[i];
            if (PlayerPrefs.HasKey(field.Name))
            {
                if (field.FieldType == typeof(double) || field.FieldType == typeof(float))
                {
                    var value = PlayerPrefs.GetFloat(field.Name);
                    //Debug.Log("Field name:" + field.Name + ", Field value:" + value.ToString());
                    field.SetValue(this,(double) value);                    
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
        if (autofocus != -1)
        {
            capture.SetCaptureProperty(CapProp.Focus, focus);
        }
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