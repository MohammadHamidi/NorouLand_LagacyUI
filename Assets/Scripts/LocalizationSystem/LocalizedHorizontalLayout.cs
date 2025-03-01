using UnityEngine;
using UnityEngine.UI;
using LocalizationSystem;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class LocalizedHorizontalLayout : MonoBehaviour
{
    private HorizontalLayoutGroup layoutGroup;

    private void Start()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateLayoutDirection;
            UpdateLayoutDirection(); // Initial setup
        }
        else
        {
            Debug.LogError("LocalizationManager instance is null. Make sure it is initialized and present in the scene.");
        }
    }

    private void UpdateLayoutDirection()
    {
        if (layoutGroup == null) return;

        bool isRtl = LocalizationManager.Instance.IsCurrentLanguageRTL();
        layoutGroup.reverseArrangement = isRtl; // Set reverse arrangement for RTL

        // Force a layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateLayoutDirection;
        }
    }
}