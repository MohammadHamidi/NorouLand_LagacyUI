using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KidRadarChartController : MonoBehaviour
{
    [Header("UI References")]
    public RTLTextMeshPro nameText;
    public RTLTextMeshPro ageText;
    public RTLTextMeshPro taglineText;

    [Header("Axis Labels (for Localization)")]
    public RTLTextMeshPro memoryLabel;
    public RTLTextMeshPro languageLabel;
    public RTLTextMeshPro lifeSkillsLabel;
    public RTLTextMeshPro mathAndLogicLabel;
    public RTLTextMeshPro attentionLabel;

    [Header("Radar Chart Image")]
    public Image radarShapeImage; // The colored polygon

    [Header("Layout / RTL Settings")]
    [Tooltip("Parent container or layout group that flips when isRTL is true.")]
    public RectTransform mainLayout; // We'll flip or anchor if needed

    // Example: you might have a polygon or mesh-based chart that updates
    // skill levels. For simplicity, we show a single sprite image that
    // changes color/shape. If you have a dynamic polygon, you'd update
    // its vertices in code or via a specialized chart library.

    /// <summary>
    /// Populate the UI with the given data.
    /// </summary>
    /// <param name="data">KidRadarData with name, age, tagline, skill levels, sprite, and isRTL.</param>
    public void PopulateChart(KidRadarData data)
    {
        // 1. Update text fields
        // For actual localization, you'd set something like:
        // nameText.text = LocalizationManager.Instance.GetLocalizedValue("kid_name_key");
        // For now, we just set the raw strings from data:
        nameText.text = data.childName;
        ageText.text = data.childAge + (data.isRTL ? " ساله" : " yo");
        taglineText.text = data.tagline;

        // 2. Update skill labels (if using localization, you'd call your loc system)
        // memoryLabel.text = LocalizationManager.Instance.GetLocalizedValue("memory_label");
        // ...
        // For demonstration, we assume they're already assigned in Inspector or updated externally.

        // 3. Update the radar shape sprite (the colored polygon)
        if (radarShapeImage != null && data.radarShapeSprite != null)
        {
            radarShapeImage.sprite = data.radarShapeSprite;
            radarShapeImage.SetNativeSize(); // optional if you want the sprite's original size
        }

        // 4. (Optional) Update a dynamic polygon or skill bars
        //    If you had a dynamic polygon, you'd call something like:
        //    UpdateRadarPolygon(data.memory, data.language, data.lifeSkills, data.mathAndLogic, data.attention);

        // 5. Handle RTL flipping if needed
        HandleRTLLayout(data.isRTL);
    }

    /// <summary>
    /// Example method that flips or re-anchors the UI if the language is RTL.
    /// You can also switch alignment on text fields, etc.
    /// </summary>
    private void HandleRTLLayout(bool isRTL)
    {
        // If you have an entire layout group, you could swap anchor points or pivot
        // For example, flipping horizontally:
        // mainLayout.localScale = isRTL ? new Vector3(-1, 1, 1) : Vector3.one;
        // Then also flip the text alignment if needed.
        if (isRTL)
        {
            // Example: Right-align text
            nameText.alignment = TextAlignmentOptions.Right;
            ageText.alignment = TextAlignmentOptions.Right;
            taglineText.alignment = TextAlignmentOptions.Right;
        }
        else
        {
            // Example: Left-align text
            nameText.alignment = TextAlignmentOptions.Left;
            ageText.alignment = TextAlignmentOptions.Left;
            taglineText.alignment = TextAlignmentOptions.Left;
        }
    }

    // If you want to animate the shape in with DOTween, you can add methods here,
    // e.g., to fade or scale the chart in.
}