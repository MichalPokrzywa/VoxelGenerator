using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateText : MonoBehaviour
{
    [SerializeField] public TMP_Text text;
    [SerializeField] public Slider slider;

    void Start()
    {
        UpdateTextFrom();
    }
    public void UpdateTextFrom()
    {
        this.text.text = Math.Round((decimal)slider.value, 3).ToString();
    }

    public void UpdateTextFromInt()
    {
        this.text.text = slider.value.ToString();
    }
}
