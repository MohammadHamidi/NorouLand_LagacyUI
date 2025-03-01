using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;

#if UNITY_EDITOR

#endif

/// <summary>
/// A custom script that arranges digit buttons and a delete key in a grid layout.
/// You provide two prefabs: one for the digit buttons and one for the delete key.
/// If centerZero is enabled, the last row will have an empty cell, the "0" button centered,
/// and the delete button placed in the right cell. The grid starts at the top‑left corner
/// of the parent container. The cell sizes are automatically calculated based on the parent's size.
/// </summary>
public class ButtonGrid : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The RectTransform that will hold all the buttons. If left null, this object's RectTransform is used.")]
    [SerializeField] private RectTransform container;

    [Header("Prefabs")]
    [Tooltip("Prefab for the digit buttons (e.g. 1-9, or 1-0 if centerZero is false).")]
    [SerializeField] private GameObject digitButtonPrefab;
    [Tooltip("Prefab for the delete/backspace button.")]
    [SerializeField] private GameObject deleteButtonPrefab;

    [Header("Layout Settings")]
    [Tooltip("Number of columns in the grid.")]
    [SerializeField] private int columns = 3;
    [Tooltip("Horizontal space between cells.")]
    [SerializeField] private float spacingX = 10f;
    [Tooltip("Vertical space between cells.")]
    [SerializeField] private float spacingY = 10f;

    [Header("Special Layout Options")]
    [Tooltip("If true, the grid will omit the '0' button from the main sequence and instead create a final row with an empty cell, the '0' button, and the delete button.")]
    [SerializeField] private bool centerZero = true;
    
    [SerializeField] private NumberPlaceholders numberPlaceholders;
    
    // All digit keys for the localization system
    private readonly string[] digitKeys = new string[] { "number_1", "number_2", "number_3", "number_4", "number_5", "number_6", "number_7", "number_8", "number_9", "number_0" };
    // Raw digit values for internal use
    private readonly string[] rawDigits = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

    // The starting position for the grid's first cell (row 0, col 0) in local space.
    private Vector2 gridStart = Vector2.zero;
    
    // Computed cell dimensions based on the parent's current size.
    private float cellWidth;
    private float cellHeight;
    
    // Cached row count (depends on centerZero)
    private int rowCount;

    private void Awake()
    {
        if (container == null)
            container = GetComponent<RectTransform>();
            
        // Verify the numberPlaceholders reference
        if (numberPlaceholders == null)
        {
            Debug.LogError("NumberPlaceholders reference is missing in ButtonGrid script!", this);
            numberPlaceholders = FindObjectOfType<NumberPlaceholders>();
            if (numberPlaceholders == null)
            {
                Debug.LogError("Could not find NumberPlaceholders in the scene!", this);
            }
            else
            {
                Debug.Log("Found NumberPlaceholders in the scene and assigned it.", this);
            }
        }
    }

    private IEnumerator Start()
    {
        // Wait for one frame so that the parent's layout is updated.
        yield return new WaitForEndOfFrame();
        CreateGrid();
        
        // Subscribe to language change event to update button texts
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateButtonTexts;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from language change event
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateButtonTexts;
        }
    }

    // Update button texts when language changes
    private void UpdateButtonTexts()
    {
        if (container == null) return;
        
        // Update digit buttons
        for (int i = 0; i < rawDigits.Length; i++)
        {
            Transform buttonTransform = container.Find("Button_" + rawDigits[i]);
            if (buttonTransform != null)
            {
                TMP_Text label = buttonTransform.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    // Get localized version of the digit
                    string localizedDigit = LocalizationManager.Instance.GetLocalizedValue(digitKeys[i]);
                    label.text = localizedDigit;
                }
            }
        }
    }

    // Optionally, update the grid if the parent's dimensions change.
    private void OnRectTransformDimensionsChange()
    {
        if (Application.isPlaying)
        {
            CreateGrid();
        }
    }

    // Rebuild the grid when inspector values change (in play mode)
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CreateGrid();
        }
    }

    private void CreateGrid()
    {
        // Check if numberPlaceholders is still null (double-check)
        if (numberPlaceholders == null)
        {
            Debug.LogError("NumberPlaceholders is still null when creating the grid!", this);
            return;
        }
        
        // Clear existing buttons.
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        // Determine the number of rows used by the grid.
        if (centerZero)
        {
            // For centerZero mode, digits "1" to "9" occupy three rows, plus a fourth row for the "0" and delete button.
            rowCount = 4;
        }
        else
        {
            int totalButtons = rawDigits.Length + 1; // +1 for delete button.
            rowCount = Mathf.CeilToInt(totalButtons / (float)columns);
        }

        // Get the parent's current local size.
        float parentWidth = container.rect.width;
        float parentHeight = container.rect.height;

        // Compute cell size based on parent's size.
        // For horizontal: available width minus total horizontal spacing divided by number of columns.
        cellWidth = (parentWidth - (columns - 1) * spacingX) / columns;
        // For vertical: available height minus total vertical spacing divided by row count.
        cellHeight = (parentHeight - (rowCount - 1) * spacingY) / rowCount;

        // Use GetWorldCorners to obtain the parent's world-space corners.
        Vector3[] worldCorners = new Vector3[4];
        container.GetWorldCorners(worldCorners);
        // The parent's top-left corner is at worldCorners[1].
        Vector3 localTopLeft = container.InverseTransformPoint(worldCorners[1]);

        // Compute gridStart so that the first cell's center is at the parent's top-left corner plus half the cell's width and minus half the cell's height.
        gridStart = new Vector2(localTopLeft.x + cellWidth * 0.5f, localTopLeft.y - cellHeight * 0.5f);

        if (centerZero)
        {
            // Place digit buttons for "1" to "9".
            int totalDigits = 9;
            for (int i = 0; i < totalDigits; i++)
            {
                GameObject btnGO = Instantiate(digitButtonPrefab, container);
                btnGO.name = "Button_" + rawDigits[i];
                int row = i / columns;
                int col = i % columns;
                Vector2 pos = CalculatePosition(row, col);
                RectTransform rt = btnGO.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cellWidth, cellHeight);
                rt.anchoredPosition = pos;

                TMP_Text label = btnGO.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    // Get localized version of the digit
                    string localizedDigit = LocalizationManager.Instance != null 
                        ? LocalizationManager.Instance.GetLocalizedValue(digitKeys[i]) 
                        : rawDigits[i];
                    label.text = localizedDigit;
                }

                Button btn = btnGO.GetComponent<Button>();
                if (btn != null)
                {
                    int digitIndex = i; // Capture the index in a local variable
                    btn.onClick.RemoveAllListeners(); // Clear any existing listeners
                    btn.onClick.AddListener(() => OnDigitButtonPressed(rawDigits[digitIndex]));
                }
            }

            // Final row: empty cell, "0" button, and delete button.
            int finalRow = 3; // rows 0,1,2 filled above.
            // Empty cell in column 0.
            GameObject emptyGO = new GameObject("EmptyCell", typeof(RectTransform));
            emptyGO.transform.SetParent(container, false);
            RectTransform emptyRT = emptyGO.GetComponent<RectTransform>();
            emptyRT.sizeDelta = new Vector2(cellWidth, cellHeight);
            emptyRT.anchoredPosition = CalculatePosition(finalRow, 0);

            // "0" button in column 1.
            GameObject zeroButtonGO = Instantiate(digitButtonPrefab, container);
            zeroButtonGO.name = "Button_0";
            RectTransform zeroRT = zeroButtonGO.GetComponent<RectTransform>();
            zeroRT.sizeDelta = new Vector2(cellWidth, cellHeight);
            zeroRT.anchoredPosition = CalculatePosition(finalRow, 1);
            TMP_Text zeroLabel = zeroButtonGO.GetComponentInChildren<TMP_Text>();
            if (zeroLabel != null)
            {
                // Get localized version of 0
                string localizedZero = LocalizationManager.Instance != null 
                    ? LocalizationManager.Instance.GetLocalizedValue("number_0") 
                    : "0";
                zeroLabel.text = localizedZero;
            }
            Button zeroBtn = zeroButtonGO.GetComponent<Button>();
            if (zeroBtn != null)
            {
                zeroBtn.onClick.RemoveAllListeners(); // Clear any existing listeners
                zeroBtn.onClick.AddListener(() => OnDigitButtonPressed("0"));
            }

            // Delete button in column 2.
            GameObject deleteButtonGO = Instantiate(deleteButtonPrefab, container);
            deleteButtonGO.name = "Button_Delete";
            RectTransform deleteRT = deleteButtonGO.GetComponent<RectTransform>();
            deleteRT.sizeDelta = new Vector2(cellWidth, cellHeight);
            deleteRT.anchoredPosition = CalculatePosition(finalRow, 2);
            Button delBtn = deleteButtonGO.GetComponent<Button>();
            if (delBtn != null)
            {
                delBtn.onClick.RemoveAllListeners(); // Clear any existing listeners
                delBtn.onClick.AddListener(OnDeleteButtonPressed);
            }
        }
        else
        {
            // Place all digits (1 to 0) and then the delete button in sequential order.
            int totalButtons = rawDigits.Length + 1;
            for (int i = 0; i < totalButtons; i++)
            {
                if (i < rawDigits.Length)
                {
                    GameObject btnGO = Instantiate(digitButtonPrefab, container);
                    btnGO.name = "Button_" + rawDigits[i];
                    int row = i / columns;
                    int col = i % columns;
                    RectTransform rt = btnGO.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cellWidth, cellHeight);
                    rt.anchoredPosition = CalculatePosition(row, col);

                    TMP_Text label = btnGO.GetComponentInChildren<TMP_Text>();
                    if (label != null)
                    {
                        // Get localized version of the digit
                        string localizedDigit = LocalizationManager.Instance != null 
                            ? LocalizationManager.Instance.GetLocalizedValue(digitKeys[i]) 
                            : rawDigits[i];
                        label.text = localizedDigit;
                    }

                    Button btn = btnGO.GetComponent<Button>();
                    if (btn != null)
                    {
                        int digitIndex = i; // Capture the index in a local variable
                        btn.onClick.RemoveAllListeners(); // Clear any existing listeners
                        btn.onClick.AddListener(() => OnDigitButtonPressed(rawDigits[digitIndex]));
                    }
                }
                else
                {
                    int row = i / columns;
                    int col = i % columns;
                    GameObject deleteButtonGO = Instantiate(deleteButtonPrefab, container);
                    deleteButtonGO.name = "Button_Delete";
                    RectTransform rt = deleteButtonGO.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(cellWidth, cellHeight);
                    rt.anchoredPosition = CalculatePosition(row, col);

                    Button btn = deleteButtonGO.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners(); // Clear any existing listeners
                        btn.onClick.AddListener(OnDeleteButtonPressed);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates the anchored position for a given row and column index,
    /// starting from gridStart (which is based on the parent's top-left corner).
    /// The grid grows rightward and downward.
    /// </summary>
    private Vector2 CalculatePosition(int row, int col)
    {
        float xPos = col * (cellWidth + spacingX);
        float yPos = row * (cellHeight + spacingY);
        return gridStart + new Vector2(xPos, -yPos);
    }

    // Methods for button actions:
    private void OnDigitButtonPressed(string digit)
    {
        Debug.Log($"DIGIT BUTTON");
        
        if (numberPlaceholders == null)
        {
            Debug.LogError("NumberPlaceholders is null when trying to set digit: " + digit, this);
            return;
        }
        
        Debug.Log("Digit pressed: " + digit);
        
        try
        {
            numberPlaceholders.SetDigit(digit[0]);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error setting digit '{digit}': {ex.Message}", this);
        }
    }

    private void OnDeleteButtonPressed()
    {
        if (numberPlaceholders == null)
        {
            Debug.LogError("NumberPlaceholders is null when trying to delete a digit", this);
            return;
        }
        
        Debug.Log("Delete pressed");
        
        try
        {
            numberPlaceholders.RemoveLastDigit();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error removing last digit: {ex.Message}", this);
        }
    }
}