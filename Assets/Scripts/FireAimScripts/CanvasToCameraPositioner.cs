using System;
using UnityEngine;

namespace FireAimScripts
{
    public class CanvasToCameraPositioner : MonoBehaviour
    {
        private void Update()
        {
            Transform camPos = Camera.main?.transform;
            Vector3 lookPos = new Vector3(camPos.position.x, camPos.position.y, transform.position.z);
            
            transform.LookAt(lookPos);

        }
    }
}
