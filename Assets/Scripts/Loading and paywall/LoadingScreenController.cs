using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LocalizationSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Progress UI")]
    [SerializeField] private Image progressCircle;       // Circle progress indicator
    [SerializeField] private Image progressDot;          // Small dot indicator for early progress
    [SerializeField] private TextMeshProUGUI percentText; // Text showing percentage
    
    [Header("Checklist Items")]
    [SerializeField] private GameObject[] checklistItems;
    
    [Header("Animation Settings")]
    [SerializeField] private float totalLoadingTime = 3.5f;
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float dotToCircleThreshold = 0.15f; // When to switch from dot to circle
    [SerializeField] private float itemRevealDelay = 0.8f;       // Time before first item appears
    [SerializeField] private float itemRevealInterval = 0.4f;    // Time between items
    
    [Header("Transition")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool useFlowManager = true;
    
    private PersonalizationFlowManager flowManager;
    private bool isRTL = false;
    private bool isLoading = false;
    private Sequence loadingSequence;
    
    private void Awake()
    {
        // Find flow manager if needed
        if (useFlowManager)
        {
            flowManager = FindObjectOfType<PersonalizationFlowManager>();
        }
        
        // Setup initial UI state
        if (progressCircle != null)
        {
            progressCircle.fillAmount = 0f;
            progressCircle.gameObject.SetActive(false);
        }
        
        if (progressDot != null)
        {
            progressDot.gameObject.SetActive(true);
        }
        
        if (percentText != null)
        {
            percentText.text = "0%";
        }
        
        // Hide all checklist items initially
        if (checklistItems != null)
        {
            foreach (var item in checklistItems)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void OnEnable()
    {
        // Check RTL/LTR direction
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            LocalizationManager.Instance.OnLanguageChanged += UpdateRTLState;
        }
    
        // Get child name to update placeholders
        string childName = "";
        if (flowManager != null)
        {
            childName = flowManager.GetChildName();
        }
        else
        {
            childName = PlayerPrefs.GetString("ChildName", "[Name]");
        }
    
        // Optional: Check if child name is just the default placeholder
        if (childName == "[Name]")
        {
            // Could use a generic name or leave as is
            // childName = "Explorer";
        }
    
        // No need to manually update text that uses LocalizedText component
    
        // Start loading animation
        StartLoading();
    }
    
    private void OnDisable()
    {
        // Clean up
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateRTLState;
        }
        
        // Kill any active tweens
        if (loadingSequence != null && loadingSequence.IsActive())
        {
            loadingSequence.Kill();
        }
    }
    
    private void UpdateRTLState()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            
            // Localized text components handle themselves, no need to update them here
        }
    }
    
    public void StartLoading()
    {
        if (isLoading)
            return;
            
        isLoading = true;
        
        // Kill any existing sequence
        if (loadingSequence != null && loadingSequence.IsActive())
        {
            loadingSequence.Kill();
        }
        
        // Create a new sequence
        loadingSequence = DOTween.Sequence();
        
        // Add initial delay
        loadingSequence.AppendInterval(initialDelay);
        
        // Setup progress animation from 0% to 100%
        float switchTime = totalLoadingTime * dotToCircleThreshold;
        
        // First phase: dot only (0% to threshold%)
        loadingSequence.Append(DOTween.To(() => 0f, x => UpdateProgress(x), dotToCircleThreshold, switchTime)
            .SetEase(Ease.InSine));
        
        // Second phase: circle (threshold% to 100%)
        loadingSequence.Append(DOTween.To(() => dotToCircleThreshold, x => UpdateProgress(x), 1f, totalLoadingTime - switchTime)
            .SetEase(Ease.OutSine));
        
        // Add checklist item reveals
        for (int i = 0; i < checklistItems.Length; i++)
        {
            int index = i; // Capture for lambda
            
            // Calculate when this item should appear (as a percentage of total time)
            float itemTime = itemRevealDelay + (i * itemRevealInterval);
            float normalizedTime = itemTime / totalLoadingTime;
            
            // Add a callback at this point in the sequence
            loadingSequence.InsertCallback(itemTime, () => {
                if (index < checklistItems.Length && checklistItems[index] != null)
                {
                    RevealChecklistItem(index);
                }
            });
        }
        
        // Add completion callback
        loadingSequence.OnComplete(() => {
            isLoading = false;
            TransitionToNextScreen();
        });
        
        // Start the sequence
        loadingSequence.Play();
    }
    
    private void UpdateProgress(float progress)
    {
        // Update progress UI based on current progress
        if (percentText != null)
        {
            int percentage = Mathf.RoundToInt(progress * 100);
            percentText.text = percentage + "%";
        }
        
        // Handle dot to circle transition
        if (progress < dotToCircleThreshold)
        {
            // Show dot for early progress
            if (progressDot != null)
                progressDot.gameObject.SetActive(true);
                
            if (progressCircle != null)
                progressCircle.gameObject.SetActive(false);
        }
        else
        {
            // Switch to circle
            if (progressDot != null)
                progressDot.gameObject.SetActive(false);
                
            if (progressCircle != null)
            {
                progressCircle.gameObject.SetActive(true);
                
                // Calculate relative progress for the circle (from threshold to 100%)
                float relativeProgress = (progress - dotToCircleThreshold) / (1f - dotToCircleThreshold);
                progressCircle.fillAmount = relativeProgress;
            }
        }
    }
    
    private void RevealChecklistItem(int index)
    {
        if (index < 0 || index >= checklistItems.Length)
            return;
            
        GameObject item = checklistItems[index];
        if (item == null)
            return;
            
        // Make the item visible
        item.SetActive(true);
        
        // Animate with DOTween (fade in and scale)
        CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = item.AddComponent<CanvasGroup>();
        }
        
        // Set initial state
        canvasGroup.alpha = 0f;
        item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        // Create animation sequence for this item
        Sequence itemSequence = DOTween.Sequence();
        
        // Add fade and scale animations
        itemSequence.Append(canvasGroup.DOFade(1f, 0.3f));
        itemSequence.Join(item.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        
        // Play the sequence
        itemSequence.Play();
    }
    
    private void TransitionToNextScreen()
    {
        // Make sure all checklist items are visible
        foreach (var item in checklistItems)
        {
            if (item != null)
            {
                item.SetActive(true);
            }
        }
        
        // Small delay before transition
        DOVirtual.DelayedCall(0.5f, () => {
            // Use flow manager if available
            if (useFlowManager && flowManager != null)
            {
                flowManager.GoToNextPage();
            }
            // Otherwise load the next scene
            else if (!string.IsNullOrEmpty(nextSceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        });
    }
    
    // Public method to skip loading (for testing)
    public void SkipLoading()
    {
        // Kill existing sequence
        if (loadingSequence != null && loadingSequence.IsActive())
        {
            loadingSequence.Kill();
        }
        
        // Force progress to 100%
        UpdateProgress(1f);
        
        // Show all checklist items
        for (int i = 0; i < checklistItems.Length; i++)
        {
            if (checklistItems[i] != null)
            {
                checklistItems[i].SetActive(true);
                
                // Reset any animations
                CanvasGroup canvasGroup = checklistItems[i].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
                
                checklistItems[i].transform.localScale = Vector3.one;
            }
        }
        
        // Transition to next screen
        TransitionToNextScreen();
    }
}