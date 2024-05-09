using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tracking
{
    public class CircleFinder
    {
        public CircleF[] circles;

        public CircleF[] FindCircles(Mat image)
        {
            double cannyThreshold = 200.0;
            double circleAccumulatorThreshold = 2;            

            CircleF[] circles = CvInvoke.HoughCircles(image, HoughType.Gradient, 1.0, 50.0, cannyThreshold, circleAccumulatorThreshold, 1, 15);
            this.circles = circles;
            return circles;
        }

        public Mat DrawCircles(Mat image)
        {
            int maxDrawed = 0;
            int drawed = 0;
            if (circles != null)
                for (int i =0; i < circles.Length; i++)
                {
                    CvInvoke.Circle(image, Point.Round(circles[i].Center), (int)circles[i].Radius, new MCvScalar(200, 0, 0), 2);
                    drawed++;
                    if (drawed > maxDrawed) break;
                }                                    
            return image;
        }
    }
}
