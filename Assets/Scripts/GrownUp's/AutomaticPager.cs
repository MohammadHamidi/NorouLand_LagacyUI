using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class AutomaticPager : MonoBehaviour
{
    [Header("Page Settings")]
    [Tooltip("Parent container holding the page GameObjects (each page is a child).")]
    public RectTransform pageContainer;
    [Tooltip("Time delay (in seconds) before transitioning to the next page.")]
    public float pageSwitchDelay = 3f;
    [Tooltip("Duration (in seconds) of the page transition animation.")]
    public float transitionDuration = 0.5f;

    [Header("Indicator Settings")]
    [Tooltip("Parent container holding the indicator images (one per page).")]
    public RectTransform indicatorContainer;
    [Tooltip("Sprite for an inactive page indicator (dot).")]
    public Sprite inactiveIndicatorSprite;
    [Tooltip("Sprite for the active page indicator (line).")]
    public Sprite activeIndicatorSprite;

    // List of pages (populated from the children of pageContainer)
    private List<RectTransform> pages = new List<RectTransform>();
    // List of indicator images (populated from the children of indicatorContainer)
    private List<Image> indicators = new List<Image>();

    // Current active page index
    private int currentPageIndex = 0;
    // The width of the pageContainer (used for calculating off-screen positions)
    private float containerWidth;

    private Coroutine pagingCoroutine;

    private void Start()
    {
        // Validate container and indicator container.
        if (pageContainer == null)
        {
            Debug.LogError("AutomaticPager: Page Container is not assigned.");
            return;
        }
        if (indicatorContainer == null)
        {
            Debug.LogError("AutomaticPager: Indicator Container is not assigned.");
            return;
        }

        // Cache all pages (assume direct children).
        pages.Clear();
        for (int i = 0; i < pageContainer.childCount; i++)
        {
            RectTransform page = pageContainer.GetChild(i) as RectTransform;
            if (page != null)
            {
                pages.Add(page);
                // Ensure pages start at (0,0) except the first
                if (i != 0)
                {
                    page.anchoredPosition = new Vector2(pageContainer.rect.width, 0f);
                    page.gameObject.SetActive(false);
                }
            }
        }

        // Cache all indicator images.
        indicators.Clear();
        for (int i = 0; i < indicatorContainer.childCount; i++)
        {
            Image img = indicatorContainer.GetChild(i).GetComponent<Image>();
            if (img != null)
            {
                indicators.Add(img);
            }
        }

        // Update containerWidth.
        containerWidth = pageContainer.rect.width;

        // Set the first page active.
        if (pages.Count > 0)
            pages[0].gameObject.SetActive(true);

        // Initialize indicator states.
        UpdateIndicators();

        // Start the paging coroutine.
        if (pages.Count > 1)
            pagingCoroutine = StartCoroutine(PagingRoutine());
    }

    /// <summary>
    /// Coroutine that waits a delay and then transitions to the next page.
    /// </summary>
    private IEnumerator PagingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pageSwitchDelay);
            TransitionToNextPage();
        }
    }

    /// <summary>
    /// Transitions from the current page to the next using DOTween.
    /// </summary>
    private void TransitionToNextPage()
    {
        if (pages.Count == 0)
            return;

        // Determine next page index (wrap around)
        int nextPageIndex = (currentPageIndex + 1) % pages.Count;

        RectTransform currentPage = pages[currentPageIndex];
        RectTransform nextPage = pages[nextPageIndex];

        // Activate the next page and set its starting position (off-screen right)
        nextPage.gameObject.SetActive(true);
        nextPage.anchoredPosition = new Vector2(containerWidth, 0f);
        nextPage.GetComponent<CanvasGroup>()?.DOFade(0f, 0f);  // Optional: if using CanvasGroup for fade

        // Ensure current page has a CanvasGroup for fading.
        CanvasGroup currentCG = currentPage.GetComponent<CanvasGroup>();
        if (currentCG == null)
        {
            currentCG = currentPage.gameObject.AddComponent<CanvasGroup>();
            currentCG.alpha = 1f;
        }
        // Ensure next page has a CanvasGroup.
        CanvasGroup nextCG = nextPage.GetComponent<CanvasGroup>();
        if (nextCG == null)
        {
            nextCG = nextPage.gameObject.AddComponent<CanvasGroup>();
            nextCG.alpha = 0f;
        }

        // Create a DOTween sequence for transition.
        Sequence seq = DOTween.Sequence();

        // Animate current page: move left (off-screen) and fade out.
        seq.Join(currentPage.DOAnchorPos(new Vector2(-containerWidth, 0f), transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(currentCG.DOFade(0f, transitionDuration));

        // Animate next page: move from right to center and fade in.
        seq.Join(nextPage.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad));
        seq.Join(nextCG.DOFade(1f, transitionDuration));

        // When complete, update indices and reset previous page.
        seq.OnComplete(() =>
        {
            currentPage.gameObject.SetActive(false);
            currentCG.alpha = 1f; // reset for future use

            currentPageIndex = nextPageIndex;
            UpdateIndicators();
        });
    }

    /// <summary>
    /// Updates the indicator images: the active page's indicator uses activeIndicatorSprite,
    /// while others use inactiveIndicatorSprite.
    /// </summary>
    private void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            if (i == currentPageIndex)
                indicators[i].sprite = activeIndicatorSprite;
            else
                indicators[i].sprite = inactiveIndicatorSprite;
        }
    }

    private void OnDestroy()
    {
        if (pagingCoroutine != null)
            StopCoroutine(pagingCoroutine);
    }
}