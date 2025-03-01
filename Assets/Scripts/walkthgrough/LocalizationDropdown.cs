using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LocalizationSystem;  

public class LocalizationDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private void Start()
    {
        // Optionally, clear existing options and add language options dynamically.
        languageDropdown.ClearOptions();
        List<string> options = new List<string> { "English", "Farsi" };
        languageDropdown.AddOptions(options);

        // Listen for dropdown value changes.
        languageDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDestroy()
    {
        languageDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    // Called when the dropdown value is changed.
    private void OnDropdownValueChanged(int index)
    {
        // Ensure that the dropdown options are in the same order as your LanguageOption enum.
        LocalizationManager.LanguageOption selectedLanguage = (LocalizationManager.LanguageOption)index;
        LocalizationManager.Instance.SetLanguage(selectedLanguage);
    }
}