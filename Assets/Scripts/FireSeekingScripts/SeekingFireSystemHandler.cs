using Standard_Assets.ParticleSystems.Scripts;
using Tracking;
using UnityEngine;
using UnityStandardAssets.Effects;

namespace FireSeekingScripts
{
    public class SeekingFireSystemHandler : MonoBehaviour
    {
        [SerializeField] private GameObject[] fires;
        [SerializeField] private float fireStopSpeed = 1f;
        [SerializeField] private float fireGrowSpeed = 1f;
        [SerializeField] private FireLight fireLight;
        [SerializeField] private float overallLife;
        [SerializeField] private FireStopper stopper;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject sounds;

        public bool fake { get; set; }
        public bool startOver { get; set; }
        
        private float[] _firesLife;
        private float _startFireIntensity;
        private bool _started;
        private bool _ended;
        private AudioSource _sound;
        private AudioSource _aSoundStart;
        private AudioSource _aSoundFire;

        private FireSystemPathMover _mainFireRoundMover;

        private bool _isNewFireSpawned = true;
        private bool _isFirstDisabled = false;
        private int _newFireIndex = 1;
        private int _amountOfActiveFires = 1;
        
        
        private void Start()
        {
            
            
            //////////////FOR DEBUG ONLY///////////////
            /// TODO delete this
            GameManager.Instance.FireSeekGameMode = true;
            //////////////////////////////////////////
            
            _mainFireRoundMover = fires[0].GetComponent<FireSystemPathMover>();
            
            _firesLife = new float[fires.Length];
            for (int i = 0; i < _firesLife.Length; i++)
            {
                _firesLife[i] = 100f;
                fires[i].SetActive(false);            
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

        private void HandleMainFireMovement(Ray ray)
        {
            if (fires[0].GetComponent<Collider>().Raycast(ray, out var hitMain, Mathf.Infinity) &&
                MyInput.Instance.IsTrackedCursor)
            {
                stopper.Orient(hitMain.point);
                if(!_mainFireRoundMover.IsMoving) _mainFireRoundMover.IsMoving = true;
                if(_isNewFireSpawned) _isNewFireSpawned = false;
            }

            else
            {
                if(_mainFireRoundMover.IsMoving)
                    _mainFireRoundMover.IsMoving = false;
                if(!_isNewFireSpawned && !_mainFireRoundMover.IsRoundPassed)
                    SpawnNewFire();
            }
        }

        private void SpawnNewFire()
        {
            fires[_newFireIndex].SetActive(true);
            fires[_newFireIndex].transform.position = fires[0].transform.position;
            fires[_newFireIndex].GetComponent<ParticleSystemMultiplier>().multiplier = 0.4f;
            _isNewFireSpawned = true;
            _newFireIndex++;
            _amountOfActiveFires++;
        }

        private void DisableFire(int i)
        {
            if (i == 0 && !_isFirstDisabled)
            {
                _isFirstDisabled = true;
                _amountOfActiveFires--;
            }

            if (i != 0)
            {
                fires[i].SetActive(false);
                _amountOfActiveFires--;
            }
        }
        
        private void Update()
        {
            Debug.LogError($"Amount of active fires: {_amountOfActiveFires}");
            if (_started && _amountOfActiveFires == 0 && !_ended)
            {
                _ended = true;
                GameManager.Instance.EndScene();
            }
            
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(MyInput.Instance.X,MyInput.Instance.Y));
            
            //checking if main fire is hitted
            HandleMainFireMovement(ray);

            float allLightsMult = 0;
            int hittedFire = 0;
            int hittedFireLastIndex = 0;
            for (int i = 0; i < fires.Length; i++)
            {
                
                if (fires[i].GetComponent<ParticleSystemMultiplier>().multiplier == 0 && fires[i].activeSelf)
                    DisableFire(i);

                
                //check mouse cursor in fire            
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                RaycastHit hit;
                var particle = fires[i].GetComponent<ParticleSystemMultiplier>();
                var fireCollider = fires[i].GetComponent<Collider>();            

                if (particle.multiplier > 0 && fires[i].activeSelf)
                {
                    if (fireCollider.Raycast(ray, out hit, Mathf.Infinity) && MyInput.Instance.IsTrackedCursor)
                    {
                        stopper.Orient(hit.point);
                        
                        
                        if(i == 0 && !_mainFireRoundMover.IsRoundPassed) {}
                        else
                        {
                            if (!(fake && i == 0) && !(startOver && i == 0))
                                particle.multiplier -= fireStopSpeed * Time.deltaTime;
                            if (startOver && particle.multiplier >= 0.1f && i == 0)
                                particle.multiplier -= fireStopSpeed * Time.deltaTime;
                        }

                        _firesLife[i] = particle.multiplier * 100f;
                        hittedFire++;
                        hittedFireLastIndex = i;
                        
                        if (_firesLife[i] <= 0) //check fire life and turn off
                            particle.Stop();
                        
                        if (fake && i==0)
                        {
                            if (particle.multiplier < 1f && fires[i].activeSelf)
                            {
                                particle.multiplier += fireGrowSpeed * Time.deltaTime;
                                _firesLife[i] = particle.multiplier * 100f;
                            }
                        }
                    }
                    else
                    {      
                        
                        if (particle.multiplier < 1f && fires[i].activeSelf)
                        {
                            particle.multiplier += (fireGrowSpeed * Time.deltaTime);
                            _firesLife[i] = particle.multiplier * 100f;
                        }
                    }
                }
                
                
                allLightsMult += particle.multiplier;
                _sound.volume = overallLife;
                
            }
            if (fires.Length > 0)
            {
                //Debug.LogError($"overallLife: {overallLife} = allLightsMult: {allLightsMult} / lenghth: {fires.Length}");
                overallLife = allLightsMult / fires.Length;
            }
            else
            {
                overallLife = 0;
            }
            if (overallLife < 0.008f) overallLife = 0;
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
            fires[0].SetActive(true);
            var particle = fires[0].GetComponent<ParticleSystemMultiplier>();
            particle.multiplier = 0.8f;
            _started = true;
            Debug.Log("Start fire!");        
            _sound.Play();
            _sound.volume = 0;
            if (_aSoundStart.volume>=0.1) StartCoroutine(AudioFade.FadeOut(_aSoundStart, 5f));
            StartCoroutine(AudioFade.FadeIn(_aSoundFire, 0.5f));
        }
        
    }
}
