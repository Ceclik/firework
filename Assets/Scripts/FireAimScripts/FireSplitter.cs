using ExternalAssets.Standard_Assets.ParticleSystems.Scripts;
using TMPro;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        public delegate void DecreaseLives();

        [Space(10)] [Header("Timer parameters")] [SerializeField]
        private TextMeshProUGUI timerText;

        [SerializeField] private float timerIncreasingTime;
        [SerializeField] private float timerStartIncreasingValue;
        private Vector3 _endLocalScale;
        private float _extinguishingTime;
        private float _scaleTimer;
        private Vector3 _startLocalScale;
        private float _timer;

        public float StartTimerValue { get; set; }

        public bool IsExtinguishing { get; set; }

        public float FireStopSpeed { get; private set; }
        public float FireStopTime { get; set; }

        private void Start()
        {
            _timer = StartTimerValue;
            _startLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
            _endLocalScale = new Vector3(3.0f, 3.0f, 3.0f);
            FireStopSpeed = CountFireStopSpeed();
        }

        private void Update()
        {
            if (IsExtinguishing) _extinguishingTime += Time.deltaTime;

            if (!IsExtinguishing)
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
                SplitFire();
            }
        }

        public event DecreaseLives OnFireSplitted;

        private float CountFireStopSpeed()
        {
            return 0.5f * 1.26f / FireStopTime;
        }

        public void OnEndExtinguishing()
        {
            IsExtinguishing = false;
            UnityEngine.Debug.Log($"Extinguishing time: {_extinguishingTime}");
        }

        private void SplitFire()
        {
            timerText.rectTransform.localScale = _startLocalScale;
            UnityEngine.Debug.Log("In split fire");
            OnFireSplitted?.Invoke();
            GetComponent<ParticleSystemMultiplier>().multiplier = 0;
        }
    }
}