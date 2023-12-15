using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CalculateBlockTypesJobs : MonoBehaviour,IJobParallelFor
{
    public NativeArray<MeshUtils.BlockType> chunkData;
    public int width;
    public int height;
    public Vector3 location;
    public NativeArray<Unity.Mathematics.Random> randoms;

    public virtual void Execute(int i)
    {
        //int x = i % width + (int)location.x;
        //int y = (i / width) % height + (int)location.y;
        //int z = i / (width * height) + (int)location.z;

        //var random = randoms[i];

        //float surfaceHeight = (int)MeshUtils.fBM(x, z, WorldCreator.surfaceSettings.octaves,
        //    WorldCreator.surfaceSettings.scale,
        //    WorldCreator.surfaceSettings.heightScale, WorldCreator.surfaceSettings.heightOffset);
        //float stoneHeight = (int)MeshUtils.fBM(x, z, WorldCreator.stoneSettings.octaves,
        //    WorldCreator.stoneSettings.scale,
        //    WorldCreator.stoneSettings.heightScale, WorldCreator.stoneSettings.heightOffset);

        //int diamondTHeight = (int)MeshUtils.fBM(x, z, WorldCreator.diamondTSettings.octaves,
        //    WorldCreator.diamondTSettings.scale,
        //    WorldCreator.diamondTSettings.heightScale, WorldCreator.diamondTSettings.heightOffset);

        //int diamondDHeight = (int)MeshUtils.fBM(x, z, WorldCreator.diamondDSettings.octaves,
        //    WorldCreator.diamondDSettings.scale,
        //    WorldCreator.diamondDSettings.heightScale, WorldCreator.diamondDSettings.heightOffset);

        //int digCave = (int)MeshUtils.fBM3D(x, y, z, WorldCreator.caveSettings.octaves,
        //    WorldCreator.caveSettings.scale,
        //    WorldCreator.caveSettings.heightScale, WorldCreator.caveSettings.heightOffset);

        //if (y == 0)
        //{
        //    chunkData[i] = MeshUtils.BlockType.BEDROCK;
        //    return;
        //}

        //if (digCave < WorldCreator.caveSettings.probability)
        //{
        //    chunkData[i] = MeshUtils.BlockType.AIR;
        //    return;
        //}

        //if (surfaceHeight == y)
        //    chunkData[i] = MeshUtils.BlockType.GRASSSIDE;
        //else if (y < diamondTHeight && y > diamondDHeight &&
        //         random.NextFloat(1) <= WorldCreator.diamondTSettings.probability)
        //{
        //    chunkData[i] = MeshUtils.BlockType.DIAMOND;
        //}
        //else if (stoneHeight > y && random.NextFloat(1) <= WorldCreator.stoneSettings.probability)
        //    chunkData[i] = MeshUtils.BlockType.STONE;
        //else if (surfaceHeight > y)
        //    chunkData[i] = MeshUtils.BlockType.DIRT;
        //else
        //    chunkData[i] = MeshUtils.BlockType.AIR;
    }
}
