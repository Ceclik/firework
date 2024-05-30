using UnityEngine;

namespace FireSeekingScripts
{
    public class FireSystemPathMover : MonoBehaviour
    {
        [SerializeField] private SeekingFireSystemHandler seekingFireSystem;
        [SerializeField] private Transform movingPathParent;
        [Space(10)] [SerializeField] private float movingSpeed;

        private Transform[] _points;
        private int _index;
        
        public bool IsRoundPassed { get; private set; }
        public bool IsMoving { get; set; }

        private void Start()
        {
            _points = new Transform[movingPathParent.childCount];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = movingPathParent.GetChild(i);
            _index = 0;
        }

        private void Update()
        {
            if (GameManager.Instance.IsFireStarted && IsMoving && _index < _points.Length && !seekingFireSystem.fake)
            {
                Vector3 direction = (_points[_index].position - transform.position).normalized;
                Vector3 movement = direction * movingSpeed * Time.deltaTime;
                
                transform.Translate(movement);
            }

            if (_index < _points.Length && Vector3.Distance(transform.position, _points[_index].position) < 0.1f)
                _index++;

            if (_index == _points.Length && !IsRoundPassed)
                IsRoundPassed = true;
        }
    }
}
