/**
 * @file SettingsGraphManager.cs
 * @brief Defines the SettingsGraphManager class responsible for managing Perlin noise graph settings in Unity.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class SettingsGraphManager
 * @brief Manages the UI and interactions for adjusting Perlin noise graph settings in Unity.
 */
public class SettingsGraphManager : MonoBehaviour
{
    /** Reference to the PerlinGrapher for 2D graph settings. */
    [SerializeField] public PerlinGrapher perlinGrapherSettings;

    /** Reference to the Perlin3DGrapher for 3D graph settings. */
    [SerializeField] public Perlin3DGrapher perlin3DGrapherSettings;

    /** Input field for adjusting the height scale. */
    [SerializeField] TMP_InputField heightScaleInputField;

    /** Slider for adjusting the graph scale. */
    [SerializeField] Slider scaleSlider;

    /** Input field for adjusting the number of octaves. */
    [SerializeField] TMP_InputField octavesInputField;

    /** Slider for adjusting the probability value. */
    [SerializeField] Slider probabilitySlider;

    /** Input field for adjusting the height offset. */
    [SerializeField] TMP_InputField heightOffsetInputField;

    /** Background image of the UI. */
    [SerializeField] Image background;

    /**
     * @brief Initializes the UI elements with default or saved settings when the script starts.
     */
    void Start()
    {
        if (perlinGrapherSettings != null)
        {
            heightScaleInputField.text = perlinGrapherSettings.heightScale.ToString();
            scaleSlider.value = perlinGrapherSettings.scale;
            octavesInputField.text = perlinGrapherSettings.octaves.ToString();
            probabilitySlider.value = perlinGrapherSettings.probability;
            heightOffsetInputField.text = perlinGrapherSettings.heightOffset.ToString();
            background.color = perlinGrapherSettings.lineMaterial.color;
        }
        else
        {
            heightScaleInputField.text = perlin3DGrapherSettings.heightScale.ToString();
            scaleSlider.value = perlin3DGrapherSettings.scale;
            octavesInputField.text = perlin3DGrapherSettings.octaves.ToString();
            probabilitySlider.value = perlin3DGrapherSettings.drawCutOff;
            heightOffsetInputField.text = perlin3DGrapherSettings.heightOffset.ToString();
        }

        // Registering event listeners for UI elements
        heightScaleInputField.onEndEdit.AddListener(delegate { HeightScaleValueChangeCheck(); });
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChangeCheck(); });
        octavesInputField.onEndEdit.AddListener(delegate { OctavesValueChangeCheck(); });
        probabilitySlider.onValueChanged.AddListener(delegate { ProbabilityValueChangeCheck(); });
        heightOffsetInputField.onEndEdit.AddListener(delegate { HeightOffsetValueChangeCheck(); });
    }

    /**
     * @brief Handles the change in the height scale input field value.
     */
    public void HeightScaleValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
        {
            perlinGrapherSettings.heightScale = float.Parse(heightScaleInputField.text);
            perlinGrapherSettings.Graph();
        }
        else
            perlin3DGrapherSettings.heightScale = float.Parse(heightScaleInputField.text);
    }

    /**
     * @brief Handles the change in the graph scale slider value.
     */
    public void ScaleValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
        {
            perlinGrapherSettings.scale = (float)Math.Round((decimal)scaleSlider.value, 3);
            perlinGrapherSettings.Graph();
        }
        else
            perlin3DGrapherSettings.scale = (float)Math.Round((decimal)scaleSlider.value, 3);
    }

    /**
     * @brief Handles the change in the octaves input field value.
     */
    public void OctavesValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
        {
            perlinGrapherSettings.octaves = int.Parse(octavesInputField.text);
            perlinGrapherSettings.Graph();
        }
        else
            perlin3DGrapherSettings.octaves = int.Parse(octavesInputField.text);
    }

    /**
     * @brief Handles the change in the probability slider value.
     */
    public void ProbabilityValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
        {
            perlinGrapherSettings.probability = (float)Math.Round((decimal)probabilitySlider.value, 3);
            perlinGrapherSettings.Graph();
        }
        else
            perlin3DGrapherSettings.drawCutOff = (float)Math.Round((decimal)probabilitySlider.value, 3);
    }

    /**
     * @brief Handles the change in the height offset input field value.
     */
    public void HeightOffsetValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
        {
            perlinGrapherSettings.heightOffset = float.Parse(heightOffsetInputField.text);
            perlinGrapherSettings.Graph();
        }
        else
        {
            perlin3DGrapherSettings.heightOffset = float.Parse(heightOffsetInputField.text);
        }
    }
}
