using UnityEngine;
using DG.Tweening;

public class Tany_wellcome : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1f;    // Duration for fade in/out
    public float fadeStartAlpha = 0f;  // Starting alpha value
    public float fadeEndAlpha = 1f;    // Ending alpha value

    [Header("Movement Settings")]
    public float moveDuration = 1f;    // Duration for the up movement
    public float moveDistance = 50f;   // How far to move up (and back down)

    [Header("Loop Settings")]
    public bool loopIndefinitely = true;

    private CanvasGroup canvasGroup;
    private Sequence animationSequence;
    private bool isPaused = false;

    void Awake()
    {
        // Ensure the GameObject has a CanvasGroup for fading (if it's a UI element)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        CreateAnimation();
    }

    void CreateAnimation()
    {
        // Set initial alpha
        canvasGroup.alpha = fadeStartAlpha;
        
        // Create a new sequence for combined fade and movement
        animationSequence = DOTween.Sequence();

        // Fade in then fade out
        // animationSequence.Append(canvasGroup.DOFade(fadeEndAlpha, fadeDuration))
        //                  .Append(canvasGroup.DOFade(fadeStartAlpha, fadeDuration));

        // At the same time, add an up–down movement using Insert:
        // The movement will move the object up by 'moveDistance' then return to original position (Yoyo)
        animationSequence.Insert(0, transform.DOLocalMoveY(transform.position.y + moveDistance, moveDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));

        // If looping is desired, loop the sequence indefinitely.
        if (loopIndefinitely)
        {
            animationSequence.SetLoops(-1);
        }
    }

    /// <summary>
    /// Pauses the animation sequence.
    /// </summary>
    public void PauseAnimation()
    {
        if (animationSequence != null && animationSequence.IsPlaying())
        {
            animationSequence.Pause();
            isPaused = true;
        }
    }

    /// <summary>
    /// Resumes the animation sequence if it was paused.
    /// </summary>
    public void ResumeAnimation()
    {
        if (animationSequence != null && isPaused)
        {
            animationSequence.Play();
            isPaused = false;
        }
    }
}
