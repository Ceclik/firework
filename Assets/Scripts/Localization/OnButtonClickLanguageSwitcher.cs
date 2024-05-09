using UnityEngine;

namespace Localization
{
    public class OnButtonClickLanguageSwitcher : MonoBehaviour
    {
        [SerializeField] private LocalizationManager localizationManager;
        
        public void OnButtonClick()
        {
            localizationManager.CurrentLanguage = name;
        }
    }
}
