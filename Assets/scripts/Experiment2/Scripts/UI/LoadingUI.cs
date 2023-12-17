using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    private static LoadingUI _instance;
    public static LoadingUI instance => _instance;

    public Slider loadingBar;

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
    public void SetMaxValue(int size)
    {
        loadingBar.maxValue = size;
    }

    public void UpdateValue()
    {
        loadingBar.value++;
    }

    public void CloseLoading()
    {
        UIManager.instance.ChangeToHub();
    }

}
