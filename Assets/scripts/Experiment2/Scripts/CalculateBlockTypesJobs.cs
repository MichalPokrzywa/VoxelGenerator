/**
 * @file CalculateBlockTypesJobs.cs
 * @brief Defines the CalculateBlockTypesJobs and CalculateBlockTypes classes for block generation.
 */

using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/**
 * @class CalculateBlockTypesJobs
 * @brief Manages the block generation job and assigns values to the generation job instance.
 */
public class CalculateBlockTypesJobs : MonoBehaviour
{
    /** Instance of the CalculateBlockTypes class for block generation. */
    public CalculateBlockTypes generationJob;

    /**
     * @brief Assigns values to the generation job instance.
     * @param chunkData Native array to store block types.
     * @param width Width of the chunk.
     * @param height Height of the chunk.
     * @param location Location of the chunk in the world.
     * @param randoms Array of random numbers for block generation.
     */
    public virtual void AssignValues(NativeArray<MeshUtils.BlockType> chunkData, int width, int height, Vector3 location, NativeArray<Random> randoms)
    {
        generationJob = new CalculateBlockTypes()
        {
            chunkData = chunkData,
            width = width,
            height = height,
            location = location,
            randoms = randoms,
            function = 0
        };
    }
}

/**
 * @struct CalculateBlockTypes
 * @brief Job structure for parallel block generation.
 */
[Serializable]
public struct CalculateBlockTypes : IJobParallelFor
{
    /** Native array to store block types. */
    public NativeArray<MeshUtils.BlockType> chunkData;

    /** Width of the chunk. */
    public int width;

    /** Height of the chunk. */
    public int height;

    /** Location of the chunk in the world. */
    public Vector3 location;

    /** Array of random numbers for block generation. */
    public NativeArray<Unity.Mathematics.Random> randoms;

    /** Function identifier for block generation. */
    public int function;

    /**
     * @brief Executes the block generation job in parallel for each block.
     * @param i Index of the block.
     */
    public void Execute(int i)
    {
        switch (function)
        {
            case 0:
            {
                BaseGenerator(i);
                break;
            }
            case 1:
            {
                IslandGenerator(i);
                break;
            }
            case 2:
            {
                FlatGenerator(i);
                break;
            }
        }
    }
    /**
     * @brief Generates blocks based on base terrain settings.
     * @param i Index of the block.
     */
    public void BaseGenerator(int i)
    {
        int x = i % width + (int)location.x;
        int y = (i / width) % height + (int)location.y;
        int z = i / (width * height) + (int)location.z;
        
        var random = randoms[i];

        float surfaceHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[0].octaves,
            WorldCreator.worldVisualization.perlinSettings[0].scale,
            WorldCreator.worldVisualization.perlinSettings[0].heightScale, WorldCreator.worldVisualization.perlinSettings[0].heightOffset);
        float stoneHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[1].octaves,
            WorldCreator.worldVisualization.perlinSettings[1].scale,
            WorldCreator.worldVisualization.perlinSettings[1].heightScale, WorldCreator.worldVisualization.perlinSettings[1].heightOffset);

        int diamondTHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[2].octaves,
            WorldCreator.worldVisualization.perlinSettings[2].scale,
            WorldCreator.worldVisualization.perlinSettings[2].heightScale, WorldCreator.worldVisualization.perlinSettings[2].heightOffset);

        int diamondDHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[3].octaves,
            WorldCreator.worldVisualization.perlinSettings[3].scale,
            WorldCreator.worldVisualization.perlinSettings[3].heightScale, WorldCreator.worldVisualization.perlinSettings[3].heightOffset);

        int digCave = (int)MeshUtils.fBM3D(x, y, z, WorldCreator.worldVisualization.perlinSettings[4].octaves,
            WorldCreator.worldVisualization.perlinSettings[4].scale,
            WorldCreator.worldVisualization.perlinSettings[4].heightScale, WorldCreator.worldVisualization.perlinSettings[4].heightOffset);

        if (y == 0)
        {
            chunkData[i] = MeshUtils.BlockType.BEDROCK;
            return;
        }

        if (WorldCreator.useCaves && digCave < WorldCreator.worldVisualization.perlinSettings[4].probability)
        {
            chunkData[i] = MeshUtils.BlockType.AIR;
            return;
        }

        if (surfaceHeight == y)
            chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
        else if (y < diamondTHeight && y > diamondDHeight &&
                 random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[2].probability)
        {
            chunkData[i] = MeshUtils.BlockType.DIAMOND;
        }
        else if (stoneHeight > y && random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[1].probability)
            chunkData[i] = MeshUtils.BlockType.STONE;
        else if (surfaceHeight > y)
            chunkData[i] = MeshUtils.BlockType.DIRT;
        else
            chunkData[i] = MeshUtils.BlockType.AIR;
    }
    /**
     * @brief Generates blocks for an island terrain.
     * @param i Index of the block.
     */
    public void IslandGenerator(int i)
    {
        int x = i % width + (int)location.x;
        int y = (i / width) % height + (int)location.y;
        int z = i / (width * height) + (int)location.z;

        var random = randoms[i];
        float surfaceHeight1 = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[0].octaves,
            WorldCreator.worldVisualization.perlinSettings[0].scale,
            WorldCreator.worldVisualization.perlinSettings[0].heightScale, WorldCreator.worldVisualization.perlinSettings[0].heightOffset);
        float stoneHeight1 = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[1].octaves,
            WorldCreator.worldVisualization.perlinSettings[1].scale,
            WorldCreator.worldVisualization.perlinSettings[1].heightScale, WorldCreator.worldVisualization.perlinSettings[1].heightOffset);

        int bottomLine1 = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[2].octaves,
            WorldCreator.worldVisualization.perlinSettings[2].scale,
            WorldCreator.worldVisualization.perlinSettings[2].heightScale, WorldCreator.worldVisualization.perlinSettings[2].heightOffset);

        int surfaceHeight2 = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[3].octaves,
            WorldCreator.worldVisualization.perlinSettings[3].scale,
            WorldCreator.worldVisualization.perlinSettings[3].heightScale, WorldCreator.worldVisualization.perlinSettings[3].heightOffset);

        int stoneHeight2 = (int)MeshUtils.fBM3D(x, y, z, WorldCreator.worldVisualization.perlinSettings[4].octaves,
            WorldCreator.worldVisualization.perlinSettings[4].scale,
            WorldCreator.worldVisualization.perlinSettings[4].heightScale, WorldCreator.worldVisualization.perlinSettings[4].heightOffset);

        int bottomLine2 = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[5].octaves,
            WorldCreator.worldVisualization.perlinSettings[5].scale,
            WorldCreator.worldVisualization.perlinSettings[5].heightScale, WorldCreator.worldVisualization.perlinSettings[5].heightOffset);

        int digCave = (int)MeshUtils.fBM3D(x, y, z, WorldCreator.worldVisualization.perlinSettings[6].octaves,
            WorldCreator.worldVisualization.perlinSettings[6].scale,
            WorldCreator.worldVisualization.perlinSettings[6].heightScale, WorldCreator.worldVisualization.perlinSettings[6].heightOffset);

        digCave = WorldCreator.useCaves ? digCave : (int)WorldCreator.worldVisualization.perlinSettings[6].probability + 1; 


        if (y <= surfaceHeight2 && bottomLine2 >= y)
        {
            if (surfaceHeight2 == y && surfaceHeight2 <= bottomLine2)
                chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
            else if (y < surfaceHeight2 && y > stoneHeight2  &&
                     digCave > WorldCreator.worldVisualization.perlinSettings[6].probability)
                chunkData[i] = MeshUtils.BlockType.DIRT;
            else if (y == stoneHeight2)
                chunkData[i] = MeshUtils.BlockType.STONE;
            else if (y < stoneHeight2  &&
                     digCave > WorldCreator.worldVisualization.perlinSettings[6].probability)
                chunkData[i] = MeshUtils.BlockType.STONE;
            else
                chunkData[i] = MeshUtils.BlockType.AIR;

        }
        else if (y <= surfaceHeight1 && bottomLine1 >= y)
        {
            if (surfaceHeight1 == y && surfaceHeight1 <= bottomLine1)
                chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
            else if (y < surfaceHeight1 && y > stoneHeight1)
                chunkData[i] = MeshUtils.BlockType.DIRT;
            else if (y == stoneHeight1)
                chunkData[i] = MeshUtils.BlockType.STONE;
            else if (y < stoneHeight1 && y > bottomLine1&&
                     digCave > WorldCreator.worldVisualization.perlinSettings[6].probability)
                chunkData[i] = MeshUtils.BlockType.STONE;
            else
                chunkData[i] = MeshUtils.BlockType.AIR;
        }
        else
            chunkData[i] = MeshUtils.BlockType.AIR;

    }
    /**
     * @brief Generates blocks for a flat terrain.
     * @param i Index of the block.
     */
    public void FlatGenerator(int i)
    {
        int x = i % width + (int)location.x;
        int y = (i / width) % height + (int)location.y;
        int z = i / (width * height) + (int)location.z;

        var random = randoms[i];

        float surfaceHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[0].octaves,
            WorldCreator.worldVisualization.perlinSettings[0].scale,
            WorldCreator.worldVisualization.perlinSettings[0].heightScale, WorldCreator.worldVisualization.perlinSettings[0].heightOffset);
        float stoneHeight = (int)MeshUtils.fBM(x, z, WorldCreator.worldVisualization.perlinSettings[1].octaves,
            WorldCreator.worldVisualization.perlinSettings[1].scale,
            WorldCreator.worldVisualization.perlinSettings[1].heightScale, WorldCreator.worldVisualization.perlinSettings[1].heightOffset);
        int digCave = (int)MeshUtils.fBM3D(x, y, z, WorldCreator.worldVisualization.perlinSettings[2].octaves,
            WorldCreator.worldVisualization.perlinSettings[2].scale,
            WorldCreator.worldVisualization.perlinSettings[2].heightScale, WorldCreator.worldVisualization.perlinSettings[2].heightOffset);

        if (y == 0)
        {
            chunkData[i] = MeshUtils.BlockType.BEDROCK;
            return;
        }

        if (WorldCreator.useCaves && digCave < WorldCreator.worldVisualization.perlinSettings[2].probability)
        {
            chunkData[i] = MeshUtils.BlockType.AIR;
            return;
        }

        if (surfaceHeight == y)
            chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
        else if (stoneHeight > y && random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[1].probability)
            chunkData[i] = MeshUtils.BlockType.STONE;
        else if (surfaceHeight > y)
            chunkData[i] = MeshUtils.BlockType.DIRT;
        else
            chunkData[i] = MeshUtils.BlockType.AIR;
    }
}