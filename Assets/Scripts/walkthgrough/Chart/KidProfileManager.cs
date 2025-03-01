using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidProfileManager : MonoBehaviour
{
    [SerializeField] private GameObject kidCardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private float switchDelay = 3f;
    [SerializeField] private bool autoSwitch = true;
    [SerializeField] private bool isRTL = true;
    
    [Header("Radar Chart")]
    [SerializeField] private RadarChart radarChart; // Reference to the stable RadarChart
    
    [Header("Kid Profiles")]
    [SerializeField] private List<KidProfile> kidProfiles = new List<KidProfile>();
    
    private KidCardController currentCardController;
    private int currentProfileIndex = 0;
    private Coroutine switchCoroutine;
    
    private void Awake()
    {
        // Clear any stale references
        switchCoroutine = null;
    }
    
    private void OnEnable()
    {
        // Handle re-enable state
        if (kidProfiles.Count > 0)
        {
            // Refresh the current card
            CreateInitialKidCard();
            
            // Reinitialize the radar chart with current profile
            if (radarChart != null)
            {
                radarChart.SetChartValuesImmediate(
                    kidProfiles[currentProfileIndex].skillValues, 
                    kidProfiles[currentProfileIndex].chartColor
                );
            }
            
            // Restart auto switching if needed
            if (autoSwitch && kidProfiles.Count > 1)
            {
                StartAutoSwitching();
            }
        }
    }
    
    private void OnDisable()
    {
        // Properly clean up when disabled
        StopAutoSwitching();
    }
    
    private void Start()
    {
        // Make sure the radar chart exists
        if (radarChart == null)
        {
            Debug.LogError("RadarChart reference is missing! Please assign the RadarChart in the inspector.");
        }
        
        // Create initial kid card
        if (kidProfiles.Count > 0)
        {
            CreateInitialKidCard();
            
            // Initialize the radar chart with first profile
            if (radarChart != null)
            {
                radarChart.SetChartValuesImmediate(
                    kidProfiles[currentProfileIndex].skillValues, 
                    kidProfiles[currentProfileIndex].chartColor
                );
            }
            
            if (autoSwitch && kidProfiles.Count > 1)
            {
                StartAutoSwitching();
            }
        }
    }
    
    private void CreateInitialKidCard()
    {
        // Clear any existing children
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new card
        GameObject kidCardObj = Instantiate(kidCardPrefab, cardContainer);
        currentCardController = kidCardObj.GetComponent<KidCardController>();
        
        if (currentCardController != null)
        {
            // Set up the card but WITHOUT passing the radar chart reference
            currentCardController.SetupKidCard(kidProfiles[currentProfileIndex], radarChart);
        }
    }
    
    public void StartAutoSwitching()
    {
        // Ensure we don't have multiple coroutines
        StopAutoSwitching();
        
        // Only start if the object is active
        if (gameObject.activeInHierarchy)
        {
            switchCoroutine = StartCoroutine(SwitchProfilesRoutine());
        }
    }
    
    public void StopAutoSwitching()
    {
        if (switchCoroutine != null)
        {
            StopCoroutine(switchCoroutine);
            switchCoroutine = null;
        }
    }
    
    private IEnumerator SwitchProfilesRoutine()
    {
        while (kidProfiles.Count > 1 && gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(switchDelay);
            
            // Check again in case something changed during the wait
            if (!gameObject.activeInHierarchy || kidProfiles.Count <= 1)
                yield break;
            
            // Switch to next profile
            currentProfileIndex = (currentProfileIndex + 1) % kidProfiles.Count;
            SwitchToProfile(currentProfileIndex);
        }
    }
    
    public void SwitchToProfile(int profileIndex)
    {
        if (profileIndex < 0 || profileIndex >= kidProfiles.Count)
            return;
        
        currentProfileIndex = profileIndex;
        KidProfile profile = kidProfiles[currentProfileIndex];
        
        // Update the card details
        if (currentCardController != null)
        {
            currentCardController.SetupKidCard(profile, radarChart);
        }
        
        // Update the radar chart directly
        if (radarChart != null && radarChart.gameObject.activeInHierarchy)
        {
            radarChart.SetupChart(profile.skillValues, profile.chartColor);
        }
    }
    
    // Method to switch to next/previous profile (can be called from UI buttons)
    public void NextProfile()
    {
        int nextIndex = (currentProfileIndex + 1) % kidProfiles.Count;
        SwitchToProfile(nextIndex);
    }
    
    public void PreviousProfile()
    {
        int prevIndex = (currentProfileIndex - 1 + kidProfiles.Count) % kidProfiles.Count;
        SwitchToProfile(prevIndex);
    }
    
    // Method to add a profile programmatically
    public void AddKidProfile(KidProfile profile)
    {
        kidProfiles.Add(profile);
        
        // If this is the first profile, create the card
        if (kidProfiles.Count == 1)
        {
            CreateInitialKidCard();
        }
        
        // If auto-switching was off but we now have multiple profiles, start it
        if (autoSwitch && kidProfiles.Count > 1 && switchCoroutine == null && gameObject.activeInHierarchy)
        {
            StartAutoSwitching();
        }
    }
    
    // Helper method to create a profile
    public static KidProfile CreateProfile(string name, int age, string subtitle, 
        Sprite avatar, Color color, float[] skills)
    {
        KidProfile profile = new KidProfile
        {
            name = name,
            age = age,
            subtitle = subtitle,
            avatar = avatar,
            chartColor = color,
            skillValues = new float[5]
        };
        
        // Copy skill values
        for (int i = 0; i < Mathf.Min(skills.Length, 5); i++)
        {
            profile.skillValues[i] = Mathf.Clamp01(skills[i]);
        }
        
        return profile;
    }
}