using System.Collections.Generic;
using LocalizationSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayWallController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform planButtonsContainer;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI savingsText;
    [SerializeField] private GameObject discountBadge;
    
    [Header("Plan Prefabs")]
    [SerializeField] private PayWallPlanButton planButtonPrefab;
    
    [Header("Plan Configurations")]
    [SerializeField] private PlanConfig[] planConfigs;
    
    private List<PayWallPlanButton> planButtons = new List<PayWallPlanButton>();
    private string selectedPlanKey;
    private bool isRTL;
    
    [System.Serializable]
    public class PlanConfig
    {
        public string planKey;
        public string duration;
        public float originalPrice;
        public float discountedPrice;
        public float pricePerMonth;
        public PayWallPlanButton.PlanType planType;
        public bool defaultSelected;
    }
    
    private void Start()
    {
        // Check if RTL language is used
        isRTL = LocalizationManager.Instance != null && LocalizationManager.Instance.IsCurrentLanguageRTL();
        
        // Initialize plans
        InitializePlans();
        
        // Set continue button event
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        
        // Update discount badge
        UpdateDiscountBadge();
    }
    
    private void InitializePlans()
    {
        // Clear any existing buttons
        foreach (Transform child in planButtonsContainer)
        {
            Destroy(child.gameObject);
        }
        planButtons.Clear();
        
        // Create new buttons based on config
        foreach (PlanConfig config in planConfigs)
        {
            PayWallPlanButton planButton = Instantiate(planButtonPrefab, planButtonsContainer);
            planButton.Setup(
                config.planKey, 
                config.duration, 
                config.originalPrice, 
                config.discountedPrice, 
                config.pricePerMonth, 
                config.planType, 
                this, 
                isRTL
            );
            
            planButtons.Add(planButton);
            
            // Set default selection
            if (config.defaultSelected)
            {
                SelectPlan(config.planKey);
            }
        }
    }
    
    public void SelectPlan(string planKey)
    {
        selectedPlanKey = planKey;
        
        // Update all buttons
        foreach (PayWallPlanButton button in planButtons)
        {
            button.SetSelected(button.GetPlanKey() == planKey);
        }
    }
    
    private void UpdateDiscountBadge()
    {
        if (discountBadge != null && savingsText != null)
        {
            // Calculate the maximum discount percentage among all plans
            float maxDiscountPercentage = 0f;
            
            foreach (PlanConfig config in planConfigs)
            {
                if (config.originalPrice > 0 && config.discountedPrice < config.originalPrice)
                {
                    float discountPercentage = (1 - (config.discountedPrice / config.originalPrice)) * 100;
                    maxDiscountPercentage = Mathf.Max(maxDiscountPercentage, discountPercentage);
                }
            }
            
            // Show discount badge if there's a discount
            if (maxDiscountPercentage > 0)
            {
                discountBadge.SetActive(true);
                savingsText.text = "Save " + Mathf.Round(maxDiscountPercentage) + "% on the yearly plan";
            }
            else
            {
                discountBadge.SetActive(false);
            }
        }
    }
    
    private void OnContinueButtonClicked()
    {
        if (string.IsNullOrEmpty(selectedPlanKey))
        {
            Debug.LogWarning("No plan selected!");
            return;
        }
        
        // Find the selected plan button
        PayWallPlanButton selectedButton = null;
        foreach (PayWallPlanButton button in planButtons)
        {
            if (button.GetPlanKey() == selectedPlanKey)
            {
                selectedButton = button;
                break;
            }
        }
        
        if (selectedButton != null)
        {
            // Process purchase
            ProcessPurchase(selectedButton.GetPlanKey(), selectedButton.GetPlanDuration(), selectedButton.GetPrice());
        }
    }
    
    private void ProcessPurchase(string planKey, string duration, float price)
    {
        // Implement your purchase logic here
        Debug.Log($"Processing purchase: {planKey}, {duration}, {price}₮");
        
        // This could call into your IAP manager or payment system
        // IAP.PurchaseSubscription(planKey);
    }
    
    private void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
        }
    }
}