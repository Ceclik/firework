using ExternalAssets.Standard_Assets.ParticleSystems.Scripts;
using FireSeekingScripts;
using Scenes;
using Tracking;
using UnityEngine;

namespace FireAimScripts
{
    public class AimFireSystemHandler : FireSystem
    {
        private SpawnAdditionalFires _spawnAdditionalFires;
        private FireSplitter[] _splitters;
        private int _livesCount  = 3;

        public int AmountOfActiveFires { get; set; }

        protected override void Start()
        {
            _spawnAdditionalFires = GameObject.Find("Fires").GetComponent<SpawnAdditionalFires>();
            if (!GameManager.Instance.FireAimGameMode)
                GameManager.Instance.FireAimGameMode = true;

            _splitters = new FireSplitter[fires.Length];
            for (var i = 0; i < fires.Length; i++)
            {
                _splitters[i] = fires[i].GetComponent<FireSplitter>();
                _splitters[i].OnFireSplitted += DecreaseLive;
            }
            
            base.Start();
        }

        protected override void Update()
        {
            if ((Started && AmountOfActiveFires == 0 && !Ended) || _livesCount <= 0)
            {
                Ended = true;
                if (_livesCount <= 0)
                    GameManager.Instance.EndScene(false);
                else
                {
                    GameObject.Find("Fires").GetComponent<ScoreCounterInAimMode>().CountScore();
                    GameManager.Instance.EndScene(true);
                }
            }


            var ray = Camera.main!.ScreenPointToRay(new Vector2(MyInput.Instance.X, MyInput.Instance.Y));

            float allLightsMult = 0;
            int activeFiresCount = 0;
            int hitFire = 0;
            int hitFireLastIndex = 0;
            for (var i = 0; i < fires.Length; i++)
            {
                if (fires[i].GetComponent<ParticleSystemMultiplier>().multiplier == 0 && fires[i].activeSelf)
                    DisableFire(i);

                //check mouse cursor in fire            
                UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                var particle = fires[i].GetComponent<ParticleSystemMultiplier>();
                var fireCollider = fires[i].GetComponent<Collider>();

                var particleGrowSpeedValue = fireGrowSpeed;

                if (!fires[i].GetComponent<ParticleSystemMultiplier>().IsGrown)
                    particleGrowSpeedValue = 2f;


                if (particle.multiplier > 0 && fires[i].activeSelf)
                {
                    if (fireCollider.Raycast(ray, out var hit, Mathf.Infinity) && MyInput.Instance.IsTrackedCursor &&
                        fires[i].GetComponent<ParticleSystemMultiplier>().IsGrown)
                    {
                        if (!fires[i].GetComponent<FireSplitter>().IsExtinguishing)
                            fires[i].GetComponent<FireSplitter>().IsExtinguishing = true;

                        fireStopSpeed = fires[i].GetComponent<FireSplitter>().FireStopSpeed;

                        ExtinguishFire(ref particle, ref hit, ref hitFire, ref hitFireLastIndex, i, particleGrowSpeedValue);
                        
                        CountFireComplexParameters(i);
                    }
                    else
                    {
                        if (fires[i].GetComponent<FireSplitter>().IsExtinguishing)
                            fires[i].GetComponent<FireSplitter>().IsExtinguishing = false;

                        GrowFire(ref particle, i, particleGrowSpeedValue);
                    }
                }

                activeFiresCount++;
                allLightsMult += particle.multiplier;
                Sound.volume = overallLife;
            }

            HandleFireLife(allLightsMult, activeFiresCount, hitFire, hitFireLastIndex);
        }

        protected override void OnDisable()
        {
            foreach (var splitter in _splitters)
                splitter.OnFireSplitted -= DecreaseLive;

            stopper.Hide();
        }

        private void DecreaseLive()
        {
            _livesCount--;
        }

        private void DisableFire(int i)
        {
            if (_spawnAdditionalFires.IsNewSpawned)
                _spawnAdditionalFires.IsNewSpawned = false;
            AmountOfActiveFires--;
            fires[i].GetComponent<ParticleSystemMultiplier>().IsFinished = true;
            fires[i].GetComponent<FireSplitter>().OnEndExtinguishing();
            fires[i].SetActive(false);
            UnityEngine.Debug.Log($"Amount of active fires: {AmountOfActiveFires}\ndisabled fire index: {i}");
        }

        private void CountFireComplexParameters(int index)
        {
            var paramsCounter = fires[index].GetComponent<FireComplexParametersCounter>();
            if (!paramsCounter.IsExtinguishing)
            {
                paramsCounter.IsExtinguishing = true;
                paramsCounter.ExtinguishAttempts++;
            }
        }

        public override void StartFire()
        {
            if (explosion != null) explosion.SetActive(true);

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