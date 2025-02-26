using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using TMPro;
using LocalizationSystem;

public class ContactInfoController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private TMP_InputField contactInputField;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button remindLaterButton;
    
    [Header("Localization Keys")]
    [SerializeField] private string titleKey = "stay_updated";
    [SerializeField] private string subtitleKey = "info_privacy_note";
    [SerializeField] private string inputPlaceholderKey = "email_phone_placeholder";
    [SerializeField] private string remindLaterKey = "do_later";
    [SerializeField] private string continueButtonKey = "next_button";
    
    private OnboardingFlowManager flowManager;
    private INameStorage nameStorage;
    private bool isRTL;
    
    private void Awake()
    {
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        
        // Initialize name storage
        nameStorage = new PlayerPrefsNameStorage();
        
        // Setup buttons
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        
        if (remindLaterButton != null)
        {
            remindLaterButton.onClick.AddListener(OnRemindLaterButtonClicked);
        }
        
        // Setup input field
        if (contactInputField != null)
        {
            contactInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }
    }
    
    private void Start()
    {
        // Set localized text
        UpdateLocalizedTexts();
        
        // Check RTL/LTR direction
        UpdateTextDirection();
        
        // Set initial button state
        UpdateContinueButton();
        
        // Subscribe to language change
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
        
        // Load previous contact info if available
        LoadSavedContactInfo();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }
        
        if (remindLaterButton != null)
        {
            remindLaterButton.onClick.RemoveListener(OnRemindLaterButtonClicked);
        }
        
        if (contactInputField != null)
        {
            contactInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        }
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void OnLanguageChanged()
    {
        UpdateTextDirection();
        UpdateLocalizedTexts();
    }
    
    private void UpdateTextDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Apply RTL-specific layout adjustments for the input field
            if (contactInputField != null)
            {
                // Update text alignment based on language
                if (contactInputField.textComponent != null)
                {
                    contactInputField.textComponent.alignment = isRTL ? 
                        TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
                
                // Update placeholder alignment based on language
                TMP_Text placeholderText = contactInputField.placeholder as TMP_Text;
                if (placeholderText != null)
                {
                    placeholderText.alignment = isRTL ? 
                        TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
                
                // Hide default caret for RTL (it won't position correctly)
                if (isRTL)
                {
                    contactInputField.customCaretColor = true;
                    Color invisibleColor = contactInputField.caretColor;
                    invisibleColor.a = 0f;
                    contactInputField.caretColor = invisibleColor;
                }
                else
                {
                    contactInputField.customCaretColor = false;
                }
            }
        }
    }
    
    private void UpdateLocalizedTexts()
    {
        if (LocalizationManager.Instance != null)
        {
            // Set title text with name if available
            if (titleText != null)
            {
                string titleValue = LocalizationManager.Instance.GetLocalizedValue(titleKey);
                
                // Check if we need to insert the child's name
                if (nameStorage.HasName() && (titleValue.Contains("[Name]") || titleValue.Contains("(اسم)")))
                {
                    string childName = nameStorage.LoadName();
                    titleValue = titleValue.Replace("[Name]", childName)
                                        .Replace("(اسم)", childName);
                }
                
                titleText.text = titleValue;
            }
            
            // Set subtitle text
            if (subtitleText != null)
            {
                subtitleText.text = LocalizationManager.Instance.GetLocalizedValue(subtitleKey);
            }
            
            // Set input placeholder
            if (contactInputField != null && contactInputField.placeholder != null)
            {
                TMP_Text placeholderText = contactInputField.placeholder as TMP_Text;
                if (placeholderText != null)
                {
                    placeholderText.text = LocalizationManager.Instance.GetLocalizedValue(inputPlaceholderKey);
                }
            }
            
            // Set remind later button text
            if (remindLaterButton != null)
            {
                RTLTextMeshPro remindText = remindLaterButton.GetComponentInChildren<RTLTextMeshPro>();
                if (remindText != null)
                {
                    remindText.text = LocalizationManager.Instance.GetLocalizedValue(remindLaterKey);
                }
            }
            
            // Set continue button text
            if (continueButton != null)
            {
                RTLTextMeshPro continueText = continueButton.GetComponentInChildren<RTLTextMeshPro>();
                if (continueText != null)
                {
                    continueText.text = LocalizationManager.Instance.GetLocalizedValue(continueButtonKey);
                }
            }
        }
    }
    
    private void OnInputFieldValueChanged(string value)
    {
        UpdateContinueButton();
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null && contactInputField != null)
        {
            // Enable continue button only if contact information is entered
            // Optional: Add validation for proper email or phone format
            continueButton.interactable = !string.IsNullOrWhiteSpace(contactInputField.text);
        }
    }
    
    private void OnContinueButtonClicked()
    {
        // Save contact information
        if (contactInputField != null && !string.IsNullOrWhiteSpace(contactInputField.text))
        {
            PlayerPrefs.SetString("ContactInfo", contactInputField.text);
            PlayerPrefs.Save();
        }
    
        // Continue to next page or finish
        if (flowManager != null)
        {
            flowManager.ContinueToNextPage(); // Changed from commented-out call
        }
    }
    
    private void OnRemindLaterButtonClicked()
    {
        // Skip contact info requirement
        if (flowManager != null)
        {
            flowManager.OnRemindMeLaterClicked();
        }
    }
    
    private void LoadSavedContactInfo()
    {
        if (contactInputField != null)
        {
            string savedContact = PlayerPrefs.GetString("ContactInfo", "");
            if (!string.IsNullOrEmpty(savedContact))
            {
                contactInputField.text = savedContact;
                UpdateContinueButton();
            }
        }
    }
}

// Name storage interface implementation (reused from previous artifacts)
