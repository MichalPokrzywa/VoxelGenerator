using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateWorldUIManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> avaibleMenus;
    [SerializeField] public TMP_Dropdown menuDropdown;
    [SerializeField] public Slider worldDimensionSlider;
    [SerializeField] public Slider chunkDimensionSlider;
    [SerializeField] public Slider renderDistanceSlider;
    [SerializeField] public Button createWorld;
    [SerializeField] public Button showGraphs;
    [SerializeField] public Button showCave;
    [SerializeField] public List<GameObject> toHide;
    public int currentIndexOfDropdown = 0;
    public WorldVisualization chosenWorldVisualization;

    void Start()
    {
        menuDropdown.onValueChanged.AddListener(delegate { ChooseMenu(); });
        createWorld.onClick.AddListener(WorldCreator.instance.StartWorld);
        showGraphs.onClick.AddListener(CheckGraphs);

    }

    public void ChooseMenu()
    {
        avaibleMenus[currentIndexOfDropdown].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(false);
        avaibleMenus[currentIndexOfDropdown].gameObject.SetActive(false);
        avaibleMenus[menuDropdown.value].gameObject.SetActive(true);
        avaibleMenus[menuDropdown.value].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(true);
        currentIndexOfDropdown = menuDropdown.value;
    }

    public void CheckGraphs()
    {
        foreach (GameObject g in toHide)
        {
            g.SetActive(!g.activeSelf);
        }
    }
}