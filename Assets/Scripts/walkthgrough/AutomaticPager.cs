using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AutomaticPager : MonoBehaviour
{
    [Header("Page Settings")]
    [Tooltip("List of RectTransforms to use as pages")]
    public List<RectTransform> pageRectTransforms = new List<RectTransform>();
    [Tooltip("Time delay (in seconds) before transitioning to the next page.")]
    public float pageSwitchDelay = 3f;
    [Tooltip("Duration (in seconds) of the page transition animation.")]
    public float transitionDuration = 0.5f;
    [Tooltip("Size of the dot when inactive.")]
    public Vector2 inactiveIndicatorSize = new Vector2(30f, 30f);

    [Tooltip("Size of the line (active) indicator.")]
    public Vector2 activeIndicatorSize = new Vector2(60f, 30f);

    [Header("Indicator Settings")]
    [Tooltip("Parent container holding the indicator images (one per page).")]
    public RectTransform indicatorContainer;
    [Tooltip("Sprite for an inactive page indicator (dot).")]
    public Sprite inactiveIndicatorSprite;
    [Tooltip("Sprite for the active page indicator (line).")]
    public Sprite activeIndicatorSprite;
    [Tooltip("Size of each indicator button")]
    public Vector2 indicatorSize = new Vector2(30f, 30f);
    [Tooltip("Spacing between indicators")]
    public float indicatorSpacing = 10f;

    private List<Button> indicators = new List<Button>();
    private int currentPageIndex = 0;
    private RectTransform referenceRect;
    private float containerWidth;
    private Coroutine pagingCoroutine;
    private Vector2[] originalPositions;
    private bool isTransitioning = false;

    private void Awake()
    {
        referenceRect = GetComponent<RectTransform>();
        if (referenceRect == null && pageRectTransforms.Count > 0)
        {
            referenceRect = pageRectTransforms[0];
        }
    }

    private void Start()
    {
        if (pageRectTransforms.Count == 0)
        {
            Debug.LogError("AutomaticPager: No pages assigned.");
            return;
        }
        if (indicatorContainer == null)
        {
            Debug.LogError("AutomaticPager: Indicator Container is not assigned.");
            return;
        }

        // Remove duplicate referenceRect initialization
        containerWidth = referenceRect.rect.width;
        
        StoreOriginalPositions();
        InitializePages();
        CreateIndicators(); // This will create the indicators
        
        if (pageRectTransforms.Count > 0)
        {
            pageRectTransforms[0].gameObject.SetActive(true);
            currentPageIndex = 0;
            UpdateIndicators(); // Now safe to call after CreateIndicators
        }

        if (pageRectTransforms.Count > 1)
        {
            pagingCoroutine = StartCoroutine(PagingRoutine());
        }
    }
    private void CreateIndicators()
    {
        // Clean up old indicators
        foreach (Transform child in indicatorContainer)
            Destroy(child.gameObject);

        indicators.Clear();

        // Actually set up the layout group if needed
        SetupLayoutGroup();

        for (int i = 0; i < pageRectTransforms.Count; i++)
        {
            GameObject indicatorObj = new GameObject($"Indicator_{i}", typeof(RectTransform));
            indicatorObj.transform.SetParent(indicatorContainer, false);

            RectTransform indicatorRect = indicatorObj.GetComponent<RectTransform>();
            // If you want a manual size for the dot, do:
            // indicatorRect.sizeDelta = indicatorSize;
            // (Then remove if you want the sprite to define its own size)

            // Add Button
            Button button = indicatorObj.AddComponent<Button>();

            // Add Image
            Image image = indicatorObj.AddComponent<Image>();
            image.sprite = inactiveIndicatorSprite;
            image.preserveAspect = true;    // If you want to keep the aspect ratio
            // image.SetNativeSize();      // If you want the sprite to define its actual size

            // Keep reference
            indicators.Add(button);

            int pageIndex = i; 
            button.onClick.AddListener(() => TransitionToPage(pageIndex));
        }
    }

    // For example, in Start() or Awake(), after you validate indicatorContainer != null
    private void SetupLayoutGroup()
    {
        // If already added, skip or re-check
        HorizontalLayoutGroup hlg = indicatorContainer.GetComponent<HorizontalLayoutGroup>();
        if (hlg == null)
            hlg = indicatorContainer.gameObject.AddComponent<HorizontalLayoutGroup>();

        // Set spacing, alignment, etc.
        hlg.spacing = indicatorSpacing;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;   // Let each indicator keep its own width
        hlg.childForceExpandHeight = false;  // Let each indicator keep its own height
        hlg.childControlWidth = false;       // We'll set the size in code or let sprite define it
        hlg.childControlHeight = false;      // same for height

        // For automatically sizing this container to fit all children:
        ContentSizeFitter fitter = indicatorContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = indicatorContainer.gameObject.AddComponent<ContentSizeFitter>();

        // Make the container's width automatically match the sum of children’s widths
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit   = ContentSizeFitter.FitMode.MinSize; // or PreferredSize if you like
    }

    public void TransitionToPage(int targetIndex)
    {
        if (isTransitioning || targetIndex == currentPageIndex || 
            targetIndex < 0 || targetIndex >= pageRectTransforms.Count)
            return;

        // Stop auto-paging coroutine
        if (pagingCoroutine != null)
        {
            StopCoroutine(pagingCoroutine);
            pagingCoroutine = null;
        }

        isTransitioning = true;

        RectTransform currentPage = pageRectTransforms[currentPageIndex];
        RectTransform targetPage = pageRectTransforms[targetIndex];

        // Keep original Y positions
        float currentY = currentPage.anchoredPosition.y;
        float targetY = targetPage.anchoredPosition.y;

        // Determine direction of transition
        float targetStartX = targetIndex > currentPageIndex ? containerWidth : -containerWidth;
        float currentEndX = targetIndex > currentPageIndex ? -containerWidth : containerWidth;

        targetPage.gameObject.SetActive(true);
        targetPage.anchoredPosition = new Vector2(targetStartX, targetY);

        CanvasGroup currentCG = EnsureCanvasGroup(currentPage);
        CanvasGroup targetCG = EnsureCanvasGroup(targetPage);
        targetCG.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // Animate current page
        seq.Join(currentPage.DOAnchorPosX(currentEndX, transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(currentCG.DOFade(0f, transitionDuration));

        // Animate target page
        seq.Join(targetPage.DOAnchorPosX(originalPositions[targetIndex].x, transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(targetCG.DOFade(1f, transitionDuration));

        seq.OnComplete(() =>
        {
            currentPage.gameObject.SetActive(false);
            currentCG.alpha = 1f;
            currentPage.anchoredPosition = new Vector2(currentPage.anchoredPosition.x, currentY);
            
            currentPageIndex = targetIndex;
            UpdateIndicators();
            isTransitioning = false;

            // Restart auto-paging
            if (pageRectTransforms.Count > 1)
            {
                pagingCoroutine = StartCoroutine(PagingRoutine());
            }
        });
    }

    private void StoreOriginalPositions()
    {
        originalPositions = new Vector2[pageRectTransforms.Count];
        for (int i = 0; i < pageRectTransforms.Count; i++)
        {
            originalPositions[i] = pageRectTransforms[i].anchoredPosition;
        }
    }

    private void InitializePages()
    {
        for (int i = 0; i < pageRectTransforms.Count; i++)
        {
            var page = pageRectTransforms[i];
            if (page != null)
            {
                if (i != 0)
                {
                    // Only modify X position, keep original Y
                    float originalY = page.anchoredPosition.y;
                    page.anchoredPosition = new Vector2(referenceRect.rect.width, originalY);
                    page.gameObject.SetActive(false);
                }
            }
        }
    }

   

    private IEnumerator PagingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pageSwitchDelay);
            TransitionToNextPage();
        }
    }

    private void TransitionToNextPage()
    {
        if (pageRectTransforms.Count == 0)
            return;

        int nextPageIndex = (currentPageIndex + 1) % pageRectTransforms.Count;

        RectTransform currentPage = pageRectTransforms[currentPageIndex];
        RectTransform nextPage = pageRectTransforms[nextPageIndex];

        // Keep original Y positions
        float currentY = currentPage.anchoredPosition.y;
        float nextY = nextPage.anchoredPosition.y;

        nextPage.gameObject.SetActive(true);
        nextPage.anchoredPosition = new Vector2(containerWidth, nextY);

        CanvasGroup currentCG = EnsureCanvasGroup(currentPage);
        CanvasGroup nextCG = EnsureCanvasGroup(nextPage);
        nextCG.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // Animate only X position for current page
        seq.Join(currentPage.DOAnchorPosX(-containerWidth, transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(currentCG.DOFade(0f, transitionDuration));

        // Animate only X position for next page
        seq.Join(nextPage.DOAnchorPosX(originalPositions[nextPageIndex].x, transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(nextCG.DOFade(1f, transitionDuration));

        seq.OnComplete(() =>
        {
            currentPage.gameObject.SetActive(false);
            currentCG.alpha = 1f;
            
            // Restore original Y position
            currentPage.anchoredPosition = new Vector2(currentPage.anchoredPosition.x, currentY);
            
            currentPageIndex = nextPageIndex;
            UpdateIndicators();
        });
    }

    private CanvasGroup EnsureCanvasGroup(RectTransform rt)
    {
        CanvasGroup cg = rt.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = rt.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 1f;
        }
        return cg;
    }

    private void UpdateIndicators()
    {
        if (indicators == null || indicators.Count == 0 ||
            currentPageIndex < 0 || currentPageIndex >= indicators.Count)
        {
            return;
        }

        for (int i = 0; i < indicators.Count; i++)
        {
            Image indicatorImage = indicators[i].GetComponent<Image>();
            RectTransform indicatorRect = indicators[i].GetComponent<RectTransform>();
            if (indicatorImage == null || indicatorRect == null) continue;
        
            bool isActive = (i == currentPageIndex);
            indicatorImage.sprite = isActive ? activeIndicatorSprite : inactiveIndicatorSprite;
        
            // Override size for active vs. inactive
            indicatorRect.sizeDelta = isActive ? activeIndicatorSize : inactiveIndicatorSize;
        }
    }

    private void OnDestroy()
    {
        if (pagingCoroutine != null)
        {
            StopCoroutine(pagingCoroutine);
        }

        // Restore all original positions
        for (int i = 0; i < pageRectTransforms.Count; i++)
        {
            if (pageRectTransforms[i] != null)
            {
                pageRectTransforms[i].anchoredPosition = originalPositions[i];
                pageRectTransforms[i].gameObject.SetActive(true);
            }
        }
    }

    public void AddPage(RectTransform newPage)
    {
        if (newPage != null)
        {
            System.Array.Resize(ref originalPositions, originalPositions.Length + 1);
            originalPositions[originalPositions.Length - 1] = newPage.anchoredPosition;

            pageRectTransforms.Add(newPage);

            float originalY = newPage.anchoredPosition.y;
            newPage.anchoredPosition = new Vector2(referenceRect.rect.width, originalY);
            newPage.gameObject.SetActive(false);

            // Recreate indicators to include new page
            CreateIndicators();
            UpdateIndicators();
        }
    }


    public void RemovePage(RectTransform pageToRemove)
    {
        int index = pageRectTransforms.IndexOf(pageToRemove);
        if (index != -1)
        {
            pageToRemove.anchoredPosition = originalPositions[index];
            pageToRemove.gameObject.SetActive(true);

            pageRectTransforms.RemoveAt(index);
            Vector2[] newPositions = new Vector2[originalPositions.Length - 1];
            System.Array.Copy(originalPositions, 0, newPositions, 0, index);
            System.Array.Copy(originalPositions, index + 1, newPositions, index, originalPositions.Length - index - 1);
            originalPositions = newPositions;

            if (currentPageIndex >= pageRectTransforms.Count)
            {
                currentPageIndex = 0;
            }

            // Recreate indicators to remove old page
            CreateIndicators();
            UpdateIndicators();
        }
    }
}