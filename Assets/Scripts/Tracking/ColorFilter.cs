using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts.Tracking
{
    public class ColorFilter : MonoBehaviour
    {
        [SerializeField] private WebCam webcam;

        private NativeArray<Color32> m_NativeColors;

        private JobHandle m_RGBComplementBurstJobHandle;
    }
}