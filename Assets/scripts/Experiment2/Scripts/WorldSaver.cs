using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class WorldData
{
    //HashSet<Vector3Int> ChunkChecker = new HashSet<Vector3Int>();
    //HashSet<Vector2Int> ChunkColumns = new HashSet<Vector2Int>();
    //Dictionary<Vector3Int, ChunkBlock> chunks = new Dictionary<Vector3Int, ChunkBlock>();
    public int[] chunkCheckerValues;
    public int[] chunkColumnsValues;
    public int[] chunkData;
    public int fpcX;
    public int fpcY;
    public int fpcZ;

    public WorldData() { }

    public WorldData(HashSet<Vector3Int> cc, HashSet<Vector2Int> cCols, Dictionary<Vector3Int, ChunkBlock> chunks,
        Vector3 fpc)
    {
        chunkCheckerValues = new int[cc.Count * 3];
        int index = 0;
        foreach (Vector3Int vector in cc)
        {
            chunkCheckerValues[index] = vector.x;
            chunkCheckerValues[index +1] = vector.y;
            chunkCheckerValues[index +2] = vector.z;
            index =+ 3;
        }

        chunkColumnsValues = new int[cCols.Count * 2];
        index = 0;
        foreach (Vector2Int vector in cCols)
        {
            chunkColumnsValues[index] = vector.x;
            chunkColumnsValues[index + 1] = vector.y;
            index =+ 2;
        }

        chunkData = new int[chunks.Count * WorldCreator.chunkDimensions.x * WorldCreator.chunkDimensions.y *
                            WorldCreator.chunkDimensions.z];
        index = 0;
        foreach (KeyValuePair<Vector3Int, ChunkBlock> chunk in chunks)
        {
            foreach (MeshUtils.BlockType blockType in chunk.Value.cData)
            {
                chunkData[index] = (int)blockType;
                index++;
            }
        }

        fpcX = (int)fpc.x;
        fpcY = (int)fpc.y;
        fpcZ = (int)fpc.z;
    }
    
}

public static class WorldSaver
{
    static WorldData worldData;

    static string BuildFileName()
    {
        return $"{Application.persistentDataPath}/savedata/World_" +
               $"{WorldCreator.chunkDimensions.x}_{WorldCreator.chunkDimensions.y}_{WorldCreator.chunkDimensions.z}_" +
               $"{WorldCreator.worldDimensions.x}_{WorldCreator.worldDimensions.y}_{WorldCreator.worldDimensions.z}.dat";
    }

    public static void Save(WorldCreator worldCreator)
    {
        string fileName = BuildFileName();
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate);
        worldData = new WorldData(worldCreator.ChunkChecker,worldCreator.ChunkColumns,worldCreator.chunks,worldCreator.fpc.transform.position);
        bf.Serialize(fileStream,worldData);
        Debug.Log($"Saving World to file {fileName}");
    }
}
