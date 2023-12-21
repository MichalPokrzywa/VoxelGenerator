using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FloatingIslandsCalculateBlockType : CalculateBlockTypesJobs
{
    public override void AssignValues(NativeArray<MeshUtils.BlockType> chunkData, int width, int height, Vector3 location, NativeArray<Random> randoms)
    {
        generationJob = new CalculateBlockTypes()
        {
            chunkData = chunkData,
            width = width,
            height = height,
            location = location,
            randoms = randoms,
            function = 1,
        };
    }
}
