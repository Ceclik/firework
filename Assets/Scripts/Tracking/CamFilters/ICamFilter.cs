using Emgu.CV;

namespace Assets.Scripts.Tracking.CamFilters
{
    public interface ICamFilter
    {
        Mat ProcessFrame(Mat input);
    }
}