using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class BgrToRgb : ICamFilter
    {
        public Mat ProcessFrame(Mat input)
        {
            var output = new Mat();
            CvInvoke.CvtColor(input, output, ColorConversion.Bgr2Rgb);
            return output;
        }
    }
}