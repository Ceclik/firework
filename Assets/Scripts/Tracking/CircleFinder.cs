using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace Assets.Scripts.Tracking
{
    public class CircleFinder
    {
        public CircleF[] circles;

        public CircleF[] FindCircles(Mat image)
        {
            var cannyThreshold = 200.0;
            double circleAccumulatorThreshold = 2;

            var circles = CvInvoke.HoughCircles(image, HoughType.Gradient, 1.0, 50.0, cannyThreshold,
                circleAccumulatorThreshold, 1, 15);
            this.circles = circles;
            return circles;
        }

        public Mat DrawCircles(Mat image)
        {
            var maxDrawed = 0;
            var drawed = 0;
            if (circles != null)
                for (var i = 0; i < circles.Length; i++)
                {
                    CvInvoke.Circle(image, Point.Round(circles[i].Center), (int)circles[i].Radius,
                        new MCvScalar(200, 0, 0), 2);
                    drawed++;
                    if (drawed > maxDrawed) break;
                }

            return image;
        }
    }
}