using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
[System.Serializable]
public class ReviewData
{
    [FormerlySerializedAs("reviewerName")] public string ReviewerName;
    [FormerlySerializedAs("reviewContent")] public string ReviewText;
    public Sprite reviewerAvatar;
    [FormerlySerializedAs("rating")] public int Rating; // Out of 5 stars
}
public class ReviewCardsContainer : MonoBehaviour
{
    [Header("UI Configuration")]
    [SerializeField] private Transform reviewsContainer;
    [SerializeField] private ReviewCardItem reviewCardPrefab;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform paginationDotsContainer;
    [SerializeField] private GameObject paginationDotPrefab;
    
    [Header("Reviews Data")]
    [SerializeField] private List<ReviewData> reviewsData = new List<ReviewData>();
    
    
   
    [Header("Slideshow Settings")]
    [SerializeField] private bool autoSlide = true;
    [SerializeField] private float autoSlideInterval = 5f;
    [SerializeField] private float slideTransitionDuration = 0.35f;
    [SerializeField] private bool reverseAnimation = false;
    [SerializeField] private int visibleCardsCount = 1; // Number of cards visible at once
    [SerializeField] private float cardSpacing = 20f; // Space between cards
    private List<ReviewCardItem> reviewCards = new List<ReviewCardItem>();
    private List<GameObject> paginationDots = new List<GameObject>();
    private int currentIndex = 0;
    private bool isTransitioning = false;
    private Coroutine autoSlideCoroutine;
    
    private void Start()
    {

        List<ReviewData> Mockreview = new List<ReviewData>()
        {
            new ReviewData
            {
                ReviewerName = "Mahboube",
                Rating = 5,
                ReviewText = "The only mobile game that's actually useful and educational."
            },
            new ReviewData
            {
                ReviewerName = "Liam",
                Rating = 5,
                ReviewText = "The best choice for kids. Keeps them entertained and helps them learn something new."
            },
            new ReviewData
            {
                ReviewerName = "Mary",
                Rating = 5,
                ReviewText = "My son has learned so many things. It even teaches them writing and pronunciation."
            },
            new ReviewData
            {
                ReviewerName = "محبوبه",
                Rating = 5,
                ReviewText = "نورولند تنها بازی موبایله که واقعا مفید و آموزنده است."
            },
            new ReviewData
            {
                ReviewerName = "بهراد",
                Rating = 5,
                ReviewText = "بهترین انتخاب برای بچه‌هاست. که هم سرگرم شن هم یه چیزی یاد بگیرن."
            },
            new ReviewData
            {
                ReviewerName = "مهریسا",
                Rating = 5,
                ReviewText = "این بازی مخ خواهر کوچیکمو دو روزه راه انداخت. =))) واقعا خوبه."
            },
            new ReviewData
            {
                ReviewerName = "مریم",
                Rating = 5,
                ReviewText = "پسرم کلی چیز جدید با نورولند یاد گرفته. حتی زبان و تلفظ نوشتن هم یاد میده بهشون."
            }
        };

        reviewsData = Mockreview;
        // Initialize the reviews
        InitializeReviews();
        
        // Setup navigation buttons
        if (previousButton != null)
            previousButton.onClick.AddListener(ShowPreviousReview);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNextReview);
        
        // Start auto-slide if enabled
        if (autoSlide)
            StartAutoSlide();
            
        // Show the first review
        UpdateReviewsVisibility();
        UpdatePaginationDots();
    }
    
    private void InitializeReviews()
    {
        // Clear existing cards
        ClearReviews();
        
        // Create review cards
        for (int i = 0; i < reviewsData.Count; i++)
        {
            ReviewCardItem reviewCard = Instantiate(reviewCardPrefab, reviewsContainer);
            reviewCard.Init(reviewsData[i]);
            reviewCards.Add(reviewCard);
            
            // Initial positioning (all cards off-screen to the right)
            float xPosition = i == 0 ? 0 : reviewCard.GetWidth() + cardSpacing;
            reviewCard.SetHorizontalPosition(xPosition);
            
            // Initial visibility
            reviewCard.SetAlpha(i < visibleCardsCount ? 1f : 0f);
        }
        
        // Create pagination dots
        CreatePaginationDots();
        
        // Update navigation buttons state
        UpdateNavigationButtons();
    }
    
    private void ClearReviews()
    {
        // Destroy all existing cards
        foreach (ReviewCardItem card in reviewCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        reviewCards.Clear();
        
        // Clear pagination dots
        foreach (GameObject dot in paginationDots)
        {
            if (dot != null)
                Destroy(dot);
        }
        paginationDots.Clear();
    }
    
    private void CreatePaginationDots()
    {
        if (paginationDotsContainer == null || paginationDotPrefab == null)
            return;
            
        // Calculate how many pages of reviews we have
        int pagesCount = Mathf.CeilToInt((float)reviewsData.Count / visibleCardsCount);
        
        for (int i = 0; i < pagesCount; i++)
        {
            GameObject dot = Instantiate(paginationDotPrefab, paginationDotsContainer);
            paginationDots.Add(dot);
            
            // Add click listener to each dot
            int pageIndex = i; // Need to capture the index in a local variable
            Button dotButton = dot.GetComponent<Button>();
            if (dotButton != null)
            {
                dotButton.onClick.AddListener(() => GoToPage(pageIndex));
            }
        }
    }
    
    public void ShowNextReview()
    {
        if (isTransitioning) return;
        int newIndex = (currentIndex + 1) % reviewCards.Count;
        SlideToIndex(newIndex, reverseAnimation);
    }
    
    
    public void ShowPreviousReview()
    {
        if (isTransitioning) return;
        int newIndex = (currentIndex - 1 + reviewCards.Count) % reviewCards.Count;
        SlideToIndex(newIndex, !reverseAnimation);
    }
    
    public void GoToPage(int pageIndex)
    {
        if (isTransitioning)
            return;
            
        int itemIndex = pageIndex * visibleCardsCount;
        if (itemIndex < reviewCards.Count)
        {
            SlideToIndex(itemIndex,reverseAnimation);
        }
    }
    
    private void SlideToIndex(int newIndex, bool reverse)
    {
        StopAutoSlide();
        StartCoroutine(SlideTransition(newIndex, reverse));
        if (autoSlide) StartAutoSlide();
    }
    
    private IEnumerator SlideTransition(int newIndex, bool reverse)
    {
        isTransitioning = true;
        float direction = reverse ? -1f : 1f;
        var currentCard = reviewCards[currentIndex];
        var nextCard = reviewCards[newIndex];
        
        nextCard.SetHorizontalPosition(direction * Screen.width);
        nextCard.SetAlpha(0f);
        
        float elapsedTime = 0;
        while (elapsedTime < slideTransitionDuration)
        {
            float t = elapsedTime / slideTransitionDuration;
            currentCard.SetHorizontalPosition(Mathf.Lerp(0, -direction * Screen.width, t));
            currentCard.SetAlpha(Mathf.Lerp(1f, 0f, t));
            nextCard.SetHorizontalPosition(Mathf.Lerp(direction * Screen.width, 0, t));
            nextCard.SetAlpha(Mathf.Lerp(0f, 1f, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        currentCard.SetAlpha(0f);
        nextCard.SetAlpha(1f);
        currentIndex = newIndex;
        isTransitioning = false;
    }
    
    private void UpdateReviewsVisibility()
    {
        for (int i = 0; i < reviewCards.Count; i++)
        {
            int relativePos = i - currentIndex;
            if (relativePos < 0)
                relativePos += reviewCards.Count;
                
            if (relativePos < visibleCardsCount)
            {
                // Visible card
                reviewCards[i].SetHorizontalPosition(relativePos * (reviewCards[i].GetWidth() + cardSpacing));
                reviewCards[i].SetAlpha(1f);
            }
            else
            {
                // Hidden card
                reviewCards[i].SetHorizontalPosition(reviewCards.Count * (reviewCards[i].GetWidth() + cardSpacing));
                reviewCards[i].SetAlpha(0f);
            }
        }
    }
    
    private void UpdatePaginationDots()
    {
        if (paginationDots.Count == 0)
            return;
            
        int currentPage = currentIndex / visibleCardsCount;
        
        for (int i = 0; i < paginationDots.Count; i++)
        {
            // Get the image component to change its color
            Image dotImage = paginationDots[i].GetComponent<Image>();
            if (dotImage != null)
            {
                // Set active/inactive color
                dotImage.color = (i == currentPage) ? 
                    new Color(1f, 0.6f, 0f) : // Active dot (orange)
                    new Color(0.7f, 0.7f, 0.7f); // Inactive dot (gray)
            }
        }
    }
    
    private void UpdateNavigationButtons()
    {
        if (reviewCards.Count <= visibleCardsCount)
        {
            // Hide navigation if not needed
            if (previousButton != null) previousButton.gameObject.SetActive(false);
            if (nextButton != null) nextButton.gameObject.SetActive(false);
            return;
        }
        
        // Show navigation buttons
        if (previousButton != null) previousButton.gameObject.SetActive(true);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }
    
    private void StartAutoSlide()
    {
        StopAutoSlide();
        autoSlideCoroutine = StartCoroutine(AutoSlideCoroutine());
    }

    
    private void StopAutoSlide()
    {
        if (autoSlideCoroutine != null)
        {
            StopCoroutine(autoSlideCoroutine);
            autoSlideCoroutine = null;
        }
    }
    
    private IEnumerator AutoSlideCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSlideInterval);
            
            if (!isTransitioning && reviewCards.Count > visibleCardsCount)
            {
                ShowNextReview();
            }
        }
    }
    
    // Public method to populate reviews at runtime
    public void PopulateReviews(List<ReviewData> newReviews)
    {
        reviewsData = newReviews;
        currentIndex = 0;
        
        // Clear and reinitialize
        InitializeReviews();
    }
    
    // Method to add a single review
    public void AddReview(ReviewData newReview)
    {
        reviewsData.Add(newReview);
        
        // Create new review card
        ReviewCardItem reviewCard = Instantiate(reviewCardPrefab, reviewsContainer);
        reviewCard.Init(newReview);
        reviewCards.Add(reviewCard);
        
        // Position off-screen initially
        reviewCard.SetHorizontalPosition(reviewCards.Count * (reviewCard.GetWidth() + cardSpacing));
        reviewCard.SetAlpha(0f);
        
        // Update pagination dots
        CreatePaginationDots();
        UpdatePaginationDots();
        
        // Update navigation buttons
        UpdateNavigationButtons();
    }
    
    private void OnDestroy()
    {
        StopAutoSlide();
        
        if (previousButton != null)
            previousButton.onClick.RemoveAllListeners();
        
        if (nextButton != null)
            nextButton.onClick.RemoveAllListeners();
            
        foreach (GameObject dot in paginationDots)
        {
            Button dotButton = dot.GetComponent<Button>();
            if (dotButton != null)
                dotButton.onClick.RemoveAllListeners();
        }
    }
}