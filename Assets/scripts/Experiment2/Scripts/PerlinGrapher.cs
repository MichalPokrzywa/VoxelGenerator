/**
 * @file PerlinGrapher.cs
 * @brief Defines the PerlinGrapher class for graphing 2D Perlin noise in Unity using a LineRenderer.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class PerlinGrapher
 * @brief Graphs 2D Perlin noise using a LineRenderer in Unity.
 * @details This class is marked with [ExecuteInEditMode] to allow for graphing in the Unity Editor.
 */
[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    /** LineRenderer component for drawing the graph. */
    public LineRenderer lr;

    /** Height scale factor applied to the Perlin noise. */
    public float heightScale = 2;

    /** Scale factor influencing the scale of the Perlin noise. */
    [Range(0, 1)]
    public float scale = 0.5f;

    /** Number of octaves used in the Perlin noise calculation. */
    public int octaves = 1;

    /** Offset applied to the height values in the Perlin noise. */
    public float heightOffset = 1;

    /** Probability factor influencing the graph appearance. */
    [Range(0, 1)]
    public float probability = 1;

    /** Material used for the LineRenderer. */
    public Material lineMaterial;

    /** Flag indicating whether the initialization is complete. */
    bool isInitComplete = false;

    /**
     * @brief Called when the script is loaded or a value is changed in the Inspector.
     */
    void Start()
    {
        if (!isInitComplete)
            StartSetup();
        Graph();
    }

    /**
     * @brief Initializes the LineRenderer and sets up initial parameters.
     */
    public void StartSetup()
    {
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = 100;
        Material newMaterial = new Material(lineMaterial);
        newMaterial.color = new Color(Random.value, Random.value, Random.value);
        lineMaterial = newMaterial;
        lr.material = newMaterial;
        isInitComplete = true;
    }

    /**
     * @brief Graphs the 2D Perlin noise using the LineRenderer.
     */
    public void Graph()
    {
        lr.positionCount = 100;
        int z = 11;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int x = 0; x < lr.positionCount; x++)
        {
            float y = MeshUtils.fBM(x, z, octaves, scale, heightScale, heightOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lr.SetPositions(positions);
    }

    /**
     * @brief Called when the script is loaded or a value is changed in the Inspector.
     */
    void OnValidate()
    {
        Graph();
    }
}
