using System.Collections.Generic;
using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using RTLTMPro;
using LocalizationSystem;
using UnityEngine.UI;

public class LearningGoalsController : MonoBehaviour
{
    [System.Serializable]
    public class LearningGoalOption
    {
        public string goalId;
        public GameObject optionGameObject;
        public Button toggleButton;
        public Image backgroundImage;
        public RTLTextMeshPro labelText;
        public Image iconImage;
    }
    
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private Button continueButton;
    
    [Header("Learning Goal Options")]
    [SerializeField] private LearningGoalOption[] goalOptions;
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 1f); // White
    [SerializeField] private Color deselectedColor = new Color(0.95f, 0.95f, 0.95f); // Light Gray
    
    private OnboardingFlowManager flowManager;
    private List<string> selectedGoalIds = new List<string>();
    private bool isRTL;
    
    private void Awake()
    {
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
    }
    
    private void OnEnable()
    {
        // Setup option buttons
        for (int i = 0; i < goalOptions.Length; i++)
        {
            int index = i; // Capture for lambda
            if (goalOptions[i].toggleButton != null)
            {
                goalOptions[i].toggleButton.onClick.AddListener(() => ToggleGoalOption(index));
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
        for (int i = 0; i < goalOptions.Length; i++)
        {
            if (goalOptions[i].toggleButton != null)
            {
                goalOptions[i].toggleButton.onClick.RemoveAllListeners();
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
            
            // Apply RTL-specific adjustments to learning goal items
            for (int i = 0; i < goalOptions.Length; i++)
            {
                if (goalOptions[i].labelText != null && goalOptions[i].iconImage != null)
                {
                    RectTransform labelRect = goalOptions[i].labelText.GetComponent<RectTransform>();
                    RectTransform iconRect = goalOptions[i].iconImage.GetComponent<RectTransform>();
                    
                    if (isRTL)
                    {
                        // Move icon to right side and text to left side for RTL
                        // This depends on your UI layout, might need to be adjusted
                        labelRect.anchoredPosition = new Vector2(-Mathf.Abs(labelRect.anchoredPosition.x), labelRect.anchoredPosition.y);
                        iconRect.anchoredPosition = new Vector2(Mathf.Abs(iconRect.anchoredPosition.x), iconRect.anchoredPosition.y);
                    }
                    else
                    {
                        // Standard LTR layout
                        labelRect.anchoredPosition = new Vector2(Mathf.Abs(labelRect.anchoredPosition.x), labelRect.anchoredPosition.y);
                        iconRect.anchoredPosition = new Vector2(-Mathf.Abs(iconRect.anchoredPosition.x), iconRect.anchoredPosition.y);
                    }
                }
            }
        }
    }
    
    private void ToggleGoalOption(int index)
    {
        // Only process if valid index
        if (index >= 0 && index < goalOptions.Length)
        {
            string goalId = goalOptions[index].goalId;
            
            // Toggle selection
            if (selectedGoalIds.Contains(goalId))
            {
                // Deselect
                selectedGoalIds.Remove(goalId);
                if (goalOptions[index].backgroundImage != null)
                {
                    goalOptions[index].backgroundImage.color = deselectedColor;
                }
            }
            else
            {
                // Select
                selectedGoalIds.Add(goalId);
                if (goalOptions[index].backgroundImage != null)
                {
                    goalOptions[index].backgroundImage.color = selectedColor;
                }
            }
            
            // Update continue button
            UpdateContinueButton();
            
            // Notify flow manager
            if (flowManager != null)
            {
                flowManager.ToggleLearningGoal(goalId);
            }
        }
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            // Enable continue button only if at least one goal is selected
            continueButton.interactable = selectedGoalIds.Count > 0;
        }
    }
    
    // Get the selected goals
    public List<string> GetSelectedGoalIds()
    {
        return new List<string>(selectedGoalIds);
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
    
    // Clear all selections
    public void ClearSelections()
    {
        selectedGoalIds.Clear();
        
        for (int i = 0; i < goalOptions.Length; i++)
        {
            if (goalOptions[i].backgroundImage != null)
            {
                goalOptions[i].backgroundImage.color = deselectedColor;
            }
        }
        
        UpdateContinueButton();
    }
}