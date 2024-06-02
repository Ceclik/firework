using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireAimScripts
{
    public class StartLevelFireSpawner : MonoBehaviour
    {
        [SerializeField] private int amountOfFiresToSpawn;
        [SerializeField] private Transform fireLocationsParent;

        private Transform[] _fireLocations;
        private GameObject[] _fires;
        private AimFireSystemHandler _fireSystemHandler;

        private void Start()
        {
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
    }
}
