using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using UnityEngine;
using UnityEngine.UI;

public class CamFilter : MonoBehaviour
{
    public CircleF[] circles;
    public RawImage filteredImage;
    public RawImage previewImage;
    public RawImage cropImage;

    private VideoCapture _capture;
    private bool _captureInProgress;
    private byte[] cropPreview;

    private bool drawCircles;
    private byte[] rawImage;
    private byte[] rawImagePreview;

    private Texture2D tex;
    private Texture2D texCrop;

    private Texture2D texPreview;

    // Use this for initialization
    private void Start()
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
            UnityEngine.Debug.Log(_capture.CaptureSource);
            UnityEngine.Debug.Log(_capture.Width);
            UnityEngine.Debug.Log(_capture.Height);
            UnityEngine.Debug.Log("autofocus:" + _capture.GetCaptureProperty(CapProp.Autofocus));
        }
        catch (NullReferenceException excpt)
        {
            UnityEngine.Debug.Log(excpt.Message);
        }

        _capture.Start();
        tex = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);
        texPreview = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);
        texCrop = new Texture2D(_capture.Width, _capture.Height, TextureFormat.RGB24, false, true);

        cropImage.texture = texCrop;
        filteredImage.texture = tex;
        previewImage.texture = texPreview;
    }

    // Update is called once per frame
    private void Update()
    {
        if (rawImage != null)
        {
            tex.LoadRawTextureData(rawImage);
            tex.Apply();
        }

        if (rawImagePreview != null)
        {
            texPreview.LoadRawTextureData(rawImagePreview);
            texPreview.Apply();
        }

        if (cropPreview != null)
        {
            texCrop.LoadRawTextureData(cropPreview);
            texCrop.Apply();
        }

        if (Input.GetKeyDown(KeyCode.A)) UnityEngine.Debug.Log(_capture.GetCaptureProperty(CapProp.Focus));
    }

    private void OnDisable()
    {
        _capture.ImageGrabbed -= ProcessFrame;
        _capture.Stop();
    }

    private void OnApplicationQuit()
    {
        _capture.ImageGrabbed -= ProcessFrame;
        _capture.Stop();
        _capture.Dispose();
    }


    public void SwitchCircles(bool isOn)
    {
        drawCircles = isOn;
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
            new ScalarArray(new MCvScalar(255, 255, 255)), _hueT);
        CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
            new ScalarArray(new MCvScalar(255, 255, 255)), _satT);
        CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
            new ScalarArray(new MCvScalar(255, 255, 255)), _valueT);
        CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _crop);
        return _crop;
    }

    private CircleF[] FindCircles(Mat image)
    {
        var cannyThreshold = 200.0;
        double circleAccumulatorThreshold = 2;
        UnityEngine.Debug.Log("look for circles");

        var circles = CvInvoke.HoughCircles(image, HoughType.Gradient, 1.0, 50.0, cannyThreshold,
            circleAccumulatorThreshold, 1, 15);
        UnityEngine.Debug.Log("circles founded:" + circles.Length);
        return circles;
    }

    private Mat SmoothImage(Mat image)
    {
        CvInvoke.PyrDown(image, _pyrDown);
        return _pyrDown;
    }

    private Mat DrawCircles(Mat image)
    {
        var maxDrawed = 10;
        var drawed = 0;
        if (circles != null)
            foreach (var circle in circles)
            {
                CvInvoke.Circle(image, Point.Round(circle.Center), (int)circle.Radius, new MCvScalar(200, 0, 0), 2);
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
            _capture.Retrieve(_frame);
            if (_frame != null)
            {
                CvInvoke.CvtColor(_frame, _hsv, ColorConversion.Bgr2Hsv); //BBG to HSV

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

                rawImage = _hsv.GetData(); //Images assignment
                rawImagePreview = BgrToRgb(_frame).GetData();

                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)), //Extracting white values
                    new ScalarArray(new MCvScalar(255, 255, 255)), _hueT);
                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                    new ScalarArray(new MCvScalar(255, 255, 255)), _satT);
                CvInvoke.InRange(_hsv, new ScalarArray(new MCvScalar(250, 250, 250)),
                    new ScalarArray(new MCvScalar(255, 255, 255)), _valueT);
                CvInvoke.Merge(new VectorOfMat(_hueT, _satT, _valueT), _crop);

                var grayImage = new Mat(); //Convert Image to gray and smooth result
                CvInvoke.CvtColor(_crop, grayImage, ColorConversion.Bgr2Gray);

                //use image pyr to remove noise
                var pyrDown = new Mat();
                CvInvoke.PyrDown(grayImage, pyrDown);
                CvInvoke.PyrUp(pyrDown, grayImage);

                CvInvoke.GaussianBlur(grayImage, pyrDown, new Size(7, 9), 6);

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

    #region Materials

    private readonly Mat _frame = new();
    private Mat _convertedFrame = new();

    private readonly VectorOfMat _hsvChannels = new();
    private VectorOfMat _channels = new();
    private Mat _redImage = new();
    private Mat _greenImage = new();
    private Mat _blueImage = new();
    private Mat _edges = new();

    private readonly Mat _hsv = new();

    private Mat _hue = new();
    private Mat _value = new();
    private Mat _sat = new();

    private readonly Mat _hueT = new();
    private readonly Mat _valueT = new();
    private readonly Mat _satT = new();

    private readonly Mat _rgb = new();

    private readonly Mat _crop = new();

    private readonly Mat _pyrDown = new();

    #endregion

    #region Thresold Values

    private byte HMin;
    private byte HMax = 255;
    private byte SMin;
    private byte SMax = 255;
    private byte VMin;
    private byte VMax = 255;

    #endregion


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
        if (hMax >= HMin) HMax = hMax;
        return false;
    }

    public bool SetSMin(byte sMin)
    {
        if (sMin <= SMax) SMin = sMin;
        return false;
    }

    public bool SetSMax(byte sMax)
    {
        if (sMax >= SMin) SMax = sMax;
        return false;
    }

    public bool SetVMin(byte vMin)
    {
        if (vMin <= VMax) VMin = vMin;
        return false;
    }

    public bool SetVMax(byte vMax)
    {
        if (vMax >= VMin) VMax = vMax;
        return false;
    }

    #endregion
}