/**
 * @file Perlin3DGrapher.cs
 * @brief Defines the Perlin3DGrapher class for graphing 3D Perlin noise in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Perlin3DGrapher
 * @brief Graphs 3D Perlin noise in a 3D grid of cubes in Unity.
 * @details This class is marked with [ExecuteInEditMode] to allow for graphing in the Unity Editor.
 */
[ExecuteInEditMode]
public class Perlin3DGrapher : MonoBehaviour
{
    /** Dimensions of the 3D grid. */
    Vector3 dimensions = new Vector3(10, 10, 10);

    /** Height scale factor applied to the Perlin noise. */
    public float heightScale = 2;

    /** Scale factor influencing the scale of the Perlin noise. */
    [Range(0.0f, 1.0f)]
    public float scale = 0.5f;

    /** Number of octaves used in the Perlin noise calculation. */
    public int octaves = 1;

    /** Offset applied to the height values in the Perlin noise. */
    public float heightOffset = 1;

    /** Cutoff value for drawing cubes based on the Perlin noise. */
    [Range(0.0f, 10.0f)]
    public float drawCutOff = 1;

    /**
     * @brief Creates a grid of cubes in the scene.
     */
    void CreateCubes()
    {
        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "perlin_cube";
                    cube.transform.parent = this.transform;
                    cube.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    /**
     * @brief Graphs the 3D Perlin noise in the grid of cubes.
     */
    public void Graph()
    {
        // Destroy existing cubes
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if (cubes.Length == 0)
            CreateCubes();

        if (cubes.Length == 0) return;

        for (int z = 0; z < dimensions.z; z++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    float p3d = MeshUtils.fBM3D(x, y, z, octaves, scale, heightScale, heightOffset);
                    if (p3d < drawCutOff)
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = false;
                    else
                        cubes[x + (int)dimensions.x * (y + (int)dimensions.z * z)].enabled = true;
                }
            }
        }
    }

    /**
     * @brief Called when the script is loaded or a value is changed in the Inspector.
     */
    void OnValidate()
    {
        Graph();
    }
}
