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
        
        [Space(20)][Header("Colors for target")]
        [SerializeField] private Color redShade;
        [SerializeField] private Color orangeShade;
        [SerializeField] private Color yellowShade;
        [SerializeField] private Color greenShade;

        [Space(20)] [Header("For Scaling")] [SerializeField]
        private Transform camera;
        
        private ParticleSystemMultiplier _particleMultiplier;

        private void Start()
        {
            _particleMultiplier = GetComponent<ParticleSystemMultiplier>();

            if (Vector3.Distance(transform.position, camera.position) >= 3.0f)
                transform.localScale *= 1.5f;
        }

        private void HandleScale()
        {
            outerCircle.rectTransform.localScale =
                new Vector3(_particleMultiplier.multiplier, _particleMultiplier.multiplier,
                    _particleMultiplier.multiplier);
            
            //if (_particleMultiplier.multiplier < 0.5f)
            if(outerCircle.rectTransform.localScale.x <= 0.75f)
            {
                float doubledMultiplier = _particleMultiplier.multiplier * 1.2f;
                middleCircle.rectTransform.localScale =
                    new Vector3(doubledMultiplier, doubledMultiplier, doubledMultiplier);
            }
            Debug.LogError($"middle scale: {middleCircle.rectTransform.localScale.x}");
        }

        private void HandleColor()
        {
            Color resultColor;
            float multiplierValue = _particleMultiplier.multiplier;
            if(multiplierValue > 0.66f)
                resultColor = Color.Lerp(orangeShade, redShade, (multiplierValue - 0.66f)/(1f - 0.66f));
            else if (multiplierValue > 0.33f)
                resultColor = Color.Lerp(yellowShade, orangeShade, (multiplierValue - 0.33f) / (0.66f - 0.33f));
            else
                resultColor = Color.Lerp(greenShade, yellowShade, multiplierValue / 0.33f);

            outerCircle.color = new Color(resultColor.r, resultColor.g, resultColor.b, 0.3f);
            middleCircle.color = new Color(resultColor.r, resultColor.g, resultColor.b, 0.5f);
            innerCircle.color = resultColor;
        }

        private void Update()
        {
            HandleScale();
            HandleColor();
        }
    }
}
