/**
 * @file UIManager.cs
 * @brief Defines the UIManager class responsible for managing UI elements in the game.
 */

using UnityEngine;

/**
 * @class UIManager
 * @brief Manages UI elements in the game.
 */
public class UIManager : MonoBehaviour
{
    /** Instance of the UIManager to allow for easy access from other scripts. */
    private static UIManager _instance;
    public static UIManager instance => _instance;

    /** Reference to the settings UI GameObject. */
    [SerializeField] public GameObject settings;
    /** Reference to the visualization UI GameObject. */
    [SerializeField] public GameObject visualization;
    /** Reference to the loading UI GameObject. */
    [SerializeField] public GameObject loading;
    /** Reference to the game hub UI GameObject. */
    [SerializeField] public GameObject gameHub;

    /**
     * @brief Method called when the script instance is being loaded. Ensures only one instance of UIManager exists.
     */
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

    /**
     * @brief Changes UI elements to display loading state.
     */
    public void ChangeToLoading()
    {
        settings.SetActive(false);
        visualization.SetActive(false);
        loading.SetActive(true);
    }

    /**
     * @brief Changes UI elements to display the game hub state.
     */
    public void ChangeToHub()
    {
        loading.SetActive(false);
        gameHub.SetActive(true);
    }

    /**
     * @brief Closes the application.
     */
    public void CloseApp()
    {
        Application.Quit();
    }
}