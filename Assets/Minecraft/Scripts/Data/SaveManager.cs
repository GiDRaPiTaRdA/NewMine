using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Assets;
using Assets.Scripts;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SaveManager
{
    public string DirPath { get; } = Application.persistentDataPath + "/saves";

    public string GetSavePath(string saveName) => $"{this.DirPath}/{saveName}.save";

    private static SaveManager instance;
    public static SaveManager Instance => instance ?? (instance = new SaveManager());

    public void SaveData(StaticWorld world, string saveName)
    {
        Stopwatch s = Stopwatch.StartNew();

        Dictionary<Position, Chunk> chunks = world.Chunks;

        WorldData data = new WorldData(world.worldSize, world.columnHeight);

        foreach (KeyValuePair<Position, Chunk> keyValuePair in chunks)
        {
            Position position = keyValuePair.Key / World.chunkSize;

            Chunk chunk = keyValuePair.Value;

            ChData chunkData = data[position.X, position.Y, position.Z] = new ChData();

            for (int z = 0; z < World.chunkSize; z++)
                for (int y = 0; y < World.chunkSize; y++)
                    for (int x = 0; x < World.chunkSize; x++)
                    {
                        chunkData[x, y, z] = chunk.ChunkData.chunkData[x, y, z].Type;
                    }
        }

        Debug.Log("Parse " + s.ElapsedMilliseconds);

        this.SaveData(data, saveName);

        Debug.Log("Save " + s.ElapsedMilliseconds);
    }

    public void SaveData(WorldData data, string saveName)
    {
        if (!Directory.Exists(this.DirPath))
            Directory.CreateDirectory(this.DirPath);

        using (FileStream stream = new FileStream(this.GetSavePath(saveName), FileMode.Create))
        {
            XmlSerializer binaryFormatter = new XmlSerializer(typeof(WorldData));
            binaryFormatter.Serialize(stream, data);
        }
    }

    public WorldData LoadData(string saveName)
    {
        WorldData data = null;

        string savePath = this.GetSavePath(saveName);

        if (File.Exists(savePath))
        {
            using (FileStream stream = new FileStream(savePath, FileMode.Open))
            {
                XmlSerializer binaryFormatter = new XmlSerializer(typeof(WorldData));
                data = (WorldData)binaryFormatter.Deserialize(stream);
            }
        }
        else
        {
            Debug.LogError("Save file not fount in " + savePath);
        }

        return data;
    }
}

//public class SerializablePosition
//{
//    public SerializablePosition(Position position)
//    {
//        this.X = position.X;
//        this.Y = position.Y;
//        this.Z = position.Z;
//    }

//    public int X { get; set; }
//    public int Y { get; set; }
//    public int Z { get; set; }

//    public static implicit operator Position(SerializablePosition rValue)
//    {
//        return new Position(rValue.X, rValue.Y, rValue.Z);
//    }

//    public static implicit operator SerializablePosition(Position rValue)
//    {
//        return new SerializablePosition(rValue);
//    }
//}

[Serializable]
public class WorldData
{
    public WorldData() { }

    public WorldData(int worldSize, int worldColumnHeight)
    {
        this.WorldSize = worldSize;
        this.WorldColumnHeight = worldColumnHeight; 
        this.chunks = new ChData[this.WorldSize * this.WorldColumnHeight * this.WorldSize];
    }

    public int WorldSize { get; set; }

    public int WorldColumnHeight { get; set; }

    public ChData[] chunks;

    public ChData this[int x, int y, int z]
    {
        get => this.chunks[z + y * this.WorldSize + x * this.WorldColumnHeight * this.WorldSize];
        set => this.chunks[z + y * this.WorldSize + x * this.WorldColumnHeight * this.WorldSize] = value;
    }
}

[Serializable]
public class ChData
{
    public BlockType[] blocks;

    public ChData()
    {
        this.blocks = new BlockType[World.chunkSize * World.chunkSize * World.chunkSize];
    }

    public BlockType this[int x, int y, int z]
    {
        get => this.blocks[z + y * World.chunkSize + x * World.chunkSize * World.chunkSize];
        set => this.blocks[z + y * World.chunkSize + x * World.chunkSize * World.chunkSize] = value;
    }
}



