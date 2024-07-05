using Assets.Scripts.Tracking.CamFilters;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Tracking.CamFilters
{
    public class HsvThresholdRgb : ICamFilter
    {
        public float HMax = 255;
        public float HMin = 0;
        public float SMax = 255;
        public float SMin = 0;
        public float VMax = 255;
        public float VMin = 0;

        /// <summary>
        ///     Split HSV image to channels and apply threshold
        /// </summary>
        /// <param name="input">HSV image</param>
        /// <returns>RGB image</returns>
        public virtual Mat ProcessFrame(Mat input)
        {
            var merged = new Mat();
            var channels = new VectorOfMat();
            var hueResult = new Mat();
            var satResult = new Mat();
            var valResult = new Mat();

            CvInvoke.Split(input, channels);

            CvInvoke.InRange(channels[0], new ScalarArray(new MCvScalar(HMin, HMin, HMin)),
                new ScalarArray(new MCvScalar(HMax, HMax, HMax)), hueResult);

            CvInvoke.InRange(channels[1], new ScalarArray(new MCvScalar(SMin, SMin, SMin)),
                new ScalarArray(new MCvScalar(SMax, SMax, SMax)), satResult);

            CvInvoke.InRange(channels[2], new ScalarArray(new MCvScalar(VMin, VMin, VMin)),
                new ScalarArray(new MCvScalar(VMax, VMax, VMax)), valResult);

            CvInvoke.Merge(new VectorOfMat(hueResult, satResult, valResult), merged);

            //CvInvoke.CvtColor(merged, output, ColorConversion.Hsv2Rgb);
            return merged;
        }
    }
}