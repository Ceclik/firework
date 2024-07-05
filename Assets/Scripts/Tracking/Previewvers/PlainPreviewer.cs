using Assets.Scripts.Tracking.CamFilters;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class PlainPreviewer : Previewer
    {
        private void Awake()
        {
            _filters = new ICamFilter[1]
            {
                new BgrToRgb()
            };
        }
    }
}