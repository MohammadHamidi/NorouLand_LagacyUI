using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using UnityEngine.SceneManagement;

public class PersonalizationFlowManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject stepperScreen;
    [SerializeField] private GameObject subscriptionScreen;
    [SerializeField] private GameObject congratulationsScreen;
    [SerializeField] private string startGameSceneName;
    
    [Header("Screen Prefabs")]
    [SerializeField] private GameObject loadingScreenPrefab;
    [SerializeField] private GameObject stepperScreenPrefab;
    [SerializeField] private GameObject subscriptionScreenPrefab;
    [SerializeField] private GameObject congratulationsScreenPrefab;
    
    [Header("Navigation")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private Transform screenContainer;
    
    [Header("Transition")]
    [SerializeField] private float screenTransitionDuration = 0.4f;
    [SerializeField] private Ease screenTransitionEase = Ease.OutQuad;
    
    [Header("Data")]
    [SerializeField] private string defaultChildName = "[Name]";
    
    private string childName;
    private string selectedPlanId;
    private GameObject currentScreen;
    private int currentStep = 0;
    
    private void Awake()
    {
        // Get child name from PlayerPrefs
        childName = PlayerPrefs.GetString("ChildName", defaultChildName);
        
        // Get selected plan from PlayerPrefs if exists
        selectedPlanId = PlayerPrefs.GetString("SelectedPlan", "");
        
        // Initialize - find screens if not set
        if (screenContainer == null)
        {
            screenContainer = transform;
        }
        
        // If screens are null but prefabs exist, instantiate them
        if (loadingScreen == null && loadingScreenPrefab != null)
        {
            loadingScreen = Instantiate(loadingScreenPrefab, screenContainer);
            loadingScreen.SetActive(false);
        }
        
        if (stepperScreen == null && stepperScreenPrefab != null)
        {
            stepperScreen = Instantiate(stepperScreenPrefab, screenContainer);
            stepperScreen.SetActive(false);
        }
        
        if (subscriptionScreen == null && subscriptionScreenPrefab != null)
        {
            subscriptionScreen = Instantiate(subscriptionScreenPrefab, screenContainer);
            subscriptionScreen.SetActive(false);
        }
        
        if (congratulationsScreen == null && congratulationsScreenPrefab != null)
        {
            congratulationsScreen = Instantiate(congratulationsScreenPrefab, screenContainer);
            congratulationsScreen.SetActive(false);
        }
    }
    
    private void Start()
    {
        // Start with initial screen based on saved state
        int savedState = PlayerPrefs.GetInt("PersonalizationState", 0);
        
        switch (savedState)
        {
            case 0: // Not started or initial state
                ShowLoadingScreen();
                break;
                
            case 1: // Completed loading, show stepper
                ShowStepperScreen();
                break;
                
            case 2: // Plan selection stage
                ShowSubscriptionScreen();
                break;
                
            case 3: // Completed, show congratulations
                ShowCongratulationsScreen();
                break;
                
            default:
                ShowLoadingScreen();
                break;
        }
    }
    
    // Method to get the child's name
    public string GetChildName()
    {
        if (string.IsNullOrEmpty(childName))
        {
            childName = PlayerPrefs.GetString("ChildName", defaultChildName);
        }
        return childName;
    }
    
    // Set the child's name
    public void SetChildName(string name)
    {
        childName = name;
        PlayerPrefs.SetString("ChildName", name);
        PlayerPrefs.Save();
    }
    
    // Set the selected subscription plan
    public void SetSelectedPlan(string planId)
    {
        selectedPlanId = planId;
        PlayerPrefs.SetString("SelectedPlan", planId);
        PlayerPrefs.Save();
    }
    
    // Get the selected plan ID
    public string GetSelectedPlan()
    {
        return selectedPlanId;
    }
    
    // Method to show loading screen
    public void ShowLoadingScreen()
    {
        ShowScreen(loadingScreen);
        currentStep = 0;
        SaveCurrentState();
    }
    
    // Method to show stepper screen
    public void ShowStepperScreen()
    {
        ShowScreen(stepperScreen);
        currentStep = 1;
        SaveCurrentState();
    }
    
    // Method to show subscription screen
    public void ShowSubscriptionScreen()
    {
        ShowScreen(subscriptionScreen);
        currentStep = 2;
        SaveCurrentState();
    }
    
    // Method to show plans screen (same as subscription in this case)
    public void ShowPlansScreen()
    {
        ShowSubscriptionScreen();
    }
    
    // Method to show congratulations screen
    public void ShowCongratulationsScreen()
    {
        ShowScreen(congratulationsScreen);
        currentStep = 3;
        SaveCurrentState();
    }
    
    // Method to show privacy policy
    public void ShowPrivacyPolicy()
    {
        // Could load a web view or separate privacy policy screen
        Application.OpenURL("https://example.com/privacy-policy");
    }
    
    // Helper method to show a specific screen
    private void ShowScreen(GameObject screen)
    {
        if (screen == null)
            return;
            
        // Hide current screen with animation if it exists
        if (currentScreen != null && currentScreen != screen)
        {
            // Prepare for animation
            CanvasGroup currentCanvasGroup = currentScreen.GetComponent<CanvasGroup>();
            if (currentCanvasGroup == null)
            {
                currentCanvasGroup = currentScreen.AddComponent<CanvasGroup>();
            }
            
            // Animate out
            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(currentCanvasGroup.DOFade(0f, screenTransitionDuration)
                .SetEase(screenTransitionEase));
                
            fadeOutSequence.OnComplete(() => {
                currentScreen.SetActive(false);
            });
            
            fadeOutSequence.Play();
        }
        
        // Show new screen with animation
        screen.SetActive(true);
        currentScreen = screen;
        
        // Prepare for animation
        CanvasGroup newCanvasGroup = screen.GetComponent<CanvasGroup>();
        if (newCanvasGroup == null)
        {
            newCanvasGroup = screen.AddComponent<CanvasGroup>();
        }
        
        // Set initial state and animate in
        newCanvasGroup.alpha = 0f;
        
        Sequence fadeInSequence = DOTween.Sequence();
        fadeInSequence.Append(newCanvasGroup.DOFade(1f, screenTransitionDuration)
            .SetEase(screenTransitionEase));
            
        fadeInSequence.Play();
    }
    
    // Method to navigate to the next step
    public void GoToNextPage()
    {
        switch (currentStep)
        {
            case 0: // From loading to stepper
                ShowStepperScreen();
                break;
                
            case 1: // From stepper to subscription
                ShowSubscriptionScreen();
                break;
                
            case 2: // From subscription to congratulations
                ShowCongratulationsScreen();
                break;
                
            case 3: // From congratulations to game
                StartChildExperience();
                break;
                
            default:
                break;
        }
    }
    
    // Method to close the current screen and go back
    public void CloseCurrentScreen()
    {
        // Go back one step based on current state
        switch (currentStep)
        {
            case 1: // From stepper to loading
                ShowLoadingScreen();
                break;
                
            case 2: // From subscription to stepper
                ShowStepperScreen();
                break;
                
            case 3: // From congratulations to subscription
                ShowSubscriptionScreen();
                break;
                
            default:
                // If no previous screen, just close the current one
                if (currentScreen != null)
                {
                    currentScreen.SetActive(false);
                    currentScreen = null;
                }
                break;
        }
    }
    
    // Method for confirming subscription
    public void ConfirmSubscription(string planId)
    {
        // Set the plan
        SetSelectedPlan(planId);
        
        // Move to congratulations screen
        ShowCongratulationsScreen();
    }
    
    // Method to complete personalization and transition to game
    public void CompletePersonalization()
    {
        // Set state as completed
        PlayerPrefs.SetInt("PersonalizationCompleted", 1);
        PlayerPrefs.Save();
        
        // Show congratulations screen
        ShowCongratulationsScreen();
        
        // Notify any listeners
        if (OnPersonalizationCompleted != null)
        {
            OnPersonalizationCompleted.Invoke();
        }
    }
    
    // Method to start the child's experience (actual game/app)
    public void StartChildExperience()
    {
        // Clean up first
        DOTween.Kill(gameObject);
        
        // Determine which scene to load
        string sceneToLoad = !string.IsNullOrEmpty(startGameSceneName) ? 
                            startGameSceneName : 
                            !string.IsNullOrEmpty(nextSceneName) ?
                            nextSceneName : "";
                            
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Load the game scene
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No next scene specified for child experience");
        }
    }
    
    // Method to skip directly to a specific step (for testing)
    public void SkipToStep(int step)
    {
        switch(step)
        {
            case 0:
                ShowLoadingScreen();
                break;
            case 1:
                ShowStepperScreen();
                break;
            case 2:
                ShowSubscriptionScreen();
                break;
            case 3:
                ShowCongratulationsScreen();
                break;
            default:
                break;
        }
    }
    
    // Helper method to save current state
    private void SaveCurrentState()
    {
        PlayerPrefs.SetInt("PersonalizationState", currentStep);
        PlayerPrefs.Save();
    }
    
    // Event for completion notification
    public System.Action OnPersonalizationCompleted;
}