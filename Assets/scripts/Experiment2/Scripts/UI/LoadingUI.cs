/**
 * @file LoadingUI.cs
 * @brief Defines a class for managing loading UI in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class LoadingUI
 * @brief Manages loading UI elements, such as a loading bar.
 */
public class LoadingUI : MonoBehaviour
{
    private static LoadingUI _instance;

    /**
     * @property instance
     * @brief Gets the singleton instance of the LoadingUI class.
     */
    public static LoadingUI instance => _instance;

    /**
     * @var loadingBar
     * @brief Reference to the loading bar UI element.
     */
    public Slider loadingBar;

    /**
     * @brief Called when the script instance is being loaded.
     */
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // Destroy the duplicate instance if it already exists.
            Destroy(gameObject);
        }
        else
        {
            // Set this instance as the singleton.
            _instance = this;
        }
    }

    /**
     * @brief Sets the maximum value of the loading bar.
     * @param size The maximum value for the loading bar.
     */
    public void SetMaxValue(int size)
    {
        loadingBar.maxValue = size;
    }

    /**
     * @brief Updates the value of the loading bar.
     */
    public void UpdateValue()
    {
        loadingBar.value++;
    }

    /**
     * @brief Closes the loading UI and switches to the hub UI.
     */
    public void CloseLoading()
    {
        UIManager.instance.ChangeToHub();
    }
}