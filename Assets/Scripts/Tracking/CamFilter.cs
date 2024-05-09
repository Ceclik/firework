using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using UnityEngine.UI;
using Emgu.CV.Util;
using System.Drawing;

public class CamFilter : MonoBehaviour
{

    private Emgu.CV.VideoCapture _capture = null;
    private bool _captureInProgress;

    private bool drawCircles;

    #region Materials
    private Mat _frame = new Mat();
    private Mat _convertedFrame = new Mat();

    VectorOfMat _hsvChannels = new VectorOfMat();
    VectorOfMat _channels = new VectorOfMat();
    Mat _redImage = new Mat();
    Mat _greenImage = new Mat();
    Mat _blueImage = new Mat();
    Mat _edges = new Mat();

    Mat _hsv = new Mat();

    Mat _hue = new Mat();
    Mat _value = new Mat();
    Mat _sat = new Mat();

    Mat _hueT = new Mat();
    Mat _valueT = new Mat();
    Mat _satT = new Mat();

    Mat _rgb = new Mat();

    Mat _crop = new Mat();

    Mat _pyrDown = new Mat();

    #endregion    

    #region Thresold Values

    private byte HMin = 0;
    private byte HMax = 255;
    private byte SMin = 0;
    private byte SMax = 255;
    private byte VMin = 0;
    private byte VMax = 255;

    #endregion

    public CircleF[] circles;

    private Texture2D tex;
    private Texture2D texCrop;
    private Texture2D texPreview;
    private Byte[] rawImage;
    private Byte[] rawImagePreview;
    private Byte[] cropPreview;
    public RawImage filteredImage;
    public RawImage previewImage;
    public RawImage cropImage;


    #region ThresoldSetup
    public bool SetHMin(byte hMin)
    {
        if (hMin <= HMax)
        {
            HMin = hMin;
            return true;
        }
        return false;
    }
    public bool SetHMax(byte hMax)
    {
        if (hMax >= HMin)
        {
            HMax = hMax;
        }
        return false;
    }
    public bool SetSMin(byte sMin)
    {
        if (sMin <= SMax)
        {
            SMin = sMin;
        }
        return false;
    }
    public bool SetSMax(byte sMax)
    {
        if (sMax >= SMin)
        {
            SMax = sMax;
        }
        return false;
    }
    public bool SetVMin(byte vMin)
    {
        if (vMin <= VMax)
        {
            VMin = vMin;
        }
        return false;
    }
    public bool SetVMax(byte vMax)
    {
        if (vMax >= VMin)
        {
            VMax = vMax;
        }
        return false;
    }
    #endregion


    public void SwitchCircles(bool isOn)
    {
        drawCircles = isOn;
    }
    // Use this for initialization
    void Start()
    {
        CvInvoke.UseOpenCL = false;
        try
        {
            _capture = new VideoCapture();

            _capture.ImageGrabbed += ProcessFrame;
            _capture.FlipVertical = true;
            _capture.SetCaptureProperty(CapProp.Exposure, -3);
            _capture.SetCaptureProperty(CapProp.FrameWidth, 1280);
            _capture.SetCaptureProperty(CapProp.FrameHeight, 720);
            _capture.SetCaptureProperty(CapProp.Fps, 60);
            _capture.SetCaptureProperty(CapProp.Autofocus, 5);
            _capture.SetCaptureProperty(CapProp.WhiteBalanceRedV, -1);
            _capture.SetCaptureProperty(CapProp.Focus, 1);
            Debug.Log(_capture.CaptureSource);
            Debug.Log(_capture.Width);
            Debug.Log(_capture.Height);
            Debug.Log("autofocus:" + _capture.GetCaptureProperty(CapProp.Autofocus));
        }
        catch (NullReferenceException excpt)
        {
            Debug.Log(excpt.Message);
        }
        _capture.Start(null);
        tex = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);
        texPreview = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);
        texCrop = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);

        cropImage.texture = texCrop;
        filteredImage.texture = tex;
        previewImage.texture = texPreview;
    }

    private void OnApplicationQuit()
    {
        _capture.ImageGrabbed -= ProcessFrame;
        _capture.Stop();
        _capture.Dispose();
    }

    private void OnDisable()
    {
        _capture.ImageGrabbed -= ProcessFrame;
        _capture.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (rawImage != null)
        {
            tex.LoadRawTextureData(rawImage);
            tex.Apply();            
        }
        if (rawImagePreview !=null)
        {
            texPreview.LoadRawTextureData(rawImagePreview);
            texPreview.Apply();
        }
        if (cropPreview !=null)
        {
            texCrop.LoadRawTextureData(cropPreview);
            texCrop.Apply();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(_capture.GetCaptureProperty(CapProp.Focus));
        }
    }
    private Mat BgrToRgb(Mat bgr)
    {
        CvInvoke.CvtColor(bgr, _rgb, ColorConversion.Bgr2Rgb);
        return _rgb;
    }

    private Mat BgrToHSV(Mat bgr)
    {
        CvInvoke.CvtColor(bgr, _hsv, ColorConversion.Bgr2Hsv);
        return _hsv;
    }

    private void SplitHSV()
    {
        CvInvoke.Split(_hsv, _hsvChannels);
        _hue = _hsvChannels[0];
        _sat = _hsvChannels[1];
        _value = _hsvChannels[2];
    }

    private Mat MergeHSV()
    {
        CvInvoke.Merge(new VectorOfMat(_hue, _sat, _value), _hsv);
        return _hsv;
    }

    private Mat HsvToRgb(Mat hsv)
    {
        CvInvoke.CvtColor(hsv, _rgb, ColorConversion.Hsv2Rgb);
        return _rgb;
    }

    private Mat HsvThreshold()
    {
        //CvInvoke.Threshold(_hue, _hueT, HMin, HMax, ThresholdType.Binary);
        //CvInvoke.Threshold(_value, _valueT, VMin, VMax, ThresholdType.Binary);
        //CvInvoke.Threshold(_sat, _satT, SMin, SMax, ThresholdType.Binary);

        CvInvoke.InRange(_hue, new ScalarArray(new MCvScalar(HMin, HMin, HMin)),
                           new ScalarArray(new MCvScalar(HMax, HMax, HMax)), _hueT);

        CvInvoke.InRange(_sat, new ScalarArray(new MCvScalar(SMin, SMin, SMin)),
                           new ScalarArray(new MCvScalar(SMax, SMax, SMax)), _satT);

        CvInvoke.InRange(_value, new ScalarArray(new MCvScalar(VMin, VMin, VMin)),
                           new ScalarArray(new MCvScalar(VMax, VMax, VMax)), _valueT);

        CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _hsv);        
        //CvInvoke.CvtColor(_hsv, _rgb, ColorConversion.Hsv2Rgb);
        return _hsv;
    }

    private Mat CropWhite()
    {
        CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                            new ScalarArray(new MCvScalar(255, 255, 255)),_hueT);
        CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                            new ScalarArray(new MCvScalar(255, 255, 255)), _satT);
        CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                            new ScalarArray(new MCvScalar(255, 255, 255)), _valueT);
        CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _crop);
        return _crop;
    }

    private CircleF[] FindCircles(Mat image)        
    {             
        double cannyThreshold = 200.0;
        double circleAccumulatorThreshold = 2;
        Debug.Log("look for circles");                       

        CircleF[] circles = CvInvoke.HoughCircles(image, HoughType.Gradient, 1.0, 50.0, cannyThreshold, circleAccumulatorThreshold, 1,15);
        Debug.Log("circles founded:" + circles.Length);
        return circles;
    }    

    private Mat SmoothImage(Mat image)
    {
        CvInvoke.PyrDown(image, _pyrDown);
        return _pyrDown;
    }

    private Mat DrawCircles(Mat image)
    {
        int maxDrawed = 10;
        int drawed = 0;
        if (circles!=null)
            foreach (var circle in circles)
            {
                CvInvoke.Circle(image, Point.Round(circle.Center), (int)circle.Radius, new MCvScalar(200, 0, 0),2);                
                drawed++;
                if (drawed > maxDrawed) break;
            }
        return image;
    }

    private void ProcessFrame(object sender, EventArgs arg)
    {
        //_capture.Retrieve(currentFrameBgra);
        if (_capture != null && _capture.Ptr != IntPtr.Zero)
        {
            //_frame = _capture.QueryFrame();
            _capture.Retrieve(_frame, 0);
            if (_frame != null)
            {
                CvInvoke.CvtColor(_frame, _hsv, ColorConversion.Bgr2Hsv);//BBG to HSV

                CvInvoke.Split(_hsv, _hsvChannels); //Split HSV
                _hue = _hsvChannels[0];
                _sat = _hsvChannels[1];
                _value = _hsvChannels[2];

                CvInvoke.InRange(_hue, new ScalarArray(new MCvScalar(HMin, HMin, HMin)), //HSV Thresold
                           new ScalarArray(new MCvScalar(HMax, HMax, HMax)), _hueT);

                CvInvoke.InRange(_sat, new ScalarArray(new MCvScalar(SMin, SMin, SMin)),
                                   new ScalarArray(new MCvScalar(SMax, SMax, SMax)), _satT);

                CvInvoke.InRange(_value, new ScalarArray(new MCvScalar(VMin, VMin, VMin)),
                                   new ScalarArray(new MCvScalar(VMax, VMax, VMax)), _valueT);

                CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _hsv);

                rawImage = _hsv.GetData();//Images assignment
                rawImagePreview = BgrToRgb(_frame).GetData();

                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),//Extracting white values
                            new ScalarArray(new MCvScalar(255, 255, 255)), _hueT);
                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                                    new ScalarArray(new MCvScalar(255, 255, 255)), _satT);
                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                                    new ScalarArray(new MCvScalar(255, 255, 255)), _valueT);
                CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _crop);

                Mat grayImage = new Mat();//Convert Image to gray and smooth result
                CvInvoke.CvtColor(_crop, grayImage, ColorConversion.Bgr2Gray);

                //use image pyr to remove noise
                Mat pyrDown = new Mat();                
                CvInvoke.PyrDown(grayImage, pyrDown);
                CvInvoke.PyrUp(pyrDown, grayImage);

                CvInvoke.GaussianBlur(grayImage, pyrDown,new Size(7,9),6);

                //Contrast Up

                CvInvoke.EqualizeHist(pyrDown, grayImage);
                                               

                if (drawCircles)
                {                    
                    circles = FindCircles(grayImage);
                    cropPreview = DrawCircles(_rgb).GetData();
                }
                else
                {
                    //Convert Gray Image to Color
                    CvInvoke.CvtColor(grayImage, _rgb, ColorConversion.Gray2Rgb);
                    cropPreview = _rgb.GetData();
                }
            }
        }
    }    
}
