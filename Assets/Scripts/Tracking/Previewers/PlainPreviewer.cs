using Assets.Scripts.Tracking.CamFilters;
using Tracking.CamFilters;
using Tracking.Previewers;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class PlainPreviewer : Previewer
    {
        private void Awake()
        {
            Filters = new ICamFilter[1]
            {
                new BgrToRgb()
            };
        }
    }
}