using System;
using UnityEngine;

namespace FireSeekingScripts
{
    public class FireSystemPathMover : MonoBehaviour
    {
        [SerializeField] private Transform movingPathParent;
        [Space(10)] [SerializeField] private float movingSpeed;

        private Transform[] _points;
        private int _index;

        private void Start()
        {
            _points = new Transform[movingPathParent.childCount];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = movingPathParent.GetChild(i);
            _index = 0;
        }

        private void Update()
        {
            if (GameManager.Instance.IsFireStarted)
            {
                Vector3 direction = (_points[_index].position - transform.position).normalized;
                Debug.LogError($"Target position: {direction}\nindex: {_index}");
                Vector3 movement = direction * movingSpeed * Time.deltaTime;
                
                transform.Translate(movement);
            }

            if (Vector3.Distance(transform.position, _points[_index].position) < 0.1f)
            {
                _index++;
            }
        }
    }
}
