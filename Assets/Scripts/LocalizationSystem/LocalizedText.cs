using System;
using UnityEngine;
using TMPro;
using LocalizationSystem;
using RTLTMPro;

public class LocalizedText : MonoBehaviour
{
    [Header("Localization Settings")]
    public string localizationKey;  // The key to retrieve localized text
    [SerializeField]
    private RTLTextMeshPro rtlText; // Reference to the text component

    [SerializeField] private bool renameObjcet = false;
    [SerializeField] private bool renameParent = false;
    
    private void OnValidate()
    {
        if (!string.IsNullOrWhiteSpace(localizationKey))
        {
            if (renameObjcet)
            {
                gameObject.name = localizationKey;    
            }

            if (renameParent)
            {
                transform.parent.gameObject.name = $"TP_{localizationKey}";
            }

            
            if (rtlText!=null)
            {
                rtlText.text = localizationKey;
            }
        }
    }

    private void Start()
    {
        // Try to get the RTLTextMeshPro component on this GameObject
        rtlText = GetComponent<RTLTextMeshPro>();

        // If key is empty or component not found, do nothing
        if (string.IsNullOrEmpty(localizationKey) || rtlText == null)
            return;

        // Subscribe to the language change event
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateLocalizedText;
            UpdateLocalizedText();
        }
        else
        {
            Debug.LogError("LocalizationManager instance is null. Make sure it is initialized and present in the scene.");
        }


        // Set the initial text value
        UpdateLocalizedText();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateLocalizedText;
        }
    }

    private void UpdateLocalizedText()
    {
        if (!string.IsNullOrEmpty(localizationKey) && rtlText != null)
        {
            rtlText.text = LocalizationManager.Instance.GetLocalizedValue(localizationKey);
        }
    }
}