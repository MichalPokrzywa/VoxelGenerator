using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    private static WorldCreator _instance;
    public static WorldCreator instance => _instance;

    public static Vector3Int worldDimensions = new Vector3Int(3, 4, 3);

    public static Vector3Int extraWorldDimensions = new Vector3Int(1, 4, 1);

    public static Vector3Int chunkDimensions = new Vector3Int(10, 10, 10);

    public GameObject chunkPrefab;

    public GameObject fpc;
    public GameObject mCamera;

    public int drawRadius = 5;
    public static WorldVisualization worldVisualization;

    public HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();
    public HashSet<Vector2Int> ChunkColumns = new HashSet<Vector2Int>();
    public Dictionary<Vector3Int, ChunkBlock> chunks = new Dictionary<Vector3Int, ChunkBlock>();
    public List<GameObject> createdGameObjects = new List<GameObject>();
    Vector3Int lastBuildPosition;
    public bool load = true;
    public static bool useCaves;
    public static bool hideTerrain;
    Queue<IEnumerator> buildQueue = new Queue<IEnumerator>();

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
    public void SaveWorld()
    {
        WorldSaver.Save(this);
    }
    IEnumerator BuildCoordinator()
    {
        while (true)
        {
            while (buildQueue.Count > 0)
                yield return StartCoroutine(buildQueue.Dequeue());
            yield return null;
            
        }
    }
    public void StartWorld(WorldVisualization chosenWorldVisualization,Vector3Int dataVector, bool useCaveschoose)
    {
        worldVisualization = chosenWorldVisualization;
        worldVisualization.CreateSettings();
        worldDimensions = new Vector3Int(dataVector.x, (int)(worldVisualization.perlinSettings[0].heightScale + worldVisualization.perlinSettings[0].heightOffset), dataVector.x);
        chunkDimensions = new Vector3Int(dataVector.y, dataVector.y, dataVector.y);
        drawRadius = dataVector.z;
        hideTerrain = false;
        useCaves = useCaveschoose;
        Debug.Log(worldVisualization);
        UIManager.instance.ChangeToLoading();
        LoadingUI.instance.SetMaxValue(worldDimensions.x * worldDimensions.z);
        StartBuilding(false);
    }

    public void StartBuilding(bool fromFile,string fileName = "")
    {
        load = fromFile;
        StartCoroutine(load ? LoadWorldFromFile(fileName) : BuildWorld());
    }
    IEnumerator BuildChunkColumn(int x, int z,bool meshEnable = true)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int position = new Vector3Int(x , y * chunkDimensions.y, z );
            if (!chunkChecker.Contains(position))
            {
                GameObject chunk = Instantiate(chunkPrefab);
                chunk.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z ;
                ChunkBlock c = chunk.GetComponent<ChunkBlock>();
                c.CreateChunk(chunkDimensions, position);
                chunkChecker.Add(position);
                chunks.Add(position,c);
                //createdGameObjects.Add(chunk);
            }
            //GameObject chunk2 = Instantiate(chunks[position].gameObject, chunks[position].gameObject.transform.position, chunks[position].transform.rotation);
            chunks[position].gameObject.SetActive(true);
            chunks[position].meshRenderer.enabled = meshEnable;

            yield return null;

        }
        ChunkColumns.Add(new Vector2Int(x, z));

    }

    public void HideChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkChecker.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
                chunks[pos].gameObject.SetActive(false);
                //GameObject chunkObject = chunks[pos]?.gameObject; // Retrieve the game object
                //if (chunkObject != null)
                //{
                //    Debug.Log(createdGameObjects.Contains(chunks[pos]?.gameObject));
                //    Destroy(chunks[pos].gameObject);
                //}
                //createdGameObjects.Remove(chunkObject);
            }
        }
    }

    IEnumerator HideCollums(int x, int z)
    {
        Vector2Int fpcPos = new Vector2Int(x, z);
        foreach (Vector2Int chunkColumn in ChunkColumns)
        {
            if ((chunkColumn - fpcPos).magnitude >= drawRadius * chunkDimensions.x)
            {
                HideChunkColumn(chunkColumn.x,chunkColumn.y);
            }
        }

        yield return null;
    }

    IEnumerator BuildExtraWorld()
    {
        int zEnd = worldDimensions.z + extraWorldDimensions.z;
        int zStart = worldDimensions.z;
        int xEnd = worldDimensions.x + extraWorldDimensions.x;
        int xStart = worldDimensions.x ;

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
                StartCoroutine(BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z,false));
                yield return null;
            }
        }
    }

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++)
            {
                StartCoroutine(BuildChunkColumn(x * chunkDimensions.x,z * chunkDimensions.z));
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

    IEnumerator LoadWorldFromFile(string fileName)
    {
        WorldData worldData = WorldSaver.Load(fileName);
        worldVisualization = GetComponent<WorldVisualization>();
        UIManager.instance.ChangeToLoading();
        LoadingUI.instance.SetMaxValue(worldDimensions.x * worldDimensions.z);
        worldVisualization.perlinSettings = worldData.perlinSettings.ToList();
        worldVisualization.calculate.generationJob = worldData.calculateBlockTypes;
        chunkDimensions = new Vector3Int(worldData.chunkDimensions[0], worldData.chunkDimensions[1],
            worldData.chunkDimensions[2]);
        worldDimensions = new Vector3Int(worldData.worldDimensions[0], worldData.worldDimensions[1],
            worldData.worldDimensions[2]);
        Debug.Log(worldVisualization.perlinSettings.Count);
        chunkChecker.Clear();
        for (int i = 0; i < worldData.chunkCheckerValues.Length; i+=3)
        {
            chunkChecker.Add(new Vector3Int(worldData.chunkCheckerValues[i], 
                worldData.chunkCheckerValues[i + 1], 
                worldData.chunkCheckerValues[i + 2]));
        }

        ChunkColumns.Clear();
        for (int i = 0; i < worldData.chunkColumnsValues.Length; i+=2)
        {
            ChunkColumns.Add(new Vector2Int(worldData.chunkColumnsValues[i],
                worldData.chunkColumnsValues[i + 1]));
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
            c.CreateChunk(chunkDimensions,chunkPos);
            chunks.Add(chunkPos,c);
            RedrawChunk(c);
            c.meshRenderer.enabled = worldData.chunkVisibility[vIndex];
            vIndex++;
            LoadingUI.instance.UpdateValue();
            yield return null;
        }
        fpc.transform.position = new Vector3(worldData.fpcX,worldData.fpcY+1,worldData.fpcZ);
        mCamera.SetActive(false);
        fpc.SetActive(true);
        LoadingUI.instance.CloseLoading();
        lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
        StartCoroutine(BuildCoordinator());
        StartCoroutine(UpdateWorld());
    }
    void RedrawChunk(ChunkBlock c)
    {
        DestroyImmediate(c.GetComponent<MeshFilter>());
        DestroyImmediate(c.GetComponent<MeshRenderer>());
        DestroyImmediate(c.GetComponent<Collider>());
        c.CreateChunk(chunkDimensions, c.location, false);
    }
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

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastBuildPosition - fpc.transform.position).magnitude > chunkDimensions.x)
            {
                lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
                int posx = (int)(fpc.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posz = (int)(fpc.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                buildQueue.Enqueue(BuildRecursiveWorld(posx,posz,drawRadius));
                if(hideTerrain)
                    buildQueue.Enqueue(HideCollums(posx,posz));
            }

            yield return waitForSeconds;
        }
    }

}
