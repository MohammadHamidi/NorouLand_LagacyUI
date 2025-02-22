using System;
using RTLTMPro;
using UnityEngine;
using TMPro;

public class NumberPlaceholders : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro[] placeholderTexts;
    private int currentIndex = 0;

    private void OnEnable()
    {
        ResetPlaceholders();
    }

    public void SetDigit(char digit)
    {
        if (currentIndex < placeholderTexts.Length)
        {
            placeholderTexts[currentIndex].text = digit.ToString();
            currentIndex++;
        }
    }

    public void RemoveLastDigit()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            placeholderTexts[currentIndex].text = "_";
        }
    }

    public void ResetPlaceholders()
    {
        for (int i = 0; i < placeholderTexts.Length; i++)
        {
            placeholderTexts[i].text = "_";
        }
        currentIndex = 0;
    }
}