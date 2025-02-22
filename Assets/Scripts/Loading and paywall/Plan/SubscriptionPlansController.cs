using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using System.Collections.Generic;
using RTLTMPro;

public class SubscriptionPlansController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro headerText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Transform plansContainer;

    [Header("Plan Options")]
    [SerializeField] private List<PlanOptionController> planOptions = new List<PlanOptionController>();
    [SerializeField] private GameObject planOptionPrefab;
    
    [Header("Testimonial")]
    [SerializeField] private Image testimonialAvatar;
    [SerializeField] private RTLTextMeshPro testimonialName;
    [SerializeField] private RTLTextMeshPro testimonialText;
    [SerializeField] private GameObject starsContainer;
    
    [Header("Animation")]
    [SerializeField] private float entryDelayBetweenPlans = 0.1f;
    [SerializeField] private float planEntryDuration = 0.3f;
    [SerializeField] private Ease planEntryEase = Ease.OutBack;
    
    private PersonalizationFlowManager flowManager;
    private string selectedPlanId;
    
    private void Awake()
    {
        // Find the flow manager
        flowManager = FindObjectOfType<PersonalizationFlowManager>();
        
        // Set up button listeners
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        if (privacyPolicyButton != null)
        {
            privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }
        
        // Find plan options if not already set
        if (planOptions.Count == 0)
        {
            PlanOptionController[] foundPlans = plansContainer != null ? 
                plansContainer.GetComponentsInChildren<PlanOptionController>(true) : 
                GetComponentsInChildren<PlanOptionController>(true);
                
            planOptions.AddRange(foundPlans);
        }
        
        // If we still have no plan options but have a prefab and container, create default plans
        if (planOptions.Count == 0 && planOptionPrefab != null && plansContainer != null)
        {
            CreateDefaultPlans();
        }
    }
    
    private void Start()
    {
        // Update texts for localization
        UpdateTexts();
        
        // Animate plan options entrance
        AnimatePlanEntrances();
        
        // Select default plan
        string savedPlanId = PlayerPrefs.GetString("SelectedPlan", "");
        if (!string.IsNullOrEmpty(savedPlanId))
        {
            SelectPlan(savedPlanId);
        }
        else
        {
            SelectPlan(GetDefaultPlanId());
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }
        
        if (privacyPolicyButton != null)
        {
            privacyPolicyButton.onClick.RemoveListener(OnPrivacyPolicyClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }
    }
    
    private void CreateDefaultPlans()
    {
        // Define default plans as seen in the images
        PlanOptionInitData[] defaultPlans = new PlanOptionInitData[]
        {
            new PlanOptionInitData
            {
                planId = "monthly",
                durationMonths = 1,
                totalPrice = 59.000f,
                pricePerMonth = 59.000f,
                isPopular = false,
                isEconomical = false
            },
            new PlanOptionInitData
            {
                planId = "quarterly",
                durationMonths = 3,
                totalPrice = 115.000f,
                pricePerMonth = 38.000f,
                isPopular = true,
                isEconomical = false
            },
            new PlanOptionInitData
            {
                planId = "yearly",
                durationMonths = 12,
                totalPrice = 340.000f,
                pricePerMonth = 28.000f,
                isPopular = false,
                isEconomical = true
            }
        };
        
        // Create plan UI elements
        for (int i = 0; i < defaultPlans.Length; i++)
        {
            GameObject planObj = Instantiate(planOptionPrefab, plansContainer);
            PlanOptionInitData planData = defaultPlans[i];
            
            // Get the controller and initialize it
            PlanOptionController planController = planObj.GetComponent<PlanOptionController>();
            if (planController != null)
            {
                planController.Initialize(
                    planData.planId,
                    planData.durationMonths,
                    planData.totalPrice,
                    planData.pricePerMonth,
                    planData.isPopular,
                    planData.isEconomical
                );
                
                planOptions.Add(planController);
            }
        }
    }
    
    private void UpdateTexts()
    {
        // Update header text with discount info
        if (headerText != null)
        {
            // Direct text update for now
            headerText.text = "Save 60% on the yearly plan";
        }
        
        // Update testimonial if necessary
        if (testimonialName != null && testimonialText != null)
        {
            // Direct update for testimonials
            testimonialName.text = "Mahboube";
            testimonialText.text = "The only mobile game that's actually useful and educational.";
        }
    }
    
    private string GetDefaultPlanId()
    {
        // Default to most popular plan
        foreach (var plan in planOptions)
        {
            if (plan != null && plan.IsPopular)
            {
                return plan.GetPlanId();
            }
        }
        
        // If no popular plan, return first plan or empty string
        return planOptions.Count > 0 ? planOptions[0].GetPlanId() : "";
    }
    
    private void AnimatePlanEntrances()
    {
        for (int i = 0; i < planOptions.Count; i++)
        {
            var planTransform = planOptions[i].transform;
            
            // Set initial state
            planTransform.localScale = Vector3.zero;
            
            // Create animation
            planTransform.DOScale(1f, planEntryDuration)
                .SetEase(planEntryEase)
                .SetDelay(i * entryDelayBetweenPlans);
        }
    }
    
    public void SelectPlan(string planId)
    {
        selectedPlanId = planId;
        
        // Update UI for all plans
        foreach (var plan in planOptions)
        {
            bool isSelected = plan.GetPlanId() == planId;
            plan.SetSelected(isSelected);
        }
        
        // Save selected plan
        if (flowManager != null)
        {
            flowManager.SetSelectedPlan(selectedPlanId);
        }
        else
        {
            PlayerPrefs.SetString("SelectedPlan", selectedPlanId);
            PlayerPrefs.Save();
        }
        
        // Update continue button
        if (continueButton != null)
        {
            // Make sure it's enabled
            continueButton.interactable = true;
        }
    }
    
    private void OnContinueClicked()
    {
        // Proceed with selected plan
        if (flowManager != null)
        {
            flowManager.ConfirmSubscription(selectedPlanId);
        }
        else
        {
            // Fallback if flow manager not found
            Debug.Log("Subscription confirmed: " + selectedPlanId);
        }
    }
    
    private void OnPrivacyPolicyClicked()
    {
        // Show privacy policy
        if (flowManager != null)
        {
            flowManager.ShowPrivacyPolicy();
        }
        else
        {
            // Fallback if flow manager not found
            Application.OpenURL("https://example.com/privacy");
        }
    }
    
    private void OnCloseClicked()
    {
        // Handle closing this screen
        if (flowManager != null)
        {
            flowManager.CloseCurrentScreen();
        }
        else
        {
            // Fallback if flow manager not found
            gameObject.SetActive(false);
        }
    }
    
    // Helper class for creating default plans
    private class PlanOptionInitData
    {
        public string planId;
        public int durationMonths;
        public float totalPrice;
        public float pricePerMonth;
        public bool isPopular;
        public bool isEconomical;
    }
}