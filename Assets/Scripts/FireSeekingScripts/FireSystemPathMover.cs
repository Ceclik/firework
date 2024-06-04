using System.Linq;
using UnityEngine;

namespace FireSeekingScripts
{
    public class FireSystemPathMover : MonoBehaviour
    {
        [SerializeField] private SeekingFireSystemHandler seekingFireSystem;
        [SerializeField] private Transform[] movingPathParents;
        [Space(10)] [SerializeField] private float startMainFireVelocity;

        [Space(10)] [SerializeField] private GameObject explosion;
        

        private Transform[] _points;
        private int _movingIndex;
        private int _pathIndex;

        private float _mainFireVelocity;
        
        public bool IsRoundPassed { get; private set; }
        public bool IsMoving { get; set; }
        public bool SlowMoving { get; set; }

        private void Start()
        {
            _pathIndex = Random.Range(0, movingPathParents.Length);
            
            _points = new Transform[movingPathParents[_pathIndex].childCount];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = movingPathParents[_pathIndex].GetChild(i);
            _movingIndex = 0;
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
            
            if (GameManager.Instance.IsFireStarted && IsMoving && _movingIndex < _points.Length && !seekingFireSystem.fake)
            {
                if (SlowMoving) _mainFireVelocity /= 2;
                Vector3 direction = (_points[_movingIndex].position - transform.position).normalized;
                Vector3 movement = direction * _mainFireVelocity * Time.deltaTime;
                
                transform.Translate(movement);
            }

            if (_movingIndex < _points.Length && Vector3.Distance(transform.position, _points[_movingIndex].position) < 0.1f)
                _movingIndex++;

            if (_movingIndex == _points.Length && !IsRoundPassed)
            {
                IsRoundPassed = true;
                explosion.transform.position = _points[_movingIndex - 1].position;
                explosion.SetActive(true);
            }
        }
    }
}
