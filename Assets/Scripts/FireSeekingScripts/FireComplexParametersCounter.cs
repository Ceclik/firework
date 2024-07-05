using UnityEngine;

namespace FireSeekingScripts
{
    public class FireComplexParametersCounter : MonoBehaviour
    {
        public int ExtinguishAttempts { get; set; } = 0;
        public float TimeFromStart { get; private set; }
        public float TimeOfExtinguishing { get; set; } = 0;

        public bool IsExtinguishing { get; set; }

        private void Update()
        {
            TimeFromStart += Time.deltaTime;
        }
    }
}