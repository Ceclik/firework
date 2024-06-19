using TMPro;
using UnityEngine;

namespace Instructions
{
    public class CountDownTimer : MonoBehaviour
    {
        [SerializeField] private float countdownValue;
        [SerializeField] private float increasingNumberDuration;
        [SerializeField] private GameObject instructionsGameObject;
        private TextMeshProUGUI _timerText;
        private float _scaleTimer;
        private RectTransform _rectTransform;

        private Vector3 _startScale;
        private Vector3 _finishScale;
        private bool _isBelowOne;
        
        public delegate void StartLevel();

        public event StartLevel OnCountdownPassed;

        private void Start()
        {
            _startScale = new Vector3(1.0f, 1.0f, 1.0f);
            _finishScale = new Vector3(2.0f, 2.0f, 2.0f);
            _timerText = GetComponent<TextMeshProUGUI>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (countdownValue <= 1 && !_isBelowOne)
            {
                _isBelowOne = true;
                _timerText.text = "Туши!";
            }
            
            _scaleTimer += Time.deltaTime;
            countdownValue -= Time.deltaTime;
            if(!_isBelowOne)
                _timerText.text = ((int)countdownValue).ToString();

            if (_scaleTimer < 1 && _rectTransform.localScale.x < 2.0f)
                _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, _finishScale,
                    _scaleTimer / increasingNumberDuration);
            
            if (_scaleTimer >= 1)
            {
                _scaleTimer = 0;
                _rectTransform.localScale = _startScale;
            }

            if (countdownValue <= 0)
            {
                OnCountdownPassed?.Invoke();
                instructionsGameObject.SetActive(false);
            }
            
        }
    }
}
