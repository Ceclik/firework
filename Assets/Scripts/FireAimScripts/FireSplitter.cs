using System;
using UnityEngine;

namespace FireAimScripts
{
    public class FireSplitter : MonoBehaviour
    {
        [SerializeField] private float timeToSplit;
        [SerializeField] private GameObject complexClone;

        private float _timeSpent = 0;

        private void Update()
        {
            Debug.Log($"Time spent: {_timeSpent}");
            _timeSpent += Time.deltaTime;

            if (_timeSpent >= timeToSplit)
            {
                SplitFire();
                _timeSpent = 0;
            }
                
        }

        private void SplitFire()
        {
            if(!complexClone.activeSelf)
                complexClone.SetActive(true);
        }
    }
}
