using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FlatCalculateBlockType : CalculateBlockTypesJobs
{
    public override void AssignValues(NativeArray<MeshUtils.BlockType> chunkData, int width, int height, Vector3 location, NativeArray<Random> randoms)
    {
        jobParallelFor = new CalculateBlockTypes()
        {
            chunkData = chunkData,
            width = width,
            height = height,
            location = location,
            randoms = randoms,
            function = 2,
        };
    }
}
