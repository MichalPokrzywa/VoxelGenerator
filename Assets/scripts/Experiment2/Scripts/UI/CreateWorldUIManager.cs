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
    [SerializeField] public Toggle caveToggle;
    [SerializeField] public Toggle hideWorld;
    [SerializeField] public List<GameObject> toHide;
    public int currentIndexOfDropdown = 0;
    public bool useCave = false;
    public bool hideTerrain = false;
    public WorldVisualization chosenWorldVisualization;

    void Start()
    {
        menuDropdown.onValueChanged.AddListener(delegate { ChooseMenu(); });
        createWorld.onClick.AddListener(GenerateWorld);
        showGraphs.onClick.AddListener(CheckGraphs);
        caveToggle.onValueChanged.AddListener(delegate { ChangeColor(); });
        hideWorld.onValueChanged.AddListener(delegate {ChangeHideTerrain();});
        menuDropdown.value = 0;
        avaibleMenus[menuDropdown.value].gameObject.SetActive(true);
        avaibleMenus[menuDropdown.value].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(true);
        menuDropdown.RefreshShownValue();
    }

    public void ChooseMenu()
    {
        avaibleMenus[currentIndexOfDropdown].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(false);
        avaibleMenus[currentIndexOfDropdown].gameObject.SetActive(false);
        avaibleMenus[menuDropdown.value].gameObject.SetActive(true);
        avaibleMenus[menuDropdown.value].gameObject.GetComponent<GenerationWorldUI>().worldVisualization.gameObject.SetActive(true);
        currentIndexOfDropdown = menuDropdown.value;
    }

    public void RefreshMenu()
    {

    }

    public void ChangeHideTerrain()
    {
        hideTerrain = hideWorld.isOn;
    }

    public void CheckGraphs()
    {
        foreach (GameObject g in toHide)
        {
            g.SetActive(!g.activeSelf);
        }
    }

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

    public void GenerateWorld()
    {
        Vector3Int dataVector = new Vector3Int((int)worldDimensionSlider.value,(int)chunkDimensionSlider.value,(int)renderDistanceSlider.value);
        chosenWorldVisualization = avaibleMenus[currentIndexOfDropdown].GetComponent<GenerationWorldUI>().worldVisualization;
        WorldCreator.instance.StartWorld(chosenWorldVisualization,dataVector,useCave,hideTerrain);
    }
}