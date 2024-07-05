using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class FireTypesGenerator : MonoBehaviour
    {
        [SerializeField] private TargetType[] targetTypes = new TargetType[3];

        public Target GenerateTypeATarget()
        {
            var extinguishTime =
                Random.Range(targetTypes[0].minExtinguishingTime, targetTypes[0].maxExtinguishingTime);
            var timerTime = Random.Range(targetTypes[0].minTimerTime, targetTypes[0].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }

        public Target GenerateTypeBTarget()
        {
            var extinguishTime =
                Random.Range(targetTypes[1].minExtinguishingTime, targetTypes[1].maxExtinguishingTime);
            var timerTime = Random.Range(targetTypes[1].minTimerTime, targetTypes[1].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }

        public Target GenerateTypeCTarget()
        {
            var extinguishTime =
                Random.Range(targetTypes[2].minExtinguishingTime, targetTypes[2].maxExtinguishingTime);
            var timerTime = Random.Range(targetTypes[2].minTimerTime, targetTypes[2].maxTimerTime);
            return new Target(extinguishTime, timerTime);
        }

        [Serializable]
        private struct TargetType
        {
            public float minExtinguishingTime;
            public float maxExtinguishingTime;
            [Space(7)] public float minTimerTime;
            public float maxTimerTime;
        }
    }

    public class Target
    {
        public Target(float extinguishingTime, float timerTime)
        {
            ExtinguishingTime = extinguishingTime;
            TimerTime = timerTime;
        }

        public float ExtinguishingTime { get; private set; }
        public float TimerTime { get; private set; }
    }
}