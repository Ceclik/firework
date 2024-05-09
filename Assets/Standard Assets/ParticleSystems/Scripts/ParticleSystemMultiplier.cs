using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    //This script changes values of multiple particle systems of which fire is made up and start/stop them
    
    public class ParticleSystemMultiplier : MonoBehaviour
    {
        // a simple script to scale the size, speed and lifetime of a particle system

        public float multiplier = 1;

        private List<float> startSizes = new List<float>();
        private List<float> startSpeeds = new List<float>();
        private List<float> startLifetimes = new List<float>();


        private void Start()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
				ParticleSystem.MainModule mainModule = system.main;                
				mainModule.startSizeMultiplier *= 1f;
                mainModule.startSpeedMultiplier *= 1f;
                mainModule.startLifetimeMultiplier *= Mathf.Lerp(1f, 1, 0.5f);
                startSizes.Add(mainModule.startSizeMultiplier);
                startSpeeds.Add(mainModule.startSpeedMultiplier);
                startLifetimes.Add(mainModule.startLifetimeMultiplier);
                system.Clear();
                system.Play();
            }            
        }
        private void Update()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();
            int i = 0;
            foreach (ParticleSystem system in systems)
            {
                ParticleSystem.MainModule mainModule = system.main;
                mainModule.startSizeMultiplier = startSizes[i]* multiplier;
                mainModule.startSpeedMultiplier = startSpeeds[i] * multiplier;
                mainModule.startLifetimeMultiplier = startLifetimes[i] * Mathf.Lerp(multiplier, 1, 0.5f);
                i++;
            }
            if (multiplier < 0) multiplier = 0;
        }

        public void Stop()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();            
            foreach (ParticleSystem system in systems)
            {
                ParticleSystem.MainModule mainModule = system.main;
                mainModule.startSizeMultiplier = 0;
                mainModule.startSpeedMultiplier = 0;
                mainModule.startLifetimeMultiplier = 0;
                system.Stop();
            }
        }

        public void StartLight()
        {
            Start();
        }
    }
}
