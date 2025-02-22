using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

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