/**
 * @file CreateWorldUIManager.cs
 * @brief Defines a class for managing UI elements related to world creation.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class CreateWorldUIManager
 * @brief Manages UI elements related to world creation.
 */
public class CreateWorldUIManager : MonoBehaviour
{
    // List of available menus
    [SerializeField] public List<GameObject> avaibleMenus;

    // Dropdown for selecting menus
    [SerializeField] public TMP_Dropdown menuDropdown;

    // Slider for world dimensions
    [SerializeField] public Slider worldDimensionSlider;

    // Slider for chunk dimensions
    [SerializeField] public Slider chunkDimensionSlider;

    // Slider for render distance
    [SerializeField] public Slider renderDistanceSlider;

    // Button for creating a new world
    [SerializeField] public Button createWorld;

    // Button for toggling visibility of graphs
    [SerializeField] public Button showGraphs;

    // Toggle for enabling/disabling caves
    [SerializeField] public Toggle caveToggle;

    // Toggle for hiding/showing terrain
    [SerializeField] public Toggle hideWorld;

    // List of GameObjects to hide/show
    [SerializeField] public List<GameObject> toHide;

    // Index of the currently selected dropdown item
    public int currentIndexOfDropdown = 0;

    // Flag indicating whether caves are enabled
    public bool useCave = false;

    // Flag indicating whether terrain is hidden
    public bool hideTerrain = false;

    // Chosen world visualization
    public WorldVisualization chosenWorldVisualization;
    /**
     * @brief Initializes the UI manager.
     */
    void Start()
    {
        menuDropdown.onValueChanged.AddListener(delegate { ChooseMenu(); });
        createWorld.onClick.AddListener(GenerateWorld);
        showGraphs.onClick.AddListener(CheckGraphs);
        caveToggle.onValueChanged.AddListener(delegate { ChangeColor(); });
        hideWorld.onValueChanged.AddListener(delegate { ChangeHideTerrain(); });
        menuDropdown.value = 0;
        avaibleMenus[menuDropdown.value].gameObject.SetActive(true);
        avaibleMenus[menuDropdown.value].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(true);
        menuDropdown.RefreshShownValue();
    }

    /**
     * @brief Switches between different UI menus.
     */
    public void ChooseMenu()
    {
        avaibleMenus[currentIndexOfDropdown].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(false);
        avaibleMenus[currentIndexOfDropdown].gameObject.SetActive(false);
        avaibleMenus[menuDropdown.value].gameObject.SetActive(true);
        avaibleMenus[menuDropdown.value].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(true);
        currentIndexOfDropdown = menuDropdown.value;
    }

    /**
     * @brief Toggles the visibility of terrain in the world.
     */
    public void ChangeHideTerrain()
    {
        hideTerrain = hideWorld.isOn;
    }

    /**
     * @brief Toggles visibility of terrain graphs.
     */
    public void CheckGraphs()
    {
        foreach (GameObject g in toHide)
        {
            g.SetActive(!g.activeSelf);
        }
    }

    /**
     * @brief Changes the color of the cave toggle button.
     */
    public void ChangeColor()
    {
        if (!useCave)
        {
            caveToggle.targetGraphic.color = Color.red;
            useCave = true;
        }
        else
        {
            caveToggle.targetGraphic.color = Color.white;
            useCave = false;
        }
    }

    /**
     * @brief Generates a new world based on the selected parameters.
     */
    public void GenerateWorld()
    {
        Vector3Int dataVector = new Vector3Int((int)worldDimensionSlider.value, (int)chunkDimensionSlider.value, (int)renderDistanceSlider.value);
        chosenWorldVisualization = avaibleMenus[currentIndexOfDropdown].GetComponent<GenerationWorldUI>().worldVisualization;
        WorldCreator.instance.StartWorld(chosenWorldVisualization, dataVector, useCave, hideTerrain);
    }
}
