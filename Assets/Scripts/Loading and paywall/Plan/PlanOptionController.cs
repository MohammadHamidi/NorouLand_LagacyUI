using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using RTLTMPro;

public class PlanOptionController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro priceText;
    [SerializeField] private RTLTextMeshPro pricePerMonthText;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private GameObject popularTag;
    [SerializeField] private GameObject economicalTag;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;
    
    [Header("Plan Data")]
    [SerializeField] private string planId;
    [SerializeField] private int durationMonths;
    [SerializeField] private float totalPrice;
    [SerializeField] private float pricePerMonth;
    [SerializeField] private bool _isPopular;
    [SerializeField] private bool _isEconomical;
    
    [Header("Selection Animation")]
    [SerializeField] private float selectionPunchDuration = 0.3f;
    [SerializeField] private Color selectedColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private Color normalColor = Color.white;
    
    private SubscriptionPlansController parentController;
    private bool isSelected = false;
    
    // Public properties
    public bool IsPopular { get { return _isPopular; } }
    public bool IsEconomical { get { return _isEconomical; } }
    
    private void Awake()
    {
        // Find parent controller
        parentController = GetComponentInParent<SubscriptionPlansController>();
        
        // Set up button listener
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnPlanSelected);
        }
        else
        {
            // If no dedicated button, make the whole plan clickable
            Button mainButton = GetComponent<Button>();
            if (mainButton != null)
            {
                selectButton = mainButton;
                selectButton.onClick.AddListener(OnPlanSelected);
            }
        }
        
        // Initialize UI
        UpdateUI();
    }
    
    private void Start()
    {
        // Check if this plan should be selected by default
        if (_isPopular && parentController != null)
        {
            parentController.SelectPlan(planId);
        }
        
        // Apply animation to popular tag if present
        if (popularTag != null && popularTag.activeSelf)
        {
            // Small bounce animation
            popularTag.transform.localScale = Vector3.zero;
            popularTag.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
        }
        
        // Apply animation to economical tag if present
        if (economicalTag != null && economicalTag.activeSelf)
        {
            // Small bounce animation
            economicalTag.transform.localScale = Vector3.zero;
            economicalTag.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.3f);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up
        if (selectButton != null)
        {
            selectButton.onClick.RemoveListener(OnPlanSelected);
        }
    }
    
    public void Initialize(string id, int months, float price, float monthlyPrice, bool popular, bool economical)
    {
        planId = id;
        durationMonths = months;
        totalPrice = price;
        pricePerMonth = monthlyPrice;
        _isPopular = popular;
        _isEconomical = economical;
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Update title text
        if (titleText != null)
        {
            // Look for LocalizedText component
            LocalizedText localizedTitle = titleText.GetComponent<LocalizedText>();
            if (localizedTitle != null)
            {
                // Let the LocalizedText component handle it
            }
            else
            {
                // Fallback text based on duration
                string title = "";
                switch (durationMonths)
                {
                    case 1: title = "1 Month"; break;
                    case 3: title = "3 Months"; break;
                    case 12: title = "1 Year"; break;
                    default: title = durationMonths + " Months"; break;
                }
                titleText.text = title;
            }
        }
        
        // Update price text
        if (priceText != null)
        {
            priceText.text = totalPrice.ToString("N0") + "₺";
        }
        
        // Update price per month text
        if (pricePerMonthText != null)
        {
            if (durationMonths > 1)
            {
                pricePerMonthText.text = pricePerMonth.ToString("N0") + "₺/Month";
                pricePerMonthText.gameObject.SetActive(true);
            }
            else
            {
                pricePerMonthText.gameObject.SetActive(false);
            }
        }
        
        // Show/hide tags
        if (popularTag != null)
        {
            popularTag.SetActive(_isPopular);
        }
        
        if (economicalTag != null)
        {
            economicalTag.SetActive(_isEconomical);
        }
        
        // Initialize selection state
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(false);
        }
    }
    
    private void OnPlanSelected()
    {
        // Notify parent controller
        if (parentController != null)
        {
            parentController.SelectPlan(planId);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        // Update selection indicator
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(selected);
            
            if (selected)
            {
                // Add a little animation for the selection
                selectedIndicator.transform.localScale = Vector3.zero;
                selectedIndicator.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }
        }
        
        // Update background color
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
        
        // Apply punch effect when selected
        if (selected)
        {
            transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), selectionPunchDuration, 2, 0.5f);
        }
    }
    
    public string GetPlanId()
    {
        return planId;
    }
}