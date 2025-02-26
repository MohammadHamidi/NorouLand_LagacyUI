using LocalizationSystem;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class NamePageController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private RTLTextMeshPro placeholderText;
    [SerializeField] private RTLTextMeshPro text;
    [SerializeField] private Button continueButton;
    
    [Header("Validation")]
    [SerializeField] private int minNameLength = 1;

    private OnboardingFlowManager flowManager;
    private bool isRTL;
    private INameStorage nameStorage;
    
   
    
    private void OnDisable()
    {
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.RemoveListener(OnNameChanged);
        }
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateTextDirection;
        }
    }
    
    private void UpdateTextDirection()
    {
        if (LocalizationManager.Instance != null)
        {
            isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
         
            
            
            nameInputField.textComponent.alignment = TextAlignmentOptions.Right;
            // //nameInputField.textComponent.alignment = TextAlignmentOptions.Right;
            // if (placeholderText != null && text != null)
            // {
            //
            placeholderText.text = isRTL ? "اسم فرزند شما" : "Your child's name";
            placeholderText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            text.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            nameInputField.customCaretColor = true;

            // Set caret color with alpha = 0 to make it invisible
            Color invisibleColor = nameInputField.caretColor;
            invisibleColor.a = 0f;
            nameInputField.caretColor = invisibleColor;
            

            // }
        }
        else
        {
            Debug.LogError($"Localizaation Manger was null");
        }
    }
    
    private void OnNameChanged(string name)
    {
        UpdateContinueButton();
    }
    
    private void UpdateContinueButton()
    {
        if (continueButton != null && nameInputField != null)
        {
            continueButton.interactable = nameInputField.text.Trim().Length >= minNameLength;
        }
    }
    

    
    
    private void Awake()
    {
        // Initialize the name storage
        nameStorage = new PlayerPrefsNameStorage();
    
        // Debug checks
        Debug.Log("NamePageController Awake called");
    
        flowManager = FindObjectOfType<OnboardingFlowManager>();
        if (flowManager == null)
            Debug.LogError("FlowManager is NULL!");
        else
            Debug.Log("FlowManager found successfully");
    
        if (continueButton == null)
            Debug.LogError("ContinueButton is NULL!");
        else
            Debug.Log("ContinueButton found successfully");
    
       
    }
    
    private void OnContinuePressed()
    {
        Debug.Log("Continue button pressed directly!");
        SaveChildName();
        
            flowManager.ContinueToNextPage();
        
    }
    
    private void Start()
    {
        if (nameInputField != null)
        {
            // Load existing name if available
            if (nameStorage.HasName())
            {
                nameInputField.text = nameStorage.LoadName();
            }
            
            nameInputField.onValueChanged.AddListener(OnNameChanged);
            nameInputField.ActivateInputField();
        }
        
        UpdateTextDirection();
        UpdateContinueButton();
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateTextDirection;
        }
        if (continueButton != null && flowManager != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinuePressed);
        }
        
    }
    
    
    
    public string GetChildName()
    {
        return nameInputField != null ? nameInputField.text.Trim() : "";
    }
    
    // New method to save the name
    public void SaveChildName()
    {
        string name = GetChildName();
        if (!string.IsNullOrEmpty(name))
        {
            nameStorage.SaveName(name);
        }
    }


}
