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
        private int _indexOfFire;
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
            int[] pickedIndexes = new int[amountOfFiresToSpawn];
            
            for (int i = 0; i < amountOfFiresToSpawn; i++)
                pickedIndexes[i] = -1;
            
            int indexOfPickedFireLocation = 0;
            bool wrongPick = false;
            while (indexOfPickedFireLocation < amountOfFiresToSpawn - 1)
            {
                int indexOfFireLocation = Random.Range(0, fireLocationsParent.childCount);
                foreach(var index in pickedIndexes)
                    if (indexOfFireLocation == index)
                        wrongPick = true;
                
                if(wrongPick) continue;
                
                pickedIndexes[indexOfPickedFireLocation] = indexOfFireLocation;
                indexOfPickedFireLocation++;
            }

            for (int i = 0; i < amountOfFiresToSpawn; i++)
            {
                _fires[i].SetActive(true);
                _fireSystemHandler.AmountOfActiveFires++;
                if(i != 0)
                    _fires[i].transform.position = _fireLocations[pickedIndexes[i]].position;
            }
        }
    }
}
