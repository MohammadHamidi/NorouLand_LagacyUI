using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using System.Collections;
using RTLTMPro;

public class CongratulationsScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro congratsText;
    [SerializeField] private RTLTextMeshPro instructionText;
    [SerializeField] private Button startPlayingButton;
    [SerializeField] private GameObject mascotCharacter;
    [SerializeField] private GameObject starIcon;
    
    [Header("Animation")]
    [SerializeField] private float mascotBounceDuration = 0.6f;
    [SerializeField] private Ease mascotBounceEase = Ease.OutBack;
    [SerializeField] private float starBounceDuration = 0.5f;
    [SerializeField] private Ease starBounceEase = Ease.OutBack;
    [SerializeField] private float textFadeInDelay = 0.3f;
    [SerializeField] private float textFadeInDuration = 0.5f;
    [SerializeField] private float starFloatDuration = 1.5f;
    [SerializeField] private float starFloatDistance = 0.2f;
    
    [Header("Confetti")]
    [SerializeField] private GameObject confettiParticleSystem;
    [SerializeField] private float confettiDuration = 3f;
    
    private PersonalizationFlowManager flowManager;
    
    private void Awake()
    {
        // Find the flow manager
        flowManager = FindObjectOfType<PersonalizationFlowManager>();
        
        // Set up button listener
        if (startPlayingButton != null)
        {
            startPlayingButton.onClick.AddListener(OnStartPlayingClicked);
        }
        
        // Set initial state for animations
        if (mascotCharacter != null)
        {
            mascotCharacter.transform.localScale = Vector3.zero;
        }
        
        if (starIcon != null)
        {
            starIcon.transform.localScale = Vector3.zero;
        }
        
        if (congratsText != null)
        {
            // Add canvas group for fading if needed
            CanvasGroup congratsGroup = congratsText.GetComponent<CanvasGroup>();
            if (congratsGroup == null)
            {
                congratsGroup = congratsText.gameObject.AddComponent<CanvasGroup>();
            }
            congratsGroup.alpha = 0f;
        }
        
        if (instructionText != null)
        {
            // Add canvas group for fading if needed
            CanvasGroup instructionGroup = instructionText.GetComponent<CanvasGroup>();
            if (instructionGroup == null)
            {
                instructionGroup = instructionText.gameObject.AddComponent<CanvasGroup>();
            }
            instructionGroup.alpha = 0f;
        }
        
        UpdateChildNameDisplay();
    }
    
    private void Start()
    {
        // Start animations
        PlayEntranceAnimation();
    }
    
    private void OnDestroy()
    {
        // Clean up
        DOTween.Kill(gameObject);
        
        if (startPlayingButton != null)
        {
            startPlayingButton.onClick.RemoveListener(OnStartPlayingClicked);
        }
    }
    
    private void UpdateChildNameDisplay()
    {
        string childName = "[Name]";
        
        if (flowManager != null)
        {
            childName = flowManager.GetChildName();
        }
        else
        {
            childName = PlayerPrefs.GetString("ChildName", "[Name]");
        }
        
        // Update instruction text with child name
        if (instructionText != null)
        {
            // Look for LocalizedText component to handle localization
            LocalizedText localizedInstruction = instructionText.GetComponent<LocalizedText>();
            if (localizedInstruction != null)
            {
                // The LocalizedText component will handle this
                // Assuming your localization system supports parameter replacement
            }
            else
            {
                // Direct text replacement - adjust for RTL if needed
                string originalText = instructionText.text;
                instructionText.text = originalText.Replace("[Name]", childName);
            }
        }
    }
    
    private void PlayEntranceAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        
        // First bring in the mascot
        if (mascotCharacter != null)
        {
            sequence.Append(mascotCharacter.transform.DOScale(1f, mascotBounceDuration)
                .SetEase(mascotBounceEase));
        }
        
        // Then show the star
        if (starIcon != null)
        {
            sequence.Append(starIcon.transform.DOScale(1f, starBounceDuration)
                .SetEase(starBounceEase));
                
            // Add floating animation for the star
            sequence.AppendCallback(() => {
                // Create floating loop animation
                Sequence starFloatSequence = DOTween.Sequence();
                starFloatSequence.Append(starIcon.transform.DOLocalMoveY(
                    starIcon.transform.localPosition.y + starFloatDistance, 
                    starFloatDuration).SetEase(Ease.InOutSine));
                    
                starFloatSequence.Append(starIcon.transform.DOLocalMoveY(
                    starIcon.transform.localPosition.y, 
                    starFloatDuration).SetEase(Ease.InOutSine));
                    
                starFloatSequence.SetLoops(-1);
                starFloatSequence.Play();
            });
        }
        
        // Then fade in the congrats text
        if (congratsText != null)
        {
            CanvasGroup congratsGroup = congratsText.GetComponent<CanvasGroup>();
            if (congratsGroup != null)
            {
                sequence.Append(congratsGroup.DOFade(1f, textFadeInDuration));
            }
        }
        
        // Finally fade in the instruction text
        if (instructionText != null)
        {
            CanvasGroup instructionGroup = instructionText.GetComponent<CanvasGroup>();
            if (instructionGroup != null)
            {
                sequence.Append(instructionGroup.DOFade(1f, textFadeInDuration));
            }
        }
        
        // Play confetti effect if available
        if (confettiParticleSystem != null)
        {
            sequence.AppendCallback(() => {
                confettiParticleSystem.SetActive(true);
                StartCoroutine(DeactivateConfettiAfterDelay());
            });
        }
        
        sequence.Play();
    }
    
    private IEnumerator DeactivateConfettiAfterDelay()
    {
        yield return new WaitForSeconds(confettiDuration);
        
        if (confettiParticleSystem != null)
        {
            confettiParticleSystem.SetActive(false);
        }
    }
    
    private void OnStartPlayingClicked()
    {
        // Start the game / learning experience
        if (flowManager != null)
        {
            flowManager.StartChildExperience();
        }
        else
        {
            // Fallback if flow manager not found
            Debug.Log("Starting child experience");
            // Load the appropriate scene or show the appropriate UI
        }
    }
}