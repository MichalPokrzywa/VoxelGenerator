using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lr;
    public float heightScale = 2;
    [Range(0, 1)]
    public float scale = 0.5f;
    public int octaves = 1;
    public float heightOffset = 1;
    [Range(0,1)]
    public float probability = 1;
    public Material lineMaterial;
    bool isInitComplete =false;
    void Start()
    {
        if (!isInitComplete)
            StartSetup();
        Graph();
    }

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

    public void Graph()
    {
        lr.positionCount = 100;
        int z = 11;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int x = 0; x < lr.positionCount; x++)
        {
            float y = MeshUtils.fBM(x, z,octaves,scale,heightScale,heightOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lr.SetPositions(positions);
    }

    void OnValidate()
    {
        Graph();
    }
}
