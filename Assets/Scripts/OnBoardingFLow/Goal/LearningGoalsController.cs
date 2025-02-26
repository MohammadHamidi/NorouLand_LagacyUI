// The prefab component script for each learning goal item
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using LocalizationSystem;
// Updated controller to use the prefab system
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using LocalizationSystem;


[System.Serializable]
public class GoalOption
{
    public string goalKey;
    public Sprite icon;
}

public class LearningGoalsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro titleText;
    [SerializeField] private RTLTextMeshPro subtitleText;
    [SerializeField] private Button continueButton;
    [SerializeField] private RectTransform goalsContainer;
    
    [Header("Prefab Reference")]
    [SerializeField] private LearningGoalItem goalItemPrefab;
    
    [Header("Goal Options")]
    [SerializeField] private List<GoalOption> goalOptions = new List<GoalOption>();
    
    [Header("Localization Keys")]
    [SerializeField] private string titleKey = "help_areas_question";
    [SerializeField] private string subtitleKey = "select_all_that_apply";
    
    private OnboardingFlowManager flowManager;
    private INameStorage nameStorage;
    private bool isRTL;
    private HashSet<string> selectedGoalKeys = new HashSet<string>();
    private List<LearningGoalItem> spawnedGoalItems = new List<LearningGoalItem>();
    
    private void Awake()
    {
        // Find flow manager
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        
        // Initialize name storage
        nameStorage = new PlayerPrefsNameStorage();
    }
    
    private void Start()
    {
        // Clear any existing children in the container
        ClearGoalItems();
        
        // Check RTL/LTR direction first (needed for item setup)
        UpdateTextDirection();
        
        // Spawn goal items based on the options list
        SpawnGoalItems();
        
        // Set localized text
        UpdateLocalizedTexts();
        
        // Set initial button state
        UpdateContinueButton();
        
        // Subscribe to language change
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }
    
    private void OnDestroy()
    {
        // Clean up spawned items
        ClearGoalItems();
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }
    
    private void ClearGoalItems()
    {
        foreach (var item in spawnedGoalItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        spawnedGoalItems.Clear();
    }
    
    private void SpawnGoalItems()
    {
        if (goalItemPrefab == null || goalsContainer == null)
        {
            Debug.LogError("Goal item prefab or container not assigned!");
            return;
        }
        
        foreach (var option in goalOptions)
        {
            // Instantiate the prefab
            LearningGoalItem newItem = Instantiate(goalItemPrefab, goalsContainer);
            
            // Setup the item
            newItem.Setup(option.goalKey, option.icon, this, isRTL);
            
            // Add to our list for tracking
            spawnedGoalItems.Add(newItem);
        }
    }
    
    private void OnLanguageChanged()
    {
        UpdateTextDirection();
        UpdateLocalizedTexts();
        
        // Update item text direction
        foreach (var item in spawnedGoalItems)
        {
            if (item != null)
            {
                item.ApplyTextDirection(isRTL);
            }
        }
    }
    
    private void UpdateTextDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
        }
    }
    
    private void UpdateLocalizedTexts()
    {
        if (LocalizationManager.Instance != null)
        {
            // Set title text with name if available
            if (titleText != null)
            {
                string titleValue = LocalizationManager.Instance.GetLocalizedValue(titleKey);
                
                // Check if we need to insert the child's name
                if (nameStorage.HasName() && (titleValue.Contains("[Name]") || titleValue.Contains("(اسم)")))
                {
                    string childName = nameStorage.LoadName();
                    titleValue = titleValue.Replace("[Name]", childName)
                                        .Replace("(اسم)", childName);
                }
                
                titleText.text = titleValue;
            }
            
            // Set subtitle text
            if (subtitleText != null)
            {
                subtitleText.text = LocalizationManager.Instance.GetLocalizedValue(subtitleKey);
            }
        }
    }
    
    // This method is called by goal items when clicked
    public void ToggleGoalSelection(string goalKey)
    {
        // Toggle selection in our local HashSet
        if (selectedGoalKeys.Contains(goalKey))
        {
            selectedGoalKeys.Remove(goalKey);
        }
        else
        {
            selectedGoalKeys.Add(goalKey);
        }
        
        // Update visual state for all items
        foreach (var item in spawnedGoalItems)
        {
            if (item != null && item.GetGoalKey() == goalKey)
            {
                item.SetSelected(selectedGoalKeys.Contains(goalKey));
                break;
            }
        }
        
        // Notify the flow manager
        if (flowManager != null)
        {
            flowManager.ToggleLearningGoal(goalKey);
        }
        
        // Update continue button state
        UpdateContinueButton();
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            // Enable continue button only if at least one goal is selected
            continueButton.interactable = selectedGoalKeys.Count > 0;
        }
    }
}

