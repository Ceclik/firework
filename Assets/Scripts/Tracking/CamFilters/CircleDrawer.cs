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
    public class CircleDrawer : ICamFilter
    {
        private CircleFinder _cFinder;

        public CircleDrawer(CircleFinder finder)
        {
            _cFinder = finder;
        }

        public Mat ProcessFrame(Mat input)
        {            
            _cFinder.DrawCircles(input);

            return input;
        }
    }
}
