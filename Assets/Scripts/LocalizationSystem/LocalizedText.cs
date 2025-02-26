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

    [Header("Highlight Settings")]
    [SerializeField] private string highlightText = "60%"; // Text to highlight
    [SerializeField] private Color highlightColor = new Color(1f, 165f / 255f, 0f); // Default Orange (#FFA500)

    [Header("Link Settings")]
    [SerializeField] private bool isClickable = false;
    [SerializeField] private Color linkColor = new Color(0f, 85f / 255f, 255f / 255f); // Bright blue color matching the reference
    [SerializeField] private UnityEvent onLinkClicked;

    private float originalFontSize;
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
                // UpdateLocalizedText();
            }
        }
    }

    private void Start()
    {
        rtlText = GetComponent<RTLTextMeshPro>();

        if (string.IsNullOrEmpty(localizationKey) || rtlText == null)
            return;

        if (!initialized)
        {
            originalFontSize = rtlText.fontSize;
            initialized = true;
        }

        SetupTextQuality();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateLocalizedText;
            UpdateLocalizedText();
        }
        else
        {
            Debug.LogError("LocalizationManager instance is null. Make sure it is initialized and present in the scene.");
        }
    }

    private void SetupTextQuality()
    {
        if (rtlText != null)
        {
            rtlText.enableAutoSizing = true;
            rtlText.fontSizeMin = originalFontSize * 0.8f;
            rtlText.fontSizeMax = originalFontSize * 1.2f;
            rtlText.overflowMode = TextOverflowModes.Overflow;
            rtlText.renderMode = TextRenderFlags.DontRender | TextRenderFlags.Render;
            rtlText.geometrySortingOrder = VertexSortingOrder.Normal;
            rtlText.extraPadding = true;
            rtlText.ForceMeshUpdate();
        }
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateLocalizedText;
        }
    }

    private void UpdateLocalizedText()
    {
        if (!string.IsNullOrEmpty(localizationKey) && rtlText != null)
        {
            UpdateTextWithHighlight();
        }
    }

    private void UpdateTextWithHighlight()
    {
        string localizedText = LocalizationManager.Instance.GetLocalizedValue(localizationKey);

        if (!string.IsNullOrEmpty(highlightText) && localizedText.Contains(highlightText))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);
            localizedText = localizedText.Replace(highlightText, $"<color=#{colorHex}>{highlightText}</color>");
        }

        rtlText.text = localizedText;
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
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one for click detection to work.");
        }
    }
}
