using System.Collections;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float bounceDuration = 1.5f;
    [SerializeField] private float bounceHeight = 10f;
    [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Idle Animation")]
    [SerializeField] private float blinkInterval = 3f;
    [SerializeField] private GameObject eyesOpen;
    [SerializeField] private GameObject eyesClosed;
    
    private Vector3 originalPosition;
    private bool isAnimating = false;
    
    private void Start()
    {
        originalPosition = transform.localPosition;
        
        // Start idle animations
        StartCoroutine(BlinkRoutine());
        StartCoroutine(BounceRoutine());
    }
    
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // Random time between blinks
            float waitTime = Random.Range(blinkInterval * 0.7f, blinkInterval * 1.3f);
            yield return new WaitForSeconds(waitTime);
            
            // Blink
            if (eyesOpen != null && eyesClosed != null)
            {
                eyesOpen.SetActive(false);
                eyesClosed.SetActive(true);
                
                // Blink duration
                yield return new WaitForSeconds(0.15f);
                
                eyesOpen.SetActive(true);
                eyesClosed.SetActive(false);
            }
        }
    }
    
    private IEnumerator BounceRoutine()
    {
        while (true)
        {
            // Wait before starting bounce
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            
            float elapsedTime = 0f;
            
            while (elapsedTime < bounceDuration)
            {
                float normalizedTime = elapsedTime / bounceDuration;
                float curveValue = bounceCurve.Evaluate(normalizedTime);
                float yOffset = bounceHeight * curveValue;
                
                transform.localPosition = originalPosition + new Vector3(0f, yOffset, 0f);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Reset position
            transform.localPosition = originalPosition;
        }
    }
    
    // Call this from buttons to trigger a happy reaction
    public void PlayHappyReaction()
    {
        if (!isAnimating)
        {
            StartCoroutine(HappyReactionRoutine());
        }
    }
    
    private IEnumerator HappyReactionRoutine()
    {
        isAnimating = true;
        
        // Play a quick bounce animation
        float duration = 0.5f;
        float height = bounceHeight * 1.5f;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            float curveValue = Mathf.Sin(normalizedTime * Mathf.PI);
            float yOffset = height * curveValue;
            
            transform.localPosition = originalPosition + new Vector3(0f, yOffset, 0f);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
        isAnimating = false;
    }
}