using System.Diagnostics;
using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;

namespace FireAimScripts
{
    public class NewFiresSpawner : MonoBehaviour
    {
        [SerializeField] private Transform fireAvailableLocations;
        [SerializeField] private int amountOfFiresToSpawn;
        private Transform[] _spawnPoints;
        private FireSplitter[] _fires;
        private AimFireSystemHandler _aimFireSystemHandler;
        private FireTypesGenerator _firesGenerator;

        private void Start()
        {
            _firesGenerator = GetComponent<FireTypesGenerator>();
            _aimFireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _spawnPoints = new Transform[fireAvailableLocations.childCount];
            _fires = new FireSplitter[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                _fires[i] = transform.GetChild(i).GetComponent<FireSplitter>();
                _fires[i].OnFireSplitted += SpawnNewFire;
            }

            for (int i = 0; i < fireAvailableLocations.childCount; i++)
                _spawnPoints[i] = fireAvailableLocations.GetChild(i);
        }

        private void SpawnNewFire()
        {
            for (int j = 0; j < amountOfFiresToSpawn; j++)
            {
                int indexOfLocation = Random.Range(0, _spawnPoints.Length);
                while (_spawnPoints[indexOfLocation].GetComponent<SpawnPoint>().IsUsing)
                    indexOfLocation = Random.Range(0, _spawnPoints.Length);

                int indexOfFire = -1;
                for (int i = 0; i < _fires.Length; i++)
                    if (_fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f &&
                        !_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished)
                    {
                        indexOfFire = i;
                        break;
                    }

                SetFireSystem(indexOfFire, j == 1);

                _fires[indexOfFire].gameObject.SetActive(true);
                _fires[indexOfFire].GetComponent<ParticleSystemMultiplier>().multiplier = 0.05f;
                _aimFireSystemHandler.AmountOfActiveFires++;
                _fires[indexOfFire].transform.position = _spawnPoints[indexOfLocation].position;
                _spawnPoints[indexOfLocation].GetComponent<SpawnPoint>().IsUsing = true;
            }
        }
        
        private void SetFireSystem(int indexOfIre, bool isSecondFire)
        {
            FireSplitter splitter = _fires[indexOfIre].GetComponent<FireSplitter>();
            bool isCorrectTime = false;

            Target target = _firesGenerator.GenerateTypeCTarget();
            
            splitter.StartTimerValue = target!.TimerTime;
            if (isSecondFire)
                splitter.FireStopTime = 5 - _fires[indexOfIre - 1].GetComponent<FireSplitter>().FireStopTime;
            splitter.FireStopTime = target!.ExtinguishingTime;
        }

        private void OnDisable()
        {
            foreach (var fire in _fires)
                fire.OnFireSplitted -= SpawnNewFire;
        }
    }
}
