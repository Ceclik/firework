using ExternalAssets.Standard_Assets.ParticleSystems.Scripts;
using Tracking;
using UnityEngine;

namespace Scenes
{
    public class FireSystem : MonoBehaviour
    {
        [SerializeField] public GameObject[] fires;
        [SerializeField] protected float fireStopSpeed = 1f;
        [SerializeField] protected float fireGrowSpeed = 1f;
        [SerializeField] protected FireLight fireLight;
        [SerializeField] protected float overallLife;
        [SerializeField] protected FireStopper stopper;
        [SerializeField] protected GameObject sounds;
        [SerializeField] protected GameObject explosion;
        
        public bool fake;
        public bool startOver;
        
        protected AudioSource ASoundFire;
        protected AudioSource ASoundStart;
        protected bool Ended;

        protected float[] FiresLife;
        protected AudioSource Sound;
        protected bool Started;

        protected virtual void Start()
        {
            FiresLife = new float[fires.Length];
            for (var i = 0; i < FiresLife.Length; i++)
            {
                FiresLife[i] = 100f;
                fires[i].SetActive(false);
            }
        }

        protected void ExtinguishFire(ref ParticleSystemMultiplier particle, ref RaycastHit hit, ref int hitFire,
            ref int hitFireLastIndex, int i, float particleGrowSpeedValue)
        {
            stopper.Orient(hit.point);

            if (!(fake && i == 0) && !(startOver && i == 0))
                particle.multiplier -= fireStopSpeed * Time.deltaTime;
            if (startOver && particle.multiplier >= 0.1f && i == 0)
                particle.multiplier -= fireStopSpeed * Time.deltaTime;
            
            FiresLife[i] = particle.multiplier * 100f;
            hitFire++;
            hitFireLastIndex = i;
            if (FiresLife[i] <= 0) //check fire life and turn off
            {
                particle.Stop();
            }

            if (fake && i == 0)
            {
                if (particle.multiplier < 1f && fires[i].activeSelf)
                {
                    particle.multiplier += particleGrowSpeedValue * Time.deltaTime;
                    FiresLife[i] = particle.multiplier * 100f;
                }
            }
        }

        protected void GrowFire(ref ParticleSystemMultiplier particle, int i, float particleGrowSpeedValue)
        {
            if (particle.multiplier < 1f && fires[i].activeSelf)
            {
                particle.multiplier += particleGrowSpeedValue * Time.deltaTime;
                FiresLife[i] = particle.multiplier * 100f;
            }
        }

        protected virtual void Update()
        {
            var ray = Camera.main!.ScreenPointToRay(new Vector2(MyInput.Instance.X, MyInput.Instance.Y));

            float allLightsMult = 0;
            var activeFiresCount = 0;
            var hitFire = 0;
            var hitFireLastIndex = 0;
            for (var i = 0; i < fires.Length; i++)
            {
                //check mouse cursor in fire            
                UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                var particle = fires[i].GetComponent<ParticleSystemMultiplier>();
                var fireCollider = fires[i].GetComponent<Collider>();

                if (particle.multiplier > 0 && fires[i].activeSelf)
                {
                    if (fireCollider.Raycast(ray, out var hit, Mathf.Infinity) && MyInput.Instance.IsTrackedCursor)
                       ExtinguishFire(ref particle, ref hit, ref hitFire, ref hitFireLastIndex, i, fireGrowSpeed);
                    else
                        GrowFire(ref particle, i, fireGrowSpeed);
                }

                activeFiresCount++;
                allLightsMult += particle.multiplier;
                Sound.volume = overallLife;
            }

            HandleFireLife(allLightsMult, activeFiresCount, hitFire, hitFireLastIndex);
        }

        protected void HandleFireLife(float allLightsMult,  int activeFiresCount, int hitFire, int hitFireLastIndex)
        {
            if (fires.Length > 0)
                overallLife = allLightsMult / fires.Length;
            else
                overallLife = 0;
            if (overallLife < 0.008f) overallLife = 0;
            if (activeFiresCount == 0) overallLife = 0;

            if (hitFire > 0)
                stopper.StartFireStopping(hitFireLastIndex, fake, startOver);
            else
                stopper.StopFireStopping();
        }

        private void OnEnable()
        {
            Sound = GetComponent<AudioSource>();
            var allSounds = sounds.GetComponents<AudioSource>();
            ASoundStart = allSounds[0];
            ASoundFire = allSounds[1];
            StartCoroutine(AudioFade.FadeIn(ASoundStart, 2f));
        }

        protected virtual void OnDisable()
        {
            foreach (var t in fires)
                t.GetComponent<Collider>().enabled = false;

            stopper.Hide();
        }

        public virtual void StartFire()
        {
            if (explosion != null) explosion.SetActive(true);
            fires[0].SetActive(true);
            var particle = fires[0].GetComponent<ParticleSystemMultiplier>();
            particle.multiplier = 0.8f;
            Started = true;
            UnityEngine.Debug.Log("Start fire!");
            Sound.Play();
            Sound.volume = 0;
            if (ASoundStart.volume >= 0.1) StartCoroutine(AudioFade.FadeOut(ASoundStart, 5f));
            StartCoroutine(AudioFade.FadeIn(ASoundFire, 0.5f));
        }
        
    }
}