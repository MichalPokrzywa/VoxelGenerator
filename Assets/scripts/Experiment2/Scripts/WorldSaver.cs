using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class WorldData
{
    public int[] chunkCheckerValues;
    public int[] chunkColumnsValues;
    public int[] chunkData;
    public bool[] chunkVisibility;
    public int fpcX;
    public int fpcY;
    public int fpcZ;
    public int[] worldDimensions;
    public int[] chunkDimensions;
    public PerlinSettings[] perlinSettings;
    public CalculateBlockTypes calculateBlockTypes;
    public WorldData() { }

    public WorldData(HashSet<Vector3Int> cc, HashSet<Vector2Int> cCols, Dictionary<Vector3Int, ChunkBlock> chunks,
        Vector3 fpc,Vector3Int worldDimension,Vector3Int chunkDimension, WorldVisualization worldVisualization)
    {
        chunkCheckerValues = new int[cc.Count * 3];
        int index = 0;
        foreach (Vector3Int vector in cc)
        {
            chunkCheckerValues[index] = vector.x;
            chunkCheckerValues[index + 1] = vector.y;
            chunkCheckerValues[index + 2] = vector.z;
            index += 3;
        }

        chunkColumnsValues = new int[cCols.Count * 2];
        index = 0;
        foreach (Vector2Int vector in cCols)
        {
            chunkColumnsValues[index] = vector.x;
            chunkColumnsValues[index + 1] = vector.y;
            index += 2;
        }

        chunkData = new int[chunks.Count * WorldCreator.chunkDimensions.x * WorldCreator.chunkDimensions.y *
                            WorldCreator.chunkDimensions.z];
        chunkVisibility = new bool[chunks.Count];
        int vIndex = 0;
        index = 0;
        foreach (KeyValuePair<Vector3Int, ChunkBlock> chunk in chunks)
        {
            foreach (MeshUtils.BlockType blockType in chunk.Value.cData)
            {
                chunkData[index] = (int)blockType;
                index++;
            }
            chunkVisibility[vIndex] = chunk.Value.meshRenderer.enabled;
            vIndex++;
        }

        index = 0;
        fpcX = (int)fpc.x;
        fpcY = (int)fpc.y;
        fpcZ = (int)fpc.z;
        worldDimensions = new int[] { worldDimension.x, worldDimension.y, worldDimension.z };
        chunkDimensions = new int[] { chunkDimension.x, chunkDimension.y, chunkDimension.z };
        perlinSettings = new PerlinSettings[worldVisualization.perlinSettings.Count];
        foreach (PerlinSettings setting in worldVisualization.perlinSettings)
        {
            perlinSettings[index] = setting;
            index++;
        }

        calculateBlockTypes = worldVisualization.calculate.generationJob;

    }
}

public static class WorldSaver
{
    static WorldData worldData;
    public static List<string> allFiles;
    static string CreateBuildFileName()
    {
        return $"{Application.persistentDataPath}/savedata/World_{System.DateTime.Today.ToString().Replace(" ", "").Replace(":", "").Replace(".", "")}.json";
    }
    static string LoadBuildFileName(int index)
    {
        return allFiles[index];
    }
    public static void Save(WorldCreator worldCreator)
    {
        string fileName = CreateBuildFileName();
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        }

        worldData = new WorldData(worldCreator.chunkChecker, worldCreator.ChunkColumns, worldCreator.chunks, worldCreator.fpc.transform.position,WorldCreator.worldDimensions,WorldCreator.chunkDimensions,WorldCreator.worldVisualization);
        string json = JsonUtility.ToJson(worldData);
        File.WriteAllText(fileName, json);

        Debug.Log($"Saving World to file {fileName}");
    }

    public static WorldData Load(string fileLocation)
    {
        string fileName = fileLocation;
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            worldData = JsonUtility.FromJson<WorldData>(json);

            Debug.Log($"Loading World from file {fileName}");
            return worldData;
        }

        return null;
    }
}
