using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using TMPro;

public class RadarChart : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage radarFill;
    [SerializeField] private RectTransform[] skillLabels;
    
    [Header("Radar Chart Settings")]
    [SerializeField] private Material radarMaterial;
    [SerializeField] private float outlineWidth = 0.005f;
    [SerializeField] private float lerpSpeed = 2f;
    
    [Header("RTL Support")]
    [SerializeField] private bool isRTL = false;
    
    // Current values and targets for lerping
    private float[] currentValues = new float[5];
    private float[] targetValues = new float[5];
    private Color currentColor;
    private Color targetColor;
    private Coroutine lerpCoroutine;
    private Material materialInstance;
    
    private void Awake()
    {
        InitializeMaterial();
        
        // Set up RTL if needed
        if (isRTL)
        {
            SetupRTL();
        }
    }
    
    private void OnEnable()
    {
        // Ensure material is initialized when object is re-enabled
        if (materialInstance == null)
        {
            InitializeMaterial();
        }
        
        // Update the radar chart with current values
        UpdateRadarChart();
        
        // Restart any active lerping if we have different target values
        bool needsLerping = false;
        for (int i = 0; i < 5; i++)
        {
            if (Mathf.Abs(currentValues[i] - targetValues[i]) > 0.01f)
            {
                needsLerping = true;
                break;
            }
        }
        
        if (needsLerping || Vector4.Distance(currentColor, targetColor) > 0.01f)
        {
            // Clear any stale coroutine reference
            lerpCoroutine = null;
            
            // Start fresh lerping
            lerpCoroutine = StartCoroutine(LerpValuesRoutine());
        }
    }
    
    private void OnDisable()
    {
        // Clean up coroutine
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }
    }
    
    private void InitializeMaterial()
    {
        // Create a new material instance if needed
        if (materialInstance == null)
        {
            materialInstance = new Material(radarMaterial);
            if (radarFill != null)
            {
                radarFill.material = materialInstance;
            }
        }
    }
    
    private void SetupRTL()
    {
        // Flip skill labels for RTL
        foreach (RectTransform label in skillLabels)
        {
            if (label != null)
            {
                Vector2 pos = label.anchoredPosition;
                label.anchoredPosition = new Vector2(-pos.x, pos.y);
                
                // If using TextMeshPro
                RTLTextMeshPro text = label.GetComponent<RTLTextMeshPro>();
                if (text != null)
                {
                    text.alignment = TextAlignmentOptions.Right;
                }
                else
                {
                    // If using regular Text component
                    Text regularText = label.GetComponent<Text>();
                    if (regularText != null)
                    {
                        regularText.alignment = TextAnchor.MiddleRight;
                    }
                }
            }
        }
    }
    
    // Main method to set up the chart with new values
    public void SetupChart(float[] skillValues, Color chartColor)
    {
        // Only proceed if the object is active
        if (!gameObject.activeInHierarchy)
            return;
            
        // Stop any existing lerping
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }
        
        // Set target values for lerping
        for (int i = 0; i < skillValues.Length && i < 5; i++)
        {
            targetValues[i] = Mathf.Clamp01(skillValues[i]);
        }
        
        targetColor = chartColor;
        
        // Start lerping values
        lerpCoroutine = StartCoroutine(LerpValuesRoutine());
    }
    
    // Immediately set chart values without animation
    public void SetChartValuesImmediate(float[] skillValues, Color chartColor)
    {
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }
        
        // Set values directly
        for (int i = 0; i < skillValues.Length && i < 5; i++)
        {
            currentValues[i] = Mathf.Clamp01(skillValues[i]);
            targetValues[i] = currentValues[i];
        }
        
        currentColor = chartColor;
        targetColor = chartColor;
        
        // Update chart immediately
        UpdateRadarChart();
    }
    
    private IEnumerator LerpValuesRoutine()
    {
        // Ensure material is initialized
        if (materialInstance == null)
        {
            InitializeMaterial();
        }
        
        bool stillLerping = true;
        // Debug.Log("Starting lerp routine with target values: " + 
        //           string.Join(", ", new List<float>(targetValues).ConvertAll(v => v.ToString())));

        
        // Initialize current values if they're at zero
        if (currentValues[0] == 0 && currentValues[1] == 0 && currentValues[2] == 0 && 
            currentValues[3] == 0 && currentValues[4] == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                currentValues[i] = targetValues[i];
            }
            currentColor = targetColor;
            UpdateRadarChart();
            yield break;
        }
        
        while (stillLerping && gameObject.activeInHierarchy)
        {
            stillLerping = false;
            
            // Lerp each value
            for (int i = 0; i < 5; i++)
            {
                if (Mathf.Abs(currentValues[i] - targetValues[i]) > 0.01f)
                {
                    currentValues[i] = Mathf.Lerp(currentValues[i], targetValues[i], Time.deltaTime * lerpSpeed);
                    stillLerping = true;
                }
                else
                {
                    currentValues[i] = targetValues[i];
                }
            }
            
            // Lerp color
            if (Vector4.Distance(currentColor, targetColor) > 0.01f)
            {
                currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * lerpSpeed);
                stillLerping = true;
            }
            else
            {
                currentColor = targetColor;
            }
            
            // Update chart with lerped values
            UpdateRadarChart();
            
            yield return null;
        }
        
        // Ensure we end with the exact target values
        for (int i = 0; i < 5; i++)
        {
            currentValues[i] = targetValues[i];
        }
        currentColor = targetColor;
        UpdateRadarChart();
        
        // Clear the coroutine reference
        lerpCoroutine = null;
    }
    
    private void UpdateRadarChart()
    {
        // Skip if the object is inactive
        if (!gameObject.activeInHierarchy)
            return;
            
        // Ensure material is initialized
        if (materialInstance == null)
        {
            InitializeMaterial();
        }
        
        if (materialInstance == null || radarFill == null)
        {
            Debug.LogError("Cannot update radar chart: " + 
                           (materialInstance == null ? "Material is null" : "RawImage is null"));
            return;
        }
        
        // Debug.Log("UpdateRadarChart with values: " + 
        //           string.Join(", ", new List<float>(currentValues).ConvertAll(v => v.ToString())));

        // Pass values to shader
        materialInstance.SetFloat("_Value1", currentValues[0]);    // Memory
        materialInstance.SetFloat("_Value2", currentValues[1]);    // Language
        materialInstance.SetFloat("_Value3", currentValues[2]);    // SoftSkills
        materialInstance.SetFloat("_Value4", currentValues[3]);    // Math
        materialInstance.SetFloat("_Value5", currentValues[4]);    // Attention
        
        // Set fill color with 50% alpha
        Color fillColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0.5f);
        materialInstance.SetColor("_FillColor", fillColor);
        
        // Set outline color with 100% alpha
        Color outlineColor = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        materialInstance.SetColor("_OutlineColor", outlineColor);
        
        materialInstance.SetFloat("_OutlineWidth", outlineWidth);
       // Debug.Log($"Set fill color: {fillColor}, outline color: {outlineColor}");
    }
    
    // Set a single skill value
    public void SetSkillValue(int skillIndex, float value)
    {
        if (skillIndex >= 0 && skillIndex < 5)
        {
            targetValues[skillIndex] = Mathf.Clamp01(value);
            
            // Start lerping if not already doing so and object is active
            if (lerpCoroutine == null && gameObject.activeInHierarchy)
            {
                lerpCoroutine = StartCoroutine(LerpValuesRoutine());
            }
        }
    }
    
    // Set chart color
    public void SetChartColor(Color color)
    {
        targetColor = color;
        
        // Start lerping if not already doing so and object is active
        if (lerpCoroutine == null && gameObject.activeInHierarchy)
        {
            lerpCoroutine = StartCoroutine(LerpValuesRoutine());
        }
    }
}