using LocalizationSystem;
using TMPro;
using UnityEngine;

public class RTLInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RectTransform placeholderTransform;
    
    private void Start()
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }
        
        // Setup initial direction
        UpdateInputDirection();
        
        // Subscribe to language changes
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateInputDirection;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateInputDirection;
        }
    }
    
    private void UpdateInputDirection()
    {
        if (LocalizationManager.Instance != null && inputField != null)
        {
            var isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Set text alignment
            if (isRTL)
            {
                inputField.textComponent.alignment = TextAlignmentOptions.Right;
                if (inputField.placeholder is TextMeshProUGUI placeholder)
                {
                    placeholder.alignment = TextAlignmentOptions.Right;
                }
                
                // Adjust placeholder position if needed
                if (placeholderTransform != null)
                {
                    Vector2 anchoredPosition = placeholderTransform.anchoredPosition;
                    placeholderTransform.anchoredPosition = new Vector2(-Mathf.Abs(anchoredPosition.x), anchoredPosition.y);
                }
            }
            else
            {
                inputField.textComponent.alignment = TextAlignmentOptions.Left;
                if (inputField.placeholder is TextMeshProUGUI placeholder)
                {
                    placeholder.alignment = TextAlignmentOptions.Left;
                }
                
                // Adjust placeholder position if needed
                if (placeholderTransform != null)
                {
                    Vector2 anchoredPosition = placeholderTransform.anchoredPosition;
                    placeholderTransform.anchoredPosition = new Vector2(Mathf.Abs(anchoredPosition.x), anchoredPosition.y);
                }
            }
        }
    }
}