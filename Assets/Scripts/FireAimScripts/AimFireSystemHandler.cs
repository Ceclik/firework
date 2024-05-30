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
        
        void Start()
        {
            if (!GameManager.Instance.FireAimGameMode)
                GameManager.Instance.FireAimGameMode = true;
            
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
            for (int i = 0; i < fires.Length; i++)
            {
                fires[i].GetComponent<Collider>().enabled = false;
            }
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
            fires[i].SetActive(false);
        }
        
        void Update()
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
                               
                        if (!(fake && i==0) && !(startOver && i==0)) particle.multiplier -= (fireStopSpeed * Time.deltaTime);
                        if (startOver && particle.multiplier>=0.1f && i==0) particle.multiplier -= (fireStopSpeed * Time.deltaTime);
                        _firesLife[i] = particle.multiplier * 100f;
                        hittedFire++;
                        hittedFireLastIndex = i;
                        if (_firesLife[i] <= 0) //check fire life and turn off
                        {
                            particle.Stop();
                            _expandTimers = new float[fires.Length];
                        }         
                        if (fake && i==0)
                        {
                            if (particle.multiplier < 1f && fires[i].activeSelf)
                            {
                                particle.multiplier += (fireGrowSpeed * Time.deltaTime);
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
                        if (particle.multiplier < 1f && fires[i].activeSelf)
                        {
                            particle.multiplier += (fireGrowSpeed * Time.deltaTime);
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
            
            fires[0].SetActive(true);
            fires[2].SetActive(true);
            fires[5].SetActive(true);
            
            
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
