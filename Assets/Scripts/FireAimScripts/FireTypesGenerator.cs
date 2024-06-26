using UnityEngine;

namespace FireAimScripts
{
    public class FireTypesGenerator : MonoBehaviour
    {
        [SerializeField] private TargetType[] _targetTypes = new TargetType[3];
        
        private struct TargetType
        {
            public float MinExtinguishingTime;
            public float MaxExtinguishingTime;
            [Space(7)] public float MinTimerTime;
            public float MaxTimerTime;
        }

        public Target GenerateTypeATarget()
        {
            float extinguishTime =
                Random.Range(_targetTypes[0].MinExtinguishingTime, _targetTypes[0].MaxExtinguishingTime);
            float timerTime = Random.Range(_targetTypes[0].MinTimerTime, _targetTypes[0].MaxTimerTime);
            return new Target(extinguishTime, timerTime);
        }
        
        public Target GenerateTypeBTarget()
        {
            float extinguishTime =
                Random.Range(_targetTypes[1].MinExtinguishingTime, _targetTypes[1].MaxExtinguishingTime);
            float timerTime = Random.Range(_targetTypes[1].MinTimerTime, _targetTypes[1].MaxTimerTime);
            return new Target(extinguishTime, timerTime);
        }
        
        public Target GenerateTypeCTarget()
        {
            float extinguishTime =
                Random.Range(_targetTypes[2].MinExtinguishingTime, _targetTypes[2].MaxExtinguishingTime);
            float timerTime = Random.Range(_targetTypes[2].MinTimerTime, _targetTypes[2].MaxTimerTime);
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
