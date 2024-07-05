using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;

namespace FireAimScripts
{
    public class NewFiresSpawner : MonoBehaviour
    {
        [SerializeField] private Transform fireAvailableLocations;
        [SerializeField] private int amountOfFiresToSpawn;
        [SerializeField] private Timer timer;
        private AimFireSystemHandler _aimFireSystemHandler;
        private FireSplitter[] _fires;
        private FireTypesGenerator _firesGenerator;
        private Transform[] _spawnPoints;

        private void Start()
        {
            _firesGenerator = GetComponent<FireTypesGenerator>();
            _aimFireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _spawnPoints = new Transform[fireAvailableLocations.childCount];
            _fires = new FireSplitter[transform.childCount];

            for (var i = 0; i < transform.childCount; i++)
            {
                _fires[i] = transform.GetChild(i).GetComponent<FireSplitter>();
                _fires[i].OnFireSplitted += SpawnNewFire;
            }

            for (var i = 0; i < fireAvailableLocations.childCount; i++)
                _spawnPoints[i] = fireAvailableLocations.GetChild(i);
        }

        private void OnDisable()
        {
            foreach (var fire in _fires)
                fire.OnFireSplitted -= SpawnNewFire;
        }

        private void SpawnNewFire()
        {
            if (timer.startTime * 60 - timer.value < 40)
                for (var j = 0; j < amountOfFiresToSpawn; j++)
                {
                    var indexOfLocation = Random.Range(0, _spawnPoints.Length);
                    while (_spawnPoints[indexOfLocation].GetComponent<SpawnPoint>().IsUsing)
                        indexOfLocation = Random.Range(0, _spawnPoints.Length);

                    var indexOfFire = -1;
                    for (var i = 0; i < _fires.Length; i++)
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
            var splitter = _fires[indexOfIre].GetComponent<FireSplitter>();

            var target = _firesGenerator.GenerateTypeCTarget();
            GetComponent<ScoreCounterInAimMode>().AmountOfC++;

            splitter.StartTimerValue = target!.TimerTime;
            if (isSecondFire)
                splitter.FireStopTime = 5 - _fires[indexOfIre - 1].GetComponent<FireSplitter>().FireStopTime;
            splitter.FireStopTime = target!.ExtinguishingTime;
        }
    }
}