using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance => _instance;

    [SerializeField] public GameObject settings;
    [SerializeField] public GameObject visualization;
    [SerializeField] public GameObject loading;
    [SerializeField] public GameObject gameHub;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    public void ChangeToLoading()
    {
        settings.SetActive(false);
        visualization.SetActive(false);
        loading.SetActive(true);
    }

    public void ChangeToHub()
    {
        loading.SetActive(false);
        gameHub.SetActive(true);
    }

    public void CloseApp()
    {
        Application.Quit();
    }

}
