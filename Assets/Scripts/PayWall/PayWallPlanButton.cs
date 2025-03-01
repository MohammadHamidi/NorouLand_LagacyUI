using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PayWallPlanButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI discountedPriceText;
    [SerializeField] private TextMeshProUGUI pricePerMonthText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private Button selectionButton;
    [SerializeField] private GameObject popularTagObject;
    [SerializeField] private GameObject economicalTagObject;

    [Header("Settings")]
    [SerializeField] private Color selectedColor = new Color(1f, 234f/255f, 201f/255f);
    [SerializeField] private Color deselectedColor = Color.white;
    [SerializeField] private Color selectedBorderColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color deselectedBorderColor = new Color(0.9f, 0.9f, 0.9f); // Light gray

    [Header("Manual Setup (For Inspector Configuration)")]
    [SerializeField] private string planKey;
    [SerializeField] private string planDuration;
    [SerializeField] private float originalPrice;
    [SerializeField] private float discountedPrice;
    [SerializeField] private float pricePerMonth;
    [SerializeField] private PlanType planType;

    private PayWallController controller;
    private bool isSelected = false;

    public enum PlanType
    {
        Regular,
        Popular,
        Economical
    }

    private void Start()
    {
        UpdateVisualState(false);
        
        // Set button event
        if (selectionButton != null)
        {
            selectionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    // Setup method to be called after instantiating the prefab
    public void Setup(string key, string duration, float original, float discounted, float monthly, 
                      PlanType type, PayWallController parentController, bool isRTL)
    {
        planKey = key;
        planDuration = duration;
        originalPrice = original;
        discountedPrice = discounted;
        pricePerMonth = monthly;
        planType = type;
        controller = parentController;
        
        // Set localized text
        if (titleText != null)
        {
            titleText.text = planDuration;
        }
        
        if (priceText != null)
        {
            priceText.text = originalPrice.ToString("N0") + "₮";
            // If the original price and discounted price are the same, hide the original price text
            priceText.gameObject.SetActive(originalPrice != discountedPrice);
        }
        
        if (discountedPriceText != null)
        {
            discountedPriceText.text = discountedPrice.ToString("N0") + "₮";
        }
        
        if (pricePerMonthText != null && planType != PlanType.Regular)
        {
            pricePerMonthText.text = pricePerMonth.ToString("N0") + "₮/Month";
        }
        else if (pricePerMonthText != null)
        {
            pricePerMonthText.gameObject.SetActive(false);
        }
        
        // Set button event
        if (selectionButton != null)
        {
            selectionButton.onClick.RemoveAllListeners();
            selectionButton.onClick.AddListener(OnButtonClicked);
        }
        
        // Show/hide tags based on plan type
        if (popularTagObject != null)
        {
            popularTagObject.SetActive(planType == PlanType.Popular);
        }
        
        if (economicalTagObject != null)
        {
            economicalTagObject.SetActive(planType == PlanType.Economical);
        }
        
        // Apply RTL/LTR settings
        ApplyTextDirection(isRTL);
        
        // Set initial selection state (deselected)
        UpdateVisualState(false);
    }
    
    // For manually configured buttons from inspector
    public void SetController(PayWallController parentController, bool isRTL)
    {
        controller = parentController;
        
        // Apply text direction without changing the content
        ApplyTextDirection(isRTL);
        
        // Make sure tags are properly set
        if (popularTagObject != null)
        {
            popularTagObject.SetActive(planType == PlanType.Popular);
        }
        
        if (economicalTagObject != null)
        {
            economicalTagObject.SetActive(planType == PlanType.Economical);
        }
    }
    
    public void ApplyTextDirection(bool isRTL)
    {
        // Align text based on language direction
        if (titleText != null)
        {
            titleText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        }
        
        if (priceText != null)
        {
            priceText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Right;
        }
        
        if (discountedPriceText != null)
        {
            discountedPriceText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Right;
        }
        
        if (pricePerMonthText != null)
        {
            pricePerMonthText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        }
    }
    
    private void OnButtonClicked()
    {
        if (controller != null)
        {
            // Tell the controller this plan was selected
            controller.SelectPlan(planKey);
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
        }
        
        // Show/hide selection border
        if (selectionBorder != null)
        {
            selectionBorder.gameObject.SetActive(true);
            selectionBorder.color = selected ? selectedBorderColor : deselectedBorderColor;
        }
    }
    
    public string GetPlanKey()
    {
        return planKey;
    }
    
    public float GetPrice()
    {
        return discountedPrice;
    }
    
    public string GetPlanDuration()
    {
        return planDuration;
    }
    
    private void OnDestroy()
    {
        if (selectionButton != null)
        {
            selectionButton.onClick.RemoveAllListeners();
        }
    }
}