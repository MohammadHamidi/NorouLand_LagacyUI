using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgePageController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private Button continueButton;
    
    // Change this line in the AgePageController
    [SerializeField] private string nameFormatKey = "child_age_question"; // Instead of "age_page_title_format"

// And update this method to handle the specific format
   
    private INameStorage nameStorage;
    private OnboardingFlowManager flowManager;
    private AgeSelectionButton[] ageButtons;
    private int selectedAgeIndex = -1;
    private bool isRTL;
    
    private void Awake()
    {
        Debug.Log("AgePageController Awake called");
    
        // Find age selection buttons in children
        ageButtons = GetComponentsInChildren<AgeSelectionButton>();
    
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        if (flowManager == null)
            Debug.LogError("AgePageController: FlowManager is NULL!");
        else
            Debug.Log("AgePageController: FlowManager found successfully");
    
        // Initialize name storage
        nameStorage = new PlayerPrefsNameStorage();
    }

    private void Start()
    {
        Debug.Log("AgePageController Start called");
    
        // Update name in title text if available
        UpdateChildNameInTitle();
    
        // Check RTL/LTR direction
        UpdateTextDirection();
    
        // Initial button state
        UpdateContinueButton();
    
        // Subscribe to language change
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    
        if (continueButton != null) 
        {
            Debug.Log("AgePageController: Continue button found");
            if (flowManager != null)
            {
                Debug.Log("AgePageController: Setting up continue button listener");
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => {
                    Debug.Log("AgePageController: Continue button clicked!");
                    flowManager.ContinueToNextPage();
                });
            }
        }
        else
        {
            Debug.LogError("AgePageController: Continue button is NULL!");
        }
    }
    
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void OnLanguageChanged()
    {
        UpdateTextDirection();
        UpdateChildNameInTitle();
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
    
    public void SelectAgeOption(int index)
    {
        Debug.Log($"Age option selected: {index}");
    
        // Update selected index
        selectedAgeIndex = index;
    
        // Update visuals for all buttons
        foreach (var button in ageButtons)
        {
            button.SetSelected(button.GetAgeGroupIndex() == selectedAgeIndex);
        }
    
        // Update continue button
        UpdateContinueButton();
        Debug.Log($"Continue button interactable set to: {continueButton.interactable}");
    
        // Notify flow manager
        if (flowManager != null)
        {
            flowManager.SetChildAge(selectedAgeIndex);
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
    
    // Method to update child name in the title
    private void UpdateChildNameInTitle()
    {
        if (titleText != null && nameStorage.HasName())
        {
            string childName = nameStorage.LoadName();
            if (!string.IsNullOrEmpty(childName))
            {
                // Get the localized format string (e.g. "(اسم) چند ساله است؟")
                string titleFormat = LocalizationManager.Instance.GetLocalizedValue(nameFormatKey);
            
                // Replace placeholder with child name
                string formattedTitle = titleFormat.Replace("[Name]", childName)
                    .Replace("(اسم)", childName);
            
                titleText.text = formattedTitle;
            }
        }
        else
        {
            if (titleText != null)
            {
                
                titleText.text = LocalizationManager.Instance.GetLocalizedValue(nameFormatKey);
                
            }
        }
    }

    // Get the selected age
    public int GetSelectedAgeIndex()
    {
        return selectedAgeIndex;
    }
}