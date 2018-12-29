using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Assets;
using Assets.Scripts;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

public class Chunk
{

    public Material cubeMaterial;
    public ChunkData chunkData;
    public GameObject chunk;
    public Vector3 Position { get; set; }

    public List<MeshFilter> MeshFilters { get; set; }

   

    BlockData bd;

    //static string BuildChunkFileName(Vector3 v)
    //{
    //    return Application.persistentDataPath + "/savedata/Chunk_" +
    //           (int)v.x + "_" +
    //           (int)v.y + "_" +
    //           (int)v.z +
    //           "_" + World.chunkSize +
    //           "_" + DynamicWorld.radius +
    //           ".dat";
    //}

    //bool Load() //read data from file
    //{
    //    string chunkFile = BuildChunkFileName(this.chunk.transform.position);
    //    if (File.Exists(chunkFile))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();

    //        using (FileStream file = File.Open(chunkFile, FileMode.Open))
    //        {
    //            this.bd = new BlockData();
    //            this.bd = (BlockData)bf.Deserialize(file);  
    //        }

    //        return true;
    //        //Debug.Log("Loading chunk from file: " + chunkFile);

    //    }
    //    return false;
    //}

    //public void Save() //write data to file
    //{
    //    string chunkFile = BuildChunkFileName(this.chunk.transform.position);;

    //    if (!File.Exists(chunkFile))
    //    {
    //        Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
    //    }

    //    using (FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate))
    //    {
    //        this.bd = new BlockData(this.chunkData);

    //        BinaryFormatter bf = new BinaryFormatter();
    //        bf.Serialize(file, this.bd);
    //    }

    //    //Debug.Log("Saving chunk from file: " + chunkFile);
    //}

    void BuildChunk()
    {
        this.chunkData = new ChunkData(new Block[World.chunkSize, World.chunkSize, World.chunkSize]);
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + this.chunk.transform.position.x);
                    int worldY = (int)(y + this.chunk.transform.position.y);
                    int worldZ = (int)(z + this.chunk.transform.position.z);
                    int surfaceHeight = Utils.GenerateHeight(worldX, worldZ);
                    //int surfaceHeight = StaticWorld.columnHeight * World.chunkSize;

                    if (worldY == 0)
                        this.chunkData[x, y, z] = new Block(BlockType.BEDROCK, pos, this.chunk.gameObject, this);
                    else if (Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
                        this.chunkData[x, y, z] = new Block(BlockType.AIR, pos, this.chunk.gameObject, this);

                    else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        this.chunkData[x, y, z] = new Block(BlockType.STONE, pos, this.chunk.gameObject, this);
                    }
                    else if (worldY == surfaceHeight)
                    {
                        this.chunkData[x, y, z] = new Block(BlockType.GRASS, pos, this.chunk.gameObject, this);
                    }
                    else if (worldY < surfaceHeight)
                        this.chunkData[x, y, z] = new Block(BlockType.DIRT, pos, this.chunk.gameObject, this);
                    else
                    {
                        this.chunkData[x, y, z] = new Block(BlockType.AIR, pos, this.chunk.gameObject, this);
                    }
                }

    }

    public void DrawChunk()
    {
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    this.chunkData[x, y, z].Draw();
                }

        this.CombineQuads();
        MeshCollider collider = this.chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = this.chunk.transform.GetComponent<MeshFilter>().mesh;
    }

    // Use this for initialization
    public Chunk(Vector3 position, Material c)
    {
        this.chunk = new GameObject(position.ToString());
        this.chunk.transform.position = position;
        this.cubeMaterial = c;
        this.Position = position;
        this.MeshFilters = new List<MeshFilter>();
        this.BuildChunk();
    }


    public void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = this.chunk.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)this.chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = this.chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = this.cubeMaterial;

        ////5. Delete all uncombined children
        foreach (Transform quad in this.chunk.transform)
        {
            quad.gameObject.SetActive(false);
        }

    }

    public IEnumerator RemoveBlock(Vector3 position)
    {
        Stopwatch s = Stopwatch.StartNew();

        Block removeBlock = this.chunkData[position];

        removeBlock.SetType(BlockType.AIR);

        removeBlock.Draw();

        Vector3[] dVectors =
        {
            new Vector3(0,0,-1),
            new Vector3(0,0,1),
            new Vector3(0,-1,0),
            new Vector3(0,1,0),
            new Vector3(-1,0,0),
            new Vector3(1,0,0)
        };
        foreach (Vector3 vector3 in dVectors)
        {
            Vector3 globalPos = vector3 + position+this.Position;

            if (this.IsInChunk(globalPos))
            {
                Vector3 localPos = vector3 + position;
                this.chunkData[localPos].Draw();
            }
            else
            {
                Block b = StaticWorld.GetWorldBlock(globalPos);
                if (b != null)
                {
                    b.Draw();
                    b.owner.ReMeshFilter();
                }
                else
                {
                    Debug.Log("No such block to remove");
                }
            }
        }

        this.ReMeshFilter();


        s.Stop();
        //Debug.Log(s.ElapsedMilliseconds);

        yield return null;
    }

    public IEnumerator AddBlock(Vector3 position)
    {
        Stopwatch s = Stopwatch.StartNew();

        Block addBlock = StaticWorld.GetWorldBlock(position + this.Position);

        addBlock.SetType(BlockType.STONE);

        addBlock.Draw();

        Vector3[] dVectors =
        {
            new Vector3(0,0,-1),
            new Vector3(0,0,1),
            new Vector3(0,-1,0),
            new Vector3(0,1,0),
            new Vector3(-1,0,0),
            new Vector3(1,0,0)
        };
        foreach (Vector3 vector3 in dVectors)
        {
            Vector3 globalPos = vector3 + position + this.Position;

            if (this.IsInChunk(globalPos))
            {
                Vector3 localPos = vector3 + position;
                this.chunkData[localPos].Draw();
            }
            else
            {
                Block b = StaticWorld.GetWorldBlock(globalPos);
                if (b != null)
                {
                    b.Draw();
                    b.owner.ReMeshFilter();
                }
                else
                {
                    Debug.Log("No such block to remove");
                }
            }
        }

        this.ReMeshFilter();


        s.Stop();
        //Debug.Log(s.ElapsedMilliseconds);

        yield return null;
    }

    private bool IsInChunk(Vector3 vector)
    {
        return vector.x >= 0 + this.Position.x &&
               vector.y >= 0 + this.Position.y &&
               vector.z >= 0 + this.Position.z &&
               vector.x <= World.chunkSize - 1 + this.Position.x &&
               vector.y <= World.chunkSize - 1 + this.Position.y &&
               vector.z <= World.chunkSize - 1 + this.Position.z;
    }

    public void ReMeshFilter(List<MeshFilter> meshFilters)
    {

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }


        Object.DestroyImmediate(this.chunk.GetComponent<MeshFilter>());
        MeshFilter mf = (MeshFilter)this.chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);


        //Object.DestroyImmediate(this.chunk.GetComponent<MeshRenderer>());
        //MeshRenderer renderer = this.chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //renderer.material = this.cubeMaterial;


        //Object.DestroyImmediate(this.chunk.GetComponent<MeshCollider>());
        //MeshCollider collider = this.chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        this.chunk.GetComponent<MeshCollider>().sharedMesh = mf.mesh;


    }

    public void ReMeshFilter()
    {
        List<MeshFilter> meshFilters = this.MeshFilters;

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }


        Object.DestroyImmediate(this.chunk.GetComponent<MeshFilter>());
        MeshFilter mf = (MeshFilter)this.chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);


        //Object.DestroyImmediate(this.chunk.GetComponent<MeshRenderer>());
        //MeshRenderer renderer = this.chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //renderer.material = this.cubeMaterial;


        //Object.DestroyImmediate(this.chunk.GetComponent<MeshCollider>());
        //MeshCollider collider = this.chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        this.chunk.GetComponent<MeshCollider>().sharedMesh = mf.mesh;


    }

    public void ReDrawChunk()
    {
        Object.DestroyImmediate(this.chunk.GetComponent<MeshFilter>());
        Object.DestroyImmediate(this.chunk.GetComponent<MeshRenderer>());
        Object.DestroyImmediate(this.chunk.GetComponent<Collider>());
        this.DrawChunk();
    }

}
