using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class CircleFilter : ICamFilter
    {
        private CircleFinder _cFinder;

        public CircleFilter(CircleFinder finder)
        {
            _cFinder = finder;
        }

        public Mat ProcessFrame(Mat input)
        {
            var output = new Mat();
            var grayImage = new Mat();

            CvInvoke.CvtColor(input, grayImage, ColorConversion.Bgr2Gray);

            _cFinder.FindCircles(grayImage);                                        

            return input;
        }
    }
}
