/**
 * @file DropdownChangeMenu.cs
 * @brief Defines a class for changing active menus based on the selection in a dropdown in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * @class DropdownChangeMenu
 * @brief Manages the activation and deactivation of menus based on the selection in a dropdown.
 */
public class DropdownChangeMenu : MonoBehaviour
{
    /// The list of menus to change based on dropdown selection.
    public List<GameObject> listToChange;

    /// The TMP_Dropdown component responsible for menu selection.
    public TMP_Dropdown dropdown;

    /// The index of the currently active menu.
    int activeMenu = 0;

    /**
     * @brief Initializes the event listener for dropdown value changes.
     */
    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { ChangeMenu(); });
    }

    /**
     * @brief Changes the active menu based on the selection in the dropdown.
     */
    void ChangeMenu()
    {
        listToChange[activeMenu].gameObject.SetActive(false);
        listToChange[dropdown.value].gameObject.SetActive(true);
        activeMenu = dropdown.value;
    }
}