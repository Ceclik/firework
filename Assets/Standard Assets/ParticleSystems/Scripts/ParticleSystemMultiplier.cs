using System.Collections.Generic;
using UnityEngine;

namespace Standard_Assets.ParticleSystems.Scripts
{
    //This script changes values of multiple particle systems of which fire is made up and start/stop them
    
    public class ParticleSystemMultiplier : MonoBehaviour
    {
        // a simple script to scale the size, speed and lifetime of a particle system

        public float multiplier = 1;

        private List<float> _startSizes = new List<float>();
        private List<float> _startSpeeds = new List<float>();
        private List<float> _startLifetimes = new List<float>();

        public bool IsFinished { get; set; }
        
        public bool IsGrown { get; private set; }


        private void Start()
        {
            Mathf.Clamp(multiplier, 0.01f, 1.0f);
            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
				ParticleSystem.MainModule mainModule = system.main;                
				mainModule.startSizeMultiplier *= 1f;
                mainModule.startSpeedMultiplier *= 1f;
                mainModule.startLifetimeMultiplier *= Mathf.Lerp(1f, 1, 0.5f);
                _startSizes.Add(mainModule.startSizeMultiplier);
                _startSpeeds.Add(mainModule.startSpeedMultiplier);
                _startLifetimes.Add(mainModule.startLifetimeMultiplier);
                system.Clear();
                system.Play();
            }            
        }
        private void Update()
        {
            if (!IsGrown)
            {
                if (multiplier == 1)
                    IsGrown = true;
            }
            var systems = GetComponentsInChildren<ParticleSystem>();
            int i = 0;
            foreach (ParticleSystem system in systems)
            {
                ParticleSystem.MainModule mainModule = system.main;
                mainModule.startSizeMultiplier = _startSizes[i]* multiplier;
                mainModule.startSpeedMultiplier = _startSpeeds[i] * multiplier;
                mainModule.startLifetimeMultiplier = _startLifetimes[i] * Mathf.Lerp(multiplier, 1, 0.5f);
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
