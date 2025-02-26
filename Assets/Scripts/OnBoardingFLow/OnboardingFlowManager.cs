using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalizationSystem;
using UnityEngine;
using TMPro;
using RTLTMPro;
using LocalizationSystem;
using UnityEngine.UI;

public class OnboardingFlowManager : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private List<GameObject> pageObjects = new List<GameObject>();
    
    [Header("Navigation")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton; // This is the back arrow
    
    [Header("Indicators")]
    [SerializeField] private QuestionsIndicator questionsIndicator;
    
    [Header("References")]
    [SerializeField] private TMPro.TMP_InputField nameInputField;
    [SerializeField] private TMPro.TMP_InputField phoneInputField;
    
    private int currentPageIndex = 0;
    private string childName;
    private int childAge = 0; // 0: Under 5, 1: 5-7, 2: More than 7
    private List<string> selectedLearningGoals = new List<string>();
    
    // RTL Support
    private bool isRTL;
    
    private void Start()
    {
        // Find components if not assigned
        if (questionsIndicator == null)
            questionsIndicator = FindObjectOfType<QuestionsIndicator>();
            
        // Initialize UI direction based on current language
        UpdateUIDirection();
        
        // Subscribe to language change event
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateUIDirection;
        }
        
        // Setup initial page
        ShowPage(0);
        
        // Setup buttons
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateUIDirection;
        }
    }
    // Add this public method to OnboardingFlowManager
    public void ContinueToNextPage()
    {
        Debug.LogError($"Continue Button Clicked");
        // Save current page data
        SaveCurrentPageData();
    
        // Move to next page or finish
        if (currentPageIndex < pageObjects.Count - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
        else
        {
            // Last page, complete onboarding
            CompleteOnboarding();
        }
    }
    
    private void UpdateUIDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Update the canvas scale for RTL/LTR
            if (isRTL)
            {
                // For RTL, flip the UI horizontally
                transform.localScale = new Vector3(-1, 1, 1);
                // Fix text components so they don't appear mirrored
                FixTextComponentsForRTL(true);
            }
            else
            {
                // For LTR, use normal scale
                transform.localScale = Vector3.one;
                FixTextComponentsForRTL(false);
            }
        }
    }
    
    private void FixTextComponentsForRTL(bool isRTL)
    {
        // Get all text components that need to be fixed
        var textComponents = GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        var rtlTextComponents = GetComponentsInChildren<RTLTMPro.RTLTextMeshPro>(true);
        
        // For regular TMP text components
        foreach (var text in textComponents)
        {
            // Text shouldn't be flipped when parent is flipped
            if (isRTL)
                text.transform.localScale = new Vector3(-1, 1, 1);
            else
                text.transform.localScale = Vector3.one;
        }
        
        // For RTL TMP components, they handle text direction internally
        foreach (var rtlText in rtlTextComponents)
        {
            if (isRTL)
                rtlText.transform.localScale = new Vector3(-1, 1, 1);
            else
                rtlText.transform.localScale = Vector3.one;
        }
    }
    
    public void ShowPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageObjects.Count)
            return;
            
        // Hide all pages
        foreach (var page in pageObjects)
        {
            page.SetActive(false);
        }
        
        // Show requested page
        pageObjects[pageIndex].SetActive(true);
        currentPageIndex = pageIndex;
        
        // Update the questions indicator
        if (questionsIndicator != null)
        {
            questionsIndicator.SetCurrentStep(currentPageIndex);
        }
        
        // Update navigation
        UpdateNavigationState();
        
        // Check if current page inputs are valid
        ValidateCurrentPageInputs();
    }
    
    private void UpdateNavigationState()
    {
        // Back button should only be visible if not on first page
        if (backButton != null)
        {
            backButton.gameObject.SetActive(currentPageIndex > 0);
        }
    }
    
    // This checks if the requirements for the current page are met
    private void ValidateCurrentPageInputs()
    {
        bool inputsValid = false;
        
        switch (currentPageIndex)
        {
            case 0: // Name page
                // Enable continue button only if name is entered
                inputsValid = nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text);
                break;
                
            case 1: // Age page
                // Enable continue if an age option has been selected
                inputsValid = childAge >= 0; // Assuming childAge is set when an option is selected
                break;
                
            case 2: // Learning goals page
                // Enable continue if at least one goal is selected
                inputsValid = selectedLearningGoals.Count > 0;
                break;
                
            case 3: // Phone number page
                // Phone might be optional (there's a "Remind me later" button)
                inputsValid = true;
                break;
        }
        
        // Enable/disable continue button based on validation
        if (continueButton != null)
        {
            continueButton.interactable = inputsValid;
        }
    }
    
    private void OnContinueButtonClicked()
    {
        // Save current page data
        SaveCurrentPageData();
        
        // Move to next page or finish
        if (currentPageIndex < pageObjects.Count - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
        else
        {
            // Last page, complete onboarding
            CompleteOnboarding();
        }
    }
    
    private void OnBackButtonClicked()
    {
        if (currentPageIndex > 0)
        {
            ShowPage(currentPageIndex - 1);
        }
    }
    
    private void SaveCurrentPageData()
    {
        switch (currentPageIndex)
        {
            case 0: // Name page
                if (nameInputField != null)
                {
                    childName = nameInputField.text;
                }
                break;
                
            case 1: // Age page
                // Age selection is handled by button callbacks
                break;
                
            case 2: // Learning goals page
                // Learning goals are handled by toggle callbacks
                break;
                
            case 3: // Phone number page
                if (phoneInputField != null)
                {
                    // Store phone number directly from the input field
                }
                break;
        }
    }
    
    // Public method to be called from age selection buttons
    public void SetChildAge(int age)
    {
        childAge = age;
        ValidateCurrentPageInputs(); // Re-validate to enable continue button
    }
    
    // Public method to be called from learning goal toggles
    public void ToggleLearningGoal(string goalId)
    {
        if (selectedLearningGoals.Contains(goalId))
        {
            selectedLearningGoals.Remove(goalId);
        }
        else
        {
            selectedLearningGoals.Add(goalId);
        }
        
        ValidateCurrentPageInputs(); // Re-validate to enable continue button
    }
    
    // Called when text fields change
    public void OnInputFieldValueChanged()
    {
        ValidateCurrentPageInputs(); // Re-validate to enable continue button
    }
    
    // Public method for "Remind me later" button
    public void OnRemindMeLaterClicked()
    {
        // Skip phone number requirement and continue
        CompleteOnboarding();
    }
    
    private void CompleteOnboarding()
    {
        // Save all collected information
        PlayerPrefs.SetString("ChildName", childName);
        PlayerPrefs.SetInt("ChildAge", childAge);
        PlayerPrefs.SetString("LearningGoals", string.Join(",", selectedLearningGoals));
        
        if (phoneInputField != null && !string.IsNullOrEmpty(phoneInputField.text))
        {
            PlayerPrefs.SetString("PhoneNumber", phoneInputField.text);
        }
        
        PlayerPrefs.Save();
        
        // Proceed to main application
        Debug.Log("Onboarding complete! Loading main application...");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainApp");
    }
}