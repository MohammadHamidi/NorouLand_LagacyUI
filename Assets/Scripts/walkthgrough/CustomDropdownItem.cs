using RTLTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomDropdownItem : MonoBehaviour
{
    [Header("UI References")]
    public RTLTextMeshPro itemText;            // The label for the dropdown item
    public Button itemButton;        // The button component to capture clicks
    
    [Tooltip("Tick icon that appears on the right when this item is selected")]
    public GameObject tickIcon;      // Assign your tick icon GameObject here (e.g., an Image)

    /// <summary>
    /// Sets the text shown on this dropdown item.
    /// </summary>
    public void SetItemText(string text)
    {
        if (itemText != null)
        {
            itemText.text = text;
        }
    }

    /// <summary>
    /// Adds a click listener to the item's button.
    /// </summary>
    public void AddClickListener(UnityAction callback)
    {
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(callback);
        }
    }

    /// <summary>
    /// Clears all listeners from the item's button. (Optional helper)
    /// </summary>
    public void ClearClickListeners()
    {
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
        }
    }
    
    /// <summary>
    /// Displays or hides the tick icon.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (tickIcon != null)
        {
            tickIcon.SetActive(isSelected);
        }
    }
}