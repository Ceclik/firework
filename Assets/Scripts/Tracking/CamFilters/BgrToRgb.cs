using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class BgrToRgb : ICamFilter
    {
        public Mat ProcessFrame(Mat input)
        {
            Mat output = new Mat();
            CvInvoke.CvtColor(input, output, ColorConversion.Bgr2Rgb);
            return output;
        }
    }
}
