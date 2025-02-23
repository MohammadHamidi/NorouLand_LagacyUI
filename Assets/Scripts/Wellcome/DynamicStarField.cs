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
    public Vector2 starSizeRange = new Vector2(2f, 20f);
    [Range(1f, 5f)]
    public float sizeBias = 2.5f;
    [Tooltip("Minimum distance between stars in pixels")]
    public float minStarDistance = 50f;
    [Tooltip("Maximum attempts to place a star before skipping")]
    public int maxPlacementAttempts = 30;

    [Header("Field Size Settings")]
    [Tooltip("Multiplier for width relative to screen width (1 = screen width)")]
    public float widthRatio = 1f;
    [Tooltip("Base multiplier for height relative to screen height (2 = twice screen height)")]
    public float heightRatio = 2f;
    [Tooltip("Additional multiplier for scrollable height (1 = normal height, 2 = double height, etc)")]
    [Range(1f, 10f)]
    public float scrollHeightMultiplier = 1f;
    [Tooltip("Additional padding to increase star spread (0 = no extra spread)")]
    [Range(0f, 5f)]
    public float starSpreadMultiplier = 1f;

    [Header("Scroll Settings")]
    public bool enableScroll = true;
    public float scrollDuration = 0.5f;
    [Tooltip("How much of the total height to scroll each time (0-1)")]
    [Range(0f, 1f)]
    public float scrollStepRatio = 0.5f;

    [Header("Flicker Settings")]
    public bool enableFlicker = true;
    public float fadeDuration = 0.5f;
    public Vector2 flickerDelayRange = new Vector2(0.1f, 0.5f);
    public float flickerFraction = 0.1f;

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private float lastScreenWidth;
    private float lastScreenHeight;
    private Vector2 targetPosition;
    private bool isScrolling = false;
    private Vector2 lastFieldSize;
    private float totalScrollableHeight;
    private float currentScrollPosition = 0f;

    private class StarData
    {
        public GameObject gameObject;
        public RectTransform rectTransform;
        public Image image;
        public float baseAlpha;
        public float relativeXPosition;
        public float relativeYPosition;
        public float size;
        public Vector2 position; // Actual position for distance checking
    }
    private List<StarData> stars = new List<StarData>();

    void OnValidate()
    {
        widthRatio = Mathf.Max(0.1f, widthRatio);
        heightRatio = Mathf.Max(0.1f, heightRatio);
        scrollHeightMultiplier = Mathf.Max(1f, scrollHeightMultiplier);
        minStarDistance = Mathf.Max(0f, minStarDistance);
        
        if (Application.isPlaying && rectTransform != null)
        {
            UpdateStarFieldSize();
        }
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        
        InitializeStarField();
        
        if (enableFlicker)
        {
            StartCoroutine(FlickerRoutine());
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void Update()
    {
        if (Mathf.Abs(Screen.width - lastScreenWidth) > 0.01f || 
            Mathf.Abs(Screen.height - lastScreenHeight) > 0.01f)
        {
            UpdateStarFieldSize();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void InitializeStarField()
    {
        UpdateCanvasDimensions(out float canvasWidth, out float canvasHeight);
    
        // Calculate total height including scroll multiplier
        totalScrollableHeight = canvasHeight * heightRatio * scrollHeightMultiplier;
    
        // Set initial star field size with ratios, accounting for canvas scale
        Vector2 fieldSize = new Vector2(canvasWidth * widthRatio, totalScrollableHeight);
        rectTransform.sizeDelta = fieldSize;
        lastFieldSize = fieldSize;
    
        // Initialize at bottom position
        currentScrollPosition = 0f;
        rectTransform.anchoredPosition = new Vector2(0, currentScrollPosition);
    
        // Calculate first scroll target
        float scrollStep = totalScrollableHeight * scrollStepRatio;
        targetPosition = new Vector2(0, currentScrollPosition + scrollStep);

        CreateStars();
    }
    void UpdateCanvasDimensions(out float width, out float height)
    {
        if (parentCanvas != null)
        {
            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
            float scaleFactor = parentCanvas.scaleFactor;
        
            // Get the actual screen dimensions
            width = Screen.width / scaleFactor;
            height = Screen.height / scaleFactor;

            // Apply these to get the correct canvas space dimensions
            width *= scaleFactor;
            height *= scaleFactor;
        }
        else
        {
            width = Screen.width;
            height = Screen.height;
        }
    }
    bool IsPositionValid(Vector2 newPos, float starSize)
    {
        float minDistance = minStarDistance + (starSize / 2f); // Adjust distance based on star size
        
        foreach (var star in stars)
        {
            float distance = Vector2.Distance(newPos, star.position);
            if (distance < minDistance + (star.size / 2f))
            {
                return false;
            }
        }
        return true;
    }

    Vector2 GenerateRandomPosition(float spreadWidth, float spreadHeight)
    {
        float posX = Mathf.Lerp(-spreadWidth / 2f, spreadWidth / 2f, Random.value);
        float posY = Mathf.Lerp(-spreadHeight / 2f, spreadHeight / 2f, Random.value);
        return new Vector2(posX, posY);
    }
   void CreateStars()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        stars.Clear();

        float totalHeight = rectTransform.sizeDelta.y;
        float totalWidth = rectTransform.sizeDelta.x;
        float spreadWidth = totalWidth * starSpreadMultiplier;
        float spreadHeight = totalHeight * starSpreadMultiplier;

        int actualStarCount = 0;
        int attemptedStars = 0;

        while (actualStarCount < starCount && attemptedStars < starCount * 2)
        {
            attemptedStars++;
            
            // Generate star size first
            float sizeFactor = Mathf.Pow(Random.value, sizeBias);
            float starSize = Mathf.Lerp(starSizeRange.x, starSizeRange.y, sizeFactor);

            // Try to find a valid position
            Vector2 position = Vector2.zero;
            bool validPositionFound = false;
            int attempts = 0;

            while (!validPositionFound && attempts < maxPlacementAttempts)
            {
                position = GenerateRandomPosition(spreadWidth, spreadHeight);
                validPositionFound = IsPositionValid(position, starSize);
                attempts++;
            }

            if (!validPositionFound)
            {
                continue; // Skip this star if we couldn't find a valid position
            }

            GameObject starObj = new GameObject("Star" + actualStarCount, typeof(RectTransform));
            starObj.transform.SetParent(transform, false);
            
            StarData data = new StarData();
            data.gameObject = starObj;
            data.rectTransform = starObj.GetComponent<RectTransform>();
            data.image = starObj.AddComponent<Image>();
            data.image.sprite = starSprite;
            data.size = starSize;
            data.position = position;

            // Calculate relative positions for future updates
            data.relativeXPosition = (position.x + spreadWidth / 2f) / spreadWidth;
            data.relativeYPosition = (position.y + spreadHeight / 2f) / spreadHeight;

            data.rectTransform.sizeDelta = new Vector2(data.size, data.size);
            data.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            data.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            data.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            data.rectTransform.anchoredPosition = position;

            float alphaBase = Mathf.Lerp(0.3f, 1f, sizeFactor);
            float alphaVariation = (1 - sizeFactor) * 0.5f;
            data.baseAlpha = Mathf.Clamp01(alphaBase + Random.Range(-alphaVariation, alphaVariation));
            
            Color c = data.image.color;
            c.a = data.baseAlpha;
            data.image.color = c;

            stars.Add(data);
            actualStarCount++;
        }

        if (actualStarCount < starCount)
        {
            Debug.LogWarning($"Could only place {actualStarCount} stars out of {starCount} requested due to minimum distance constraints.");
        }
    }

    void UpdateStarFieldSize()
    {
        UpdateCanvasDimensions(out float canvasWidth, out float canvasHeight);
    
        // Update total scrollable height
        totalScrollableHeight = canvasHeight * heightRatio * scrollHeightMultiplier;
    
        // Calculate new field size based on ratios, accounting for canvas scale
        Vector2 newSize = new Vector2(canvasWidth * widthRatio, totalScrollableHeight);
        rectTransform.sizeDelta = newSize;
    
        // Maintain relative scroll position
        float scrollProgress = currentScrollPosition / lastFieldSize.y;
        currentScrollPosition = scrollProgress * newSize.y;
        rectTransform.anchoredPosition = new Vector2(0, currentScrollPosition);
    
        // Update target position maintaining relative progress
        float scrollStep = totalScrollableHeight * scrollStepRatio;
        targetPosition = new Vector2(0, currentScrollPosition + scrollStep);

        float spreadWidth = newSize.x * starSpreadMultiplier;
        float spreadHeight = newSize.y * starSpreadMultiplier;

        // Update star positions with correct scaling
        foreach (StarData data in stars)
        {
            float posX = Mathf.Lerp(-spreadWidth / 2f, spreadWidth / 2f, data.relativeXPosition);
            float posY = Mathf.Lerp(-spreadHeight / 2f, spreadHeight / 2f, data.relativeYPosition);
            data.position = new Vector2(posX, posY);
            data.rectTransform.anchoredPosition = data.position;
        }

        lastFieldSize = newSize;
    }

    


    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float delay = Random.Range(flickerDelayRange.x, flickerDelayRange.y);
            yield return new WaitForSeconds(delay);

            int flickerCount = Mathf.Max(1, Mathf.FloorToInt(stars.Count * flickerFraction));

            for (int i = 0; i < flickerCount; i++)
            {
                int randomIndex = Random.Range(0, stars.Count);
                StarData data = stars[randomIndex];

                if (data.image.color.a > 0.1f)
                {
                    DOTween.Sequence()
                        .Append(data.image.DOFade(0f, fadeDuration))
                        .Append(data.image.DOFade(data.baseAlpha, fadeDuration));
                }
            }
        }
    }

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

        // Calculate the scroll step
        float scrollStep = totalScrollableHeight * scrollStepRatio;

        // Create a duplicate position for seamless transition
        if (currentScrollPosition >= totalScrollableHeight - scrollStep)
        {
            // Create a copy of the star field at the top
            Vector2 duplicatePos = new Vector2(0, -totalScrollableHeight);
            rectTransform.anchoredPosition = duplicatePos;
            startPos = duplicatePos;
            currentScrollPosition = 0;
        }

        // Calculate next target
        targetPosition = new Vector2(0, startPos.y + scrollStep);

        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsed / scrollDuration);
            yield return null;
        }

        // Finalize position
        rectTransform.anchoredPosition = targetPosition;
        currentScrollPosition = targetPosition.y;
    
        isScrolling = false;
    }
}