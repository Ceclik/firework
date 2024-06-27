using FireSeekingScripts;
using Standard_Assets.ParticleSystems.Scripts;
using Tracking;
using UnityEngine;
using UnityStandardAssets.Effects;

namespace FireAimScripts
{
    public class AimFireSystemHandler : MonoBehaviour
    {
        [SerializeField] private GameObject[] fires;
        [SerializeField] private float fireStopSpeed = 1f;
        [SerializeField] private float fireGrowSpeed = 1f;
        [SerializeField] private FireLight fireLight;
        [SerializeField] private float overallLife;
        [SerializeField] private FireStopper stopper;
        [SerializeField] private GameObject explosion;
        [SerializeField] private GameObject sounds;
        
        private FireSplitter[] _splitters;

        public bool fake;
        public bool startOver;   

        private float[] _firesLife;
        private float[] _expandTimers;
        private float _startFireIntensity;
        private bool _started;
        private bool _ended;
        private AudioSource _sound;
        private AudioSource _aSoundStart;
        private AudioSource _aSoundFire;

        public int LivesCount { get; private set; } = 3;

        public int AmountOfActiveFires { get; set; }

        private void DecreaseLive()
        {
            LivesCount--;
        }
        
        private void Start()
        {
            if (!GameManager.Instance.FireAimGameMode)
                GameManager.Instance.FireAimGameMode = true;

            _splitters = new FireSplitter[fires.Length];
            for (int i = 0; i < fires.Length; i++)
            {
                _splitters[i] = fires[i].GetComponent<FireSplitter>();
                _splitters[i].OnFireSplitted += DecreaseLive;
            }


            _firesLife = new float[fires.Length];
            _expandTimers = new float[fires.Length];
            for (int i = 0; i < _firesLife.Length; i++)
            {
                _firesLife[i] = 100f;
                _expandTimers[i] = 0f;
                fires[i].SetActive(false);            
            }
            _startFireIntensity = fireLight.intensity;        
        }

        private void OnDisable()
        {
            foreach (var splitter in _splitters)
                splitter.OnFireSplitted -= DecreaseLive;
            
            stopper.Hide();
        }

        private void OnEnable()
        {
            _sound = GetComponent<AudioSource>();
            var allSounds = sounds.GetComponents<AudioSource>();
            _aSoundStart = allSounds[0];
            _aSoundFire = allSounds[1];
            StartCoroutine(AudioFade.FadeIn(_aSoundStart, 2f));        
        }
        
        private void DisableFire(int i)
        {
            AmountOfActiveFires--; 
            fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished = true;
            fires[i].GetComponent<FireSplitter>().OnEndExtinguishing(); //TODO delete
            fires[i].SetActive(false);
            Debug.LogError($"Amount of active fires: {AmountOfActiveFires}");
        }
        
        private void CountFireComplexParameters(int index)
        {
            FireComplexParametersCounter paramsCounter = fires[index].GetComponent<FireComplexParametersCounter>();
            if (!paramsCounter.IsExtinguishing)
            {
                paramsCounter.IsExtinguishing = true;
                paramsCounter.ExtinguishAttempts++;
            }
        }
        
        private void Update()
        {
            if ((_started && AmountOfActiveFires == 0 && !_ended) || LivesCount <= 0)
            /*if ((_started && AmountOfActiveFires == 0 && !_ended))*/
            {
                
                _ended = true;
                if(LivesCount <= 0)
                    GameManager.Instance.EndScene(false);
                else
                {
                    GetComponent<ScoreCounterInAimMode>().CountScore();
                    GameManager.Instance.EndScene(true);
                }
            }
        

            Ray ray = Camera.main.ScreenPointToRay(new Vector2(MyInput.Instance.X,MyInput.Instance.Y));

            float allLightsMult = 0;
            int activeFiresCount = 0;
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

                float particleGrowSpeedValue = fireGrowSpeed;
                
                if (!fires[i].GetComponent<ParticleSystemMultiplier>().IsGrown)
                    particleGrowSpeedValue = 2f;
                
                
                if (particle.multiplier > 0 && fires[i].activeSelf)
                {
                    if (fireCollider.Raycast(ray, out hit, Mathf.Infinity) && MyInput.Instance.IsTrackedCursor &&
                        fires[i].GetComponent<ParticleSystemMultiplier>().IsGrown)
                    {
                        if (!fires[i].GetComponent<FireSplitter>().IsExtinguishing)
                            fires[i].GetComponent<FireSplitter>().IsExtinguishing = true;
                        
                        stopper.Orient(hit.point);

                        fireStopSpeed = fires[i].GetComponent<FireSplitter>().FireStopSpeed;
                        
                        if (!(fake && i == 0) && !(startOver && i == 0))
                            particle.multiplier -= (fireStopSpeed * Time.deltaTime);
                        if (startOver && particle.multiplier >= 0.1f && i == 0)
                            particle.multiplier -= (fireStopSpeed * Time.deltaTime);

                        CountFireComplexParameters(i);

                        _firesLife[i] = particle.multiplier * 100f;
                        hittedFire++;
                        hittedFireLastIndex = i;
                        if (_firesLife[i] <= 0) //check fire life and turn off
                        {
                            particle.Stop();
                            _expandTimers = new float[fires.Length];
                        }

                        if (fake && i == 0)
                        {
                            if (particle.multiplier < 1f && fires[i].activeSelf)
                            {
                                particle.multiplier += (particleGrowSpeedValue * Time.deltaTime);
                                _firesLife[i] = particle.multiplier * 100f;
                            }

                            if (particle.multiplier >= 1f && fires[i].activeSelf)
                            {
                                _expandTimers[i] = _expandTimers[i] + Time.deltaTime;
                            }
                        }
                    }
                    else
                    {      
                        if (fires[i].GetComponent<FireSplitter>().IsExtinguishing)
                            fires[i].GetComponent<FireSplitter>().IsExtinguishing = false;
                        
                        if (particle.multiplier < 1f && fires[i].activeSelf)
                        {
                            particle.multiplier += (particleGrowSpeedValue * Time.deltaTime);
                            _firesLife[i] = particle.multiplier * 100f;
                        }
                        if (particle.multiplier>=1f && fires[i].activeSelf)
                        {
                            _expandTimers[i] = _expandTimers[i] + Time.deltaTime;
                        }
                    }
                }

                activeFiresCount++;
                allLightsMult += particle.multiplier;
                _sound.volume = overallLife;

            }
            if (fires.Length > 0)
            {
                overallLife = allLightsMult / (float)fires.Length;
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
