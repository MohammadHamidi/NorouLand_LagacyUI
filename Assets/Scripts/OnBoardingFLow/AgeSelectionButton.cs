using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

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