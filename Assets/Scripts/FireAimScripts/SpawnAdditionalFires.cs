using System;
using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class SpawnAdditionalFires : MonoBehaviour
    {
        private AimFireSystemHandler _fireSystemHandler;
        private bool _isAdditionalSpawned = false;
        private GameObject[] _fires;
        
        [SerializeField] private Transform fireLocationsParent;
        private SpawnPoint[] _fireLocations;

        private Timer _timer;

        private void Awake()
        {
            _timer = GameObject.Find("Timer").GetComponent<Timer>();
        }

        private void Start()
        {
            _fireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _fires = new GameObject[transform.childCount];
            _fireLocations = new SpawnPoint[fireLocationsParent.childCount];

            for (int i = 0; i < transform.childCount; i++)
                _fires[i] = transform.GetChild(i).gameObject;

            for (int i = 0; i < fireLocationsParent.childCount; i++)
                _fireLocations[i] = fireLocationsParent.GetChild(i).GetComponent<SpawnPoint>();
        }

        private void Update()
        {
            if (_fireSystemHandler.AmountOfActiveFires == 1)
                foreach (var fire in _fires)
                    if (fire.activeSelf)
                        if(fire.GetComponent<ParticleSystemMultiplier>().multiplier < 0.3f || _timer.timer <= 30)
                            SpawnNewFire();
        }

        private void SpawnNewFire()
        {
            if (!_isAdditionalSpawned)
            {
                int indexOfLocation = Random.Range(0, _fireLocations.Length);
                while (!_fireLocations[indexOfLocation].IsUsing)
                    indexOfLocation = Random.Range(0, _fireLocations.Length);

                int indexOfFire = -1;
                for (int i = 0; i < _fires.Length; i++)
                    if (!_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
                        _fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f)
                    {
                        indexOfFire = i;
                        break;
                    }

                Debug.LogError($"idex of fire: {indexOfFire}\nisAdditionalSpawned: {_isAdditionalSpawned}");
                _fires[indexOfFire].SetActive(true);
                _fireSystemHandler.AmountOfActiveFires++;
                _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;
                _isAdditionalSpawned = true;
                Debug.LogError($"isAdditionalSpawned: {_isAdditionalSpawned}");
            }
        }
    }
}
