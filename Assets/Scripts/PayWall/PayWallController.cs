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
    [SerializeField] private int defaultSelectedIndex = 1; // Default to the second option (0-based index)
    [SerializeField] private bool manualPlanPopulation = false; // When true, plans are populated manually from inspector
    [SerializeField]
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
        if (manualPlanPopulation)
        {
            // When using manual population, use existing plan buttons from the inspector
            // Just update their settings and register the controller
            foreach (PayWallPlanButton planButton in planButtons)
            {
                if (planButton != null)
                {
                    // Register the controller with the button
                    planButton.SetController(this, isRTL);
                }
            }
        }
        else
        {
            // For dynamic population, clear any existing buttons first
            foreach (Transform child in planButtonsContainer)
            {
                Destroy(child.gameObject);
            }
            planButtons.Clear();

            // Create new buttons based on config
            for (int i = 0; i < planConfigs.Length; i++)
            {
                PlanConfig config = planConfigs[i];
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
            }
        }
        
        if (planButtons.Count > 0)
        {
            // Select default plan based on index
            if (defaultSelectedIndex >= 0 && defaultSelectedIndex < planButtons.Count)
            {
                SelectPlan(planButtons[defaultSelectedIndex].GetPlanKey());
            }
            else if (!manualPlanPopulation)
            {
                // Fallback to the first plan with defaultSelected=true in the config
                bool foundDefault = false;
                foreach (PlanConfig config in planConfigs)
                {
                    if (config.defaultSelected)
                    {
                        SelectPlan(config.planKey);
                        foundDefault = true;
                        break;
                    }
                }
                
                // If no default is specified, select the first one
                if (!foundDefault)
                {
                    SelectPlan(planButtons[0].GetPlanKey());
                }
            }
            else
            {
                // For manual population, just select the first one if no valid index
                SelectPlan(planButtons[0].GetPlanKey());
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
    
    // New method to select plan by index
    public void SelectPlanByIndex(int index)
    {
        if (index >= 0 && index < planButtons.Count)
        {
            SelectPlan(planButtons[index].GetPlanKey());
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