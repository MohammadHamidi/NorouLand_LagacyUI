using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RTLTMPro;

[RequireComponent(typeof(RectTransform))]
public class DynamicStarField : MonoBehaviour
{
    [Header("Star Field Settings")]
    public Sprite starSprite;
    public int starCount = 100;
    // Star sizes in pixels (min, max)
    public Vector2 starSizeRange = new Vector2(2f, 20f); // Wider range with smaller minimum
    // Control how many large stars appear (higher = fewer large stars)
    [Range(1f, 5f)]
    public float sizeBias = 2.5f;

    [Header("Scroll Settings")]
    public bool enableScroll = true;
    // Duration of the scroll action (in seconds) called from outside
    public float scrollDuration = 0.5f;

    [Header("Flicker Settings")]
    public bool enableFlicker = true;
    // DOTween fade duration for out/in animation
    public float fadeDuration = 0.5f;
    // Random delay range between flicker events
    public Vector2 flickerDelayRange = new Vector2(0.1f, 0.5f);
    // Fraction of stars to flicker each time (0 to 1)
    public float flickerFraction = 0.1f;

    private RectTransform rectTransform;
    private float canvasWidth;
    private float canvasHeight;
    private Vector2 targetPosition;
    private bool isScrolling = false;

    // Struct for individual star data
    private struct StarData
    {
        public Image image;
        public float baseAlpha;
    }
    private List<StarData> stars = new List<StarData>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Retrieve parent Canvas dimensions (taking Canvas Scaler into account)
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            // Multiply by canvas.scaleFactor so the star field scales with the canvas
            float scaleFactor = canvas.scaleFactor;
            canvasWidth = canvasRect.rect.width * scaleFactor;
            canvasHeight = canvasRect.rect.height * scaleFactor;
        }
        else
        {
            canvasWidth = Screen.width;
            canvasHeight = Screen.height;
        }

        // Set star field size: width equals canvasWidth, height equals 2 * canvasHeight
        rectTransform.sizeDelta = new Vector2(canvasWidth, canvasHeight * 2);
        rectTransform.anchoredPosition = Vector2.zero;
        // Target: move upward by canvasHeight so the lower half becomes visible
        targetPosition = new Vector2(rectTransform.anchoredPosition.x, canvasHeight);

        // Create stars - distribution similar to image
        CreateStarField();

        // Start flicker coroutine if enabled
        if (enableFlicker)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    void CreateStarField()
    {
        // Clear any existing stars
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        stars.Clear();

        // Create a more natural distribution with varying density
        for (int i = 0; i < starCount; i++)
        {
            GameObject starObj = new GameObject("Star" + i, typeof(RectTransform));
            starObj.transform.SetParent(transform, false);
            Image img = starObj.AddComponent<Image>();
            img.sprite = starSprite;

            // Use canvasHeight * 2 as the vertical range (considering the updated canvasHeight)
            float heightValue = canvasHeight * 2;

            // Pure random position for a more natural look
            float posX = Random.Range(-canvasWidth / 2f, canvasWidth / 2f);
            float posY = Random.Range(-heightValue, heightValue);

            RectTransform starRect = starObj.GetComponent<RectTransform>();
            
            // Use a biased random distribution to create mostly small stars with a few larger ones
            float sizeFactor = Mathf.Pow(Random.value, sizeBias);
            float starSize = Mathf.Lerp(starSizeRange.x, starSizeRange.y, sizeFactor);

            starRect.sizeDelta = new Vector2(starSize, starSize);
            starRect.anchorMin = new Vector2(0.5f, 0.5f);
            starRect.anchorMax = new Vector2(0.5f, 0.5f);
            starRect.pivot = new Vector2(0.5f, 0.5f);
            starRect.anchoredPosition = new Vector2(posX, posY);

            // Brightness based on size (larger stars are brighter)
            float alphaBase = Mathf.Lerp(0.3f, 1f, sizeFactor);
            float alphaVariation = (1 - sizeFactor) * 0.5f; // More variation for smaller stars

            StarData data = new StarData();
            data.image = img;
            data.baseAlpha = alphaBase + Random.Range(-alphaVariation, alphaVariation);
            data.baseAlpha = Mathf.Clamp01(data.baseAlpha); // Ensure alpha is between 0 and 1

            Color c = img.color;
            c.a = data.baseAlpha;
            img.color = c;

            stars.Add(data);
        }
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Wait for a random delay between flicker events
            float delay = Random.Range(flickerDelayRange.x, flickerDelayRange.y);
            yield return new WaitForSeconds(delay);

            // Determine how many stars to flicker this cycle
            int flickerCount = Mathf.Max(1, Mathf.FloorToInt(stars.Count * flickerFraction));

            // Randomly select flickerCount stars
            for (int i = 0; i < flickerCount; i++)
            {
                int randomIndex = Random.Range(0, stars.Count);
                StarData data = stars[randomIndex];
                Image starImage = data.image;

                // Only flicker stars that are visible (alpha > a small threshold)
                if (starImage.color.a > 0.1f)
                {
                    // Create a DOTween sequence to fade out then fade in
                    DOTween.Sequence()
                        .Append(starImage.DOFade(0f, fadeDuration))
                        .Append(starImage.DOFade(data.baseAlpha, fadeDuration));
                }
            }
        }
    }

    // Call this externally to trigger a fast scroll
    public void TriggerScroll()
    {
        if (enableScroll && !isScrolling)
        {
            StartCoroutine(ScrollRoutine());
        }
    }

    IEnumerator ScrollRoutine()
    {
        isScrolling = true;
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsed / scrollDuration);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
        isScrolling = false;
    }
}
