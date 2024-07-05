using UnityEngine;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoScriptManualAutomatic : MonoBehaviour
    {
        public GameObject LightningPrefab;
        public Toggle AutomaticToggle;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0.0f;
                LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().Trigger(null, worldPos);
            }
        }

        public void AutomaticToggled()
        {
            LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().ManualMode = !AutomaticToggle.isOn;
        }

        public void ManualTriggerClicked()
        {
            LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().Trigger();
        }
    }
}