using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public Mesh mesh;
    ChunkBlock parentChunk;

    public Block(Vector3 offset, MeshUtils.BlockType type, ChunkBlock chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.location;
        if (type != MeshUtils.BlockType.AIR)
        {
            List<Quad> quads = new List<Quad>();
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
            {
                if (type == MeshUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, MeshUtils.BlockType.DIRT));
                else
                    quads.Add(new Quad(MeshUtils.BlockSide.BOTTOM, offset, type));
            }

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z))
            {
                if(type == MeshUtils.BlockType.GRASSSIDE)
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, MeshUtils.BlockType.GRASSTOP));
                else
                    quads.Add(new Quad(MeshUtils.BlockSide.TOP, offset, type));
            }

            if (!HasSolidNeighbour((int)blockLocalPosition.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshUtils.BlockSide.LEFT, offset, type));
            if (!HasSolidNeighbour((int)blockLocalPosition.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshUtils.BlockSide.RIGHT, offset, type));
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1))
                quads.Add(new Quad(MeshUtils.BlockSide.FRONT, offset, type));
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))
                quads.Add(new Quad(MeshUtils.BlockSide.BACK, offset, type));

            if (quads.Count == 0) return;

            Mesh[] sideMeshes = new Mesh[quads.Count];
            int m = 0;
            foreach (Quad q in quads)
            {
                sideMeshes[m] = q.mesh;
                m++;
            }

            mesh = MeshUtils.MergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }


    public bool HasSolidNeighbour(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.width ||
            y < 0 || y >= parentChunk.height ||
            z < 0 || z >= parentChunk.depth)
        {
            return false;
        }
        if(parentChunk.cData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.AIR
            || parentChunk.cData[x + parentChunk.width * (y + parentChunk.depth * z)] == MeshUtils.BlockType.WATER)
            return false;
        return true;
    }
}
