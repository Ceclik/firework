using Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;

namespace FireSeekingScripts
{
    public class NewFireSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fireComplexPrefab;
        [SerializeField] private FireSystemPathMover mainFire;
        [SerializeField] private Transform fireComplexParent;

        public bool IsSpawned { get; set; }

        public GameObject SpawnNewFireComplex()
        {
            GameObject newFireComplex = null;
            if (!IsSpawned)
            {
                newFireComplex = Instantiate(fireComplexPrefab, mainFire.transform.position, Quaternion.identity,
                    fireComplexParent);
                var particle = newFireComplex.GetComponent<ParticleSystemMultiplier>();
                particle.multiplier = 0.4f;
                IsSpawned = true;
            }

            return newFireComplex;
        }
    }
}