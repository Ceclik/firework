using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class StartLevelFireSpawner : MonoBehaviour
    {
        [SerializeField] private int amountOfFiresOfTypeA;
        [SerializeField] private int amountOfFiresOfTypeB;
        [SerializeField] private int amountOfFiresToSpawn;
        [SerializeField] private Transform fireLocationsParent;

        private Transform[] _fireLocations;
        private GameObject[] _fires;
        private AimFireSystemHandler _fireSystemHandler;
        private FireTypesGenerator _firesGenerator;

        private void Start()
        {
            _firesGenerator = fireLocationsParent.GetComponent<FireTypesGenerator>();
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
            for (int i = 0; i < amountOfFiresToSpawn; i++)
            {
                int indexOfLocation = Random.Range(0, _fireLocations.Length);
                while (_fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing)
                    indexOfLocation = Random.Range(0, _fireLocations.Length);
                
                
                
                _fires[i].SetActive(true);
                _fireSystemHandler.AmountOfActiveFires++;
                
                if (i != 0)
                {
                    _fires[i].transform.position = _fireLocations[indexOfLocation].transform.position;
                    _fireLocations[indexOfLocation].GetComponent<SpawnPoint>().IsUsing = true;
                }
            }
        }

        private void SetFireSystem(int i, int type)
        {
            FireSplitter splitter = _fires[i].GetComponent<FireSplitter>();
            Target target = null;

            switch (type)
            {
                case 0:
                    target = _firesGenerator.GenerateTypeATarget();
                    splitter.Timer = target.TimerTime;
                    
                    break;
            }
            
            //splitter.Timer = 
        }

        public IEnumerator StartRandomFiresDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartRandomFires();
        }
    }
}
