using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlipImageAnimation : MonoBehaviour
{
    [Header("Image & Sprites")]
    [Tooltip("Target Image to animate.")]
    public Image targetImage;
    [Tooltip("The original sprite.")]
    public Sprite originalSprite;
    [Tooltip("The sprite to display on flip.")]
    public Sprite flippedSprite;

    [Header("Animation Settings")]
    [Tooltip("Total duration for the flip (scale down and up).")]
    public float flipDuration = 0.5f;
    [Tooltip("Delay between consecutive flips.")]
    public float flipInterval = 2f;
    [Tooltip("Should the flip animation loop indefinitely?")]
    public bool loopAnimation = true;

    private Sequence flipSequence;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        // Ensure the image starts at full scale.
        targetImage.rectTransform.localScale = Vector3.one;
        CreateFlipAnimation();
    }

    private void CreateFlipAnimation()
    {
        // Kill any existing tween.
        if (flipSequence != null)
            flipSequence.Kill();

        flipSequence = DOTween.Sequence();

        // Scale down X from 1 to 0.
        flipSequence.Append(targetImage.rectTransform.DOScaleX(0f, flipDuration / 2f)
            .SetEase(Ease.InQuad));
        
        // Swap sprite.
        flipSequence.AppendCallback(() =>
        {
            targetImage.sprite = (targetImage.sprite == originalSprite) ? flippedSprite : originalSprite;
        });

        // Scale up X from 0 back to 1.
        flipSequence.Append(targetImage.rectTransform.DOScaleX(1f, flipDuration / 2f)
            .SetEase(Ease.OutQuad));

        // Add an interval between flips.
        flipSequence.AppendInterval(flipInterval);

        // Loop if enabled.
        if (loopAnimation)
            flipSequence.SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        if (flipSequence != null)
            flipSequence.Kill();
    }
}
