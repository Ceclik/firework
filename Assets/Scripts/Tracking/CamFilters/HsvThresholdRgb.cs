using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class HsvThresholdRgb : ICamFilter
    {
        public float HMin = 0;
        public float SMin = 0;
        public float VMin = 0;
        public float HMax = 255;
        public float SMax = 255;
        public float VMax = 255;
        /// <summary>
        /// Split HSV image to channels and apply threshold
        /// </summary>
        /// <param name="input">HSV image</param>
        /// <returns>RGB image</returns>
        public virtual Mat ProcessFrame(Mat input)
        {
            Mat merged = new Mat();            
            VectorOfMat channels = new VectorOfMat();
            Mat hueResult = new Mat();
            Mat satResult = new Mat();
            Mat valResult = new Mat();

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
