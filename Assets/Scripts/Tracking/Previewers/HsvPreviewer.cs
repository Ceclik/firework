using Assets.Scripts.Tracking;
using Assets.Scripts.Tracking.CamFilters;
using Tracking.CamFilters;

namespace Tracking.Previewers
{
    public class HsvPreviewer : Previewer
    {
        protected virtual void Awake()
        {
            Filters = new ICamFilter[2]
            {
                new BgrToHsv(),
                new HsvThresholdRgb()
            };
        }

        public void SettingsFromCamera()
        {
            HueMin = (float)EmguCamera.Instance.HMin;
            HueMax = (float)EmguCamera.Instance.HMax;
            SatMin = (float)EmguCamera.Instance.SMin;
            SatMax = (float)EmguCamera.Instance.SMax;
            ValMin = (float)EmguCamera.Instance.VMin;
            ValMax = (float)EmguCamera.Instance.VMax;
        }

        #region threshold filter settings

        public float HueMin
        {
            get => (Filters[1] as HsvThresholdRgb).HMin;
            set
            {
                (Filters[1] as HsvThresholdRgb).HMin = value;
                EmguCamera.Instance.HMin = value;
            }
        }

        public float HueMax
        {
            get => (Filters[1] as HsvThresholdRgb).HMax;
            set
            {
                (Filters[1] as HsvThresholdRgb).HMax = value;
                EmguCamera.Instance.HMax = value;
            }
        }

        public float SatMin
        {
            get => (Filters[1] as HsvThresholdRgb).SMin;
            set
            {
                (Filters[1] as HsvThresholdRgb).SMin = value;
                EmguCamera.Instance.SMin = value;
            }
        }

        public float SatMax
        {
            get => (Filters[1] as HsvThresholdRgb).SMax;
            set
            {
                (Filters[1] as HsvThresholdRgb).SMax = value;
                EmguCamera.Instance.SMax = value;
            }
        }

        public float ValMin
        {
            get => (Filters[1] as HsvThresholdRgb).VMin;
            set
            {
                (Filters[1] as HsvThresholdRgb).VMin = value;
                EmguCamera.Instance.VMin = value;
            }
        }

        public float ValMax
        {
            get => (Filters[1] as HsvThresholdRgb).VMax;
            set
            {
                (Filters[1] as HsvThresholdRgb).VMax = value;
                EmguCamera.Instance.VMax = value;
            }
        }

        #endregion
    }
}