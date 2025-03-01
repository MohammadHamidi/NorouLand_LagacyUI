using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReviewCardItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI reviewerNameText;
    [SerializeField] private TextMeshProUGUI reviewContentText;
    [SerializeField] private Image reviewerAvatar;
    [SerializeField] private Transform starContainer;
    [SerializeField] private Image starPrefab;
    [SerializeField] private Color activeStarColor = Color.yellow;
    [SerializeField] private Color inactiveStarColor = Color.gray;
    
    [Header("Animation Settings")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    
    // Internal state
    private ReviewData currentReviewData;
    
  
    
    public void Init(ReviewData data)
    {
        currentReviewData = data;
        
        // Set name
        if (reviewerNameText != null)
        {
            reviewerNameText.text = data.ReviewerName;
        }
        
        // Set review content
        if (reviewContentText != null)
        {
            reviewContentText.text = data.ReviewText;
        }
        
        // Set avatar
        if (reviewerAvatar != null && data.reviewerAvatar != null)
        {
            reviewerAvatar.sprite = data.reviewerAvatar;
            reviewerAvatar.gameObject.SetActive(true);
        }
        else if (reviewerAvatar != null)
        {
            // If no avatar provided, hide the image
            reviewerAvatar.gameObject.SetActive(false);
        }
        
        // Setup star Rating
        // SetupStarRating(data.Rating);
    }
    
    private void SetupStarRating(int rating)
    {
        // First clear any existing stars
        foreach (Transform child in starContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add 5 stars
        for (int i = 0; i < 5; i++)
        {
            Image star = Instantiate(starPrefab, starContainer);
            
            // Set color based on Rating (i.e., 4-star Rating means first 4 stars are active)
            star.color = i < rating ? activeStarColor : inactiveStarColor;
        }
    }
    
    // Animation methods
    public void SetHorizontalPosition(float position)
    {
        if (rectTransform != null)
        {
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.x = position;
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }
    
    public void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
    
    // Helper for getting the required width for animations
    public float GetWidth()
    {
        return rectTransform != null ? rectTransform.rect.width : 0f;
    }

    public float GetHorizontalPosition()
    {
        if (rectTransform != null)
        {
            return rectTransform.anchoredPosition.x;
        }
        return 0f;
    }

    public float GetAlpha()
    {
        if (canvasGroup != null)
        {
            return canvasGroup.alpha;
        }
        return 0f;
    }
}