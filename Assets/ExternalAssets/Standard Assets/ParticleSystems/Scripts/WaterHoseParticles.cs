using System.Collections.Generic;
using UnityEngine;

namespace ExternalAssets.Standard_Assets.ParticleSystems.Scripts
{
    public class WaterHoseParticles : MonoBehaviour
    {
        public static float lastSoundTime;
        public float force = 1;


        private readonly List<ParticleCollisionEvent> m_CollisionEvents = new();
        private ParticleSystem m_ParticleSystem;


        private void Start()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }


        private void OnParticleCollision(GameObject other)
        {
            var numCollisionEvents = m_ParticleSystem.GetCollisionEvents(other, m_CollisionEvents);
            var i = 0;

            while (i < numCollisionEvents)
            {
                if (Time.time > lastSoundTime + 0.2f) lastSoundTime = Time.time;

                var col = m_CollisionEvents[i].colliderComponent;
                var attachedRigidbody = col.GetComponent<Rigidbody>();
                if (attachedRigidbody != null)
                {
                    var vel = m_CollisionEvents[i].velocity;
                    attachedRigidbody.AddForce(vel * force, ForceMode.Impulse);
                }

                other.BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

                i++;
            }
        }
    }
}