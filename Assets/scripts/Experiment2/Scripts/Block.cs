/**
 * @file Block.cs
 * @brief Defines a class for representing a block in a chunk.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Block
 * @brief Represents a block in a chunk.
 */
public class Block
{
    /// The mesh associated with the block.
    public Mesh mesh;

    /// Reference to the parent chunk.
    ChunkBlock parentChunk;

    /**
     * @brief Constructor for creating a block.
     * @param offset The offset of the block.
     * @param type The type of the block.
     * @param chunk The parent chunk of the block.
     */
    public Block(Vector3 offset, MeshUtils.BlockType type, ChunkBlock chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.location;

        // Check if the block is not AIR
        if (type != MeshUtils.BlockType.AIR)
        {
            List<Quad> quads = new List<Quad>();

            // Check for the bottom face
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
            {
                if (type == MeshUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, MeshUtils.BlockType.DIRT));
                else
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, type));
            }

            // Check for the top face
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z))
            {
                if (type == MeshUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, MeshUtils.BlockType.GRASSTOP));
                else
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, type));
            }

            // Check for the left face
            if (!HasSolidNeighbour((int)blockLocalPosition.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, type));

            // Check for the right face
            if (!HasSolidNeighbour((int)blockLocalPosition.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, type));

            // Check for the front face
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1))
                quads.Add(new Quad(MeshUtils.BlockSide.FRONT, offset, type));

            // Check for the back face
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))
                quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, type));

            if (quads.Count == 0) return;

            // Merge side meshes
            Mesh[] sideMeshes = new Mesh[quads.Count];
            int m = 0;
            foreach (Quad q in quads)
            {
                sideMeshes[m] = q.mesh;
                m++;
            }

            // Merge all meshes into a single mesh
            mesh = MeshUtils.MergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }

    /**
     * @brief Checks if there is a solid neighbor at the specified position.
     * @param x The x-coordinate.
     * @param y The y-coordinate.
     * @param z The z-coordinate.
     * @return True if there is a solid neighbor; false otherwise.
     */
    public bool HasSolidNeighbour(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.width ||
            y < 0 || y >= parentChunk.height ||
            z < 0 || z >= parentChunk.depth)
        {
            return false;
        }

        if (parentChunk.cData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR
            || parentChunk.cData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER)
            return false;

        return true;
    }
}
