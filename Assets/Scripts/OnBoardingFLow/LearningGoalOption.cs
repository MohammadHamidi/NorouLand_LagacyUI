using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class LearningGoalOption : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RTLTextMeshPro labelText;
    [SerializeField] private Image iconImage;
    
    [Header("Settings")]
    [SerializeField] private string goalId;
    
    [Header("Visual States")]
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 1f); // White
    [SerializeField] private Color deselectedColor = new Color(0.95f, 0.95f, 0.95f); // Light Gray
    
    private LearningGoalsController pageController;
    private bool isSelected = false;
    private bool isRTL = false;
    
    private void Awake()
    {
        // Find page controller
        pageController = GetComponentInParent<LearningGoalsController>();
        
        // Setup button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(OnButtonClicked);
        }
        
        // Check direction
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            LocalizationManager.Instance.OnLanguageChanged += UpdateDirection;
        }
        
        // Apply initial direction
        UpdateLayout();
    }
    
    private void OnEnable()
    {
        // Reset selection state
        SetSelected(false);
    }
    
    private void OnDestroy()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(OnButtonClicked);
        }
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateDirection;
        }
    }
    
    private void UpdateDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            UpdateLayout();
        }
    }
    
    private void UpdateLayout()
    {
        // Adjust layout for RTL/LTR
        if (labelText != null && iconImage != null)
        {
            RectTransform labelRect = labelText.GetComponent<RectTransform>();
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            
            if (isRTL)
            {
                // RTL layout: Icon on right, text on left
                float labelX = -Mathf.Abs(labelRect.anchoredPosition.x);
                float iconX = Mathf.Abs(iconRect.anchoredPosition.x);
                
                labelRect.anchoredPosition = new Vector2(labelX, labelRect.anchoredPosition.y);
                iconRect.anchoredPosition = new Vector2(iconX, iconRect.anchoredPosition.y);
            }
            else
            {
                // LTR layout: Icon on left, text on right
                float labelX = Mathf.Abs(labelRect.anchoredPosition.x);
                float iconX = -Mathf.Abs(iconRect.anchoredPosition.x);
                
                labelRect.anchoredPosition = new Vector2(labelX, labelRect.anchoredPosition.y);
                iconRect.anchoredPosition = new Vector2(iconX, iconRect.anchoredPosition.y);
            }
        }
    }
    
    private void OnButtonClicked()
    {
        // Toggle selection state
        SetSelected(!isSelected);
        
        // Notify parent
        OnboardingFlowManager flowManager = FindObjectOfType<OnboardingFlowManager>();
        if (flowManager != null)
        {
            flowManager.ToggleLearningGoal(goalId);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        // Update visual state
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : deselectedColor;
        }
    }
    
    public string GetGoalId()
    {
        return goalId;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
}