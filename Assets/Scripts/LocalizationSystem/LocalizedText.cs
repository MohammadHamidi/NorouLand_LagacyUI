using System;
using UnityEngine;
using TMPro;
using LocalizationSystem;
using RTLTMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LocalizedText : MonoBehaviour, IPointerClickHandler
{
    [Header("Localization Settings")]
    public string localizationKey;  // The key to retrieve localized text
    [SerializeField]
    private RTLTextMeshPro rtlText; // Reference to the text component

    [SerializeField] private bool renameObjcet = false;
    [SerializeField] private bool renameParent = false;

    [Header("Link Settings")]
    [SerializeField] private bool isClickable = false;
    [SerializeField] private Color linkColor = new Color(0f, 85f/255f, 255f/255f); // Bright blue color matching the reference
    [SerializeField] private UnityEvent onLinkClicked;

    // Cache the original font settings
    private float originalFontSize;
    private TMP_FontAsset originalFont;
    private bool initialized = false;

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

            if (rtlText != null)
            {
                // UpdateTextWithLinkStyle();
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

        if (!initialized)
        {
            // Cache original settings
            originalFontSize = rtlText.fontSize;
            initialized = true;
        }

        // Ensure text quality settings
        SetupTextQuality();

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

    private void SetupTextQuality()
    {
        if (rtlText != null)
        {
            // Enable auto-sizing within reasonable bounds
            rtlText.enableAutoSizing = true;
            rtlText.fontSizeMin = originalFontSize * 0.8f;
            rtlText.fontSizeMax = originalFontSize * 1.2f;

            // Ensure optimal quality settings
          
            rtlText.overflowMode = TextOverflowModes.Overflow;
            rtlText.renderMode = TextRenderFlags.DontRender | TextRenderFlags.Render;
            
            // Enable subpixel rendering for sharper text
            rtlText.geometrySortingOrder = VertexSortingOrder.Normal;
            rtlText.extraPadding = true;
            
            // Force update the mesh to ensure quality
            rtlText.ForceMeshUpdate();
        }
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
            UpdateTextWithLinkStyle();
        }
    }

    private void UpdateTextWithLinkStyle()
    {
        string localizedText = LocalizationManager.Instance.GetLocalizedValue(localizationKey);
        
        if (isClickable)
        {
            // Add link tags with underline and color matching the reference
            string colorHex = ColorUtility.ToHtmlStringRGB(linkColor);
            rtlText.text = $"<link=\"{localizationKey}\"><color=#{colorHex}><u>{localizedText}</u></color></link>";
            
            // Keep original font size but ensure text quality
            SetupTextQuality();
        }
        else
        {
            rtlText.text = localizedText;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isClickable)
        {
            onLinkClicked?.Invoke();
        }
    }

    private void OnEnable()
    {
        // Make sure there's an EventSystem in the scene
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one for click detection to work.");
        }
    }
}