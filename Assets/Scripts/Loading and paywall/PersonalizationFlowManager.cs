using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using UnityEngine.SceneManagement;

public class PersonalizationFlowManager : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private GameObject nextScreenPrefab;
    
    [Header("Data")]
    [SerializeField] private string defaultChildName = "[Name]";
    
    private string childName;
    
    private void Awake()
    {
        // Get child name from PlayerPrefs
        childName = PlayerPrefs.GetString("ChildName", defaultChildName);
    }
    
    // Method to get the child's name - this was missing and causing the error
    public string GetChildName()
    {
        if (string.IsNullOrEmpty(childName))
        {
            childName = PlayerPrefs.GetString("ChildName", defaultChildName);
        }
        return childName;
    }
    
    // Method to navigate to the next page/screen
    public void GoToNextPage()
    {
        // Option 1: Load next scene by name
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        // Option 2: Instantiate next screen prefab
        else if (nextScreenPrefab != null)
        {
            Instantiate(nextScreenPrefab);
            // Optionally destroy current screen
            // Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("No next page destination configured in PersonalizationFlowManager");
        }
    }
    
    // Event for completion notification
    public System.Action OnPersonalizationCompleted;
    
    // Method to trigger completion
    public void CompletePersonalization()
    {
        if (OnPersonalizationCompleted != null)
        {
            OnPersonalizationCompleted.Invoke();
        }
    }
    
    // Method to set child name (for testing or manual override)
    public void SetChildName(string name)
    {
        childName = name;
        PlayerPrefs.SetString("ChildName", name);
        PlayerPrefs.Save();
    }
}