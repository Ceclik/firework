using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class StartLevelFireSpawner : MonoBehaviour
    {
        [SerializeField] private int amountOfFiresOfTypeA;
        [SerializeField] private int amountOfFiresOfTypeB;
        [SerializeField] private Transform fireLocationsParent;

        private Transform[] _fireLocations;
        private GameObject[] _fires;
        private AimFireSystemHandler _fireSystemHandler;
        private FireTypesGenerator _firesGenerator;

        private void Start()
        {
            _firesGenerator = GetComponent<FireTypesGenerator>();
            _fireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _fires = new GameObject[transform.childCount];
            _fireLocations = new Transform[fireLocationsParent.childCount];

            for (int i = 0; i < fireLocationsParent.childCount; i++)
                _fireLocations[i] = fireLocationsParent.GetChild(i);
            
            for (int i = 0; i < transform.childCount; i++)
                _fires[i] = transform.GetChild(i).gameObject;
            
        }

        public void StartRandomFires()
        {
            int indexOfFireType = 1;
            
            for (int i = 0; i < amountOfFiresOfTypeA + amountOfFiresOfTypeB; i++)
            {
                if (indexOfFireType <= amountOfFiresOfTypeA)
                    SetFireSystem(i, 0);
                 
                else if (indexOfFireType > amountOfFiresOfTypeA &&
                         indexOfFireType <= amountOfFiresOfTypeA + amountOfFiresOfTypeB)
                    SetFireSystem(i, 1);

                indexOfFireType++;

                _fires[i].SetActive(true);
                _fireSystemHandler.AmountOfActiveFires++;
                
                if (i != 0)
                {
                    int indexOfLocation = Random.Range(0, _fireLocations.Length);
                    while (_fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing)
                        indexOfLocation = Random.Range(0, _fireLocations.Length);
                    _fires[i].transform.position = _fireLocations[indexOfLocation].transform.position;
                    _fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing = true;
                }
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

        public IEnumerator StartRandomFiresDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartRandomFires();
        }
    }
}
