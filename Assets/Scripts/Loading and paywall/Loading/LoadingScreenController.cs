using UnityEngine;
using DG.Tweening;
using LocalizationSystem;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private DotCircleProgress dotCircleProgress; 
        // Reference to your DotCircleProgress script in the scene

    [Header("Checklist Items")]
    [SerializeField] private ChecklistItemController[] checklistItems;
        // Now each item has its own ChecklistItemController

    [Header("Timing Settings")]
    [SerializeField] private float totalLoadingTime = 3.5f;
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float itemRevealDelay = 0.8f;       
    [SerializeField] private float itemRevealInterval = 0.4f;    

    [Header("Transition")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool useFlowManager = true;

    private PersonalizationFlowManager flowManager;
    private bool isRTL = false;
    private bool isLoading = false;
    private Sequence loadingSequence;

    private void Awake()
    {
        // If using a personalization flow manager
        if (useFlowManager)
        {
            flowManager = FindObjectOfType<PersonalizationFlowManager>();
        }

        // We do NOT hide checklist items now,
        // because each ChecklistItemController sets alpha = 0.7 at start.
    }

    private void OnEnable()
    {
        // Check RTL/LTR (if you have a localization manager)
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            LocalizationManager.Instance.OnLanguageChanged += UpdateRTLState;
        }

        // Example: retrieving child name from flow manager or PlayerPrefs
        string childName = "";
        if (flowManager != null)
        {
            childName = flowManager.GetChildName();
        }
        else
        {
            childName = PlayerPrefs.GetString("ChildName", "[Name]");
        }

        // Start loading animation
        StartLoading();
    }

    private void OnDisable()
    {
        // Clean up localization callback
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateRTLState;
        }

        // Kill any active sequence
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
            // Update any RTL/LTR specific layout if needed
        }
    }

    /// <summary>
    /// Begins the loading process: animate progress from 0% to 100% and reveal checklist items.
    /// </summary>
    public void StartLoading()
    {
        if (isLoading) return;
        isLoading = true;

        // Kill any existing sequence
        if (loadingSequence != null && loadingSequence.IsActive())
        {
            loadingSequence.Kill();
        }

        // Reset DotCircleProgress (optional, if you want a fresh start each time)
        dotCircleProgress.Reset();

        // Also reset each checklist item (optional)
        foreach (var item in checklistItems)
        {
            item?.ResetToIncomplete();
        }

        // Create a new sequence
        loadingSequence = DOTween.Sequence();

        // Initial delay before starting progress
        loadingSequence.AppendInterval(initialDelay);

        // Animate the DotCircleProgress from 0 to 1 over totalLoadingTime
        loadingSequence.AppendCallback(() =>
        {
            // This will handle animating from 0 -> 100% in your DotCircleProgress
            dotCircleProgress.AnimateProgress(1f, totalLoadingTime - initialDelay);
        });

        // Schedule checklist items to "complete" at specified intervals
        for (int i = 0; i < checklistItems.Length; i++)
        {
            int index = i;
            float itemTime = itemRevealDelay + (i * itemRevealInterval);

            // Insert a callback at itemTime seconds into the sequence
            loadingSequence.InsertCallback(itemTime, () =>
            {
                if (index >= 0 && index < checklistItems.Length && checklistItems[index] != null)
                {
                    // This smoothly animates alpha from 0.7 to 1.0
                    checklistItems[index].MarkAsComplete();
                }
            });
        }

        // Once we've waited the total loading time (minus the initial delay we used at the start),
        // we consider the loading "complete".
        loadingSequence.AppendInterval(totalLoadingTime - initialDelay);

        // OnComplete => move to the next screen
        loadingSequence.OnComplete(() =>
        {
            isLoading = false;
            TransitionToNextScreen();
        });

        // Start the sequence
        loadingSequence.Play();
    }

    /// <summary>
    /// Transition to the next screen or flow step after loading completes.
    /// </summary>
    private void TransitionToNextScreen()
    {
        // Ensure all items are "complete" (alpha=1). 
        // (If you want to guarantee that in case of a skip or timing issue.)
        foreach (var item in checklistItems)
        {
            if (item != null)
            {
                item.MarkAsComplete(); 
            }
        }

        // Small delay before actually transitioning
        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (useFlowManager && flowManager != null)
            {
                // If using a Flow Manager
                flowManager.GoToNextPage();
            }
            else if (!string.IsNullOrEmpty(nextSceneName))
            {
                // Otherwise load by scene name
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        });
    }

    /// <summary>
    /// Allows skipping the loading process entirely (for testing/debug).
    /// </summary>
    public void SkipLoading()
    {
        // Kill any running sequence
        if (loadingSequence != null && loadingSequence.IsActive())
        {
            loadingSequence.Kill();
        }

        // Immediately set progress to 100%
        dotCircleProgress.UpdateProgress(1f);

        // Immediately mark all items complete (alpha=1)
        foreach (var item in checklistItems)
        {
            if (item != null) item.MarkAsComplete();
        }

        TransitionToNextScreen();
    }
}
