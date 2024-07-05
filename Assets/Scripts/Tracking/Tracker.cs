using System.Collections.Generic;
using System.Drawing;
using Assets.Scripts.Tracking;
using Emgu.CV;
using Emgu.CV.Cvb;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using UnityEngine;
using Utils;

namespace Tracking

//I think this script is using for tracking object from camera to navigate in-game cursor
{
    public class Tracker : Singleton<Tracker>
    {
        private static CvBlobDetector _blobDetector;
        private static CvTracks _tracker;

        public bool isTracked;
        public Vector2 cursorPosition;
        public Vector2 trackingPosition;
        private CircleF[] _circles;

        private EmguCamera _emgu;
        private Mat _homography;

        private bool _isEnabled;
        private bool _isOn;

        public override void Awake()
        {
            base.Awake();
            _emgu = EmguCamera.Instance;
        }

        public void Reset()
        {
            BuildMatrix();
            if (_emgu.IsCalibrated)
            {
                _isEnabled = true;
                _isOn = true;
            }
        }

        private void Update()
        {
            TrackCursor();
        }

        private void OnEnable()
        {
            if (_emgu != null)
            {
                _emgu.ProcessFrame += ProcessFrame;
                BuildMatrix();
                if (_emgu.IsCalibrated)
                {
                    _isEnabled = true;
                    _isOn = true;
                }
            }
        }

        private void OnDisable()
        {
            _emgu.ProcessFrame -= ProcessFrame;
        }

        private void BuildMatrix()
        {
            _blobDetector = new CvBlobDetector();
            _tracker = new CvTracks();

            float[,] sourcePoints =
            {
                { _emgu.LeftBottom.x, _emgu.LeftBottom.y }, { _emgu.LeftTop.x, _emgu.LeftTop.y },
                { _emgu.RightBottom.x, _emgu.RightBottom.y }, { _emgu.RightTop.x, _emgu.RightTop.y }
            };
            float[,] destPoints =
                { { 0, 0 }, { 0, _emgu.Height - 1 }, { _emgu.Width - 1, 0 }, { _emgu.Width - 1, _emgu.Height - 1 } };
            var sourceMat = new Matrix<float>(sourcePoints);
            var destMat = new Matrix<float>(destPoints);
            _homography = CvInvoke.FindHomography(sourceMat, destMat);
        }

        private void CreateCursor()
        {
        }

        public void TurnOn()
        {
            _isOn = true;
        }

        public void TurnOff()
        {
            _isOn = false;
        }

        private void FindCircles(Mat frame)
        {
            var blobs = new CvBlobs();
            _blobDetector.Detect(frame.ToImage<Gray, byte>(), blobs).ToString();
            blobs.FilterByArea(10, int.MaxValue);
            var scale = (frame.Width + frame.Width) / 5.0f;

            // Crash preventer
            foreach (var cb in blobs.Values)
                if (cb.Centroid.X > _emgu.Width - 10 || cb.Centroid.X < 10 || cb.Centroid.Y < 10 ||
                    cb.Centroid.Y > _emgu.Height - 10)
                    return;

            _tracker.Update(blobs, 0.01 * scale, 1, 1);


            var circ = new List<CircleF>();
            foreach (var pair in _tracker)
            {
                var b = pair.Value;
                circ.Add(new CircleF(new PointF((float)b.Centroid.X, (float)b.Centroid.Y), 5));
            }

            _circles = circ.ToArray();

            /*
                //var points = _detector.Detect(frame);
                //Debug.Log("poins count:" + points.Length);
                //var circ = new CircleF[points.Length];
                //for (int i = 0; i < points.Length; i++)
                //{
                //    circ[i].Center = points[i].Point;
                //    circ[i].Radius = points[i].Size;
                //    Debug.Log("finded blob: " + points[i].Point);
                //}
                //_circles = circ;


                //double cannyThreshold = 100.0;
                //double circleAccumulatorThreshold = 1;

                //_circles = CvInvoke.HoughCircles(frame, HoughType.Gradient, 1.0, 50.0, cannyThreshold, circleAccumulatorThreshold, 2, 15);

                //CvInvoke.cvSetImageCOI(frame, 0);
                //Point minPoint= new Point();
                //Point maxPoint=new Point();
                //double minVal=0;
                //double maxVal=255;
                //CvInvoke.MinMaxLoc(frame, ref minVal, ref maxVal, ref minPoint, ref maxPoint);
                //_circles = new CircleF[] {new CircleF(maxPoint,5)};
                */
        }

        private void ProcessFrame(object sender, Mat frame)
        {
            if (_emgu.IsCalibrated && _isEnabled && _isOn)
            {
                var threshold = HsvThresholdWhite(frame);
                var transformed = new Mat();
                CvInvoke.WarpPerspective(threshold, transformed, _homography, new Size(_emgu.Width, _emgu.Height));
                FindCircles(transformed);
            }
        }

        private void TrackCursor()
        {
            var trackingPoint = Vector2.zero;
            if (_circles != null && _circles.Length > 0)
            {
                var circle = ChooseCircle(_circles);
                if (circle.HasValue)
                {
                    var curCircle = circle.Value;
                    trackingPoint.x = curCircle.Center.X;
                    trackingPoint.y = curCircle.Center.Y;
                    var distPoint = trackingPoint;
                    if (distPoint != null)
                    {
                        trackingPosition = distPoint;
                        //_cursor.transform.position = trackingPosition;                        
                        isTracked = true;
                        return;
                    }
                }
            }

            isTracked = false;
        }

        private CircleF? ChooseCircle(CircleF[] circles)
        {
            foreach (var circle in circles) return circle;
            return null;
        }


        private Mat HsvThresholdWhite(Mat input)
        {
            var HMin = _emgu.HMin;
            var HMax = _emgu.HMax;
            var SMin = _emgu.SMin;
            var SMax = _emgu.SMax;
            var VMin = _emgu.VMin;
            var VMax = _emgu.VMax;

            var merged = new Mat();
            var output = new Mat();
            var channels = new VectorOfMat();
            var hueResult = new Mat();
            var satResult = new Mat();
            var valResult = new Mat();

            var hsv = new Mat();

            CvInvoke.CvtColor(input, hsv, ColorConversion.Bgr2Hsv);

            CvInvoke.Split(hsv, channels);

            CvInvoke.InRange(channels[0], new ScalarArray(new MCvScalar(HMin, HMin, HMin)),
                new ScalarArray(new MCvScalar(HMax, HMax, HMax)), hueResult);

            CvInvoke.InRange(channels[1], new ScalarArray(new MCvScalar(SMin, SMin, SMin)),
                new ScalarArray(new MCvScalar(SMax, SMax, SMax)), satResult);

            CvInvoke.InRange(channels[2], new ScalarArray(new MCvScalar(VMin, VMin, VMin)),
                new ScalarArray(new MCvScalar(VMax, VMax, VMax)), valResult);

            CvInvoke.Merge(new VectorOfMat(hueResult, satResult, valResult), merged);

            var hueT = new Mat();
            var satT = new Mat();
            var valT = new Mat();

            CvInvoke.InRange(merged, new ScalarArray(new MCvScalar(250, 250, 250)),
                new ScalarArray(new MCvScalar(255, 255, 255)), hueT);
            CvInvoke.InRange(merged, new ScalarArray(new MCvScalar(250, 250, 250)),
                new ScalarArray(new MCvScalar(255, 255, 255)), satT);
            CvInvoke.InRange(merged, new ScalarArray(new MCvScalar(250, 250, 250)),
                new ScalarArray(new MCvScalar(255, 255, 255)), valT);

            CvInvoke.Merge(new VectorOfMat(hueT, satT, valT), output);

            var grayImage = new Mat();

            CvInvoke.CvtColor(output, grayImage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            var pyrDown = new Mat();
            //CvInvoke.PyrDown(grayImage, pyrDown);
            //CvInvoke.PyrUp(pyrDown, grayImage);

            CvInvoke.GaussianBlur(grayImage, pyrDown, new Size(9, 9), 6);

            //Contrast Up

            CvInvoke.EqualizeHist(pyrDown, grayImage);

            return grayImage;
        }
    }
}