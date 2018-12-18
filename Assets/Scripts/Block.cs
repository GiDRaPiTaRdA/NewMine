using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

public class Block {

	enum Cubeside {BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK};
	public enum BlockType {GRASS, DIRT, STONE, BEDROCK, REDSTONE, DIAMOND, AIR};

	public BlockType bType;
	public bool isSolid;
    readonly Chunk owner;
    readonly GameObject parent;
	Vector3 position;

    readonly Vector2[,] blockUVs = { 
		/*GRASS TOP*/		{new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
								new Vector2( 0.125f, 0.4375f ),new Vector2( 0.1875f, 0.4375f )},
		/*GRASS SIDE*/		{new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
								new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
		/*DIRT*/			{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
								new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
		/*STONE*/			{new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
								new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},
		/*BEDROCK*/			{new Vector2( 0.3125f, 0.8125f ), new Vector2( 0.375f, 0.8125f),
								new Vector2( 0.3125f, 0.875f ),new Vector2( 0.375f, 0.875f )},
		/*REDSTONE*/		{new Vector2( 0.1875f, 0.75f ), new Vector2( 0.25f, 0.75f),
								new Vector2( 0.1875f, 0.8125f ),new Vector2( 0.25f, 0.8125f )},
		/*DIAMOND*/			{new Vector2( 0.125f, 0.75f ), new Vector2( 0.1875f, 0.75f),
								new Vector2( 0.125f, 0.8125f ),new Vector2( 0.1875f, 0.8125f )}
						}; 

	public Block(BlockType b, Vector3 pos, GameObject p, Chunk o)
	{
	    this.bType = b;
	    this.owner = o;
	    this.parent = p;
	    this.position = pos;

		if(this.bType == BlockType.AIR)
		    this.isSolid = false;
		else
		    this.isSolid = true;
	}

	public Block(BlockType b, Vector3 pos, GameObject p)
	{
	    this.bType = b;
	    this.parent = p;
	    this.position = pos;
		if(this.bType == BlockType.AIR)
		    this.isSolid = false;
		else
		    this.isSolid = true;
	}

	public void SetType(BlockType b)
	{
	    this.bType = b;
		if(this.bType == BlockType.AIR)
		    this.isSolid = false;
		else
		    this.isSolid = true;
	}

	void CreateQuad(Cubeside side)
	{
        Mesh mesh = new Mesh
        {
            name = "ScriptedMesh" + side
        };

        Vector3[] vertices = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		int[] triangles = new int[6];

		//all possible UVs
		Vector2 uv00;
		Vector2 uv10;
		Vector2 uv01;
		Vector2 uv11;

		if(this.bType == BlockType.GRASS && side == Cubeside.TOP)
		{
			uv00 = this.blockUVs[0,0];
			uv10 = this.blockUVs[0,1];
			uv01 = this.blockUVs[0,2];
			uv11 = this.blockUVs[0,3];
		}
		else if(this.bType == BlockType.GRASS && side == Cubeside.BOTTOM)
		{
			uv00 = this.blockUVs[(int)(BlockType.DIRT+1),0];
			uv10 = this.blockUVs[(int)(BlockType.DIRT+1),1];
			uv01 = this.blockUVs[(int)(BlockType.DIRT+1),2];
			uv11 = this.blockUVs[(int)(BlockType.DIRT+1),3];
		}
		else
		{
			uv00 = this.blockUVs[(int)(this.bType+1),0];
			uv10 = this.blockUVs[(int)(this.bType+1),1];
			uv01 = this.blockUVs[(int)(this.bType+1),2];
			uv11 = this.blockUVs[(int)(this.bType+1),3];
		}

		//all possible vertices 
		Vector3 p0 = new Vector3( -0.5f,  -0.5f,  0.5f );
		Vector3 p1 = new Vector3(  0.5f,  -0.5f,  0.5f );
		Vector3 p2 = new Vector3(  0.5f,  -0.5f, -0.5f );
		Vector3 p3 = new Vector3( -0.5f,  -0.5f, -0.5f );		 
		Vector3 p4 = new Vector3( -0.5f,   0.5f,  0.5f );
		Vector3 p5 = new Vector3(  0.5f,   0.5f,  0.5f );
		Vector3 p6 = new Vector3(  0.5f,   0.5f, -0.5f );
		Vector3 p7 = new Vector3( -0.5f,   0.5f, -0.5f );

		switch(side)
		{
			case Cubeside.BOTTOM:
				vertices = new[] {p0, p1, p2, p3};
				normals = new[] {Vector3.down, Vector3.down, 
											Vector3.down, Vector3.down};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] { 3, 1, 0, 3, 2, 1};
			break;
			case Cubeside.TOP:
				vertices = new[] {p7, p6, p5, p4};
				normals = new[] {Vector3.up, Vector3.up, 
											Vector3.up, Vector3.up};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] {3, 1, 0, 3, 2, 1};
			break;
			case Cubeside.LEFT:
				vertices = new[] {p7, p4, p0, p3};
				normals = new[] {Vector3.left, Vector3.left, 
											Vector3.left, Vector3.left};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] {3, 1, 0, 3, 2, 1};
			break;
			case Cubeside.RIGHT:
				vertices = new[] {p5, p6, p2, p1};
				normals = new[] {Vector3.right, Vector3.right, 
											Vector3.right, Vector3.right};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] {3, 1, 0, 3, 2, 1};
			break;
			case Cubeside.FRONT:
				vertices = new[] {p4, p5, p1, p0};
				normals = new[] {Vector3.forward, Vector3.forward, 
											Vector3.forward, Vector3.forward};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] {3, 1, 0, 3, 2, 1};
			break;
			case Cubeside.BACK:
				vertices = new[] {p6, p7, p3, p2};
				normals = new[] {Vector3.back, Vector3.back, 
											Vector3.back, Vector3.back};
				uvs = new[] {uv11, uv01, uv00, uv10};
				triangles = new[] {3, 1, 0, 3, 2, 1};
			break;
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		 
		mesh.RecalculateBounds();
		
		GameObject quad = new GameObject("Quad");
		quad.transform.position = this.position;
	    quad.transform.parent = this.parent.transform;

     	MeshFilter meshFilter = (MeshFilter) quad.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = mesh;

	}

	int ConvertBlockIndexToLocal(int i)
	{
		if(i == -1) 
			i = World.chunkSize-1; 
		else if(i == World.chunkSize) 
			i = 0;
		return i;
	}

	public bool HasSolidNeighbour(int x, int y, int z)
	{
		Block[,,] chunks;

		if(x < 0 || x >= World.chunkSize || 
		   y < 0 || y >= World.chunkSize ||
		   z < 0 || z >= World.chunkSize)
		{  //block in a neighbouring chunk
			
			Vector3 neighbourChunkPos = this.parent.transform.position + 
										new Vector3((x - (int) this.position.x)*World.chunkSize, 
											(y - (int) this.position.y)*World.chunkSize, 
											(z - (int) this.position.z)*World.chunkSize);

			x = this.ConvertBlockIndexToLocal(x);
			y = this.ConvertBlockIndexToLocal(y);
			z = this.ConvertBlockIndexToLocal(z);

		    if(World.chunks.TryGetValue(neighbourChunkPos, out Chunk nChunk))
			{
				chunks = nChunk.chunkData;
			}
			else
				return false;
		}  //block in this chunk
		else
			chunks = this.owner.chunkData;
		
		try
		{
			return chunks[x,y,z].isSolid;
		}
		catch(System.IndexOutOfRangeException){}

		return false;
	}

	public void Draw()
	{
		if(this.bType == BlockType.AIR) return;

		if(!this.HasSolidNeighbour((int) this.position.x,(int) this.position.y,(int) this.position.z + 1)) this.CreateQuad(Cubeside.FRONT);
		if(!this.HasSolidNeighbour((int) this.position.x,(int) this.position.y,(int) this.position.z - 1)) this.CreateQuad(Cubeside.BACK);
		if(!this.HasSolidNeighbour((int) this.position.x,(int) this.position.y + 1,(int) this.position.z)) this.CreateQuad(Cubeside.TOP);
		if(!this.HasSolidNeighbour((int) this.position.x,(int) this.position.y - 1,(int) this.position.z)) this.CreateQuad(Cubeside.BOTTOM);
		if(!this.HasSolidNeighbour((int) this.position.x - 1,(int) this.position.y,(int) this.position.z)) this.CreateQuad(Cubeside.LEFT);
		if(!this.HasSolidNeighbour((int) this.position.x + 1,(int) this.position.y,(int) this.position.z)) this.CreateQuad(Cubeside.RIGHT);
	}
}
