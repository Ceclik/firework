using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Localization
{
    //loading data from JSON file, setting start language, allowing to get value from loaded array
    public class LocalizationManager : MonoBehaviour
    {
        private string _currentLanguage;
        private Dictionary<string, string> _localizedText;
       
        
        public delegate void ChangeLanguageText();
        public event ChangeLanguageText OnLanguageChanged;

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                PlayerPrefs.SetString("Language", value);
                _currentLanguage = PlayerPrefs.GetString("Language");
                LoadLocalizedText(_currentLanguage);
            }
        }
        
        private void Awake()
        {
            if (!PlayerPrefs.HasKey("Language"))
            {
                if (Application.systemLanguage == SystemLanguage.Russian ||
                    Application.systemLanguage == SystemLanguage.Belarusian)
                {
                    PlayerPrefs.SetString("Language", "ru_RU");
                }
                else
                {
                    PlayerPrefs.SetString("Language", "en_US");
                }
            }
            CurrentLanguage = PlayerPrefs.GetString("Language");
            Debug.Log($"Current language is {_currentLanguage}");
        }

        private void LoadLocalizedText(string languageName)
        {
            var path = Application.streamingAssetsPath + "/Languages/" + languageName + ".json";

            if (File.Exists(path))
            {
                var dataAsJson = File.ReadAllText(path);
                var loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

                _localizedText = new Dictionary<string, string>();
                foreach (var item in loadedData.items) 
                    _localizedText.Add(item.key, item.value);
                
                OnLanguageChanged?.Invoke();
            }
            else
            {
                throw new Exception("Cannot open file!");
            }   
        }

        public string GetLocalizedValue(string key)
        {
            if (_localizedText.TryGetValue(key, out var value))
                return value;
           
            throw new Exception($"Localization text with key {key} not found!");
        }
    }
}
