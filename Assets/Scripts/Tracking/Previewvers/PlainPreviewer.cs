using Assets.Scripts.Tracking.CamFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tracking.Previewvers
{
    public class PlainPreviewer:Previewer
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
