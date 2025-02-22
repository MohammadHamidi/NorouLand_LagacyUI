using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LocalizationSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;
using RTLTMPro;

using UnityEngine;
using TMPro;
using LocalizationSystem;

using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float bounceDuration = 1.5f;
    [SerializeField] private float bounceHeight = 10f;
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Idle Animation")]
    [SerializeField] private float blinkInterval = 3f;
    [SerializeField] private GameObject eyesOpen;
    [SerializeField] private GameObject eyesClosed;
    
    private Vector3 originalPosition;
    private bool isAnimating = false;
    
    private void Start()
    {
        originalPosition = transform.localPosition;
        
        // Start idle animations
        StartCoroutine(BlinkRoutine());
        StartCoroutine(BounceRoutine());
    }
    
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // Random time between blinks
            float waitTime = Random.Range(blinkInterval * 0.7f, blinkInterval * 1.3f);
            yield return new WaitForSeconds(waitTime);
            
            // Blink
            if (eyesOpen != null && eyesClosed != null)
            {
                eyesOpen.SetActive(false);
                eyesClosed.SetActive(true);
                
                // Blink duration
                yield return new WaitForSeconds(0.15f);
                
                eyesOpen.SetActive(true);
                eyesClosed.SetActive(false);
            }
        }
    }
    
    private IEnumerator BounceRoutine()
    {
        while (true)
        {
            // Wait before starting bounce
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            
            float elapsedTime = 0f;
            
            while (elapsedTime < bounceDuration)
            {
                float normalizedTime = elapsedTime / bounceDuration;
                float curveValue = bounceCurve.Evaluate(normalizedTime);
                float yOffset = bounceHeight * curveValue;
                
                transform.localPosition = originalPosition + new Vector3(0f, yOffset, 0f);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Reset position
            transform.localPosition = originalPosition;
        }
    }
    
    // Call this from buttons to trigger a happy reaction
    public void PlayHappyReaction()
    {
        if (!isAnimating)
        {
            StartCoroutine(HappyReactionRoutine());
        }
    }
    
    private IEnumerator HappyReactionRoutine()
    {
        isAnimating = true;
        
        // Play a quick bounce animation
        float duration = 0.5f;
        float height = bounceHeight * 1.5f;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = Mathf.Sin(normalizedTime * Mathf.PI);
            float yOffset = height * curveValue;
            
            transform.localPosition = originalPosition + new Vector3(0f, yOffset, 0f);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
        isAnimating = false;
    }
}
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


public class AgeSelectionButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int ageGroupIndex; // 0: Under 5, 1: 5-7, 2: More than 7
    [SerializeField] private RTLTextMeshPro ageText;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Button button;
    
    [Header("Visual States")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color deselectedColor = new Color(0.95f, 0.95f, 0.95f); // Light gray
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private Color deselectedTextColor = Color.black;
    
    private OnboardingFlowManager flowManager;
    private bool isSelected = false;
    
    private void Start()
    {
        // Get reference to flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        
        // Setup button click event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        
        // Set initial visual state
        UpdateVisualState(false);
    }
    
    private void OnButtonClick()
    {
        // Update selection in manager
        if (flowManager != null)
        {
            flowManager.SetChildAge(ageGroupIndex);
        }
        
        // Deselect all siblings
        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                AgeSelectionButton otherButton = child.GetComponent<AgeSelectionButton>();
                if (otherButton != null && otherButton != this)
                {
                    otherButton.SetSelected(false);
                }
            }
        }
        
        // Select this button
        SetSelected(true);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState(selected);
    }
    
    private void UpdateVisualState(bool selected)
    {
        if (buttonBackground != null)
        {
            buttonBackground.color = selected ? selectedColor : deselectedColor;
        }
        
        if (ageText != null)
        {
            ageText.color = selected ? selectedTextColor : deselectedTextColor;
        }
    }
}
public class LearningGoalItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string goalId;
    [SerializeField] private Image iconImage;
    [SerializeField] private RTLTextMeshPro goalText;
    [SerializeField] private Toggle goalToggle;
    [SerializeField] private Image selectionBackground;
    
    [Header("Visual States")]
    [SerializeField] private Color selectedColor = new Color(0.95f, 0.95f, 0.95f);
    [SerializeField] private Color deselectedColor = new Color(0.9f, 0.9f, 0.9f);
    
    private OnboardingFlowManager flowManager;
    
    private void Start()
    {
        // Get reference to the flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        
        // Setup toggle callback
        if (goalToggle != null)
        {
            goalToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        // Set initial visual state
        UpdateVisualState(goalToggle.isOn);
    }
    
    private void OnToggleValueChanged(bool isOn)
    {
        // Update in the manager
        if (flowManager != null)
        {
            flowManager.ToggleLearningGoal(goalId);
        }
        
        // Update visual state
        UpdateVisualState(isOn);
    }
    
    private void UpdateVisualState(bool isSelected)
    {
        if (selectionBackground != null)
        {
            selectionBackground.color = isSelected ? selectedColor : deselectedColor;
        }
        
        // Could also animate scale or add visual effects here
    }
}
public class OnboardingFlowManager : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private List<GameObject> pageObjects = new List<GameObject>();
    
    [Header("Navigation")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button continueButton;
    
    [Header("Progress Indicators")]
    [SerializeField] private List<GameObject> progressIndicators = new List<GameObject>();
    
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
        if (nextButton != null)
            nextButton.onClick.AddListener(GoToNextPage);
        
        if (previousButton != null)
            previousButton.onClick.AddListener(GoToPreviousPage);
            
        if (continueButton != null)
            continueButton.onClick.AddListener(GoToNextPage);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateUIDirection;
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
        
        // Update progress indicators
        UpdateProgressIndicators();
        
        // Update navigation buttons
        UpdateNavigationButtons();
    }
    
    private void UpdateProgressIndicators()
    {
        for (int i = 0; i < progressIndicators.Count; i++)
        {
            if (i < currentPageIndex)
            {
                // Completed step
                progressIndicators[i].GetComponent<Image>().color = new Color(1f, 0.6f, 0f); // Orange
            }
            else if (i == currentPageIndex)
            {
                // Current step
                progressIndicators[i].GetComponent<Image>().color = new Color(1f, 0.6f, 0f); // Orange
            }
            else
            {
                // Future step
                progressIndicators[i].GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f); // Gray
            }
        }
    }
    
    private void UpdateNavigationButtons()
    {
        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(currentPageIndex > 0);
        }
        
        if (nextButton != null && continueButton != null)
        {
            nextButton.gameObject.SetActive(currentPageIndex < pageObjects.Count - 1);
            continueButton.gameObject.SetActive(currentPageIndex == pageObjects.Count - 1);
        }
    }
    
    public void GoToNextPage()
    {
        // Save data from current page
        SaveCurrentPageData();
        
        // Move to next page
        if (currentPageIndex < pageObjects.Count - 1)
        {
            ShowPage(currentPageIndex + 1);
        }
        else
        {
            // This is the last page, complete the onboarding
            CompleteOnboarding();
        }
    }
    
    public void GoToPreviousPage()
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
                // Phone number is handled by input field
                break;
        }
    }
    
    public void SetChildAge(int age)
    {
        childAge = age;
    }
    
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
    }
    
    private void CompleteOnboarding()
    {
        // Save all collected information
        PlayerPrefs.SetString("ChildName", childName);
        PlayerPrefs.SetInt("ChildAge", childAge);
        PlayerPrefs.SetString("LearningGoals", string.Join(",", selectedLearningGoals));
        PlayerPrefs.SetString("PhoneNumber", phoneInputField.text);
        PlayerPrefs.Save();
        
        // Proceed to main application
        Debug.Log("Onboarding complete! Loading main application...");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainApp");
    }
}