using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CalculateBlockTypesJobs : MonoBehaviour
{
    public CalculateBlockTypes jobParallelFor;
    public virtual void AssignValues(NativeArray<MeshUtils.BlockType> chunkData, int width, int height, Vector3 location, NativeArray<Random> randoms)
    {
        jobParallelFor = new CalculateBlockTypes()
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

public struct CalculateBlockTypes : IJobParallelFor
{
   
    public NativeArray<MeshUtils.BlockType> chunkData;
    public int width;
    public int height;
    public Vector3 location;
    public NativeArray<Unity.Mathematics.Random> randoms;
    public int function;

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

        if (digCave < WorldCreator.worldVisualization.perlinSettings[4].probability)
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

        if (y == 0)
        {
            chunkData[i] = MeshUtils.BlockType.BEDROCK;
            return;
        }

        //if (digCave < WorldCreator.worldVisualization.perlinSettings[4].probability)
        //{
        //    chunkData[i] = MeshUtils.BlockType.AIR;
        //    return;
        //}

        if (surfaceHeight1 == y)
            chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
        else if (surfaceHeight1 < y)
            chunkData[i] = MeshUtils.BlockType.DIRT;
        else if (stoneHeight1 > y && random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[1].probability)
            chunkData[i] = MeshUtils.BlockType.STONE;
        //else if (bottomLine1 > y && random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[1].probability)
        //    chunkData[i] = MeshUtils.BlockType.STONE;
        else if (surfaceHeight2 == y)
            chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
        else if (surfaceHeight2 > y)
            chunkData[i] = MeshUtils.BlockType.DIRT;
        else if (stoneHeight2 > y && random.NextFloat(1) <= WorldCreator.worldVisualization.perlinSettings[4].probability)
            chunkData[i] = MeshUtils.BlockType.STONE;
        else
            chunkData[i] = MeshUtils.BlockType.AIR;
    }

    public void FlatGenerator(int i)
    {

    }
}