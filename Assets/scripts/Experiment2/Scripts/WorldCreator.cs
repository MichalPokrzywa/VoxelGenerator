/**
 * @file WorldCreator.cs
 * @brief Defines the WorldCreator class responsible for generating and managing the game world in Unity.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * @class WorldCreator
 * @brief Generates and manages the game world in Unity.
 */
public class WorldCreator : MonoBehaviour
{
    /** Instance of the WorldCreator to allow for easy access from other scripts. */
    private static WorldCreator _instance;
    public static WorldCreator instance => _instance;

    /** Dimensions of the main world. */
    public static Vector3Int worldDimensions = new Vector3Int(3, 4, 3);

    /** Dimensions of the extra world for optimization. */
    public static Vector3Int extraWorldDimensions = new Vector3Int(1, 4, 1);

    /** Dimensions of each chunk in the world. */
    public static Vector3Int chunkDimensions = new Vector3Int(10, 10, 10);

    /** Prefab for the chunk GameObject. */
    public GameObject chunkPrefab;

    /** First-person character GameObject. */
    public GameObject fpc;

    /** Main camera GameObject. */
    public GameObject mCamera;

    /** Radius for drawing chunks around the player. */
    public int drawRadius = 5;

    /** Instance of the WorldVisualization used for creating the world. */
    public static WorldVisualization worldVisualization;

    /** HashSet to store positions of generated chunks. */
    public HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();

    /** HashSet to store column positions of generated chunks. */
    public HashSet<Vector2Int> chunkColumns = new HashSet<Vector2Int>();

    /** Dictionary to store ChunkBlock instances based on their positions. */
    public Dictionary<Vector3Int, ChunkBlock> chunks = new Dictionary<Vector3Int, ChunkBlock>();

    /** Last built position for optimizing chunk generation. */
    Vector3Int lastBuildPosition;

    /** Flag to determine if the world should be loaded from a file. */
    public bool load = true;

    /** Flag to determine if caves should be used in the world. */
    public static bool useCaves;

    /** Flag to determine if terrain should be hidden. */
    public static bool hideTerrain;

    /** Queue for managing coroutine-based build tasks. */
    Queue<IEnumerator> buildQueue = new Queue<IEnumerator>();

    /** 
     * @brief Method called when the script instance is being loaded. Ensures only one instance of WorldCreator exists.
     */
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    /**
     * @brief Saves the current state of the world.
     */
    public void SaveWorld()
    {
        WorldSaver.Save(this);
    }

    /**
     * @brief Coroutine to manage the build tasks.
     */
    IEnumerator BuildCoordinator()
    {
        while (true)
        {
            while (buildQueue.Count > 0)
                yield return StartCoroutine(buildQueue.Dequeue());
            yield return null;
        }
    }

    /**
     * @brief Initializes the world based on the provided parameters.
     * @param chosenWorldVisualization The selected model for terrain generation.
     * @param dataVector The vector containing world dimensions, chunk dimensions, and draw radius.
     * @param useCavesChoose Flag indicating whether caves should be used.
     * @param isHideTerrain Flag indicating whether terrain should be hidden.
     */
    public void StartWorld(WorldVisualization chosenWorldVisualization, Vector3Int dataVector, bool useCavesChoose, bool isHideTerrain)
    {
        worldVisualization = chosenWorldVisualization;
        worldVisualization.CreateSettings();
        worldDimensions = new Vector3Int(dataVector.x, (int)(worldVisualization.perlinSettings[0].heightScale + worldVisualization.perlinSettings[0].heightOffset), dataVector.x);
        chunkDimensions = new Vector3Int(dataVector.y, dataVector.y, dataVector.y);
        drawRadius = dataVector.z;
        hideTerrain = isHideTerrain;
        useCaves = useCavesChoose;
        UIManager.instance.ChangeToLoading();
        LoadingUI.instance.SetMaxValue(worldDimensions.x * worldDimensions.z);
        StartBuilding(false);
    }

    /**
     * @brief Initiates the building process of the world.
     * @param fromFile Flag indicating whether to load the world from a file.
     * @param fileName Name of the file to load if loading from a file.
     */
    public void StartBuilding(bool fromFile, string fileName = "")
    {
        load = fromFile;
        StartCoroutine(load ? LoadWorldFromFile(fileName) : BuildWorld());
    }

    /**
     * @brief Coroutine to build a column of chunks at the specified coordinates.
     * @param x X-coordinate of the chunk column.
     * @param z Z-coordinate of the chunk column.
     * @param meshEnable Flag indicating whether the mesh should be enabled for the chunks.
     */
    IEnumerator BuildChunkColumn(int x, int z, bool meshEnable = true)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int position = new Vector3Int(x, y * chunkDimensions.y, z);
            if (!chunkChecker.Contains(position))
            {
                GameObject chunk = Instantiate(chunkPrefab);
                chunk.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
                ChunkBlock c = chunk.GetComponent<ChunkBlock>();
                c.CreateChunk(chunkDimensions, position);
                chunkChecker.Add(position);
                chunks.Add(position, c);
            }
            chunks[position].gameObject.SetActive(true);
            chunks[position].meshRenderer.enabled = meshEnable;
            yield return null;
        }
        chunkColumns.Add(new Vector2Int(x, z));
    }

    /**
     * @brief Hides a chunk column at the specified coordinates.
     * @param x X-coordinate of the chunk column.
     * @param z Z-coordinate of the chunk column.
     */
    public void HideChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkChecker.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
                chunks[pos].gameObject.SetActive(false);
            }
        }
    }

    /**
     * @brief Coroutine to hide columns of chunks based on the player's position.
     * @param x X-coordinate of the player's position.
     * @param z Z-coordinate of the player's position.
     */
    IEnumerator HideCollums(int x, int z)
    {
        Vector2Int fpcPos = new Vector2Int(x, z);
        foreach (Vector2Int chunkColumn in chunkColumns)
        {
            if ((chunkColumn - fpcPos).magnitude >= drawRadius * chunkDimensions.x)
            {
                HideChunkColumn(chunkColumn.x, chunkColumn.y);
            }
        }
        yield return null;
    }

    /**
     * @brief Coroutine to build the extra world for optimization.
     */
    IEnumerator BuildExtraWorld()
    {
        int zEnd = worldDimensions.z + extraWorldDimensions.z;
        int zStart = worldDimensions.z;
        int xEnd = worldDimensions.x + extraWorldDimensions.x;
        int xStart = worldDimensions.x;

        for (int z = zStart; z <= zEnd; z++)
        {
            for (int x = 0; x < xEnd; x++)
            {
                StartCoroutine(BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false));
                yield return null;
            }
        }

        for (int z = 0; z < zEnd; z++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                StartCoroutine(BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false));
                yield return null;
            }
        }
    }

    /**
     * @brief Coroutine to build the main world.
     */
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++)
            {
                StartCoroutine(BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z));
                LoadingUI.instance.UpdateValue();
                yield return null;
            }
        }
        mCamera.SetActive(false);
        LoadingUI.instance.CloseLoading();
        fpc.transform.position = new Vector3((worldDimensions.x * chunkDimensions.x) / 2.0f, (int)(worldVisualization.perlinSettings[0].heightScale + worldVisualization.perlinSettings[0].heightOffset) + 20, (worldDimensions.z * chunkDimensions.z) / 2.0f);
        fpc.SetActive(true);
        lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
        StartCoroutine(BuildCoordinator());
        StartCoroutine(UpdateWorld());
        StartCoroutine(BuildExtraWorld());
    }

    /**
     * @brief Coroutine to load the world from a file.
     * @param fileName Name of the file to load.
     */
    IEnumerator LoadWorldFromFile(string fileName)
    {
        WorldData worldData = WorldSaver.Load(fileName);
        worldVisualization = GetComponent<WorldVisualization>();
        UIManager.instance.ChangeToLoading();
        LoadingUI.instance.SetMaxValue(worldDimensions.x * worldDimensions.z);
        worldVisualization.perlinSettings = worldData.perlinSettings.ToList();
        worldVisualization.calculate.generationJob = worldData.calculateBlockTypes;
        chunkDimensions = new Vector3Int(worldData.chunkDimensions[0], worldData.chunkDimensions[1], worldData.chunkDimensions[2]);
        worldDimensions = new Vector3Int(worldData.worldDimensions[0], worldData.worldDimensions[1], worldData.worldDimensions[2]);
        chunkChecker.Clear();
        for (int i = 0; i < worldData.chunkCheckerValues.Length; i += 3)
        {
            chunkChecker.Add(new Vector3Int(worldData.chunkCheckerValues[i], worldData.chunkCheckerValues[i + 1], worldData.chunkCheckerValues[i + 2]));
        }

        chunkColumns.Clear();
        for (int i = 0; i < worldData.chunkColumnsValues.Length; i += 2)
        {
            chunkColumns.Add(new Vector2Int(worldData.chunkColumnsValues[i], worldData.chunkColumnsValues[i + 1]));
        }

        int index = 0;
        int vIndex = 0;
        LoadingUI.instance.SetMaxValue(chunkChecker.Count);
        foreach (Vector3Int chunkPos in chunkChecker)
        {
            GameObject chunk = Instantiate(chunkPrefab);
            chunk.name = $"Chunk_{chunkPos.x}_{chunkPos.y}_{chunkPos.z}";
            ChunkBlock c = chunk.GetComponent<ChunkBlock>();
            int blockCount = chunkDimensions.x * chunkDimensions.y * chunkDimensions.z;
            c.cData = new MeshUtils.BlockType[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                c.cData[i] = (MeshUtils.BlockType)worldData.chunkData[i];
                index++;
            }
            c.CreateChunk(chunkDimensions, chunkPos);
            chunks.Add(chunkPos, c);
            RedrawChunk(c);
            c.meshRenderer.enabled = worldData.chunkVisibility[vIndex];
            vIndex++;
            LoadingUI.instance.UpdateValue();
            yield return null;
        }
        fpc.transform.position = new Vector3(worldData.fpcX, worldData.fpcY + 1, worldData.fpcZ);
        mCamera.SetActive(false);
        fpc.SetActive(true);
        LoadingUI.instance.CloseLoading();
        lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
        StartCoroutine(BuildCoordinator());
        StartCoroutine(UpdateWorld());
    }

    /**
     * @brief Redraws the mesh for a given chunk.
     * @param c The ChunkBlock to redraw.
     */
    void RedrawChunk(ChunkBlock c)
    {
        DestroyImmediate(c.GetComponent<MeshFilter>());
        DestroyImmediate(c.GetComponent<MeshRenderer>());
        DestroyImmediate(c.GetComponent<Collider>());
        c.CreateChunk(chunkDimensions, c.location, false);
    }

    /**
     * @brief Coroutine to build chunks recursively based on the player's position.
     * @param x X-coordinate of the player's position.
     * @param z Z-coordinate of the player's position.
     * @param rad Current radius for chunk generation.
     */
    IEnumerator BuildRecursiveWorld(int x, int z, int rad)
    {
        int nextrad = rad - 1;
        if (rad <= 0) yield break;
        StartCoroutine(BuildChunkColumn(x, z + chunkDimensions.z));
        buildQueue.Enqueue(BuildRecursiveWorld(x, z + chunkDimensions.z, nextrad));
        yield return null;
        StartCoroutine(BuildChunkColumn(x, z - chunkDimensions.z));
        buildQueue.Enqueue(BuildRecursiveWorld(x, z - chunkDimensions.z, nextrad));
        yield return null;
        StartCoroutine(BuildChunkColumn(x + chunkDimensions.x, z));
        buildQueue.Enqueue(BuildRecursiveWorld(x + chunkDimensions.x, z, nextrad));
        yield return null;
        StartCoroutine(BuildChunkColumn(x - chunkDimensions.x, z));
        buildQueue.Enqueue(BuildRecursiveWorld(x - chunkDimensions.x, z, nextrad));
        yield return null;
    }

    /** WaitForSeconds instance for controlling update intervals. */
    WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

    /**
     * @brief Coroutine to continuously update the world based on the player's position.
     */
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastBuildPosition - fpc.transform.position).magnitude > chunkDimensions.x)
            {
                lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
                int posx = (int)(fpc.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posz = (int)(fpc.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                buildQueue.Enqueue(BuildRecursiveWorld(posx, posz, drawRadius));
                if (hideTerrain)
                    buildQueue.Enqueue(HideCollums(posx, posz));
            }
            yield return waitForSeconds;
        }
    }
}
