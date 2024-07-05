using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoPlayerControllerScript : MonoBehaviour
    {
        public Text SpellLabel;
        public float Speed = 3.0F;
        public float RotateSpeed = 3.0F;

        public LightningSpellScript[] Spells;
        private GameObject rightHand;
        private int spellIndex;
        private bool spellMouseButtonDown;

        private void Start()
        {
            rightHand = gameObject.transform.Find("RightArm").Find("RightHand").gameObject;
            UpdateSpell();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DemoScript.ReloadCurrentScene();
                return;
            }

            var controller = GetComponent<CharacterController>();
            transform.Rotate(0, Input.GetAxis("Horizontal") * RotateSpeed, 0);
            var forward = transform.TransformDirection(Vector3.forward);
            var curSpeed = Speed * Input.GetAxis("Vertical");
            controller.SimpleMove(forward * curSpeed);

            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                NextSpell();
            else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) PreviousSpell();

            var spell = Spells[spellIndex];
            if (Input.GetButton("Fire1") &&
                (spellMouseButtonDown || !Input.GetMouseButton(0) || GuiElementShouldPassThrough()))
            {
                // Debug.Log("Casting spell " + spell.ToString());

                if (spell.SpellStart != null && spell.SpellStart.GetComponent<Rigidbody>() == null)
                    spell.SpellStart.transform.position = rightHand.transform.position;
                if (Input.GetMouseButton(0))
                {
                    spellMouseButtonDown = true;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    Vector3 rayEnd;
                    // send out a ray from the mouse click - if it collides with something we are interested in, alter the ray direction
                    if (Physics.Raycast(ray, out hit, spell.MaxDistance, spell.CollisionMask))
                        rayEnd = hit.point;
                    else
                        rayEnd = ray.origin + ray.direction * spell.MaxDistance;
                    spell.Direction = (rayEnd - spell.SpellStart.transform.position).normalized;
                }
                else
                {
                    spellMouseButtonDown = false;
                    spell.Direction = gameObject.transform.forward;
                }

                spell.CastSpell();
            }
            else
            {
                spellMouseButtonDown = false;
                spell.StopSpell();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                var r = contact.otherCollider.gameObject.GetComponent<Rigidbody>();
                if (r != null) r.velocity += gameObject.transform.forward * 5.0f;
            }
        }

        private bool GuiElementShouldPassThrough()
        {
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            foreach (var obj in raycastResults)
                if (obj.gameObject.GetComponent<Button>() != null ||
                    (obj.gameObject.GetComponent<Text>() == null && obj.gameObject.GetComponent<Image>() == null))
                    return false;
            return true;
        }

        private void UpdateSpell()
        {
            SpellLabel.text = Spells[spellIndex].name;
            Spells[spellIndex].ActivateSpell();
        }

        private void ChangeSpell(int dir)
        {
            Spells[spellIndex].StopSpell();
            Spells[spellIndex].DeactivateSpell();
            spellIndex += dir;
            if (spellIndex < 0)
                spellIndex = Spells.Length - 1;
            else if (spellIndex >= Spells.Length) spellIndex = 0;
            UpdateSpell();
        }

        public void PreviousSpell()
        {
            ChangeSpell(-1);
        }

        public void NextSpell()
        {
            ChangeSpell(1);
        }
    }
}