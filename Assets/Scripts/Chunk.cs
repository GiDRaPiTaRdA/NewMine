using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;

	void BuildChunk()
	{
		chunkData = new Block[World.chunkSize,World.chunkSize,World.chunkSize];

		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					Vector3 pos = new Vector3(x,y,z);
					int worldX = (int)(x + chunk.transform.position.x);
					int worldY = (int)(y + chunk.transform.position.y);
					int worldZ = (int)(z + chunk.transform.position.z);
					
					
					// BEDROCK
					if(worldY==0 || (worldY < 4 && Utils.fBM3D(worldX, worldY, worldZ, 0.7f, 2) < 0.43f))
						chunkData[x,y,z] = new Block(Block.BlockType.BEDROCK, pos, 
						                chunk.gameObject, this);

				
					// STONE
					else if(worldY <= Utils.GenerateStoneHeight(worldX,worldZ))
					{
						// DIAMOND
						if(Utils.fBM3D(worldX, worldY, worldZ, 0.27f, 4) < 0.34f && worldY < 17)
							chunkData[x,y,z] = new Block(Block.BlockType.DIAMOND, pos, 
						                chunk.gameObject, this);

							/*if(Utils.fBM3D(worldX, worldY, worldZ, 0.27f, 4) < 0.35f && worldY < 17)*/

						// REDSTONE
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.3f, 1) < 0.25f && worldY < 35)
							chunkData[x,y,z] = new Block(Block.BlockType.REDSTONE, pos, 
						                chunk.gameObject, this);

						// COAL
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.3f, 1) < 0.3f && worldY > 20)
							chunkData[x,y,z] = new Block(Block.BlockType.COAL, pos, 
						                chunk.gameObject, this);

						// IRON
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.2f, 3) < 0.35f && worldY > 10)
							chunkData[x,y,z] = new Block(Block.BlockType.IRON, pos, 
						                chunk.gameObject, this);

						// GOLD
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.2f, 2) < 0.33f  && worldY < 30)
							chunkData[x,y,z] = new Block(Block.BlockType.GOLD, pos, 
						                chunk.gameObject, this);
						// CAVES
						else if(Utils.fBM3D(worldX, worldY, worldZ, 0.1f, 2) < 0.415f)
						chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, 
						                chunk.gameObject, this);

						//STONE
						else
							chunkData[x,y,z] = new Block(Block.BlockType.STONE, pos, 
						                chunk.gameObject, this);
					}
					// GRASS
					else if(worldY == Utils.GenerateDirtHeight(worldX,worldZ))
						chunkData[x,y,z] = new Block(Block.BlockType.GRASS, pos, 
						                chunk.gameObject, this);
					// DIRT
					else if(worldY < Utils.GenerateDirtHeight(worldX,worldZ))
						chunkData[x,y,z] = new Block(Block.BlockType.DIRT, pos, 
						                chunk.gameObject, this);
					// AIR
					else
						chunkData[x,y,z] = new Block(Block.BlockType.AIR, pos, 
						                chunk.gameObject, this);
				}
	}

	public void DrawChunk()
	{
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++)
				{
					chunkData[x,y,z].Draw();	
				}
		CombineQuads();

		MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;

	}

	// Use this for initialization
	public Chunk (Vector3 position, Material c) {
		
		chunk = new GameObject(World.BuildChunkName(position));
		chunk.transform.position = position;
		cubeMaterial = c;
		BuildChunk();
	}
	
	void CombineQuads()
	{
		//1. Combine all children meshes
		MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter) chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
		MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = cubeMaterial;

		//5. Delete all uncombined children
		foreach (Transform quad in chunk.transform) {
     		GameObject.Destroy(quad.gameObject);
 		}

	}

}
