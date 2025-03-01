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

public enum TextDirection
{
    Auto,
    ForceLTR,
    ForceRTL
}

public enum TextAlignment
{
    Auto,
    Left,
    Center,
    Right,
    DirectionSpecific
}

/// <summary>
/// Common alignment patterns for RTL/LTR text
/// </summary>
public enum AlignmentPattern
{
    NaturalAlignment, // LTR=Left, RTL=Right (follows reading direction)
    ReverseAlignment, // LTR=Right, RTL=Left (opposite of reading direction)
    AllCenter,        // Center alignment for both
    AllLeft,          // Force left alignment for both
    AllRight          // Force right alignment for both
}

public class LocalizedText : MonoBehaviour, IPointerClickHandler
{
    [Header("Localization Settings")]
    public string localizationKey;
    [SerializeField] private RTLTextMeshPro rtlText;

    [SerializeField] private bool renameObjcet = false;
    [SerializeField] private bool renameParent = false;

    [Header("Direction Settings")]
    [Tooltip("Control how text direction (RTL/LTR) is determined")]
    [SerializeField] private TextDirection textDirection = TextDirection.Auto;
    [Tooltip("Control text alignment")]
    [SerializeField] private TextAlignment textAlignment = TextAlignment.Auto;
    
    [Header("Direction-Specific Alignment")]
    [Tooltip("Alignment to use specifically for LTR text")]
    [SerializeField] private TextAlignment ltrAlignment = TextAlignment.Left;
    [Tooltip("Alignment to use specifically for RTL text")]
    [SerializeField] private TextAlignment rtlAlignment = TextAlignment.Right;
    [Tooltip("When enabled, uses separate alignment settings for RTL and LTR text")]
    [SerializeField] private bool useDirectionSpecificAlignment = false;

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
    
    [Header("RTL Text Settings")]
    [Tooltip("Set to true to preserve numbers in RTL text (useful for prices)")]
    [SerializeField] private bool preserveNumbers = true;

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
            
            // RTL-specific settings
            // rtlText.preserveNumbers = preserveNumbers;
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
            // Get the localized text first
            string localizedText = LocalizationManager.Instance.GetLocalizedValue(localizationKey);
            
            // Always update the font
            UpdateFont();
            
            // Update text direction based on settings
            UpdateTextDirection();
            
            // Update text alignment based on settings
            UpdateTextAlignment();

            // Apply text effects (highlights, strikethrough, etc.)
            ApplyTextEffects(localizedText);
        }
    }

    private void UpdateFont()
    {
        bool isRtl = IsRTLEnabled();
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

    private void ApplyTextEffects(string localizedText)
    {
        // Apply highlight effect if needed.
        if (!string.IsNullOrEmpty(highlightText) && localizedText.Contains(highlightText))
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);
            localizedText = localizedText.Replace(highlightText, $"<color=#{colorHex}>{highlightText}</color>");
        }

        // Apply strikethrough effect
        if (enableStrikethrough)
        {
            // Use RTL marker if text is RTL; otherwise, use LTR marker.
            string marker = IsRTLEnabled() ? "\u200F" : "\u200E";
            localizedText = $"{marker}<s>{marker}{localizedText}{marker}</s>{marker}";
        }

        rtlText.text = localizedText;
    }



    // Helper method to determine if RTL should be enabled
    private bool IsRTLEnabled()
    {
        switch (textDirection)
        {
            case TextDirection.ForceLTR:
                return false;
            case TextDirection.ForceRTL:
                return true;
            case TextDirection.Auto:
            default:
                return LocalizationManager.Instance.IsCurrentLanguageRTL();
        }
    }
    
    private void UpdateTextDirection()
    {
        bool isRtl = IsRTLEnabled();
        rtlText.isRightToLeftText = isRtl;
        
        // Update RTLTMPro-specific settings whenever direction changes
        // rtlText.preserveNumbers = preserveNumbers;
    }
    
    private void UpdateTextAlignment()
    {
        bool isRtl = IsRTLEnabled();
        TextAlignmentOptions alignment;
        
        if (useDirectionSpecificAlignment)
        {
            // Use direction-specific alignment settings
            if (isRtl)
            {
                // For RTL languages
                alignment = rtlAlignment switch
                {
                    TextAlignment.Left => TextAlignmentOptions.Left,
                    TextAlignment.Center => TextAlignmentOptions.Center,
                    TextAlignment.Right => TextAlignmentOptions.Right,
                    _ => TextAlignmentOptions.Right // Default for RTL
                };
            }
            else
            {
                // For LTR languages
                alignment = ltrAlignment switch
                {
                    TextAlignment.Left => TextAlignmentOptions.Left,
                    TextAlignment.Center => TextAlignmentOptions.Center,
                    TextAlignment.Right => TextAlignmentOptions.Right,
                    _ => TextAlignmentOptions.Left // Default for LTR
                };
            }
        }
        else
        {
            // Use the original alignment logic
            switch (textAlignment)
            {
                case TextAlignment.Left:
                    alignment = TextAlignmentOptions.Left;
                    break;
                case TextAlignment.Center:
                    alignment = TextAlignmentOptions.Center;
                    break;
                case TextAlignment.Right:
                    alignment = TextAlignmentOptions.Right;
                    break;
                case TextAlignment.Auto:
                default:
                    // If Auto alignment, use RTL or LTR default alignment
                    alignment = isRtl ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                    break;
            }
        }
        
        rtlText.alignment = alignment;
    }

    public void SetFontStyle(FontStyle style)
    {
        currentFontStyle = style;
        UpdateFont();
    }
    
    public void SetTextDirection(TextDirection direction)
    {
        textDirection = direction;
        UpdateTextDirection();
        UpdateFont(); // Font might need to change based on direction
        UpdateLocalizedText(); // Re-apply everything with proper direction
    }
    
    public void SetTextAlignment(TextAlignment alignment)
    {
        textAlignment = alignment;
        useDirectionSpecificAlignment = false; // Disable direction-specific mode when setting general alignment
        UpdateTextAlignment();
    }
    
    /// <summary>
    /// Sets the alignment for LTR text specifically
    /// </summary>
    public void SetLTRAlignment(TextAlignment alignment)
    {
        ltrAlignment = alignment;
        useDirectionSpecificAlignment = true;
        UpdateTextAlignment();
    }

    /// <summary>
    /// Sets the alignment for RTL text specifically
    /// </summary>
    public void SetRTLAlignment(TextAlignment alignment)
    {
        rtlAlignment = alignment;
        useDirectionSpecificAlignment = true;
        UpdateTextAlignment();
    }

    /// <summary>
    /// Enables direction-specific alignment mode
    /// </summary>
    public void EnableDirectionSpecificAlignment(bool enable)
    {
        useDirectionSpecificAlignment = enable;
        UpdateTextAlignment();
    }

    /// <summary>
    /// Sets both RTL and LTR alignments at once and enables direction-specific mode
    /// </summary>
    public void SetDirectionSpecificAlignments(TextAlignment ltrAlign, TextAlignment rtlAlign)
    {
        ltrAlignment = ltrAlign;
        rtlAlignment = rtlAlign;
        useDirectionSpecificAlignment = true;
        UpdateTextAlignment();
    }
    
    /// <summary>
    /// Configure common alignment patterns with one method call
    /// </summary>
    /// <param name="alignmentPattern">Predefined alignment pattern to apply</param>
    public void SetAlignmentPattern(AlignmentPattern alignmentPattern)
    {
        switch (alignmentPattern)
        {
            case AlignmentPattern.NaturalAlignment:
                // LTR left-aligned, RTL right-aligned (reading direction)
                SetDirectionSpecificAlignments(TextAlignment.Left, TextAlignment.Right);
                break;
                
            case AlignmentPattern.ReverseAlignment:
                // LTR right-aligned, RTL left-aligned (opposite of reading direction)
                SetDirectionSpecificAlignments(TextAlignment.Right, TextAlignment.Left);
                break;
                
            case AlignmentPattern.AllCenter:
                // Both LTR and RTL center-aligned
                SetDirectionSpecificAlignments(TextAlignment.Center, TextAlignment.Center);
                break;
                
            case AlignmentPattern.AllLeft:
                // Force left alignment regardless of direction
                SetDirectionSpecificAlignments(TextAlignment.Left, TextAlignment.Left);
                break;
                
            case AlignmentPattern.AllRight:
                // Force right alignment regardless of direction
                SetDirectionSpecificAlignments(TextAlignment.Right, TextAlignment.Right);
                break;
        }
    }
    
    public void SetStrikethrough(bool enable)
    {
        enableStrikethrough = enable;
        UpdateLocalizedText();
    }
    
    public void SetPreserveNumbers(bool preserve)
    {
        preserveNumbers = preserve;
        if (rtlText != null)
        {
            // rtlText.preserveNumbers = preserveNumbers;
            UpdateLocalizedText();
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
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one for click detection to work.");
        }
    }
}