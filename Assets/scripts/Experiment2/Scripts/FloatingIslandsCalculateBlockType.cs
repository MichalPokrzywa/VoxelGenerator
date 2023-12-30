/**
 * @file FloatingIslandsCalculateBlockType.cs
 * @brief Defines a class for calculating block types in a floating islands scenario in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/**
 * @class FloatingIslandsCalculateBlockType
 * @brief Manages the calculation of block types for floating islands in a Unity environment.
 * @extends CalculateBlockTypesJobs
 */
public class FloatingIslandsCalculateBlockType : CalculateBlockTypesJobs
{
    /**
     * @brief Assigns values to the generation job for calculating block types.
     * @param chunkData NativeArray of MeshUtils.BlockType representing the chunk's block data.
     * @param width Width of the chunk.
     * @param height Height of the chunk.
     * @param location World position of the chunk.
     * @param randoms NativeArray of Unity.Mathematics.Random for generating random values.
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
            function = 1,
        };
    }
}