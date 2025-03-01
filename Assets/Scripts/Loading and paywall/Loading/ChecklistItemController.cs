using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ChecklistItemController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isComplete;
    Tween tween;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup==null)
        {
            canvasGroup=gameObject.AddComponent<CanvasGroup>();
        }
        // Initialize all items as "incomplete" with alpha = 0.7
        canvasGroup.alpha = 0.2f;
        isComplete = false;
    }

    /// <summary>
    /// Animate this checklist item from alpha 0.7 to 1 (completed state).
    /// </summary>
    public void MarkAsComplete()
    {
        if (isComplete) return;
        isComplete = true;

        // Animate alpha to 1
       tween= DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 0.3f)
            .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Optional: reset to incomplete state if needed.
    /// </summary>
    public void ResetToIncomplete()
    {
        if (tween == null)return;
        if (!isComplete)return;

        tween.Kill();
        isComplete = false;
        canvasGroup.alpha = 0.2f;
    }
}