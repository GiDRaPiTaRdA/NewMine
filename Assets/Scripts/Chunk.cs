using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;
	public enum ChunkStatus {DRAW,DONE,KEEP};
	public ChunkStatus status;
	public float touchedTime;

	void BuildChunk()
	{
	    this.touchedTime = Time.time;
	    this.chunkData = new Block[World.chunkSize,World.chunkSize,World.chunkSize];
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					Vector3 pos = new Vector3(x,y,z);
					int worldX = (int)(x + this.chunk.transform.position.x);
					int worldY = (int)(y + this.chunk.transform.position.y);
					int worldZ = (int)(z + this.chunk.transform.position.z);
					int surfaceHeight = Utils.GenerateHeight(worldX,worldZ);
					
					if(Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
					    this.chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, this.chunk.gameObject, this);
					else if(worldY == 0)
					    this.chunkData[x,y,z] = new Block(Block.BlockType.BEDROCK, pos, this.chunk.gameObject, this);
					else if(worldY <= Utils.GenerateStoneHeight(worldX,worldZ))
					{
						if(Utils.fBM3D(worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY < 40)
						    this.chunkData[x,y,z] = new Block(Block.BlockType.DIAMOND, pos, this.chunk.gameObject, this);
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20)
						    this.chunkData[x,y,z] = new Block(Block.BlockType.REDSTONE, pos, this.chunk.gameObject, this);
						else
						    this.chunkData[x,y,z] = new Block(Block.BlockType.STONE, pos, this.chunk.gameObject, this);
					}
					else if(worldY == surfaceHeight)
					{
					    this.chunkData[x,y,z] = new Block(Block.BlockType.GRASS, pos, this.chunk.gameObject, this);
					}
					else if(worldY < surfaceHeight)
					    this.chunkData[x,y,z] = new Block(Block.BlockType.DIRT, pos, this.chunk.gameObject, this);
					else
					{
					    this.chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, this.chunk.gameObject, this);
					}

				    this.status = ChunkStatus.DRAW;

				}

	}

	public void DrawChunk()
	{
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
				    this.chunkData[x,y,z].Draw();
				}

	    this.CombineQuads();
		MeshCollider collider = this.chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = this.chunk.transform.GetComponent<MeshFilter>().mesh;
	    this.status = ChunkStatus.DONE;
	}

	public Chunk(){}
	// Use this for initialization
	public Chunk (Vector3 position, Material c) {
	    this.chunk = new GameObject(position.ToString());
	    this.chunk.transform.position = position;
	    this.cubeMaterial = c;
	    this.BuildChunk();
	}

	
	public void CombineQuads()
	{
		//1. Combine all children meshes
		MeshFilter[] meshFilters = this.chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter) this.chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
		MeshRenderer renderer = this.chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = this.cubeMaterial;

		//5. Delete all uncombined children
		foreach (Transform quad in this.chunk.transform) {
     		Object.Destroy(quad.gameObject);
 		}

	}

}
