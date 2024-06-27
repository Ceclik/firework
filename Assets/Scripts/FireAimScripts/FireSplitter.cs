using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [Space(10)][Header("Timer parameters")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float timerIncreasingTime;
        [SerializeField] private float timerStartIncreasingValue;

        public float StartTimerValue { get; set; }
        private float _timer;
        private Vector3 _startLocalScale;
        private Vector3 _endLocalScale;
        private float _scaleTimer;
        
        public bool IsExtinguishing { get; set; } //TODO delete
        private float _extinguishingTime;
        
        public float FireStopSpeed { get; private set; }
        public float FireStopTime { get; set; }
        
        public delegate void DecreaseLives();
        public event DecreaseLives OnFireSplitted;

        private void Start()
        {
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            FireStopSpeed = CountFireStopSpeed();
        }
        
        private float CountFireStopSpeed()
        {
            return 0.5f * 1.26f / FireStopTime;
        }

        public void OnEndExtinguishing()
        {
            IsExtinguishing = false;
            Debug.LogError($"Extinguishing time: {_extinguishingTime}");
        }

        private void Update()
        {
            if (IsExtinguishing)
            {
                _extinguishingTime += Time.deltaTime;
            }
            
            if(!IsExtinguishing)
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
                _timer = StartTimerValue;
                _scaleTimer = 0;
                //SplitFire();
            }
        }

        private void SplitFire()
        {
            timerText.rectTransform.localScale = _startLocalScale;
            OnFireSplitted?.Invoke();
        }
    }
}
