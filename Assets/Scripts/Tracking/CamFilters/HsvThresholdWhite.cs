using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class HsvThresholdWhite : HsvThresholdRgb
    {
        public override Mat ProcessFrame(Mat input)
        {
            var merged = new Mat();
            var output = new Mat();
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


            //CvInvoke.CvtColor(merged, output, ColorConversion.Hsv2Rgb);
            return output;
        }
    }
}