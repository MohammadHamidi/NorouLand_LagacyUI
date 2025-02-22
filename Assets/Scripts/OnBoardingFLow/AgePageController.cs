using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using RTLTMPro;
using LocalizationSystem;
using UnityEngine.UI;

public class AgePageController : MonoBehaviour
{
    [System.Serializable]
    public class AgeOption
    {
        public int ageRangeIndex; // 0: Under 5, 1: 5-7, 2: Over 7
        public GameObject optionGameObject;
        public Button selectionButton;
        public Image backgroundImage;
    }
    
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private Button continueButton;
    
    [Header("Age Options")]
    [SerializeField] private AgeOption[] ageOptions;
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color deselectedColor = new Color(0.9f, 0.9f, 0.9f); // Light Gray
    
    private OnboardingFlowManager flowManager;
    private int selectedAgeIndex = -1;
    private bool isRTL;
    
    private void Awake()
    {
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
    }
    
    private void OnEnable()
    {
        // Setup option buttons
        for (int i = 0; i < ageOptions.Length; i++)
        {
            int index = i; // Capture for lambda
            if (ageOptions[i].selectionButton != null)
            {
                ageOptions[i].selectionButton.onClick.AddListener(() => SelectAgeOption(index));
            }
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
        for (int i = 0; i < ageOptions.Length; i++)
        {
            if (ageOptions[i].selectionButton != null)
            {
                ageOptions[i].selectionButton.onClick.RemoveAllListeners();
            }
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
            
            // Apply RTL-specific adjustments if needed
            // (Options are typically in a vertical layout so no reordering is needed)
        }
    }
    
    private void SelectAgeOption(int index)
    {
        // Only process if valid index
        if (index >= 0 && index < ageOptions.Length)
        {
            // Update selected index
            selectedAgeIndex = index;
            
            // Update visuals
            for (int i = 0; i < ageOptions.Length; i++)
            {
                if (ageOptions[i].backgroundImage != null)
                {
                    ageOptions[i].backgroundImage.color = (i == selectedAgeIndex) ? 
                        selectedColor : deselectedColor;
                }
            }
            
            // Update continue button
            UpdateContinueButton();
            
            // Notify flow manager
            if (flowManager != null)
            {
                flowManager.SetChildAge(ageOptions[selectedAgeIndex].ageRangeIndex);
            }
        }
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            // Enable continue button only if an age option is selected
            continueButton.interactable = selectedAgeIndex >= 0;
        }
    }
    
    // Get the selected age
    public int GetSelectedAgeIndex()
    {
        return selectedAgeIndex >= 0 ? ageOptions[selectedAgeIndex].ageRangeIndex : -1;
    }
    
    // Method to update child name in the title if needed (e.g., "How old is [Name]?")
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