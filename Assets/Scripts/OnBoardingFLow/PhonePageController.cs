using LocalizationSystem;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

public class PhonePageController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField phoneInputField;
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private RTLTextMeshPro placeholderText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button remindLaterButton;
    
    [Header("Validation")]
    [SerializeField] private bool requirePhoneNumber = false;
    [SerializeField] private int minPhoneLength = 10;
    
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
        if (phoneInputField != null)
        {
            phoneInputField.onValueChanged.AddListener(OnPhoneChanged);
            
            // Set focus to input field
            phoneInputField.Select();
            phoneInputField.ActivateInputField();
        }
        
        // Setup remind later button
        if (remindLaterButton != null)
        {
            remindLaterButton.onClick.AddListener(OnRemindLaterClicked);
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
        if (phoneInputField != null)
        {
            phoneInputField.onValueChanged.RemoveListener(OnPhoneChanged);
        }
        
        if (remindLaterButton != null)
        {
            remindLaterButton.onClick.RemoveAllListeners();
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
            if (phoneInputField != null)
            {
                // Set text alignment
                phoneInputField.textComponent.alignment = isRTL ? 
                    TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                
                // Set placeholder alignment
                if (phoneInputField.placeholder is TextMeshProUGUI placeholder)
                {
                    placeholder.alignment = isRTL ? 
                        TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
            }
        }
    }
    
    private void OnPhoneChanged(string phoneNumber)
    {
        UpdateContinueButton();
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null && phoneInputField != null)
        {
            if (requirePhoneNumber)
            {
                // Enable continue button only if phone meets minimum length
                continueButton.interactable = phoneInputField.text.Trim().Length >= minPhoneLength;
            }
            else
            {
                // Always enable if phone number is optional
                continueButton.interactable = true;
            }
        }
    }
    
    private void OnRemindLaterClicked()
    {
        // Skip phone number requirement and continue
        if (flowManager != null)
        {
            flowManager.OnRemindMeLaterClicked();
        }
    }
    
    // Get the entered phone number
    public string GetPhoneNumber()
    {
        return phoneInputField != null ? phoneInputField.text.Trim() : "";
    }
    
    // Method to update child name in the title if needed
    public void UpdateChildNameInTitle(string childName)
    {
        if (titleText != null && !string.IsNullOrEmpty(childName))
        {
            string currentText = titleText.text;
            currentText = currentText.Replace("[Name]", childName);
            titleText.text = currentText;
        }
    }
}