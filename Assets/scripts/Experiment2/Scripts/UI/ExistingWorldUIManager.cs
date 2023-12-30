/**
 * @file ExistingWorldUIManager.cs
 * @brief Defines a class for managing the user interface of existing worlds in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class ExistingWorldUIManager
 * @brief Manages the user interface for selecting and loading existing worlds in Unity.
 */
public class ExistingWorldUIManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown existingWorlds;
    [SerializeField] Slider renderSlider;
    [SerializeField] Button loadWorld;
    List<string> allWorlds;

    /**
     * @brief Gets the folder path for saving world data.
     * @return The folder path for saving world data.
     */
    static string GetFolder()
    {
        return $"{Application.persistentDataPath}/savedata/";
    }

    /**
     * @brief Initializes the UI elements and event listeners.
     */
    void Start()
    {
        loadWorld.onClick.AddListener(StartLoading);
        allWorlds = Directory.GetFiles(GetFolder(), "*.json").ToList();

        if (allWorlds.Count <= 0)
        {
            loadWorld.interactable = false;
            existingWorlds.interactable = false;
            return;
        }

        existingWorlds.ClearOptions();
        List<TMP_Dropdown.OptionData> data = new List<TMP_Dropdown.OptionData>();

        foreach (var file in allWorlds)
        {
            TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData
            {
                text = Path.GetFileName(file)
            };
            data.Add(newData);
        }

        existingWorlds.AddOptions(data);
        existingWorlds.RefreshShownValue();
    }

    /**
     * @brief Initiates the process of loading the selected existing world.
     */
    void StartLoading()
    {
        Debug.Log(allWorlds[existingWorlds.value]);
        WorldCreator.instance.drawRadius = (int)renderSlider.value;
        WorldCreator.instance.StartBuilding(true, allWorlds[existingWorlds.value]);
    }
}
