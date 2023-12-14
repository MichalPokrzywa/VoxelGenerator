using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

public class WorldCreator : MonoBehaviour
{
    public static Vector3Int worldDimensions = new Vector3Int(3, 4, 3);

    public static Vector3Int extraWorldDimensions = new Vector3Int(1, 4, 1);

    public static Vector3Int chunkDimensions = new Vector3Int(10, 10, 10);

    public GameObject chunkPrefab;

    public GameObject fpc;
    public GameObject mCamera;
    public Slider loadingBar;

    public int drawRadius = 5;
    //PerlinSettings for generation
    public static PerlinSettings surfaceSettings;
    public PerlinGrapher surface;

    public static PerlinSettings stoneSettings;
    public PerlinGrapher stone;

    public static PerlinSettings diamondTSettings;
    public PerlinGrapher diamondT;

    public static PerlinSettings diamondDSettings;
    public PerlinGrapher diamondD;

    public static PerlinSettings caveSettings;
    public Perlin3DGrapher cave;

    public HashSet<Vector3Int> chunkChecker = new HashSet<Vector3Int>();
    public HashSet<Vector2Int> ChunkColumns = new HashSet<Vector2Int>();
    public Dictionary<Vector3Int, ChunkBlock> chunks = new Dictionary<Vector3Int, ChunkBlock>();

    public Material material;
    Vector3Int lastBuildPosition;
    public bool load = true;
    Queue<IEnumerator> buildQueue = new Queue<IEnumerator>();

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
    void Start()
    {
        //Material random = Instantiate(material);
        //Debug.Log(random);
        //random.color = Color.white;
        loadingBar.maxValue = worldDimensions.x  * worldDimensions.z;
        surfaceSettings = new PerlinSettings(surface.heightScale,surface.scale,surface.octaves,surface.heightOffset,surface.probability);
        stoneSettings = new PerlinSettings(stone.heightScale, stone.scale, stone.octaves, stone.heightOffset, stone.probability);
        diamondTSettings = new PerlinSettings(diamondT.heightScale, diamondT.scale, diamondT.octaves, diamondT.heightOffset, diamondT.probability);
        diamondDSettings = new PerlinSettings(diamondD.heightScale, diamondD.scale, diamondD.octaves, diamondD.heightOffset, diamondD.probability);
        caveSettings = new PerlinSettings(cave.heightScale, cave.scale, cave.octaves, cave.heightOffset, cave.DrawCutOff);
        StartCoroutine(load ? LoadWorldFromFile() : BuildWorld());
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
            }
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
                loadingBar.value++;
                    yield return null;
            }
        }
        mCamera.SetActive(false);
        loadingBar.gameObject.SetActive(false);
        fpc.transform.position = new Vector3((worldDimensions.x * chunkDimensions.x) / 2.0f,100, (worldDimensions.z * chunkDimensions.z) / 2.0f);
        fpc.SetActive(true);
        lastBuildPosition = Vector3Int.CeilToInt(fpc.transform.position);
        StartCoroutine(BuildCoordinator());
        StartCoroutine(UpdateWorld());
        StartCoroutine(BuildExtraWorld());
    }

    IEnumerator LoadWorldFromFile()
    {
        WorldData worldData = WorldSaver.Load();
        if (worldData == null)
        {
            StartCoroutine(BuildWorld());
            yield break;
        }
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
            yield return null;
        }
        fpc.transform.position = new Vector3(worldData.fpcX,worldData.fpcY+1,worldData.fpcZ);
        mCamera.SetActive(false);
        fpc.SetActive(true);
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
    IEnumerator BuildRecursiveWolrd(int x, int z, int rad)
    {
        int nextrad = rad - 1;
        if(rad <=0) yield break;
        StartCoroutine(BuildChunkColumn(x,z + chunkDimensions.z));
        buildQueue.Enqueue(BuildRecursiveWolrd(x,z + chunkDimensions.z,nextrad));
        yield return null;

        StartCoroutine(BuildChunkColumn(x, z - chunkDimensions.z));
        buildQueue.Enqueue(BuildRecursiveWolrd(x, z - chunkDimensions.z, nextrad));
        yield return null;
        StartCoroutine(BuildChunkColumn(x + chunkDimensions.x, z));
        buildQueue.Enqueue(BuildRecursiveWolrd(x + chunkDimensions.x, z, nextrad));
        yield return null;
        StartCoroutine(BuildChunkColumn(x - chunkDimensions.x, z));
        buildQueue.Enqueue(BuildRecursiveWolrd(x - chunkDimensions.x, z, nextrad));
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
                buildQueue.Enqueue(BuildRecursiveWolrd(posx,posz,drawRadius));
                buildQueue.Enqueue(HideCollums(posx,posz));
            }

            yield return waitForSeconds;
        }
    }

}
