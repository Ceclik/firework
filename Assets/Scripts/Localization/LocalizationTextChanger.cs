using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    public class LocalizationTextChanger : MonoBehaviour //changing text to the chosen language
    {
        [SerializeField] protected string key;
        protected Text InitializableText;

        protected LocalizationManager LocalizationMgr;

        private void Awake()
        {
            InitializableText = GetComponent<Text>();

            if (LocalizationMgr == null)
                LocalizationMgr = GameObject.FindGameObjectWithTag("LocalizationManager")
                    .GetComponent<LocalizationManager>();

            if (InitializableText == null)
                InitializableText.text = LocalizationMgr.GetLocalizedValue(key);

            LocalizationMgr.OnLanguageChanged += UpdateText;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            LocalizationMgr.OnLanguageChanged -= UpdateText;
        }

        protected virtual void UpdateText()
        {
            if (gameObject == null) return;

            if (LocalizationMgr == null)
                LocalizationMgr = GameObject.FindGameObjectWithTag("LocalizationManager")
                    .GetComponent<LocalizationManager>();

            if (InitializableText == null)
                InitializableText = GetComponent<Text>();

            InitializableText.text = LocalizationMgr.GetLocalizedValue(key);
        }
    }
}