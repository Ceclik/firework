using UnityEngine;

namespace FireAimScripts
{
    public class NewFiresSpawner : MonoBehaviour
    {
        [SerializeField] private Transform fireAvailableLocations;
        private Transform[] _spawnPoints;
        private FireSplitter[] _fires;
        private AimFireSystemHandler _aimFireSystemHandler;

        private int _newFireIndex = 3;

        private void Start()
        {
            _aimFireSystemHandler = GameObject.Find("FireSystem").GetComponent<AimFireSystemHandler>();
            _spawnPoints = new Transform[fireAvailableLocations.childCount];
            _fires = new FireSplitter[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                _fires[i] = transform.GetChild(i).GetComponent<FireSplitter>();
                _fires[i].OnFireSplitted += SpawnNewFire;
            }

            for (int i = 0; i < fireAvailableLocations.childCount; i++)
                _spawnPoints[i] = fireAvailableLocations.GetChild(i);
        }

        private void SpawnNewFire()
        {
            bool notFree = true;
            int index = 0;
            bool hasFree = false;
            foreach (var point in _spawnPoints)
            {
                if (!point.GetComponent<SpawnPoint>().IsUsing)
                    hasFree = true;
            }
            
            if(!hasFree) return;
            
            while (notFree)
            {
                index = Random.Range(0, _spawnPoints.Length);
                if (!_spawnPoints[index].GetComponent<SpawnPoint>().IsUsing)
                {
                    _spawnPoints[index].GetComponent<SpawnPoint>().IsUsing = true;
                    notFree = false;
                }
            }
            
            _fires[_newFireIndex].gameObject.SetActive(true);
            _aimFireSystemHandler.AmountOfActiveFires++;
            _fires[_newFireIndex].transform.position = _spawnPoints[index].position;
            _newFireIndex++;
        }

        private void OnDisable()
        {
            foreach (var fire in _fires)
                fire.OnFireSplitted -= SpawnNewFire;
        }
    }
}
