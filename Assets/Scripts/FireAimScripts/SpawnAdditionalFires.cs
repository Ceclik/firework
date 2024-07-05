using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class SpawnAdditionalFires : MonoBehaviour
    {
        [SerializeField] private Transform fireLocationsParent;
        [SerializeField] private int amountOfFireToAdditionalSpawn;
        private SpawnPoint[] _fireLocations;
        private GameObject[] _fires;
        private FireTypesGenerator _firesGenerator;
        private AimFireSystemHandler _fireSystemHandler;
        private bool _isCVariantPassed;
        private bool _isSecondStageSpawned;
        private bool _isThirdStageSpawned;

        private Timer _timer;
        public bool IsNewSpawned { get; set; }

        private void Awake()
        {
            _timer = GameObject.Find("Timer").GetComponent<Timer>();
        }

        private void Start()
        {
            _firesGenerator = GetComponent<FireTypesGenerator>();
            _fireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _fires = new GameObject[transform.childCount];
            _fireLocations = new SpawnPoint[fireLocationsParent.childCount];

            for (var i = 0; i < transform.childCount; i++)
                _fires[i] = transform.GetChild(i).gameObject;

            for (var i = 0; i < fireLocationsParent.childCount; i++)
                _fireLocations[i] = fireLocationsParent.GetChild(i).GetComponent<SpawnPoint>();
        }

        private void Update()
        {
            if (_fireSystemHandler.AmountOfActiveFires == 1)
                foreach (var fire in _fires)
                    if (fire.activeSelf)
                        if (fire.GetComponent<ParticleSystemMultiplier>().multiplier < 0.4f)
                            SpawnNewFire();
        }

        private void SpawnNewFire()
        {
            var _scoreCounter = GetComponent<ScoreCounterInAimMode>();
            if (!IsNewSpawned && _timer.startTime * 60 - _timer.value < 40)
            {
                if (!_isSecondStageSpawned && !_isThirdStageSpawned)
                {
                    if (_timer.startTime * 60 - _timer.value < 40)
                    {
                        SpawnAPlusBVariants();
                        _scoreCounter.AmountOfA++;
                        _scoreCounter.AmountOfB++;
                    }
                    else if (_timer.startTime * 60 - _timer.value < 35)
                    {
                        SpawnBVariant();
                        _scoreCounter.AmountOfB++;
                    }
                    else if (_timer.startTime * 60 - _timer.value < 25)
                    {
                        SpawnCVariant(2);
                        _scoreCounter.AmountOfC++;
                    }
                }

                else if (!_isThirdStageSpawned && !_isCVariantPassed)
                {
                    if (_timer.startTime - _timer.value < 40)
                    {
                        SpawnAVariant();
                        _scoreCounter.AmountOfA++;
                    }
                    else if (_timer.startTime - _timer.value < 35)
                    {
                        SpawnCVariant(3);
                        _scoreCounter.AmountOfC++;
                    }
                }

                IsNewSpawned = true;
            }
        }

        private void SetFireSystem(int indexOfIre, int type)
        {
            var splitter = _fires[indexOfIre].GetComponent<FireSplitter>();

            var target = type switch
            {
                0 => _firesGenerator.GenerateTypeATarget(),
                1 => _firesGenerator.GenerateTypeBTarget(),
                2 => _firesGenerator.GenerateTypeCTarget(),
                _ => null
            };

            splitter.StartTimerValue = target!.TimerTime;
            splitter.FireStopTime = target!.ExtinguishingTime;
        }

        private void SpawnBVariant()
        {
            UnityEngine.Debug.Log($"Spawn B variant\ntime left {_timer.startTime * 60 - _timer.value}");
            var indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            var indexOfFire = -1;
            for (var i = 0; i < _fires.Length; i++)
                if (!_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
                    _fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f)
                {
                    indexOfFire = i;
                    break;
                }

            SetFireSystem(indexOfFire, 1);

            _fires[indexOfFire].SetActive(true);
            _fires[indexOfFire].GetComponent<ParticleSystemMultiplier>().multiplier = 0.05f;
            _fireLocations[indexOfLocation].IsUsing = true;
            _fireSystemHandler.AmountOfActiveFires++;
            _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;
        }

        private void SpawnAVariant()
        {
            UnityEngine.Debug.Log($"Spawn A variant\ntime left {_timer.startTime * 60 - _timer.value}");
            var indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            var indexOfFire = -1;
            for (var i = 0; i < _fires.Length; i++)
                if (!_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
                    _fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f)
                {
                    indexOfFire = i;
                    break;
                }

            SetFireSystem(indexOfFire, 0);

            _fires[indexOfFire].SetActive(true);
            _fires[indexOfFire].GetComponent<ParticleSystemMultiplier>().multiplier = 0.05f;
            _fireLocations[indexOfLocation].IsUsing = true;
            _fireSystemHandler.AmountOfActiveFires++;
            _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;

            _isThirdStageSpawned = true;
        }

        private void SpawnCVariant(int stage)
        {
            UnityEngine.Debug.Log($"Spawn C variant\ntime left {_timer.startTime * 60 - _timer.value}");
            var indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            var indexOfFire = -1;
            for (var i = 0; i < _fires.Length; i++)
                if (!_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
                    _fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f)
                {
                    indexOfFire = i;
                    break;
                }

            SetFireSystem(indexOfFire, 2);

            _fires[indexOfFire].SetActive(true);
            _fires[indexOfFire].GetComponent<ParticleSystemMultiplier>().multiplier = 0.05f;
            _fireLocations[indexOfLocation].IsUsing = true;
            _fireSystemHandler.AmountOfActiveFires++;
            _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;

            switch (stage)
            {
                case 2:
                    _isSecondStageSpawned = true;
                    break;
                case 3:
                    _isThirdStageSpawned = true;
                    break;
            }

            _isCVariantPassed = true;
        }

        private void SpawnAPlusBVariants()
        {
            UnityEngine.Debug.Log($"Spawn A+B variant\ntime left {_timer.startTime * 60 - _timer.value}");
            for (var j = 0; j < 2; j++)
            {
                var indexOfLocation = Random.Range(0, _fireLocations.Length);
                while (_fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing)
                    indexOfLocation = Random.Range(0, _fireLocations.Length);

                var indexOfFire = -1;
                for (var i = 0; i < _fires.Length; i++)
                    if (!_fires[i].activeSelf && !_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
                        _fires[i].GetComponent<ParticleSystemMultiplier>().multiplier <= 0.01f)
                    {
                        indexOfFire = i;
                        break;
                    }

                switch (j)
                {
                    case 0:
                        SetFireSystem(indexOfFire, 0);
                        break;
                    case 1:
                        SetFireSystem(indexOfFire, 1);
                        break;
                }

                UnityEngine.Debug.Log($"spawn\n index of fire: {indexOfFire}");
                _fires[indexOfFire].SetActive(true);
                _fireSystemHandler.AmountOfActiveFires++;


                _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;
                _fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing = true;
            }


            _isSecondStageSpawned = true;
        }
    }
}