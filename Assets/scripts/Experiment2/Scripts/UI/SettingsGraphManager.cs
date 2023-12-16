using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGraphManager : MonoBehaviour
{
    public PerlinGrapher perlinGrapherSettings;
    [SerializeField] TMP_InputField heightScaleInputField;
    [SerializeField] Slider scaleSlider;
    [SerializeField] TMP_InputField octavesInputField;
    [SerializeField] Slider probabilitySlider;
    [SerializeField] TMP_InputField heightOffsetInputField;

    // Start is called before the first frame update
    void Start()
    {
        heightScaleInputField.text = perlinGrapherSettings.heightScale.ToString();
        scaleSlider.value = perlinGrapherSettings.scale;
        octavesInputField.text = perlinGrapherSettings.octaves.ToString();
        probabilitySlider.value = perlinGrapherSettings.probability;
        heightOffsetInputField.text = perlinGrapherSettings.heightOffset.ToString();


        heightScaleInputField.onEndEdit.AddListener(delegate { HeightScaleValueChangeCheck(); });
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChangeCheck(); });
        octavesInputField.onEndEdit.AddListener(delegate { OctavesValueChangeCheck(); });
        probabilitySlider.onValueChanged.AddListener(delegate { ProbabilityValueChangeCheck(); });
        heightOffsetInputField.onEndEdit.AddListener(delegate { HeightOffsetValueChangeCheck(); });
    }
    // Invoked when the value of the text field changes.
    public void HeightScaleValueChangeCheck()
    {
        perlinGrapherSettings.heightScale = float.Parse(heightScaleInputField.text);
    }

    public void ScaleValueChangeCheck()
    {
        perlinGrapherSettings.scale = (float)Math.Round((decimal)scaleSlider.value, 3);
    }

    public void OctavesValueChangeCheck()
    {
        perlinGrapherSettings.octaves = int.Parse(octavesInputField.text);
    }

    public void ProbabilityValueChangeCheck()
    {
        perlinGrapherSettings.probability = (float)Math.Round((decimal)probabilitySlider.value, 3);
    }
    public void HeightOffsetValueChangeCheck()
    {
        perlinGrapherSettings.heightOffset = float.Parse(heightOffsetInputField.text);
    }
}
