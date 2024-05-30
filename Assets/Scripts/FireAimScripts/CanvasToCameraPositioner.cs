using System;
using UnityEngine;

namespace FireAimScripts
{
    public class CanvasToCameraPositioner : MonoBehaviour
    {
        private void Update()
        {
            Transform camPos = Camera.main?.transform;
            transform.LookAt(camPos.position - camPos.forward);

        }
    }
}
