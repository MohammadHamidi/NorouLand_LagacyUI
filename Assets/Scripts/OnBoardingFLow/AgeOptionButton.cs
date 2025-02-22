using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

public class AgeOptionButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RTLTextMeshPro labelText;
    
    [Header("Settings")]
    [SerializeField] private int ageRangeIndex; // 0: Under 5, 1: 5-7, 2: Over 7
    
    [Header("Visual States")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color deselectedColor = new Color(0.95f, 0.95f, 0.95f); // Light Gray
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private Color deselectedTextColor = Color.black;
    
    private AgePageController pageController;
    private bool isSelected = false;
    
    private void Awake()
    {
        // Find page controller
        pageController = GetComponentInParent<AgePageController>();
        
        // Setup button
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
    
    private void OnButtonClicked()
    {
        // Find parent controller if not already found
        if (pageController == null)
        {
            pageController = GetComponentInParent<AgePageController>();
        }
        
        // Update selection state
        SetSelected(true);
        
        // Notify parent
        OnboardingFlowManager flowManager = FindObjectOfType<OnboardingFlowManager>();
        if (flowManager != null)
        {
            flowManager.SetChildAge(ageRangeIndex);
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
        
        if (labelText != null)
        {
            labelText.color = isSelected ? selectedTextColor : deselectedTextColor;
        }
    }
    
    public int GetAgeRangeIndex()
    {
        return ageRangeIndex;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
}