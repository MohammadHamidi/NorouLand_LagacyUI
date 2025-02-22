using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using DG.Tweening;

public class PulseTween : MonoBehaviour
{
    public float pulseScale = 1.2f; // Target scale for pulsing
    public float duration = 0.5f;   // Time to scale up and down
    private Vector3 originalScale;  // Store original scale
    private Tween pulseTween;       // Store tween reference
    public Ease easeType = Ease.Linear;
    private void OnEnable()
    {
        // Save the original scale
        originalScale = transform.localScale;

        // Ensure any previous tween is killed before starting a new one
        pulseTween?.Kill();

        // Create a pulsing tween that scales up and down
        pulseTween = transform.DOScale(originalScale * pulseScale, duration)
            .SetLoops(-1, LoopType.Yoyo) // Infinite loop with Yoyo effect
            .SetEase(easeType);    // Smooth transition
    }

    private void OnDisable()
    {
        // Kill the tween when the object is disabled to avoid unwanted behavior
        pulseTween?.Kill();
    }
}