using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVisualization : MonoBehaviour
{
    [SerializeField] public CalculateBlockTypesJobs calculate;
    [SerializeField] public List<PerlinGrapher> perlinGraphers = new List<PerlinGrapher>();
    [SerializeField] public Perlin3DGrapher perlinGrapher3D;
    public static List<PerlinSettings> perlinSettings = new List<PerlinSettings>();


    void Start()
    {
        foreach (PerlinGrapher child in transform.GetComponentsInChildren<PerlinGrapher>())
        {
            Debug.Log(child.gameObject.name);
            perlinGraphers.Add(child);
        }
    }

    public void CreateSettings()
    {
        foreach (PerlinGrapher perlin in perlinGraphers)
        {
            PerlinSettings settings = new PerlinSettings(perlin.heightScale,perlin.scale,perlin.octaves,perlin.heightOffset,perlin.probability);
            perlinSettings.Add(settings);
        }
    }
}
public struct PerlinSettings
{
    public float heightScale;
    public float scale;
    public int octaves;
    public float heightOffset;
    public float probability;

    public PerlinSettings(float hs, float s, int o, float ho, float p)
    {
        heightScale = hs;
        scale = s;
        octaves = o;
        heightOffset = ho;
        probability = p;

    }
}