/**
 * @file WorldVisualization.cs
 * @brief Defines a class for visualizing the world using Perlin noise.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * @class WorldVisualization
 * @brief Visualizes the world using Perlin noise.
 */
public class WorldVisualization : MonoBehaviour
{
    // Settings for block type calculation
    [SerializeField] public CalculateBlockTypesJobs calculate;

    // List of 2D Perlin graphers
    [SerializeField] public List<PerlinGrapher> perlinGraphers = new List<PerlinGrapher>();

    // 3D Perlin grapher
    [SerializeField] public Perlin3DGrapher perlinGrapher3D;

    // List of Perlin settings
    public List<PerlinSettings> perlinSettings = new List<PerlinSettings>();

    /**
     * @brief Initializes the world visualization.
     */
    void Start()
    {
        if (perlinGraphers.Count == 0)
        {
            foreach (PerlinGrapher child in transform.GetComponentsInChildren<PerlinGrapher>())
            {
                Debug.Log(child.gameObject.name);
                child.StartSetup();
                perlinGraphers.Add(child);
            }
        }

        if (calculate == null)
        {
            calculate = new CalculateBlockTypesJobs();
        }
        if (perlinGraphers.Count > 0 || perlinGrapher3D != null)
            gameObject.SetActive(false);
    }

    /**
     * @brief Creates Perlin noise settings based on PerlinGraphers and Perlin3DGrapher.
     */
    public void CreateSettings()
    {
        foreach (PerlinGrapher perlin in perlinGraphers)
        {
            PerlinSettings settings = new PerlinSettings(perlin.heightScale, perlin.scale, perlin.octaves, perlin.heightOffset, perlin.probability);
            perlinSettings.Add(settings);
        }

        if (perlinGrapher3D != null)
        {
            PerlinSettings settings = new PerlinSettings(perlinGrapher3D.heightScale, perlinGrapher3D.scale, perlinGrapher3D.octaves, perlinGrapher3D.heightOffset, perlinGrapher3D.drawCutOff);
            perlinSettings.Add(settings);
        }
        Debug.Log(perlinSettings.Count);
    }
}

/**
 * @struct PerlinSettings
 * @brief Represents settings for Perlin noise generation.
 */
[Serializable]
public struct PerlinSettings
{
    public float heightScale;
    public float scale;
    public int octaves;
    public float heightOffset;
    public float probability;

    /**
     * @brief Initializes Perlin noise settings.
     * @param hs Height scale.
     * @param s Scale.
     * @param o Octaves.
     * @param ho Height offset.
     * @param p Probability.
     */
    public PerlinSettings(float hs, float s, int o, float ho, float p)
    {
        heightScale = hs;
        scale = s;
        octaves = o;
        heightOffset = ho;
        probability = p;
    }
}
