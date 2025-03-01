using System;
using UnityEngine;
using TMPro;
using LocalizationSystem;
using RTLTMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum FontStyle
{
    Regular,
    Bold,
    Light
}

public class LocalizedText : MonoBehaviour, IPointerClickHandler
{
    [Header("Localization Settings")]
    public string localizationKey;
    [SerializeField] private RTLTextMeshPro rtlText;

    [SerializeField] private bool renameObjcet = false;
    [SerializeField] private bool renameParent = false;

    [Header("Direction Settings")]
    [Tooltip("If true, text direction updates (RTL/LTR) will be ignored.")]
    [SerializeField] private bool ignoreTextDirection = false;

    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset ltrRegularFont;
    [SerializeField] private TMP_FontAsset ltrBoldFont;
    [SerializeField] private TMP_FontAsset ltrLightFont;
    [SerializeField] private TMP_FontAsset rtlRegularFont;
    [SerializeField] private TMP_FontAsset rtlBoldFont;
    [SerializeField] private TMP_FontAsset rtlLightFont;
    [SerializeField] private FontStyle currentFontStyle = FontStyle.Regular;

    [Header("Highlight Settings")]
    [SerializeField] private string highlightText = "60%";
    [SerializeField] private Color highlightColor = new Color(1f, 165f / 255f, 0f);

    [Header("Strikethrough Settings")]
    [SerializeField] private bool enableStrikethrough = false;

    [Header("Link Settings")]
    [SerializeField] private bool isClickable = false;
    [SerializeField] private Color linkColor = new Color(0f, 85f / 255f, 255f / 255f);
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

            if (renameParent && transform.parent != null)
            {
                transform.parent.gameObject.name = $"TP_{localizationKey}";
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
            // If we're not ignoring text direction, update font & alignment based on RTL or LTR
            if (!ignoreTextDirection)
            {
                UpdateFont();
                UpdateTextAlignment();
            }

            // Regardless of ignoring direction or not, still apply the text changes (highlights, strikethrough, etc.)
            UpdateTextWithEffects();
        }
    }

    private void UpdateFont()
    {
        bool isRtl = LocalizationManager.Instance.IsCurrentLanguageRTL();
        TMP_FontAsset selectedFont = null;

        if (isRtl)
        {
            selectedFont = currentFontStyle switch
            {
                FontStyle.Regular => rtlRegularFont,
                FontStyle.Bold    => rtlBoldFont,
                FontStyle.Light   => rtlLightFont,
                _                => rtlRegularFont
            };
        }
        else
        {
            selectedFont = currentFontStyle switch
            {
                FontStyle.Regular => ltrRegularFont,
                FontStyle.Bold    => ltrBoldFont,
                FontStyle.Light   => ltrLightFont,
                _                => ltrRegularFont
            };
        }

        if (selectedFont != null)
        {
            rtlText.font = selectedFont;
        }
        else
        {
            Debug.LogWarning($"Font not assigned for {(isRtl ? "RTL" : "LTR")} with style {currentFontStyle}");
        }
    }

    private void UpdateTextWithEffects()
    {
        string localizedText = LocalizationManager.Instance.GetLocalizedValue(localizationKey);

        // Apply highlight effect
        if (!string.IsNullOrEmpty(highlightText) && localizedText.Contains(highlightText))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);
            localizedText = localizedText.Replace(highlightText, $"<color=#{colorHex}>{highlightText}</color>");
        }

        // Apply strikethrough if enabled
        if (enableStrikethrough)
        {
            localizedText = $"<s>{localizedText}</s>";
        }

        rtlText.text = localizedText;
    }

    private void UpdateTextAlignment()
    {
        bool isRtl = LocalizationManager.Instance.IsCurrentLanguageRTL();
        // If we are ignoring text direction, skip this
        if (ignoreTextDirection) return;

        // Otherwise, align based on RTL or LTR
        if (isRtl)
        {
            rtlText.alignment = TextAlignmentOptions.Right; 
        }
        else
        {
            rtlText.alignment = TextAlignmentOptions.Left; 
        }
    }

    public void SetFontStyle(FontStyle style)
    {
        currentFontStyle = style;
        // Only update font if direction is not ignored
        if (!ignoreTextDirection)
        {
            UpdateFont();
        }
    }

    public void SetStrikethrough(bool enable)
    {
        enableStrikethrough = enable;
        UpdateTextWithEffects();
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
