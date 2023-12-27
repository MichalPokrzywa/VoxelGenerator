using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class WorldVisualization : MonoBehaviour
{
    [SerializeField] public CalculateBlockTypesJobs calculate;
    [SerializeField] public List<PerlinGrapher> perlinGraphers = new List<PerlinGrapher>();
    [SerializeField] public Perlin3DGrapher perlinGrapher3D;
    public List<PerlinSettings> perlinSettings = new List<PerlinSettings>();

    void Start()
    {
        foreach (PerlinGrapher child in transform.GetComponentsInChildren<PerlinGrapher>())
        {
            Debug.Log(child.gameObject.name);
            child.StartSetup();
            perlinGraphers.Add(child);
        }

        if (calculate == null)
        {
            calculate = new CalculateBlockTypesJobs();
        }
        if(perlinGraphers.Count > 0 || perlinGrapher3D != null)
            gameObject.SetActive(false);
    }

    public void CreateSettings()
    {
        foreach (PerlinGrapher perlin in perlinGraphers)
        {
            PerlinSettings settings = new PerlinSettings(perlin.heightScale,perlin.scale,perlin.octaves,perlin.heightOffset,perlin.probability);
            perlinSettings.Add(settings);
        }

        if (perlinGrapher3D != null)
        {
            PerlinSettings settings = new PerlinSettings(perlinGrapher3D.heightScale, perlinGrapher3D.scale, perlinGrapher3D.octaves, perlinGrapher3D.heightOffset, perlinGrapher3D.drawCutOff);
            perlinSettings.Add(settings);
        }
        Debug.Log(perlinSettings.Count);
    }
    public string SerializeToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
[Serializable]
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