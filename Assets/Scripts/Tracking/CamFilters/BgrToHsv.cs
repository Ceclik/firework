using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Tracking.CamFilters
{
    public class BgrToHsv : ICamFilter
    {
        public Mat ProcessFrame(Mat input)
        {
            var output = new Mat();
            CvInvoke.CvtColor(input, output, ColorConversion.Bgr2Hsv);
            return output;
        }
    }
}