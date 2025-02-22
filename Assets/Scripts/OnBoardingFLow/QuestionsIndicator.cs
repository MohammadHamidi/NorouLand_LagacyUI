using System.Collections.Generic;
using LocalizationSystem;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsIndicator : MonoBehaviour
{
    [Header("Indicators")]
    [SerializeField] private List<Image> stepDots = new List<Image>();
    
    [Header("Colors")]
    [SerializeField] private Color activeColor = new Color(1f, 0.6f, 0f); // Orange
    [SerializeField] private Color inactiveColor = new Color(0.9f, 0.9f, 0.9f); // Light Gray
    
    [Header("Navigation")]
    [SerializeField] private GameObject backArrow;
    [SerializeField] private GameObject forwardArrow;
    
    [Header("RTL Settings")]
    [SerializeField] private bool reverseDotsInRTL = true;
    [SerializeField] private bool flipArrowsInRTL = true;
    [SerializeField] private bool swapArrowsInRTL = true;
    
    private int currentStep = 0;
    private bool isRTL = false;
    private bool isInitialized = false;
    private List<RectTransform> dotRectTransforms = new List<RectTransform>();
    private List<int> originalDotSiblingIndices = new List<int>();
    
    private void Awake()
    {
        // Store original dot order and transforms
        foreach (Image dot in stepDots)
        {
            RectTransform rectTransform = dot.GetComponent<RectTransform>();
            dotRectTransforms.Add(rectTransform);
            originalDotSiblingIndices.Add(rectTransform.GetSiblingIndex());
        }
    }
    
    private void Start()
    {
        // Initialize based on language settings
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            LocalizationManager.Instance.OnLanguageChanged += UpdateRTLState;
        }
        
        // Apply RTL changes if needed
        ApplyRTLChanges();
        
        // Initial update
        UpdateStepDisplay();
        UpdateNavigationArrows();
        
        isInitialized = true;
    }
    
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateRTLState;
        }
    }
    
    private void UpdateRTLState()
    {
        if (LocalizationManager.Instance != null)
        {
            bool wasRTL = isRTL;
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Only reapply if the direction changed
            if (wasRTL != isRTL)
            {
                ApplyRTLChanges();
                UpdateStepDisplay();
                UpdateNavigationArrows();
            }
        }
    }
    
    private void ApplyRTLChanges()
    {
        // Handle the step dots ordering/placement for RTL
        if (reverseDotsInRTL)
        {
            // In RTL mode, we need to reverse the order of dots in the layout
            // This can be done by adjusting sibling indices or using negative scale
            
            if (isRTL)
            {
                // Option 1: Reverse the sibling order
                for (int i = 0; i < dotRectTransforms.Count; i++)
                {
                    dotRectTransforms[i].SetSiblingIndex(dotRectTransforms.Count - 1 - i);
                }
                
                // Option 2: You could alternatively use a negative scale on the parent container
                // transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // Restore original order
                for (int i = 0; i < dotRectTransforms.Count; i++)
                {
                    dotRectTransforms[i].SetSiblingIndex(originalDotSiblingIndices[i]);
                }
                
                // transform.localScale = Vector3.one;
            }
        }
        
        // Handle arrow directions
        if (flipArrowsInRTL)
        {
            // Flip arrow graphics for RTL
            if (backArrow != null)
            {
                RectTransform rectTransform = backArrow.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = new Vector3(isRTL ? -1 : 1, 1, 1);
                }
            }
            
            if (forwardArrow != null)
            {
                RectTransform rectTransform = forwardArrow.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = new Vector3(isRTL ? -1 : 1, 1, 1);
                }
            }
        }
        
        // Swap arrow positions for RTL if needed
        if (swapArrowsInRTL && backArrow != null && forwardArrow != null)
        {
            RectTransform backRect = backArrow.GetComponent<RectTransform>();
            RectTransform forwardRect = forwardArrow.GetComponent<RectTransform>();
            
            if (isRTL)
            {
                // Store positions
                Vector2 backPos = backRect.anchoredPosition;
                Vector2 forwardPos = forwardRect.anchoredPosition;
                
                // Swap X coordinates
                backRect.anchoredPosition = new Vector2(-forwardPos.x, backPos.y);
                forwardRect.anchoredPosition = new Vector2(-backPos.x, forwardPos.y);
            }
            else
            {
                // Reset to original positions (depends on your UI setup)
                // You might need to store original positions if they're not symmetric
                backRect.anchoredPosition = new Vector2(-Mathf.Abs(backRect.anchoredPosition.x), backRect.anchoredPosition.y);
                forwardRect.anchoredPosition = new Vector2(Mathf.Abs(forwardRect.anchoredPosition.x), forwardRect.anchoredPosition.y);
            }
        }
    }
    
    public void SetCurrentStep(int step)
    {
        if (step >= 0 && step < stepDots.Count)
        {
            currentStep = step;
            UpdateStepDisplay();
            UpdateNavigationArrows();
        }
    }
    
    private void UpdateStepDisplay()
    {
        // Update all dot colors based on current step
        for (int i = 0; i < stepDots.Count; i++)
        {
            int index = i;
            
            // In RTL mode, the visual display should still follow the semantic order
            // This means that if we didn't reverse the dots physically, we need to reverse the coloring logic
            if (isRTL && !reverseDotsInRTL)
            {
                index = stepDots.Count - 1 - i;
            }
            
            if (index <= currentStep)
            {
                // Current or completed steps - active color
                stepDots[i].color = activeColor;
            }
            else
            {
                // Future steps - inactive color
                stepDots[i].color = inactiveColor;
            }
        }
    }
    
    private void UpdateNavigationArrows()
    {
        // In RTL mode, the navigation logic is reversed
        bool showBackArrow, showForwardArrow;
        
        if (isRTL)
        {
            // In RTL, "back" is visually forward (next) and "forward" is visually back (previous)
            showBackArrow = currentStep < stepDots.Count - 1;  // Show when not on last step
            showForwardArrow = currentStep > 0;                // Show when not on first step
        }
        else
        {
            // Standard LTR navigation
            showBackArrow = currentStep > 0;                  // Show when not on first step
            showForwardArrow = currentStep < stepDots.Count - 1; // Show when not on last step
        }
        
        // Apply visibility
        if (backArrow != null)
        {
            backArrow.SetActive(showBackArrow);
        }
        
        if (forwardArrow != null)
        {
            forwardArrow.SetActive(showForwardArrow);
        }
    }
    
    // Public methods to navigate - these maintain the SEMANTIC direction
    // Next always means "proceed to next step" regardless of RTL/LTR
    public void NextStep()
    {
        if (currentStep < stepDots.Count - 1)
        {
            currentStep++;
            UpdateStepDisplay();
            UpdateNavigationArrows();
        }
    }
    
    // Previous always means "go back to previous step" regardless of RTL/LTR
    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateStepDisplay();
            UpdateNavigationArrows();
        }
    }
    
    // These methods handle visual button clicks based on screen layout
    // For use with UI buttons that are positioned left/right on screen
    public void OnLeftArrowClicked()
    {
        if (isRTL)
        {
            // In RTL, left arrow visually means "next"
            NextStep();
        }
        else
        {
            // In LTR, left arrow visually means "previous"
            PreviousStep();
        }
    }
    
    public void OnRightArrowClicked()
    {
        if (isRTL)
        {
            // In RTL, right arrow visually means "previous"
            PreviousStep();
        }
        else
        {
            // In LTR, right arrow visually means "next"
            NextStep();
        }
    }
}