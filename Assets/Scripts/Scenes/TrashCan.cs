using ExternalAssets.Standard_Assets.ParticleSystems.Scripts;
using UnityEngine;

//This script launches an animation of trash can fall.

namespace Scenes
{
    public class TrashCan : MonoBehaviour
    {
        public GameObject fire;
        public GameObject calendarFire1;
        public GameObject calendarFire2;

        [HideInInspector] public bool falled;

        private bool falling;
        private float fallingTime;

        private ParticleSystemMultiplier multiplier;

        private void Update()
        {
            if (falling) fallingTime += Time.deltaTime;
        }


        //In my opinion, the animation of fall is simple and could be created in code using DOTWeen.
        public void TrashFall()
        {
            falled = true;
            var trashCanAnim = GetComponent<Animator>();
            trashCanAnim.SetTrigger("TrashFall");
            UnityEngine.Debug.Log("trash fall");
            //multiplier = fire.GetComponent<ParticleSystemMultiplier>();
            //multiplier.multiplier = 0;
            falling = true;
            GameManager.Instance.StartFire();
        }
    }
}