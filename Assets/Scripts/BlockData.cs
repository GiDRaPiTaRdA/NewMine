using System;
// ReSharper disable CheckNamespace

[Serializable]
 public class BlockData
{
    public Block.BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] b)
    {
        this.matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
        for (int y = 0; y < World.chunkSize; y++)
        for (int x = 0; x < World.chunkSize; x++)
        {
            this.matrix[x, y, z] = b[x, y, z].bType;
        }
    }
}