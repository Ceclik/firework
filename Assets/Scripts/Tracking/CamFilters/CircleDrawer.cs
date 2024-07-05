using Emgu.CV;

namespace Assets.Scripts.Tracking.CamFilters
{
    public class CircleDrawer : ICamFilter
    {
        private readonly CircleFinder _cFinder;

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