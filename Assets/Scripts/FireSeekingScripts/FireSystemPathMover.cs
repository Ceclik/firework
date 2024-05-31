using System.Linq;
using UnityEngine;

namespace FireSeekingScripts
{
    public class FireSystemPathMover : MonoBehaviour
    {
        [SerializeField] private SeekingFireSystemHandler seekingFireSystem;
        [SerializeField] private Transform movingPathParent;
        [Space(10)] [SerializeField] private float startMainFireVelocity;

        [Space(10)] [SerializeField] private GameObject explosion;

        private Transform[] _points;
        private int _index;

        private float _mainFireVelocity;
        
        public bool IsRoundPassed { get; private set; }
        public bool IsMoving { get; set; }
        public bool SlowMoving { get; set; }

        private void Start()
        {
            _points = new Transform[movingPathParent.childCount];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = movingPathParent.GetChild(i);
            _index = 0;
        }

        private void CountMainFireVelocity()
        {
            _mainFireVelocity = startMainFireVelocity /
                                (1 + (seekingFireSystem.AmountOfActiveFires-1) * CountCombustionDegree());
        }

        private float CountCombustionDegree()
        {
            float combustionDegree = 0;

            foreach (var fire in seekingFireSystem.fires.Skip(0))
                if (fire.gameObject.activeSelf)
                {
                    FireComplexParametersCounter parameters = fire.GetComponent<FireComplexParametersCounter>();
                    combustionDegree += parameters.TimeFromStart * 0.1f - parameters.TimeOfExtinguishing * 0.3f;
                }
            
            return combustionDegree;
        }

        private void Update()
        {
            CountMainFireVelocity();
            
            if (GameManager.Instance.IsFireStarted && IsMoving && _index < _points.Length && !seekingFireSystem.fake)
            {
                if (SlowMoving) _mainFireVelocity /= 2;
                Vector3 direction = (_points[_index].position - transform.position).normalized;
                Vector3 movement = direction * _mainFireVelocity * Time.deltaTime;
                
                Debug.LogError($"main fire velocity: {_mainFireVelocity}");
                transform.Translate(movement);
            }

            if (_index < _points.Length && Vector3.Distance(transform.position, _points[_index].position) < 0.1f)
                _index++;

            if (_index == _points.Length && !IsRoundPassed)
            {
                IsRoundPassed = true;
                explosion.SetActive(true);
            }
        }
    }
}
