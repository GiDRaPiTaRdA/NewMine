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


public class Chunk
{
    public ChunkData ChunkData { get; set; }
    public GameObject ChunkGameObject { get; set; }
    public Transform ChunkTransform => this.ChunkGameObject.transform;

    public Vector3 Position { get; set; }

    public List<BlockQuad> MeshFiltersBlock { get; set; }


    // private BlockData bd;

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
    //    string chunkFile = BuildChunkFileName(this.chunkGameObject.transform.position);
    //    if (File.Exists(chunkFile))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();

    //        using (FileStream file = File.Open(chunkFile, FileMode.Open))
    //        {
    //            this.bd = new BlockData();
    //            this.bd = (BlockData)bf.Deserialize(file);  
    //        }

    //        return true;
    //        //Debug.Log("Loading chunkGameObject from file: " + chunkFile);

    //    }
    //    return false;
    //}

    //public void Save() //write data to file
    //{
    //    string chunkFile = BuildChunkFileName(this.chunkGameObject.transform.position);;

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

    //    //Debug.Log("Saving chunkGameObject from file: " + chunkFile);
    //}

    private void BuildChunk()
    {
        this.ChunkData = new ChunkData(new Block[World.chunkSize, World.chunkSize, World.chunkSize]);
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + this.ChunkTransform.position.x);
                    int worldY = (int)(y + this.ChunkTransform.position.y);
                    int worldZ = (int)(z + this.ChunkTransform.position.z);
                    int surfaceHeight = Utils.GenerateHeight(worldX, worldZ);
                    //int surfaceHeight = StaticWorld.columnHeight * World.chunkSize;

                    Transform chunkTransform = this.ChunkGameObject.gameObject.transform;

                    if (worldY == 0)
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.DIRT, pos, this);
                    }
                    else if (Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.AIR, pos, this);
                    }

                    else if (worldY <= Utils.GenerateStoneHeight(worldX, worldZ))
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.STONE, pos, this);
                    }
                    else if (worldY == surfaceHeight)
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.GRASS, pos, this);
                    }
                    else if (worldY < surfaceHeight)
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.DIRT, pos, this);
                    }
                    else
                    {
                        this.ChunkData[x, y, z] = new Block(BlockType.AIR, pos, this);
                    }
                }

    }

    public void DrawChunk()
    {
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    this.ChunkData[x, y, z].Draw();
                }

        this.CombineQuads();

        // Update collider
        MeshCollider collider = this.ChunkGameObject.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = this.ChunkGameObject.transform.GetComponent<MeshFilter>().mesh;
    }

    // Use this for initialization
    public Chunk(Vector3 position)
    {
        this.ChunkGameObject = new GameObject(position.ToString());
        this.ChunkGameObject.transform.position = position;
        this.Position = position;
        this.MeshFiltersBlock = new List<BlockQuad>();
        this.BuildChunk();
    }

    public void CombineQuads()
    {
        //1. Combine all children meshes
        Mesh mesh = this.ReMeshBase(this.MeshFiltersBlock.ToArray(), out Material[] issuedMaterials);

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)this.ChunkGameObject.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = mesh;

        //4. Create a renderer for the parent
        MeshRenderer renderer = this.ChunkGameObject.gameObject.AddComponent<MeshRenderer>();
        renderer.materials = issuedMaterials;

        ////5. Delete all uncombined children
        foreach (Transform quad in this.ChunkGameObject.transform)
        {
            quad.gameObject.SetActive(false);
        }

    }

    public Mesh ReMeshBase(BlockQuad[] blockQuads, out Material[] issuedMaterials)
    {
        // MaterilaName/ Array of material corresponding meshFilters
        Dictionary<string, MeshCombines> combineinstanceNew = new Dictionary<string, MeshCombines>();

        // Enumerate Block Quads to fill combine meshes
        {
            foreach (BlockQuad blockQuad in blockQuads)
            {
                CombineInstance instance = default;

                instance.mesh = blockQuad.MeshFilter.sharedMesh;
                instance.transform = blockQuad.MeshFilter.transform.localToWorldMatrix;

                CubeDescription cubeDescription = StaticWorld.Instance.CubeDescriptions[blockQuad.BlockType];
                Material material = cubeDescription.CubeContent[blockQuad.Cubeside];

                string key = material.name;

                if (!combineinstanceNew.ContainsKey(key))
                {
                    combineinstanceNew.Add(key, new MeshCombines { Materials = new List<Material>() });
                }

                combineinstanceNew[key].Materials.Add(material);
                combineinstanceNew[key].Add(instance);
            }
        }


        Mesh[] meshes = new Mesh[combineinstanceNew.Count];

        HashSet<Material> materialsHashSet = new HashSet<Material>();


        // Enumerate dictionary to define materials and mehses
        {
            for (int i = 0; i < combineinstanceNew.Count; i++)
            {
                MeshCombines tempColl = combineinstanceNew.ElementAt(i).Value;

                foreach (Material material in tempColl.Materials)
                {
                    materialsHashSet.Add(material);
                }

                Mesh mesh = new Mesh();
                mesh.CombineMeshes(tempColl.ToArray(), true, true);
                meshes[i] = mesh;
            }
        }

        issuedMaterials = materialsHashSet.ToArray();

        return Combine(meshes.ToArray());
    }

    private static Mesh Combine(Mesh[] meshes)
    {
        CombineInstance[] combineInstances = new CombineInstance[meshes.Length];

        for (int index = 0; index < meshes.Length; index++)
        {
            combineInstances[index] = new CombineInstance
            {
                subMeshIndex = 0,
                mesh = meshes[index]
            };
        }

        Mesh mainMesh = new Mesh();
        mainMesh.CombineMeshes(combineInstances, false, false);

        return mainMesh;
    }
    public IEnumerator RemoveBlock(Vector3 position)
    {
        Stopwatch s = Stopwatch.StartNew();

        Block removeBlock = this.ChunkData[position];

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
            Vector3 globalPos = vector3 + position + this.Position;

            if (this.IsInChunk(globalPos))
            {
                Vector3 localPos = vector3 + position;
                this.ChunkData[localPos].Draw();
            }
            else
            {
                Block b = StaticWorld.GetWorldBlock(globalPos);
                if (b != null)
                {
                    b.Draw();
                    b.Chunk.ReMeshFilter();
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

    public IEnumerator AddBlock(Vector3 position, BlockType blockType)
    {
        Stopwatch s = Stopwatch.StartNew();

        Block addBlock = StaticWorld.GetWorldBlock(position + this.Position);

        addBlock.SetType(blockType);

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
                this.ChunkData[localPos].Draw();
            }
            else
            {
                Block b = StaticWorld.GetWorldBlock(globalPos);
                if (b != null)
                {
                    b.Draw();
                    b.Chunk.ReMeshFilter();
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

    public void ReMeshFilter(List<BlockQuad> meshFilters = null)
    {
        if (meshFilters == null)
        {
             meshFilters = this.MeshFiltersBlock;
        }

        Mesh mesh = this.ReMeshBase(meshFilters.ToArray(), out Material[] issuedMaterials);

        this.ChunkGameObject.gameObject.GetComponent<MeshRenderer>().materials = issuedMaterials;

        this.ChunkGameObject.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        this.ChunkGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void ReDrawChunk()
    {
        Object.DestroyImmediate(this.ChunkGameObject.GetComponent<MeshFilter>());
        Object.DestroyImmediate(this.ChunkGameObject.GetComponent<MeshRenderer>());
        Object.DestroyImmediate(this.ChunkGameObject.GetComponent<Collider>());
        this.DrawChunk();
    }

}
