using LocalizationSystem;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using RTLTMPro;
using LocalizationSystem;
using UnityEngine.UI;

public class NamePageController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
   
    [SerializeField] private Button continueButton;
    
    [Header("Validation")]
    [SerializeField] private int minNameLength = 1;
    
    private OnboardingFlowManager flowManager;
    private bool isRTL;
    
    private void Awake()
    {
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
    }
    
    private void OnEnable()
    {
        // Setup input field
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.AddListener(OnNameChanged);
            
            // Set focus to input field
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
        
        // Check RTL/LTR direction
        UpdateTextDirection();
        
        // Initial button state
        UpdateContinueButton();
        
        // Subscribe to language change
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateTextDirection;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.RemoveListener(OnNameChanged);
        }
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateTextDirection;
        }
    }
    
    private void UpdateTextDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Configure input field direction
            if (nameInputField != null)
            {
                // Set text alignment
                nameInputField.textComponent.alignment = isRTL ? 
                    TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                
                // Set placeholder alignment
                if (nameInputField.placeholder is TextMeshProUGUI placeholder)
                {
                    placeholder.alignment = isRTL ? 
                        TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
            }
        }
    }
    
    private void OnNameChanged(string name)
    {
        UpdateContinueButton();
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null && nameInputField != null)
        {
            // Enable continue button only if name meets minimum length
            continueButton.interactable = nameInputField.text.Trim().Length >= minNameLength;
        }
    }
    
    // Get the entered name
    public string GetChildName()
    {
        return nameInputField != null ? nameInputField.text.Trim() : "";
    }
    
    // Method to update placeholder if needed
    
}