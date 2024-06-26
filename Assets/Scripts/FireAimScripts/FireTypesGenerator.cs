using UnityEngine;
using UnityEngine.Serialization;

namespace FireAimScripts
{
    public class FireTypesGenerator : MonoBehaviour
    {
        [SerializeField] private TargetType[] targetTypes = new TargetType[3];
        
        [System.Serializable]
        private struct TargetType
        {
            public float minExtinguishingTime;
            public float maxExtinguishingTime;
            [Space(7)] public float minTimerTime;
            public float maxTimerTime;
        }

        public Target GenerateTypeATarget()
        {
            float extinguishTime =
                Random.Range(targetTypes[0].minExtinguishingTime, targetTypes[0].maxExtinguishingTime);
            float timerTime = Random.Range(targetTypes[0].minTimerTime, targetTypes[0].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }
        
        public Target GenerateTypeBTarget()
        {
            float extinguishTime =
                Random.Range(targetTypes[1].minExtinguishingTime, targetTypes[1].maxExtinguishingTime);
            float timerTime = Random.Range(targetTypes[1].minTimerTime, targetTypes[1].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }
        
        public Target GenerateTypeCTarget()
        {
            float extinguishTime =
                Random.Range(targetTypes[2].minExtinguishingTime, targetTypes[2].maxExtinguishingTime);
            float timerTime = Random.Range(targetTypes[2].minTimerTime, targetTypes[2].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }

    }

    public class Target
    {
        public float ExtinguishingTime { get; private set; }
        public float TimerTime { get; private set; }

        public Target(float extinguishingTime, float timerTime)
        {
            ExtinguishingTime = extinguishingTime;
            TimerTime = timerTime;
        }
    }
}
