using UnityEngine;

namespace FireAimScripts
{
    public class CanvasToCameraPositioner : MonoBehaviour
    {
        private void Update()
        {
            var camPos = Camera.main?.transform;
            transform.LookAt(camPos.position - camPos.forward);
        }
    }
}