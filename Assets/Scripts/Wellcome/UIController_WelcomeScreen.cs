using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIController_WelcomeScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup textA1;
    public CanvasGroup textA2;
    public Tany_wellcome tanyWelcome;

    public CanvasGroup textB1;
    public CanvasGroup textB2;
    public Button button1;
    public Button button2;

    public DynamicStarField starField;

    [Header("Animation Settings")]
    public float fadeDuration = 1f;
    public float delayBeforeScrolling = 0.5f;
    public float delayAfterScrolling = 0.5f;
    public float autoTransitionTime = 5f;  // 5 seconds delay for auto transition

    public event Action OnButton1Click;
    public event Action OnButton2Click; 
    

    private bool hasTransitionedToStateB = false;

    private void Start()
    {
        Debug.Log($"Hi");
        // Ensure elements start at correct visibility
        SetCanvasGroupAlpha(textA1, 0);
        SetCanvasGroupAlpha(textA2, 0);
        SetCanvasGroupAlpha(textB1, 0);
        SetCanvasGroupAlpha(textB2, 0);
        SetCanvasGroupAlpha(button1.GetComponent<CanvasGroup>(), 0);
        SetCanvasGroupAlpha(button2.GetComponent<CanvasGroup>(), 0);

        // Attach button listeners
        button1.onClick.AddListener(() => OnButton1Click?.Invoke());
        button2.onClick.AddListener(() => OnButton2Click?.Invoke());

        // Start in State A
        EnterStateA();

        // Schedule auto transition after 5 seconds
        Invoke(nameof(EnterStateB), autoTransitionTime);
    }

    private void Update()
    {
        // Detect touch or click input to transition early
        if (!hasTransitionedToStateB && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            EnterStateB();
        }
    }

    private void SetCanvasGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
            group.alpha = alpha;
    }

    public void EnterStateA()
    {
        Sequence seq = DOTween.Sequence();

        // Fade in Text A1 & A2
        seq.Append(textA1.DOFade(1, fadeDuration))
           .Join(textA2.DOFade(1, fadeDuration));

        // Enable Tany animation
        if (tanyWelcome != null)
        {
            tanyWelcome.ResumeAnimation();
        }
    }

    public void EnterStateB()
    {
        if (hasTransitionedToStateB) return; // Prevent multiple calls
        hasTransitionedToStateB = true;

        Sequence seq = DOTween.Sequence();

        // Fade out Text A1, A2 and stop Tany animation
        seq.Append(textA1.DOFade(0, fadeDuration))
           .Join(textA2.DOFade(0, fadeDuration))
           .OnComplete(() => {
               if (tanyWelcome != null)
               {
                   tanyWelcome.PauseAnimation();
               }
           });

        // Start star field scrolling after a short delay
        seq.AppendInterval(delayBeforeScrolling)
           .AppendCallback(() => starField.TriggerScroll());

        // After scrolling completes, fade in new elements
        seq.AppendInterval(starField.scrollDuration + delayAfterScrolling)
           .Append(textB1.DOFade(1, fadeDuration))
           .Join(textB2.DOFade(1, fadeDuration))
           .Join(button1.GetComponent<CanvasGroup>().DOFade(1, fadeDuration))
           .Join(button2.GetComponent<CanvasGroup>().DOFade(1, fadeDuration));
    }
}
