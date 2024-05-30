using System;
using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace FireAimScripts
{
    public class TargetImageScaler : MonoBehaviour
    {
        [SerializeField] private Image targetimage;
        private ParticleSystemMultiplier _particleMultiplier;

        private void Start()
        {
            _particleMultiplier = GetComponent<ParticleSystemMultiplier>();
        }

        private void Update()
        {
            targetimage.rectTransform.localScale = new Vector3(_particleMultiplier.multiplier,
                _particleMultiplier.multiplier, _particleMultiplier.multiplier);
        }
    }
}
