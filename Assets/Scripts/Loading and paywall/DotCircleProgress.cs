using DG.Tweening;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


using UnityEngine;

public class DotCircleProgress : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image progressCircle;
    [SerializeField] private Image progressDot;
    [SerializeField] private RTLTextMeshPro percentText;
    
    [Header("Settings")]
    [SerializeField] private float dotToCircleThreshold = 0.15f;
    [SerializeField] private Color progressColor = new Color(1f, 0.6f, 0f); // Orange
    
    private Sequence progressSequence;
    private void PositionDotOnCircle(float fillAmount)
    {
        if (progressDot == null || progressCircle == null)
            return;

        RectTransform circleRect = progressCircle.GetComponent<RectTransform>();
        RectTransform dotRect    = progressDot.GetComponent<RectTransform>();

        // Half the circle's width
        float circleRadius = circleRect.rect.width * 0.5f;

        // Fill amount -> angle
        float angleDegrees  = fillAmount * 360f;
        float adjustedAngle = 90f - angleDegrees;
        float angleRadians  = adjustedAngle * Mathf.Deg2Rad;

        // If pivot is at dot's bottom, we need to shrink the radius by half the dot's height
        // so the dot's center lines up with the circle's edge.
        float dotHeight     = dotRect.rect.height;
        float effectiveRadius = circleRadius - (dotHeight * 0.5f);

        float xPos = effectiveRadius * Mathf.Cos(angleRadians);
        float yPos = effectiveRadius * Mathf.Sin(angleRadians);

        // Place the dot
        dotRect.anchoredPosition = new Vector2(xPos, yPos);
    }


    private void Awake()
    {
        // Initialize progress elements
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
    }
    
    private void OnDestroy()
    {
        // Kill any running sequences
        if (progressSequence != null && progressSequence.IsActive())
        {
            progressSequence.Kill();
        }
    }
    
    // Method to update progress display (0-1 range)
    public void UpdateProgress(float progress)
    {
        // Update percentage text
        if (percentText != null)
        {
            int percentage = Mathf.RoundToInt(progress * 100);
            percentText.text = percentage + "%";
        }

        // Handle dot/circle visibility based on progress
        if (progress < dotToCircleThreshold)
        {
            // Early progress - show dot only (center the dot in the circle, if you want)
            if (progressDot != null)
            {
                progressDot.gameObject.SetActive(true);
                // Optionally place the dot in center if the circle is not visible yet:
                var dotRect = progressDot.GetComponent<RectTransform>();
                if (dotRect != null) dotRect.anchoredPosition = Vector2.zero;
            }
            
            if (progressCircle != null)
                progressCircle.gameObject.SetActive(false);
        }
        else
        {
            // Later progress - switch to circle
            if (progressCircle != null)
            {
                progressCircle.gameObject.SetActive(true);

                // Remap progress from threshold-1 range to 0-1 range for fill amount
                float remappedProgress = (progress - dotToCircleThreshold) / (1f - dotToCircleThreshold);
                progressCircle.fillAmount = remappedProgress;

                // Position the dot as a round cap on the circle's edge
                if (progressDot != null)
                {
                    progressDot.gameObject.SetActive(true);
                    PositionDotOnCircle(remappedProgress);
                }
            }
        }
    }

    
    // Animate progress from current to target value
    public void AnimateProgress(float targetProgress, float duration)
    {
        // Get current progress
        float currentProgress = 0f;
        if (percentText != null)
        {
            string currentText = percentText.text;
            if (currentText.EndsWith("%"))
            {
                currentText = currentText.Substring(0, currentText.Length - 1);
                if (float.TryParse(currentText, out float percentage))
                {
                    currentProgress = percentage / 100f;
                }
            }
        }
        
        // Kill any running sequence
        if (progressSequence != null && progressSequence.IsActive())
        {
            progressSequence.Kill();
        }
        
        // Create new animation
        progressSequence = DOTween.Sequence();
        
        // Animate progress
        progressSequence.Append(DOTween.To(() => currentProgress, x => UpdateProgress(x), targetProgress, duration)
            .SetEase(Ease.InOutSine));
        
        // Play the sequence
        progressSequence.Play();
    }
    
    // Reset to initial state
    public void Reset()
    {
        // Kill any running sequence
        if (progressSequence != null && progressSequence.IsActive())
        {
            progressSequence.Kill();
        }
        
        // Reset UI
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
    }
}