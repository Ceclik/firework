using System.Collections;
using ExternalAssets.Standard_Assets.ParticleSystems.Scripts;
using Scenes;
using Tracking;
using UnityEngine;

namespace FireSeekingScripts
{
    public class SeekingFireSystemHandler : FireSystem
    {
        private int _indexOfExtinguishingComplex;
        private bool _isExploded;
        private bool _isFirstDisabled;
        private bool _isNewFireSpawned = true;
        private FireSystemPathMover _mainFireRoundMover;
        private int _newFireIndex = 1;
        public int AmountOfActiveFires { get; private set; }
        
        protected override void Start()
        {
            if (!GameManager.Instance.FireSeekGameMode)
                GameManager.Instance.FireSeekGameMode = true;

            _mainFireRoundMover = fires[0].GetComponent<FireSystemPathMover>();
            AmountOfActiveFires = 1;

            base.Start();
        }
        
        private new void ExtinguishFire(ref ParticleSystemMultiplier particle, ref RaycastHit hit, ref int hitFire,
            ref int hitFireLastIndex, int i, float particleGrowSpeedValue)
        {
            stopper.Orient(hit.point);

            if (i == 0 && !_mainFireRoundMover.IsRoundPassed)
            {
            }
            else
            {
                _indexOfExtinguishingComplex = i;
                if (!(fake && i == 0) && !(startOver && i == 0))
                    particle.multiplier -= fireStopSpeed * Time.deltaTime;
                if (startOver && particle.multiplier >= 0.1f && i == 0)
                    particle.multiplier -= fireStopSpeed * Time.deltaTime;

                CountComplexParameters(i);
            }

            if (i != _indexOfExtinguishingComplex)
                DisableExtinguishingMode(i);

            FiresLife[i] = particle.multiplier * 100f;
            hitFire++;
            hitFireLastIndex = i;

            if (FiresLife[i] <= 0) //check fire life and turn off
                particle.Stop();

            if (fake && i == 0)
                if (particle.multiplier < 1f && fires[i].activeSelf)
                {
                    particle.multiplier += particleGrowSpeedValue * Time.deltaTime;
                    FiresLife[i] = particle.multiplier * 100f;
                }
        }
        

        protected override void Update()
        {
            if (_mainFireRoundMover.IsRoundPassed && !_isExploded)
            {
                _isExploded = true;
                SpawnNewFire();
                fires[0].GetComponent<ParticleSystemMultiplier>().multiplier = 0.01f;
            }

            if (Started && AmountOfActiveFires == 0 && !Ended)
            {
                Ended = true;
                StartCoroutine(GetComponent<SceneFinisher>().FinishScene());
            }

            DisableAllExtinguishingModes();

            var ray = Camera.main!.ScreenPointToRay(new Vector2(MyInput.Instance.X, MyInput.Instance.Y));

            //checking if main fire is hitted
            HandleMainFireMovement(ray);

            float allLightsMult = 0;
            var hitFire = 0;
            var hitFireLastIndex = 0;
            for (var i = 0; i < fires.Length; i++)
            {
                if (fires[i].GetComponent<ParticleSystemMultiplier>().multiplier == 0 && fires[i].activeSelf)
                    DisableFire(i);


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


                allLightsMult += particle.multiplier;
                Sound.volume = overallLife;
            }

            HandleFireLife(allLightsMult, 1, hitFire, hitFireLastIndex);
        }

        private void HandleMainFireMovement(Ray ray)
        {
            if (fires[0].GetComponent<Collider>().Raycast(ray, out var hitMain, Mathf.Infinity) &&
                MyInput.Instance.IsTrackedCursor)
            {
                stopper.Orient(hitMain.point);
                if (!_mainFireRoundMover.IsMoving) _mainFireRoundMover.IsMoving = true;
                if (_mainFireRoundMover.SlowMoving) _mainFireRoundMover.SlowMoving = false;
                if (_isNewFireSpawned) _isNewFireSpawned = false;
            }

            else
            {
                if (_mainFireRoundMover.IsMoving)
                    _mainFireRoundMover.SlowMoving = true;
                if (!_isNewFireSpawned && !_mainFireRoundMover.IsRoundPassed &&
                    !_mainFireRoundMover.IsMovingToNextSegment)
                {
                    SpawnNewFire();
                    GetComponent<ScoreCounterForSeekMode>().MistakesAmount++;
                }
            }
        }

        private void SpawnNewFire()
        {
            fires[_newFireIndex].SetActive(true);
            fires[_newFireIndex].transform.position = fires[0].transform.position;
            fires[_newFireIndex].GetComponent<ParticleSystemMultiplier>().multiplier = 0.4f;
            _isNewFireSpawned = true;
            _newFireIndex++;
            AmountOfActiveFires++;
        }

        private void DisableFire(int i)
        {
            if (i == 0 && !_isFirstDisabled)
            {
                _isFirstDisabled = true;
                AmountOfActiveFires--;
            }

            if (i != 0)
            {
                fires[i].SetActive(false);
                AmountOfActiveFires--;
            }
        }

        private void CountComplexParameters(int index)
        {
            var paramsCounter = fires[index].GetComponent<FireComplexParametersCounter>();
            if (!paramsCounter.IsExtinguishing)
            {
                paramsCounter.IsExtinguishing = true;
                paramsCounter.ExtinguishAttempts++;
            }

            if (paramsCounter.IsExtinguishing)
                paramsCounter.TimeOfExtinguishing += Time.deltaTime;
        }

        private void DisableExtinguishingMode(int index)
        {
            fires[index].GetComponent<FireComplexParametersCounter>().IsExtinguishing = false;
        }

        private void DisableAllExtinguishingModes()
        {
            if (!stopper.SoundPlaying)
                foreach (var fire in fires)
                    fire.GetComponent<FireComplexParametersCounter>().IsExtinguishing = false;
        }

        public override void StartFire()
        {
            if (explosion != null)
            {
                explosion.SetActive(true);
                StartCoroutine(ExplosionDeactivate());
            }

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

        private IEnumerator ExplosionDeactivate()
        {
            yield return new WaitForSeconds(3);
            explosion.SetActive(false);
        }
    }
}