using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownChangeMenu : MonoBehaviour
{
    public List<GameObject> listToChange;
    public TMP_Dropdown dropdown;
    int activeMenu = 0;
    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { ChangeMenu();});
    }

    // Update is called once per frame
    void ChangeMenu()
    {
        listToChange[activeMenu].gameObject.SetActive(false);
        listToChange[dropdown.value].gameObject.SetActive(true);
        activeMenu = dropdown.value;
    }
}
