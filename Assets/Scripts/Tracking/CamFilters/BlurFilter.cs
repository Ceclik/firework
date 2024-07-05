using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class BlurFilter : ICamFilter
    {
        public Mat ProcessFrame(Mat input)
        {
            var output = new Mat();
            var grayImage = new Mat();

            CvInvoke.CvtColor(input, grayImage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            var pyrDown = new Mat();
            CvInvoke.PyrDown(grayImage, pyrDown);
            CvInvoke.PyrUp(pyrDown, grayImage);

            CvInvoke.GaussianBlur(grayImage, pyrDown, new Size(9, 9), 6);

            //Contrast Up

            CvInvoke.EqualizeHist(pyrDown, grayImage);

            //Convert Gray Image to Color
            CvInvoke.CvtColor(grayImage, output, ColorConversion.Gray2Rgb);

            return output;
        }
    }
}