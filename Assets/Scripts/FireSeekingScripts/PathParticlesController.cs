using UnityEngine;

namespace FireSeekingScripts
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PathParticlesController : MonoBehaviour
    {
        [SerializeField] private float distanceThreshold;
        private Vector3 _lastEmittedPosition;
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, _lastEmittedPosition) >= distanceThreshold)
            {
                EmitParticles();
                _lastEmittedPosition = transform.position;
            }
        }

        private void EmitParticles()
        {
            if (_particleSystem != null)
                _particleSystem.Emit(50);
        }
    }
}
