using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Assets;
using Assets.Scripts;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SaveManager
{
    public string SelectedSave { get; set; } = null;

    public string SelectedSaveFullPath => this.GetSavePath(this.SelectedSave);

    public string Extention { get; } = "save";

    public string DirPath { get; } = Application.persistentDataPath + "/saves";

    public string GetSavePath(string saveName) => $"{this.DirPath}/{saveName}.{this.Extention}";

    private static SaveManager instance;
    public static SaveManager Instance => instance ?? (instance = new SaveManager());

    //// SAVE
    public void SaveData(StaticWorld world) => this.SaveData(world, this.SelectedSaveFullPath);
    public void SaveData(StaticWorld world, string savePath)
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

        this.SaveData(data, savePath);

        Debug.Log("Save " + s.ElapsedMilliseconds);
    }
    public void SaveData(WorldData data, string savePath)
    {
        if (!Directory.Exists(this.DirPath))
            Directory.CreateDirectory(this.DirPath);

        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            XmlSerializer binaryFormatter = new XmlSerializer(typeof(WorldData));
            binaryFormatter.Serialize(stream, data);
        }
    }

    //// LOAD
    public WorldData LoadData()=> this.LoadData(this.SelectedSaveFullPath);
    public WorldData LoadData(string savePath)
    {
        WorldData data = null;

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

    //// LOAD MANY
    public string[] LoadSaves() => this.LoadSaves(this.DirPath);
    public string[] LoadSaves(string dir)
    {
        string[] saves = null;


        if (Directory.Exists(dir))
        {
            //$".{this.Extention}"
            saves = Directory.GetFiles(dir+"/").Select(Path.GetFileNameWithoutExtension).ToArray();
        }
        else
        {
            Debug.LogError("No directory " + dir);
        }

        return saves;
    }
}

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



