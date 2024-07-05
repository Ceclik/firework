using Assets.Scripts.Tracking.CamFilters;
using UnityEngine;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class ResultPreviewer : HsvPreviewer
    {
        private readonly CircleFinder cFinder = new();

        protected override void Awake()
        {
            _filters = new ICamFilter[]
            {
                new BgrToHsv(),
                new HsvThresholdWhite(),
                new BlurFilter(),
                new CircleFilter(cFinder),
                new CircleDrawer(cFinder)
            };
        }

        public Vector2 GetTrackingPoint()
        {
            var output = Vector2.zero;
            if (cFinder != null && cFinder.circles != null && cFinder.circles.Length > 0)
                output = new Vector2(cFinder.circles[0].Center.X, cFinder.circles[0].Center.Y);
            return output;
        }
    }
}