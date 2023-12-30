/**
 * @file ChunkBlock.cs
 * @brief Defines the ChunkBlock class responsible for generating and managing chunks of blocks in the world.
 */

using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * @class ChunkBlock
 * @brief Manages the generation and rendering of chunks in the world.
 */
public class ChunkBlock : MonoBehaviour
{
    /** Material for the chunk's blocks. */
    public Material atlas;

    /** Width of the chunk. */
    public int width = 2;

    /** Height of the chunk. */
    public int height = 2;

    /** Depth of the chunk. */
    public int depth = 2;

    /** 3D array to store individual blocks in the chunk. */
    public Block[,,] blocks;

    /** Array to store block types for each position in the chunk. */
    public MeshUtils.BlockType[] cData;

    /** Mesh renderer for the chunk. */
    public MeshRenderer meshRenderer;

    /** Location of the chunk in the world. */
    public Vector3 location;

    /** Instance of the CalculateBlockTypes class. */
    CalculateBlockTypes calculateBlockTypes;

    /** Instance of the CalculateBlockTypesJobs class. */
    CalculateBlockTypesJobs calculateBlockTypesJobs;

    /** Job handle for parallel jobs. */
    JobHandle handle;

    /** Array of random numbers for block generation. */
    public NativeArray<Unity.Mathematics.Random> RandomArray { get; private set; }

    /**
     * @brief Builds the chunk by generating block types and initializing block instances.
     */
    void BuildChunk()
    {
        int blockCount = width * depth * height;
        cData = new MeshUtils.BlockType[blockCount];
        NativeArray<MeshUtils.BlockType> blockTypes = new NativeArray<MeshUtils.BlockType>(cData, Allocator.Persistent);

        var randomArray = new Unity.Mathematics.Random[blockCount];
        var seed = new System.Random();

        for (int i = 0; i < blockCount; ++i)
            randomArray[i] = new Unity.Mathematics.Random((uint)seed.Next());

        RandomArray = new NativeArray<Unity.Mathematics.Random>(randomArray, Allocator.Persistent);
        calculateBlockTypesJobs = WorldCreator.worldVisualization.calculate;
        calculateBlockTypesJobs.AssignValues(blockTypes, width, height, location, RandomArray);
        calculateBlockTypes = calculateBlockTypesJobs.generationJob;
        handle = calculateBlockTypes.Schedule(cData.Length, 64);
        handle.Complete();
        calculateBlockTypes.chunkData.CopyTo(cData);
        blockTypes.Dispose();
        RandomArray.Dispose();
    }

    /**
     * @brief Creates a chunk with the specified dimensions and position.
     * @param dimension The dimensions of the chunk.
     * @param position The position of the chunk in the world.
     * @param rebuildBlocks Whether to rebuild blocks for the chunk.
     */
    public void CreateChunk(Vector3 dimension, Vector3 position, bool rebuildBlocks = true)
    {
        location = position;
        width = (int)dimension.x;
        height = (int)dimension.y;
        depth = (int)dimension.z;

        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer = mr;
        mr.material = atlas;
        blocks = new Block[width, height, depth];
        if (rebuildBlocks) BuildChunk();

        var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = width * height * depth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + location, cData[x + width * (y + depth * z)], this);
                    if (blocks[x, y, z].mesh != null)
                    {
                        inputMeshes.Add(blocks[x, y, z].mesh);
                        var vcount = blocks[x, y, z].mesh.vertexCount;
                        var icount = (int)blocks[x, y, z].mesh.GetIndexCount(0);
                        jobs.vertexStart[m] = vertexStart;
                        jobs.triStart[m] = triStart;
                        vertexStart += vcount;
                        triStart += icount;
                        m++;
                    }
                }
            }
        }

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart,
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

        var handle = jobs.Schedule(inputMeshes.Count, 4);
        var newMesh = new Mesh();
        newMesh.name = "ChunkBlock_" + location.x + "_" + location.y + "_" + location.z;
        var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;

        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0, sm);
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });
        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triStart.Dispose();
        newMesh.RecalculateBounds();

        mf.mesh = newMesh;
        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mf.mesh;
    }

    /**
     * @struct ProcessMeshDataJob
     * @brief Job for processing mesh data in parallel.
     */
    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor
    {
        /** Read-only array of mesh data. */
        [ReadOnly] public Mesh.MeshDataArray meshData;

        /** Output mesh data. */
        public Mesh.MeshData outputMesh;

        /** Array to store the start index of each vertex in the output mesh. */
        public NativeArray<int> vertexStart;

        /** Array to store the start index of each triangle in the output mesh. */
        public NativeArray<int> triStart;

        /**
         * @brief Executes the job in parallel for each mesh.
         * @param index The index of the mesh data to process.
         */
        public void Execute(int index)
        {
            var data = meshData[index];
            var vCount = data.vertexCount;
            var vStart = vertexStart[index];

            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());

            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());

            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vCount; i++)
            {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
            }

            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            var tStart = triStart[index];
            var tCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();
            if (data.indexFormat == IndexFormat.UInt16)
            {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < tCount; ++i)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
            else
            {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < tCount; ++i)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
        }
    }
}
