using Assets.Scripts.Tracking.CamFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class HsvPreviewer:Previewer
    {
        #region threshold filter settings
        public float HueMin
        {
            get { return (_filters[1] as HsvThresholdRgb).HMin; }
            set {
                (_filters[1] as HsvThresholdRgb).HMin = value;
                EmguCamera.Instance.HMin = value;
            }
        }

        public float HueMax
        {
            get { return (_filters[1] as HsvThresholdRgb).HMax; }
            set {
                (_filters[1] as HsvThresholdRgb).HMax = value;
                EmguCamera.Instance.HMax = value;
            }
        }

        public float SatMin
        {
            get { return (_filters[1] as HsvThresholdRgb).SMin; }
            set {
                (_filters[1] as HsvThresholdRgb).SMin = value;
                EmguCamera.Instance.SMin = value;
            }
        }

        public float SatMax
        {
            get { return (_filters[1] as HsvThresholdRgb).SMax; }
            set {
                (_filters[1] as HsvThresholdRgb).SMax = value;
                EmguCamera.Instance.SMax = value;
            }
        }

        public float ValMin
        {
            get { return (_filters[1] as HsvThresholdRgb).VMin; }
            set {
                (_filters[1] as HsvThresholdRgb).VMin = value;
                EmguCamera.Instance.VMin = value;
            }
        }

        public float ValMax
        {
            get { return (_filters[1] as HsvThresholdRgb).VMax; }
            set {
                (_filters[1] as HsvThresholdRgb).VMax = value;
                EmguCamera.Instance.VMax = value;
            }
        }
        #endregion

        protected virtual void Awake()
        {
            _filters = new ICamFilter[2]
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
    }
}
