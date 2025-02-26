using System;
using LocalizationSystem;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LearningGoalItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro goalText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectionButton;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private Image checkMark;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    [Header("Settings")]
    [SerializeField] private Color selectedColor = new Color(1f, 234f/255f, 201f/255f);
    [SerializeField] private Color deselectedColor = Color.white;
    [SerializeField] private Color selectedBorderColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color checkMarkInvisibleColor=new Color(1f, 0.6f, 0f,0); // Orange
    [SerializeField] private Color checkMarkVisibleColor=new Color(1f, 0.6f, 0f,1); // Orange
    private string goalKey;
    private LearningGoalsController controller;
    private bool isSelected = false;

    private void Start()
    {
        UpdateVisualState(false);
    }

    
    
    // Setup method to be called after instantiating the prefab
    public void Setup(string key, Sprite icon, LearningGoalsController parentController, bool isRTL)
    {
        goalKey = key;
        controller = parentController;
        
        // Set icon
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
        }
        
        // Set localized text
        if (goalText != null && LocalizationManager.Instance != null)
        {
            goalText.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
        
        // Set button event
        if (selectionButton != null)
        {
            selectionButton.onClick.AddListener(OnButtonClicked);
        }
        
        // Apply RTL/LTR settings
        ApplyTextDirection(isRTL);
        
        // Set initial selection state (deselected)
        UpdateVisualState(false);
    }
    
    public void ApplyTextDirection(bool isRTL)
    {
        // Align text based on language direction
        if (goalText != null)
        {
            goalText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            _horizontalLayoutGroup.reverseArrangement = isRTL ? true : false;
            // Set padding based on text direction
            RectTransform textRect = goalText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                Vector2 offsetMin = textRect.offsetMin;
                Vector2 offsetMax = textRect.offsetMax;
                
                if (isRTL)
                {
                    // Right padding for RTL
                    offsetMin.x = 20f;
                    offsetMax.x = -70f; // Allow space for the icon
                }
                else
                {
                    // Left padding for LTR
                    offsetMin.x = 70f; // Allow space for the icon
                    offsetMax.x = -20f;
                }
                
                textRect.offsetMin = offsetMin;
                textRect.offsetMax = offsetMax;
            }
            
        }
        
        // Adjust icon position based on text direction
        if (iconImage != null)
        {
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                // Adjust anchors based on language direction
                if (isRTL)
                {
                    iconRect.anchorMin = new Vector2(1, 0.5f);
                    iconRect.anchorMax = new Vector2(1, 0.5f);
                    iconRect.pivot = new Vector2(1, 0.5f);
                    iconRect.anchoredPosition = new Vector2(-20f, 0);
                }
                else
                {
                    iconRect.anchorMin = new Vector2(0, 0.5f);
                    iconRect.anchorMax = new Vector2(0, 0.5f);
                    iconRect.pivot = new Vector2(0, 0.5f);
                    iconRect.anchoredPosition = new Vector2(20f, 0);
                }
            }
        }
    }
    
    private void OnButtonClicked()
    {
        if (controller != null)
        {
            // Tell the controller this goal was toggled
            controller.ToggleGoalSelection(goalKey);
        }
    }
    
    // Public method to be called from the controller
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState(selected);
    }
    
    private void UpdateVisualState(bool selected)
    {
        // Update background color
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : deselectedColor;
            checkMark.color = selected ? checkMarkVisibleColor : checkMarkInvisibleColor;
        }
        
        // Show/hide selection border
        if (selectionBorder != null)
        {
            selectionBorder.color= selected ? checkMarkVisibleColor :deselectedColor ;
            
            // Update border color if it has an image component
         
            
        }
    }
    
    public string GetGoalKey()
    {
        return goalKey;
    }
    
    private void OnDestroy()
    {
        if (selectionButton != null)
        {
            selectionButton.onClick.RemoveAllListeners();
        }
    }
}