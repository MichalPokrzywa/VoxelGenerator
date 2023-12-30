/**
 * @file BaseCalculateBlockType.cs
 * @brief Defines a class for calculating block types in a chunk.
 */

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/**
 * @class BaseCalculateBlockType
 * @brief Calculates block types in a chunk.
 */
public class BaseCalculateBlockType : CalculateBlockTypesJobs
{
    /**
     * @brief Assigns values for block type calculation.
     * @param chunkData The native array containing block types for the chunk.
     * @param width The width of the chunk.
     * @param height The height of the chunk.
     * @param location The location of the chunk.
     * @param randoms The native array of random numbers.
     */
    public override void AssignValues(NativeArray<MeshUtils.BlockType> chunkData, int width, int height, Vector3 location, NativeArray<Random> randoms)
    {
        generationJob = new CalculateBlockTypes()
        {
            chunkData = chunkData,
            width = width,
            height = height,
            location = location,
            randoms = randoms,
            function = 0,
        };
    }
}