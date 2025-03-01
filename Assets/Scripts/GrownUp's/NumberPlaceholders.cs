using System;
using RTLTMPro;
using UnityEngine;
using TMPro;
using LocalizationSystem;

public class NumberPlaceholders : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro[] placeholderTexts;
    [SerializeField] private string emptyPlaceholderText = "_";
    private int currentIndex = 0;
    
    // Store the actual digits for verification later
    private char[] currentDigits;

    private void Awake()
    {
        currentDigits = new char[placeholderTexts.Length];
    }

    private void OnEnable()
    {
        ResetPlaceholders();
        
        // Subscribe to language change events if needed
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateDisplayedDigits;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from language change events
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateDisplayedDigits;
        }
    }

    /// <summary>
    /// Sets the digit at the current placeholder and advances to the next position
    /// </summary>
    /// <param name="digit">The digit to set (in standard form '0'-'9')</param>
    public void SetDigit(char digit)
    {
        if (currentIndex < placeholderTexts.Length)
        {
            // Store the actual digit internally
            currentDigits[currentIndex] = digit;
            
            // Update the display with possibly localized digit
            UpdatePlaceholderText(currentIndex);
            
            // Move to next position
            currentIndex++;
        }
    }

    /// <summary>
    /// Removes the last entered digit
    /// </summary>
    public void RemoveLastDigit()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            
            // Clear the digit at this position
            currentDigits[currentIndex] = '\0';
            
            // Update the display to show empty placeholder
            placeholderTexts[currentIndex].text = emptyPlaceholderText;
        }
    }

    /// <summary>
    /// Resets all placeholders to empty state
    /// </summary>
    public void ResetPlaceholders()
    {
        for (int i = 0; i < placeholderTexts.Length; i++)
        {
            placeholderTexts[i].text = emptyPlaceholderText;
            currentDigits[i] = '\0';
        }
        currentIndex = 0;
    }
    
    /// <summary>
    /// Updates all displayed digits according to the current language
    /// </summary>
    private void UpdateDisplayedDigits()
    {
        // Update all occupied placeholders
        for (int i = 0; i < currentIndex; i++)
        {
            UpdatePlaceholderText(i);
        }
    }
    
    /// <summary>
    /// Updates a specific placeholder with possibly localized digit
    /// </summary>
    private void UpdatePlaceholderText(int index)
    {
        if (index < 0 || index >= placeholderTexts.Length || currentDigits[index] == '\0')
            return;
            
        // Get the corresponding digit key
        string digitKey = "number_" + currentDigits[index];
        
        // Get the localized digit or use the standard digit if no localization is available
        string displayDigit = currentDigits[index].ToString();
        if (LocalizationManager.Instance != null)
        {
            displayDigit = LocalizationManager.Instance.GetLocalizedValue(digitKey);
        }
        
        // Set the text to the (possibly localized) digit
        placeholderTexts[index].text = displayDigit;
    }
    
    /// <summary>
    /// Gets the current number as a string
    /// </summary>
    public string GetCurrentNumber()
    {
        string result = "";
        for (int i = 0; i < currentIndex; i++)
        {
            result += currentDigits[i];
        }
        return result;
    }
}