/**
 * @file WorldSaver.cs
 * @brief Defines the WorldSaver class for saving and loading the game world in Unity.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/**
 * @class WorldData
 * @brief Represents the data structure for saving and loading the game world.
 */
[Serializable]
public class WorldData
{
    /** Array to store values of chunk checker positions. */
    public int[] chunkCheckerValues;

    /** Array to store values of chunk column positions. */
    public int[] chunkColumnsValues;

    /** Array to store block data for each chunk. */
    public int[] chunkData;

    /** Array to store visibility status for each chunk. */
    public bool[] chunkVisibility;

    /** X-coordinate of the first-person character. */
    public int fpcX;

    /** Y-coordinate of the first-person character. */
    public int fpcY;

    /** Z-coordinate of the first-person character. */
    public int fpcZ;

    /** Array to store the dimensions of the game world. */
    public int[] worldDimensions;

    /** Array to store the dimensions of each chunk. */
    public int[] chunkDimensions;

    /** Array to store Perlin noise settings for world generation. */
    public PerlinSettings[] perlinSettings;

    /** CalculateBlockTypes instance for generating block types in the world. */
    public CalculateBlockTypes calculateBlockTypes;

    /** Flag indicating whether terrain should be hidden. */
    public bool hideTerrain;

    /** Flag indicating whether caves should be used in world generation. */
    public bool useCave;

    /** Default constructor. */
    public WorldData() { }

    /**
     * @brief Constructor to initialize WorldData with relevant information from the game world.
     * @param cc Set of chunk checker positions.
     * @param cCols Set of chunk column positions.
     * @param chunks Dictionary containing ChunkBlock instances.
     * @param fpc Position of the first-person character.
     * @param worldDimension Dimensions of the game world.
     * @param chunkDimension Dimensions of each chunk.
     * @param worldVisualization WorldVisualization instance for Perlin noise settings.
     */
    public WorldData(HashSet<Vector3Int> cc, HashSet<Vector2Int> cCols, Dictionary<Vector3Int, ChunkBlock> chunks,
        Vector3 fpc, Vector3Int worldDimension, Vector3Int chunkDimension, WorldVisualization worldVisualization)
    {
        chunkCheckerValues = new int[cc.Count * 3];
        int index = 0;
        hideTerrain = WorldCreator.hideTerrain;
        useCave = WorldCreator.useCaves;

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

/**
 * @static
 * @class WorldSaver
 * @brief Provides methods for saving and loading the game world in Unity.
 */
public static class WorldSaver
{
    /** Instance of WorldData for storing data during saving and loading. */
    static WorldData worldData;

    /** List of all available saved files. */
    public static List<string> allFiles;

    /**
     * @brief Creates a unique file name for saving the world.
     * @return The generated file name.
     */
    static string CreateBuildFileName()
    {
        return $"{Application.persistentDataPath}/savedata/World_{System.DateTime.Now.ToString().Replace(" ", "").Replace(":", "").Replace(".", "")}.json";
    }

    /**
     * @brief Loads the file name at the specified index.
     * @param index Index of the file name in the list of available files.
     * @return The file name at the specified index.
     */
    static string LoadBuildFileName(int index)
    {
        return allFiles[index];
    }

    /**
     * @brief Saves the current state of the game world.
     * @param worldCreator Instance of WorldCreator representing the game world.
     */
    public static void Save(WorldCreator worldCreator)
    {
        string fileName = CreateBuildFileName();
        if (!File.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
        }

        worldData = new WorldData(worldCreator.chunkChecker, worldCreator.chunkColumns, worldCreator.chunks, worldCreator.fpc.transform.position, WorldCreator.worldDimensions, WorldCreator.chunkDimensions, WorldCreator.worldVisualization);
        string json = JsonUtility.ToJson(worldData);
        File.WriteAllText(fileName, json);

        Debug.Log($"Saving World to file {fileName}");
    }

    /**
     * @brief Loads the game world from a specified file.
     * @param fileLocation The location of the file to load.
     * @return The loaded WorldData instance.
     */
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
