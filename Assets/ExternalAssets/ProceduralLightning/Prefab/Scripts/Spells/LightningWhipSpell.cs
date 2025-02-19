﻿//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    public class LightningWhipSpell : LightningSpellScript
    {
        [Header("Whip")] [Tooltip("Attach the whip to what object")]
        public GameObject AttachTo;

        [Tooltip("Rotate the whip with this object")]
        public GameObject RotateWith;

        [Tooltip("Whip handle")] public GameObject WhipHandle;

        [Tooltip("Whip start")] public GameObject WhipStart;

        [Tooltip("Whip spring")] public GameObject WhipSpring;

        [Tooltip("Whip crack audio source")] public AudioSource WhipCrackAudioSource;

        /// <summary>
        ///     Callback for when the whip strikes a point
        /// </summary>
        [HideInInspector] public Action<Vector3> CollisionCallback;

        protected override void Start()
        {
            base.Start();

            WhipSpring.SetActive(false);
            WhipHandle.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();

            gameObject.transform.position = AttachTo.transform.position;
            gameObject.transform.rotation = RotateWith.transform.rotation;
        }

        private IEnumerator WhipForward()
        {
            const float distanceBack = 25.0f;
            const float springForwardTime = 0.10f;
            const float springBackwardTime = 0.25f;
            const float strikeWaitTime = 0.1f;
            const float recoilWaitTime = 0.1f;

            // remove the drag from all objects so they can move rapidly without decay
            for (var i = 0; i < WhipStart.transform.childCount; i++)
            {
                var obj = WhipStart.transform.GetChild(i).gameObject;
                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.drag = 0.0f;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            // activate the spring
            WhipSpring.SetActive(true);

            var anchor = WhipStart.GetComponent<Rigidbody>().position;

            // calculate the forward position first
            RaycastHit hit;
            Vector3 whipPositionForwards, whipPositionBackwards;
            if (Physics.Raycast(anchor, Direction, out hit, MaxDistance, CollisionMask))
            {
                var dir = (hit.point - anchor).normalized;
                whipPositionForwards = anchor + dir * MaxDistance;

                // put the spring behind the whip to yank it back in the opposite of the direction
                whipPositionBackwards = anchor - dir * distanceBack;
            }
            else
            {
                whipPositionForwards = anchor + Direction * MaxDistance;

                // put the spring behind the whip to yank it back in the opposite of the direction
                whipPositionBackwards = anchor - Direction * distanceBack;
            }


            //whipPositionBackwards -= (WhipStart.transform.forward * distanceBack);
            //whipPositionBackwards += (WhipStart.transform.up * 5.0f);

            // set back position
            WhipSpring.GetComponent<Rigidbody>().position = whipPositionBackwards;

            // wait a bit
            yield return new WaitForSeconds(springBackwardTime);

            // now put the spring in front of the whip to pull it forward
            WhipSpring.GetComponent<Rigidbody>().position = whipPositionForwards;

            yield return new WaitForSeconds(springForwardTime);

            // play whip crack sound
            if (WhipCrackAudioSource != null) WhipCrackAudioSource.Play();

            yield return new WaitForSeconds(strikeWaitTime);

            // show the strike paticle system
            if (CollisionParticleSystem != null) CollisionParticleSystem.Play();

            // create collision wherever the whip hit
            ApplyCollisionForce(SpellEnd.transform.position);

            // turn off the spring
            WhipSpring.SetActive(false);

            if (CollisionCallback != null) CollisionCallback(SpellEnd.transform.position);

            // wait a bit longer for the whip to recoil
            yield return new WaitForSeconds(recoilWaitTime);

            // put the drag back on
            for (var i = 0; i < WhipStart.transform.childCount; i++)
            {
                var obj = WhipStart.transform.GetChild(i).gameObject;
                var rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.drag = 0.5f;
                }
            }
        }

        protected override void OnCastSpell()
        {
            StartCoroutine(WhipForward());
        }

        protected override void OnStopSpell()
        {
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            WhipHandle.SetActive(true);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            WhipHandle.SetActive(false);
        }
    }
}