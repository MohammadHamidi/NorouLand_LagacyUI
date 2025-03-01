using System;
using System.Collections.Generic;
using UnityEngine;



namespace LocalizationSystem
{
    
    
    
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        public static LocalizationManager Instance => _instance;

        private ILocalization currentLocalization;
        public event Action OnLanguageChanged;

        public enum LanguageOption { English, Farsi }

        private void Awake()
        {
        
            _instance = this;
        

            // Set default language (English)
            SetLanguage(LanguageOption.Farsi);
        }

        public void SetLanguage(LanguageOption lang)
        {
            switch (lang)
            {
                case LanguageOption.English:
                    currentLocalization = new EnglishLocalization();
                    break;
                case LanguageOption.Farsi:
                    currentLocalization = new FarsiLocalization();
                    break;
                default:
                    currentLocalization = new EnglishLocalization();
                    break;
            }

            Debug.Log("Language switched to: " + currentLocalization.Language);
            OnLanguageChanged?.Invoke();
        }

        public string GetLocalizedValue(string key)
        {
            if (currentLocalization == null)
            {
                Debug.LogWarning("Localization not set. Returning key as fallback.");
                return key;
            }
            return currentLocalization.GetLocalizedValue(key);
        }

        public bool IsCurrentLanguageRTL()
        {
            return currentLocalization.Language == "Farsi";
        }
    }
}