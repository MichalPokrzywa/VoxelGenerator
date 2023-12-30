/**
 * @file UpdateText.cs
 * @brief Defines the UpdateText class responsible for updating text based on the Slider value.
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class UpdateText
 * @brief Updates the text based on the Slider value.
 */
public class UpdateText : MonoBehaviour
{
    /** Reference to the TMP_Text object to be updated. */
    [SerializeField] public TMP_Text text;
    /** Reference to the Slider object whose value affects the text. */
    [SerializeField] public Slider slider;

    /**
     * @brief Method called during the object's start. Updates the text based on the Slider value.
     */
    void Start()
    {
        UpdateTextFrom();
    }

    /**
     * @brief Updates the text based on the Slider value with rounding to 3 decimal places.
     */
    public void UpdateTextFrom()
    {
        this.text.text = Math.Round((decimal)slider.value, 3).ToString();
    }

    /**
     * @brief Updates the text based on the integer value of the Slider.
     */
    public void UpdateTextFromInt()
    {
        this.text.text = slider.value.ToString();
    }
}