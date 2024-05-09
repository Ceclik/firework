using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace Assets.Scripts.Tracking
{
    public class ColorFilter:MonoBehaviour
    {
        [SerializeField]
        WebCam webcam;

        JobHandle m_RGBComplementBurstJobHandle;

        NativeArray<Color32> m_NativeColors;
    }
}
