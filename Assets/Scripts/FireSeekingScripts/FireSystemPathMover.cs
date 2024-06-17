using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FireSeekingScripts
{
    public class FireSystemPathMover : MonoBehaviour
    {
        [SerializeField] private SeekingFireSystemHandler seekingFireSystem;
        [SerializeField] private Transform[] movingPathParents;
        [Space(10)] [SerializeField] private float startMainFireVelocity;
        [SerializeField] private int segmentCount = 5;
        [SerializeField] private float moveToNextSegmentDelay;

        [Space(10)] [SerializeField] private GameObject explosion;
        [SerializeField] private ParticleSystem fireStopperParticles;
        
        private List<List<Transform>> _segments = new List<List<Transform>>();
        private List<int> _visitedSegments = new List<int>();
        private int _currentSegment = 0;
        
        private Transform[] _wayPoints;
        private int _currentWaypointIndex = 0;
        private int _pathIndex;
        private bool _isMovingToNextSegment;

        private float _mainFireVelocity;
        
        public bool IsRoundPassed { get; private set; }
        public bool IsMoving { get; set; }
        public bool SlowMoving { get; set; }

        private void Start()
        {
            _pathIndex = Random.Range(0, movingPathParents.Length);
            MakePointsArray();
            
            int totalPoints = _wayPoints.Length;
            int baseSegmentSize = totalPoints / segmentCount;
            int remainder = totalPoints % segmentCount;
            
            int currentIndex = 0;
            for (int i = 0; i < segmentCount; i++) 
            {
                int currentSegmentSize = baseSegmentSize + (i < remainder ? 1 : 0);
                List<Transform> segment = new List<Transform>();
                for (int j = 0; j < currentSegmentSize; j++) 
                {
                    segment.Add(_wayPoints[currentIndex]);
                    currentIndex++;
                }
                _segments.Add(segment);
            }

            _currentSegment = GetNextSegmentIndex();
            transform.position = _segments[_currentSegment][0].position;
        }

        private void MakePointsArray()
        {
            _wayPoints = new Transform[movingPathParents[_pathIndex].childCount];
            for (int i = 0; i < _wayPoints.Length; i++)
                _wayPoints[i] = movingPathParents[_pathIndex].GetChild(i);
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
        
        private void MoveAlongSegment() 
        {
            if(_currentSegment == -1) {return;}
            if (GameManager.Instance.IsFireStarted &&
                _currentWaypointIndex < _segments[_currentSegment].Count && !seekingFireSystem.fake)
            {
                if (IsMoving)
                {
                    if (SlowMoving) _mainFireVelocity /= 2;
                    Transform targetWaypoint = _segments[_currentSegment][_currentWaypointIndex];
                    transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position,
                        _mainFireVelocity * Time.deltaTime);

                    if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
                        _currentWaypointIndex++;
                }
            }
            else 
            {
                if (!_isMovingToNextSegment)
                {
                    _isMovingToNextSegment = true;
                    StartCoroutine(MoveToNextSegment());
                }
            }
        }
        
        private IEnumerator MoveToNextSegment()
        {
            yield return new WaitForSeconds(moveToNextSegmentDelay);
            if (!_visitedSegments.Contains(_currentSegment)) {
                _visitedSegments.Add(_currentSegment);
            }

            _currentSegment = GetNextSegmentIndex();
            if (_currentSegment == -1) 
                _isMovingToNextSegment = false; 
            else
            {
                _currentWaypointIndex = 0;
                transform.position = _segments[_currentSegment][0].position;
                _isMovingToNextSegment = false;
            }
        }

        private void Update()
        {
            CountMainFireVelocity();
            
            MoveAlongSegment();
        }
        
        private int GetNextSegmentIndex() 
        {
            List<int> availableSegments = new List<int>();
            for (int i = 0; i < _segments.Count; i++) 
                if (!_visitedSegments.Contains(i)) 
                    availableSegments.Add(i);
            
            if (availableSegments.Count == 0) 
            {
                IsRoundPassed = true;
                explosion.transform.position = _wayPoints[_currentWaypointIndex].position;
                explosion.SetActive(true);
                return -1;
            }

            int nextSegment = availableSegments[Random.Range(0, availableSegments.Count)];
            _visitedSegments.Add(nextSegment);
            return nextSegment;
        }
    }
}
