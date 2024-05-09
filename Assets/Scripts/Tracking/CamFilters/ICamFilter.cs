using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tracking.CamFilters
{
    public interface ICamFilter
    {
        Mat ProcessFrame(Mat input);
    }
}
