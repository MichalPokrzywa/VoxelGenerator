using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGraphManager : MonoBehaviour
{
    [SerializeField] public PerlinGrapher perlinGrapherSettings;
    [SerializeField] public Perlin3DGrapher perlin3DGrapherSettings;
    [SerializeField] TMP_InputField heightScaleInputField;
    [SerializeField] Slider scaleSlider;
    [SerializeField] TMP_InputField octavesInputField;
    [SerializeField] Slider probabilitySlider;
    [SerializeField] TMP_InputField heightOffsetInputField;

    // Start is called before the first frame update
    void Start()
    {
        if (perlinGrapherSettings != null)
        {
            heightScaleInputField.text = perlinGrapherSettings.heightScale.ToString();
            scaleSlider.value = perlinGrapherSettings.scale;
            octavesInputField.text = perlinGrapherSettings.octaves.ToString();
            probabilitySlider.value = perlinGrapherSettings.probability;
            heightOffsetInputField.text = perlinGrapherSettings.heightOffset.ToString();
        }
        else
        {
            heightScaleInputField.text = perlin3DGrapherSettings.heightScale.ToString();
            scaleSlider.value = perlin3DGrapherSettings.scale;
            octavesInputField.text = perlin3DGrapherSettings.octaves.ToString();
            probabilitySlider.value = perlin3DGrapherSettings.drawCutOff;
            heightOffsetInputField.text = perlin3DGrapherSettings.heightOffset.ToString();
        }



        heightScaleInputField.onEndEdit.AddListener(delegate { HeightScaleValueChangeCheck(); });
        scaleSlider.onValueChanged.AddListener(delegate { ScaleValueChangeCheck(); });
        octavesInputField.onEndEdit.AddListener(delegate { OctavesValueChangeCheck(); });
        probabilitySlider.onValueChanged.AddListener(delegate { ProbabilityValueChangeCheck(); });
        heightOffsetInputField.onEndEdit.AddListener(delegate { HeightOffsetValueChangeCheck(); });
    }
    // Invoked when the value of the text field changes.
    public void HeightScaleValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
            perlinGrapherSettings.heightScale = float.Parse(heightScaleInputField.text);
        else
            perlin3DGrapherSettings.heightScale = float.Parse(heightScaleInputField.text);

    }

    public void ScaleValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
            perlinGrapherSettings.scale = (float)Math.Round((decimal)scaleSlider.value, 3);
        else
            perlin3DGrapherSettings.scale = (float)Math.Round((decimal)scaleSlider.value, 3);
    }

    public void OctavesValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
            perlinGrapherSettings.octaves = int.Parse(octavesInputField.text);
        else
            perlin3DGrapherSettings.octaves = int.Parse(octavesInputField.text);
    }

    public void ProbabilityValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
            perlinGrapherSettings.probability = (float)Math.Round((decimal)probabilitySlider.value, 3);
        else
            perlin3DGrapherSettings.drawCutOff = (float)Math.Round((decimal)probabilitySlider.value, 3);
    }
    public void HeightOffsetValueChangeCheck()
    {
        if (perlinGrapherSettings != null)
            perlinGrapherSettings.heightOffset = float.Parse(heightOffsetInputField.text);
        else
            perlin3DGrapherSettings.heightOffset = float.Parse(heightOffsetInputField.text);
    }
}
