using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [SerializeField] private float minTimeToSplit;
        [SerializeField] private float maxTimeToSplit;

        [Space(10)][Header("Timer parameters")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float timerIncreasingTime;
        [SerializeField] private float timerStartIncreasingValue;

        private float _timer;
        private Vector3 _startLocalScale;
        private Vector3 _endLocalScale;
        private float _scaleTimer;
        
        public float FireStopSpeed { get; private set; }
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _timer = Random.Range(minTimeToSplit, maxTimeToSplit + 1.0f);
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            FireStopSpeed = CountFireStopSpeed();
        }
        
        private float CountFireStopSpeed()  //this counting must be remade in universal 
        {
            if (_timer is >= 8.0f and < 9.0f) return 0.55f;
            if (_timer is >= 9.0f and < 10.0f) return 0.5f;
            if (_timer is >= 10.0f and < 11.0f) return 0.4f;
            if (_timer is >= 11.0f and < 12.0f) return 0.3f;
            if (_timer is >= 12.0f and < 13.0f) return 0.25f;
            if (_timer is >= 13.0f and < 14.0f) return 0.2f;
            if (_timer >= 14.0f) return 0.2f;
            return 0.2f;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            timerText.text = ((int)_timer).ToString();

            if (_timer <= timerStartIncreasingValue)
            {
                _scaleTimer += Time.deltaTime;
                timerText.rectTransform.localScale =
                    Vector3.Lerp(_startLocalScale, _endLocalScale, _scaleTimer / timerIncreasingTime);
            }

            if (_timer <= 0)
            {
                _timer = minTimeToSplit;
                _scaleTimer = 0;
                SplitFire();
            }
        }

        private void SplitFire()
        {
            timerText.rectTransform.localScale = _startLocalScale;
            OnFireSplitted?.Invoke();
        }
    }
}
