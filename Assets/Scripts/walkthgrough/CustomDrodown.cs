using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;


public class CustomDropdown : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Button that toggles the dropdown list open/closed.")]
    public Button mainButton;
    
    [Tooltip("Panel (or GameObject) that contains the list of options. Hide this by default.")]
    public GameObject dropdownList;
    
    [Tooltip("Text field that displays the currently selected option.")]
    public RTLTextMeshPro selectedText; // Or use TMPro.TextMeshProUGUI if you prefer
    
    [Tooltip("Where each option button will be placed.")]
    public Transform itemParent;
    
    [Tooltip("Prefab for each dropdown option (e.g., a Button with a Text child).")]
    public GameObject itemPrefab;
    
    [Header("Dropdown Data")]
    [Tooltip("List of options to populate in the dropdown.")]
    public List<string> options = new List<string>();
    
    [Tooltip("Event fired when a new option is selected (int = index of selected option).")]
    public UnityEvent<int> OnValueChanged;

    // The currently selected index
    private int selectedIndex = -1;

    // Whether the dropdown list is currently open
    private bool isOpen = false;

    private void Start()
    {
        // Hide the dropdown list initially
        if (dropdownList != null)
            dropdownList.SetActive(false);

        // Hook up the main button click to toggle the dropdown
        if (mainButton != null)
            mainButton.onClick.AddListener(ToggleDropdown);

        // Populate the dropdown list items
        PopulateDropdown();
        
        // Select the first option by default (optional)
        if (options.Count > 0)
        {
            SetSelectedOption(0);
        }
    }

    /// <summary>
    /// Populates the dropdown list with items from the 'options' list.
    /// </summary>
    private void PopulateDropdown()
    {
        // Clear out old items first (if any)
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each option
        for (int i = 0; i < options.Count; i++)
        {
            int index = i; // local copy for the listener
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            
            // If your itemPrefab has a Text component, set it here
            Text itemText = newItem.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = options[i];
            }
            
            // Add a listener to handle selection
            Button itemButton = newItem.GetComponent<Button>();
            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => OnItemClicked(index));
            }
        }
    }

    /// <summary>
    /// Called when an item in the dropdown is clicked.
    /// </summary>
    private void OnItemClicked(int index)
    {
        SetSelectedOption(index);
        CloseDropdown();
    }

    /// <summary>
    /// Sets the currently selected option by index.
    /// </summary>
    private void SetSelectedOption(int index)
    {
        if (index < 0 || index >= options.Count) return;

        selectedIndex = index;

        // Update the displayed text
        if (selectedText != null)
        {
            selectedText.text = options[index];
        }

        // Invoke the event so other scripts know the value changed
        OnValueChanged?.Invoke(selectedIndex);
    }

    /// <summary>
    /// Toggles the dropdown list open/closed.
    /// </summary>
    private void ToggleDropdown()
    {
        if (isOpen)
        {
            CloseDropdown();
        }
        else
        {
            OpenDropdown();
        }
    }

    /// <summary>
    /// Opens the dropdown list.
    /// </summary>
    private void OpenDropdown()
    {
        if (dropdownList != null)
            dropdownList.SetActive(true);

        isOpen = true;
    }

    /// <summary>
    /// Closes the dropdown list.
    /// </summary>
    private void CloseDropdown()
    {
        if (dropdownList != null)
            dropdownList.SetActive(false);

        isOpen = false;
    }
}
