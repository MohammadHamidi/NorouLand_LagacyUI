using UnityEngine;
using UnityEngine.UI;
using TMPro;  // If you're using TextMeshPro
using DG.Tweening;

public class SimpleBarChartAnimator : MonoBehaviour
{
    [Header("Bars")]
    public RectTransform leftBar;
    public RectTransform rightBar;

    [Header("Bar Images (Optional)")]
    public Image leftBarImage;
    public Image rightBarImage;

    [Header("Texts")]
    public TextMeshProUGUI leftBarText;
    public TextMeshProUGUI rightBarText;

    [Header("Animation Settings")]
    public float leftBarFinalHeight = 300f;
    public float rightBarFinalHeight = 420f;
    public float barGrowDuration = 1.0f;
    public float secondBarDelay = 0.5f;

    // Example final values
    private float leftBarValue = 5.76f;
    private float rightBarValue = 8.08f;

    [Header("Post-Animation Image")]
    [Tooltip("An image that will appear after both bars have finished animating.")]
    public Image finalImage;

    private void Start()
    {
        // Reset bars to zero height
        leftBar.sizeDelta = new Vector2(leftBar.sizeDelta.x, 0f);
        rightBar.sizeDelta = new Vector2(rightBar.sizeDelta.x, 0f);

        // Reset bar text to "0"
        if (leftBarText) leftBarText.text = "0.00";
        if (rightBarText) rightBarText.text = "0.00";

        // Hide the final image initially (if assigned)
        if (finalImage != null)
            finalImage.gameObject.SetActive(false);

        AnimateBars();
    }

    private void AnimateBars()
    {
        // Create a DOTween sequence
        Sequence seq = DOTween.Sequence();

        // 1. Animate the left bar from 0 to leftBarFinalHeight
        seq.Append(
            DOTween.To(
                () => leftBar.sizeDelta,
                x => leftBar.sizeDelta = x,
                new Vector2(leftBar.sizeDelta.x, leftBarFinalHeight),
                barGrowDuration
            )
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                float currentHeight = leftBar.sizeDelta.y;
                float progress = currentHeight / leftBarFinalHeight; // 0..1
                float currentValue = Mathf.Lerp(0f, leftBarValue, progress);
                if (leftBarText) leftBarText.text = currentValue.ToString("0.00");
            })
        );

        // 2. Delay before animating the right bar
        seq.AppendInterval(secondBarDelay);

        // 3. Animate the right bar from 0 to rightBarFinalHeight
        seq.Append(
            DOTween.To(
                () => rightBar.sizeDelta,
                x => rightBar.sizeDelta = x,
                new Vector2(rightBar.sizeDelta.x, rightBarFinalHeight),
                barGrowDuration
            )
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                float currentHeight = rightBar.sizeDelta.y;
                float progress = currentHeight / rightBarFinalHeight; // 0..1
                float currentValue = Mathf.Lerp(0f, rightBarValue, progress);
                if (rightBarText) rightBarText.text = currentValue.ToString("0.00");
            })
        );

        // 4. After the second bar completes, show the final image
        seq.OnComplete(() =>
        {
            if (finalImage != null)
            {
                finalImage.gameObject.SetActive(true);
            }
        });
    }
}
