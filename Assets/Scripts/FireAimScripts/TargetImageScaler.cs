using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace FireAimScripts
{
    public class TargetImageScaler : MonoBehaviour
    {
        [SerializeField] private Image outerCircle;
        [SerializeField] private Image middleCircle;
        [SerializeField] private Image innerCircle;
        private ParticleSystemMultiplier _particleMultiplier;

        private void Start()
        {
            _particleMultiplier = GetComponent<ParticleSystemMultiplier>();
        }

        private void Update()
        {
            if (_particleMultiplier.multiplier > 0.5f)
            {
                outerCircle.rectTransform.localScale =
                    new Vector3(_particleMultiplier.multiplier, _particleMultiplier.multiplier,
                        _particleMultiplier.multiplier);
            }
            else if (_particleMultiplier.multiplier < 0.5f)
            {
                float doubledMultiplier = _particleMultiplier.multiplier * 2;
                middleCircle.rectTransform.localScale =
                    new Vector3(doubledMultiplier, doubledMultiplier, doubledMultiplier);
            }
        }
    }
}
