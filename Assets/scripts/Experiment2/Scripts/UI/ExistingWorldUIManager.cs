using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExistingWorldUIManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown existingWorlds;
    [SerializeField] Slider renderSlider;
    [SerializeField] Button loadWorld;
    List<string> allWorlds;
    static string GetFolder()
    {
        return $"{Application.persistentDataPath}/savedata/";
    }
    
    void Start()
    {
        loadWorld.onClick.AddListener(StartLoading);
        allWorlds = Directory.GetFiles(GetFolder(),"*.json").ToList();
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

    void StartLoading()
    {
        Debug.Log(allWorlds[existingWorlds.value]);
        WorldCreator.instance.drawRadius = (int)renderSlider.value;
        WorldCreator.instance.StartBuilding(true,allWorlds[existingWorlds.value]);
    }

}
