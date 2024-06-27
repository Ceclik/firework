using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class SpawnAdditionalFires : MonoBehaviour
    {
        private AimFireSystemHandler _fireSystemHandler;
        private bool _isSecondStageSpawned;
        private bool _isThirdStageSpawned;
        private bool _isCVariantPassed;
        public bool IsNewSpawned { get; set; }
        private GameObject[] _fires;
        private FireTypesGenerator _firesGenerator;
        
        [SerializeField] private Transform fireLocationsParent;
        [SerializeField] private int amountOfFireToAdditionalSpawn;
        private SpawnPoint[] _fireLocations;

        private Timer _timer;

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

            for (int i = 0; i < transform.childCount; i++)
                _fires[i] = transform.GetChild(i).gameObject;

            for (int i = 0; i < fireLocationsParent.childCount; i++)
                _fireLocations[i] = fireLocationsParent.GetChild(i).GetComponent<SpawnPoint>();
        }

        private void Update()
        {
            if (_fireSystemHandler.AmountOfActiveFires == 1)
            {
                Debug.LogWarning($"AmountOfActiveFires: {_fireSystemHandler.AmountOfActiveFires}");
                foreach (var fire in _fires)
                    if (fire.activeSelf)
                        if (fire.GetComponent<ParticleSystemMultiplier>().multiplier < 0.4f)
                            SpawnNewFire();
            }
        }

        private void SpawnNewFire()
        {
            Debug.LogError("In spawn new fire");
            if (!IsNewSpawned)
            {
                if (!_isSecondStageSpawned && !_isThirdStageSpawned)
                {
                    if (_timer.value < 25)
                        SpawnAPlusBVariants();
                    else if (_timer.value < 35)
                        SpawnBVariant();
                    else if (_timer.value < 40)
                        SpawnCVariant(2);
                }

                else if (!_isThirdStageSpawned && !_isCVariantPassed)
                {
                    if (_timer.value < 35)
                        SpawnAVariant();
                    else if (_timer.value < 40)
                        SpawnCVariant(3);
                }

                IsNewSpawned = true;
            }
        }
        
        private void SetFireSystem(int indexOfIre, int type)
        {
            FireSplitter splitter = _fires[indexOfIre].GetComponent<FireSplitter>();

            Target target = type switch
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
            Debug.LogError("Spawn B variant");
            int indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            int indexOfFire = -1;
            for (int i = 0; i < _fires.Length; i++)
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
            int indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            int indexOfFire = -1;
            for (int i = 0; i < _fires.Length; i++)
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
            Debug.LogError("Spawn A variant");
            _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;

            _isThirdStageSpawned = true;
        }

        private void SpawnCVariant(int stage)
        {
            int indexOfLocation = Random.Range(0, _fireLocations.Length);
            while (_fireLocations[indexOfLocation].IsUsing)
                indexOfLocation = Random.Range(0, _fireLocations.Length);

            int indexOfFire = -1;
            for (int i = 0; i < _fires.Length; i++)
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
            Debug.LogError("Spawn C variant");
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

        private void SpawnAPlusBVariants( )
        {
            if (!_isSecondStageSpawned)
            {
                for (int j = 0; j < 2; j++)
                {
                    int indexOfLocation = Random.Range(0, _fireLocations.Length);
                    while (_fireLocations[indexOfLocation].IsUsing)
                        indexOfLocation = Random.Range(0, _fireLocations.Length);

                    int indexOfFire = -1;
                    for (int i = 0; i < _fires.Length; i++)
                        if (!_fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished &&
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

                    _fires[indexOfFire].SetActive(true);
                    _fires[indexOfFire].GetComponent<ParticleSystemMultiplier>().multiplier = 0.05f;
                    _fireLocations[indexOfLocation].IsUsing = true;
                    _fireSystemHandler.AmountOfActiveFires++;
                    Debug.LogError("Spawn A+B variant");
                    _fires[indexOfFire].transform.position = _fireLocations[indexOfLocation].transform.position;
                }

                _isSecondStageSpawned = true;
            }
        }
    }
}
