using System.Collections;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class KidCardController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro nameAgeText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Image backgroundPanel;
    
    [Header("Localization")]
    [SerializeField] private bool isRTL = true;
    
    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float cardAppearDelay = 0.2f;
    
    // Cached components
    private CanvasGroup canvasGroup;
    private Coroutine animationCoroutine;
    
    private void Awake()
    {
        // Get or add CanvasGroup for fade effects
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    private void OnEnable()
    {
        // If we're currently invisible, restart the fade-in animation
        if (canvasGroup.alpha < 0.1f)
        {
            StartAnimateCardAppearance();
        }
    }
    
    private void OnDisable()
    {
        // Clean up coroutine
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }
    
    // Public method to set up the kid card with profile data
    public void SetupKidCard(KidProfile profile, RadarChart radarChart = null)
    {
        // Update text fields
        if (nameAgeText != null)
        {
            if (isRTL)
                nameAgeText.text = profile.age + " سالە " + profile.name;
            else
                nameAgeText.text = profile.name + " " + profile.age + " years";
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = profile.subtitle;
        }
        
        // Update avatar
        if (avatarImage != null && profile.avatar != null)
        {
            avatarImage.sprite = profile.avatar;
        }
        
        // We no longer manage the radar chart directly
        // The KidProfileManager will update it for us
        
        // Apply fade-in animation if the object is active
        if (gameObject.activeInHierarchy)
        {
            StartAnimateCardAppearance();
        }
    }
    
    // Helper method to start animation safely
    private void StartAnimateCardAppearance()
    {
        // Stop any existing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        // Start new animation
        animationCoroutine = StartCoroutine(AnimateCardAppearance());
    }
    
    // Animation for card appearance
    private IEnumerator AnimateCardAppearance()
    {
        // Start fully transparent
        canvasGroup.alpha = 0f;
        
        // Wait for delay
        yield return new WaitForSeconds(cardAppearDelay);
        
        // Check if we're still active
        if (!gameObject.activeInHierarchy)
        {
            animationCoroutine = null;
            yield break;
        }
        
        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration && gameObject.activeInHierarchy)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we end at fully visible if still active
        if (gameObject.activeInHierarchy)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Clear coroutine reference
        animationCoroutine = null;
    }
    
    // Optional method to create kid card programmatically
    public static GameObject CreateKidCard(Transform parent, KidProfile profile, GameObject cardPrefab, RadarChart radarChart = null)
    {
        GameObject kidCardObj = Instantiate(cardPrefab, parent);
        KidCardController controller = kidCardObj.GetComponent<KidCardController>();
        
        if (controller != null)
        {
            controller.SetupKidCard(profile, radarChart);
        }
        else
        {
            Debug.LogError("KidCardController component not found on prefab!");
        }
        
        return kidCardObj;
    }
}