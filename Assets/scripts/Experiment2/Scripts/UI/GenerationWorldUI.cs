/**
 * @file GenerationWorldUI.cs
 * @brief Defines a class for managing the UI related to world generation in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class GenerationWorldUI
 * @brief Manages the UI elements related to world generation.
 */
public class GenerationWorldUI : MonoBehaviour
{
    /**
     * @var worldVisualization
     * @brief Reference to the WorldVisualization component used for world generation.
     */
    [SerializeField] public WorldVisualization worldVisualization;
}