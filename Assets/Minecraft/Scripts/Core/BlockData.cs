using System;
using Assets.Scripts;

// ReSharper disable CheckNamespace

[Serializable]
 public class BlockData
{
    public BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] b)
    {
        this.matrix = new BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        for (int y = 0; y < World.chunkSize; y++)
        for (int x = 0; x < World.chunkSize; x++)
        {
            this.matrix[x, y, z] = b[x, y, z].Type;
        }
    }
}