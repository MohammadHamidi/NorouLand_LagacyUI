using UnityEngine;
using DG.Tweening;
using LocalizationSystem;

[RequireComponent(typeof(CanvasGroup))]
public class ChecklistItemAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float scaleUpDuration = 0.4f;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    
    private CanvasGroup canvasGroup;
    private bool isRTL = false;
    
    private void Awake()
    {
        // Get required components
        canvasGroup = GetComponent<CanvasGroup>();
        
        // Check RTL/LTR
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            LocalizationManager.Instance.OnLanguageChanged += UpdateRTLState;
        }
    }
    
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateRTLState;
        }
        
        // Kill all tweens targeting this object
        DOTween.Kill(gameObject);
    }
    
    private void UpdateRTLState()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
            UpdateLayout();
        }
    }
    
    private void UpdateLayout()
    {
        // RTL/LTR layout
        // The actual text handling is done by your LocalizedText component
        // This is only needed if there are other elements to adjust
        
        // Get all child RectTransforms that might need adjustment
        RectTransform[] childRects = GetComponentsInChildren<RectTransform>(true);
        
        foreach (var rect in childRects)
        {
            // Skip the main transform
            if (rect == transform)
                continue;
                
            // Check if this is an icon that needs to be flipped
            // You may need to add tags or names to identify specific elements
            if (rect.name.Contains("Icon") || rect.name.Contains("Check"))
            {
                float xPos = Mathf.Abs(rect.anchoredPosition.x);
                rect.anchoredPosition = new Vector2(isRTL ? xPos : -xPos, rect.anchoredPosition.y);
            }
        }
    }
    
    public void PlayRevealAnimation()
    {
        // Kill existing tweens
        DOTween.Kill(gameObject);
        
        // Set initial state
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        // Create sequence
        Sequence sequence = DOTween.Sequence();
        
        // Add animations
        sequence.Append(canvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeEase));
        sequence.Join(transform.DOScale(1f, scaleUpDuration).SetEase(scaleEase));
        
        // Play sequence
        sequence.Play();
    }
    
    // Reset to fully visible state (for skipping animations)
    public void ResetToVisible()
    {
        DOTween.Kill(gameObject);
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
    }
}