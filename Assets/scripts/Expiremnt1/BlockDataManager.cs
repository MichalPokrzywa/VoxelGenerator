using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float textureOffset = 0.001f;
    public static float tileSizeX,tileSizeY;
    public static Dictionary<BlockType,TextureData> blockTextureDictionary = new Dictionary<BlockType,TextureData>();
    public BlockDataSO textureData;

    void Awake()
    {
        foreach (TextureData texture in textureData.textureDataList)
        {
            if (blockTextureDictionary.ContainsKey(texture.blockType) == false)
            {
                blockTextureDictionary.Add(texture.blockType, texture);
            }
        }
        tileSizeX = textureData.textureSizeX;
        tileSizeY = textureData.textureSizeY;
    }
}
