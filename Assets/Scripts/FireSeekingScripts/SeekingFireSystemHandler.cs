using Standard_Assets.ParticleSystems.Scripts;
using Tracking;
using UnityEngine;
using UnityStandardAssets.Effects;

namespace FireSeekingScripts
{
    public class SeekingFireSystemHandler : MonoBehaviour
    {
        [SerializeField] private GameObject[] mainFire;
        [SerializeField] private float fireStopSpeed = 1f;
        [SerializeField] private float fireGrowSpeed = 1f;
        [SerializeField] private float expandTime = 10f;
        [SerializeField] private FireLight fireLight;
        [SerializeField] private float overallLife;
        [SerializeField] private FireStopper stopper;
        [SerializeField] private bool fake = false;
        [SerializeField] private bool startOver = false;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject sounds;

        [Space(20)] [SerializeField] private NewFireSpawner fireSpawner;

        private float[] _firesLife;
        private float[] _expandTimers;
        private float _startFireIntensity;
        private bool _started;
        private bool _ended;
        private AudioSource _sound;
        private AudioSource _aSoundStart;
        private AudioSource _aSoundFire;

        private FireSystemPathMover _mainFireRoundMover;
        
        
        private void Start()
        {
            
            
            //////////////FOR DEBUG ONLY///////////////
            /// TODO delete this
            GameManager.Instance.FireSeekGameMode = true;
            //////////////////////////////////////////

            _mainFireRoundMover = mainFire[0].GetComponent<FireSystemPathMover>();
            
            _firesLife = new float[mainFire.Length];
            _expandTimers = new float[mainFire.Length];
            for (int i = 0; i < _firesLife.Length; i++)
            {
                _firesLife[i] = 100f;
                _expandTimers[i] = 0f;
                mainFire[i].SetActive(false);            
            }
            _startFireIntensity = fireLight.intensity;        
        }

        private void OnEnable()
        {
            _sound = GetComponent<AudioSource>();
            var allSounds = sounds.GetComponents<AudioSource>();
            _aSoundStart = allSounds[0];
            _aSoundFire = allSounds[1];
            StartCoroutine(AudioFade.FadeIn(_aSoundStart, 2f));        
        }
        
        private void Update()
        {
            if (_started && overallLife==0 && !_ended)
            {
                _ended = true;
                GameManager.Instance.EndScene();
            }
        

            Ray ray = Camera.main.ScreenPointToRay(new Vector2(MyInput.Instance.X,MyInput.Instance.Y));

            float allLightsMult = 0;
            int activeFiresCount = 0;
            int hittedFire = 0;
            int hittedFireLastIndex = 0;
            for (int i = 0; i < mainFire.Length; i++)
            {
                //check mouse cursor in fire            
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                RaycastHit hit;
                var particle = mainFire[i].GetComponent<ParticleSystemMultiplier>();
                var fireCollider = mainFire[i].GetComponent<Collider>();            

                if (particle.multiplier > 0 && mainFire[i].activeSelf)
                {
                    if (fireCollider.Raycast(ray, out hit, Mathf.Infinity) && MyInput.Instance.IsTrackedCursor)
                    {
                        if(!_mainFireRoundMover.IsMoving) _mainFireRoundMover.IsMoving = true;
                        if(fireSpawner.IsSpawned) fireSpawner.IsSpawned = false;
                        
                        stopper.Orient(hit.point);

                        if (_mainFireRoundMover.IsRoundPassed)
                        {
                            if (!(fake && i==0) && !(startOver && i==0)) particle.multiplier -= fireStopSpeed * Time.deltaTime;
                            if (startOver && particle.multiplier>=0.1f && i==0) particle.multiplier -= fireStopSpeed * Time.deltaTime;
                        }
                        
                        _firesLife[i] = particle.multiplier * 100f;
                        hittedFire++;
                        hittedFireLastIndex = i;
                        if (_firesLife[i] <= 0) //check fire life and turn off
                        {
                            particle.Stop();
                            _expandTimers = new float[mainFire.Length];
                        }         
                        if (fake && i==0)
                        {
                            if (particle.multiplier < 1f && mainFire[i].activeSelf)
                            {
                                particle.multiplier += (fireGrowSpeed * Time.deltaTime);
                                _firesLife[i] = particle.multiplier * 100f;
                            }
                            if (particle.multiplier >= 1f && mainFire[i].activeSelf)
                            {
                                _expandTimers[i] = _expandTimers[i] + Time.deltaTime;
                                if (_expandTimers[i] > expandTime)
                                {
                                    ExpandFire();
                                    _expandTimers = new float[mainFire.Length];
                                }
                            }
                        }
                    }
                    else
                    {      
                        if(_mainFireRoundMover.IsMoving)
                            _mainFireRoundMover.IsMoving = false;
                        
                        if(!fireSpawner.IsSpawned)
                            fireSpawner.SpawnNewFireComplex();
                        
                        ////////////???//////////////////////////////////////
                        if (particle.multiplier < 1f && mainFire[i].activeSelf)
                        {
                            particle.multiplier += (fireGrowSpeed * Time.deltaTime);
                            _firesLife[i] = particle.multiplier * 100f;
                        }
                        if (particle.multiplier>=1f && mainFire[i].activeSelf)
                        {
                            _expandTimers[i] = _expandTimers[i] + Time.deltaTime;
                            if (_expandTimers[i]>expandTime)
                            {
                                ExpandFire();
                                _expandTimers = new float[mainFire.Length];
                            }

                        }
                    }
                }

                activeFiresCount++;
                allLightsMult += particle.multiplier;
                _sound.volume = overallLife;

            }
            if (mainFire.Length > 0)
            {
                overallLife = allLightsMult / (float)mainFire.Length;
            }
            else
            {
                overallLife = 0;
            }
            if (overallLife < 0.008f) overallLife = 0;
            if (activeFiresCount == 0) overallLife = 0;
            fireLight.intensity = _startFireIntensity * overallLife;
            
            if (hittedFire>0)
            {
                stopper.StartFireStopping(hittedFireLastIndex,fake,startOver);
            }
            else
            {
                stopper.StopFireStopping();
            }
        }

        public void StartFire()
        {
            if (explosion!=null)
            {
                explosion.SetActive(true);
            }
            mainFire[0].SetActive(true);
            var particle = mainFire[0].GetComponent<ParticleSystemMultiplier>();
            particle.multiplier = 0.8f;
            _started = true;
            Debug.Log("Start fire!");        
            _sound.Play();
            _sound.volume = 0;
            if (_aSoundStart.volume>=0.1) StartCoroutine(AudioFade.FadeOut(_aSoundStart, 5f));
            StartCoroutine(AudioFade.FadeIn(_aSoundFire, 0.5f));
        }

        private void ExpandFire()
        {
            foreach (var t in mainFire)
            {
                var particle = t.GetComponent<ParticleSystemMultiplier>();
                if (particle.multiplier <= 0 || t.activeSelf==false)
                {
                    t.SetActive(true);                
                    particle.multiplier = 1f;
                    particle.StartLight();
                    particle.multiplier = 0.01f;
                    Debug.Log("expand fire");
                    return;
                }
            }
        }    
    }
}
