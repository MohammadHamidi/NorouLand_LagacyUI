// Age selection button component

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
    
    private AgePageController pageController;
    
    private void Start()
    {
        // Get reference to page controller
        pageController = GetComponentInParent<AgePageController>();
        
        // Setup button click event
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
    
    private void OnButtonClick()
    {
        if (pageController != null)
        {
            pageController.SelectAgeOption(ageGroupIndex);
        }
    }
    
    public void SetSelected(bool selected)
    {
        if (buttonBackground != null)
        {
            buttonBackground.color = selected ? selectedColor : deselectedColor;
        }
    }
    
    public int GetAgeGroupIndex()
    {
        return ageGroupIndex;
    }
}