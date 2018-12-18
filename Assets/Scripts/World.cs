using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class World : MonoBehaviour {

	public GameObject player;
	public Material textureAtlas;
	public static int columnHeight = 16;
	public static int chunkSize = 16;
	public static int worldSize = 1;
	public static int radius = 4;
	public static ConcurrentDictionary<string, Chunk> chunks;
	public static bool firstbuild = true;
	public static List<string> toRemove = new List<string>();

	CoroutineQueue queue;
	public static uint maxCoroutines = 2000;

	public Vector3 lastbuildPos;

	public static string BuildChunkName(Vector3 v)
	{
		return (int)v.x + "_" + 
			         (int)v.y + "_" + 
			         (int)v.z;
	}

	void BuildChunkAt(int x, int y, int z)
	{
		Vector3 chunkPosition = new Vector3(x*chunkSize, 
											y*chunkSize, 
											z*chunkSize);
					
		string n = BuildChunkName(chunkPosition);
		Chunk c;

		if(!chunks.TryGetValue(n, out c))
		{
			c = new Chunk(chunkPosition, textureAtlas);
			c.chunk.transform.parent = this.transform;
			chunks.TryAdd(c.chunk.name, c);
		}

	}

 IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad)
    {
        for (int iz = 0; iz < rad; iz++)
        {
            for (int iy = 0; iy < 2; iy++)
            {
                for (int ix = 0; ix < rad; ix++)
                {
                    if (Mathf.Pow(ix, 2) + Mathf.Pow(iy, 2) + Mathf.Pow(iz, 2) <= Mathf.Pow(radius, 2))
                    {

                        this.BuildChunkAt(ix + x, iy + y, iz + z);
                        this.BuildChunkAt(ix + x, iy + y, -iz + z);

                        this.BuildChunkAt(ix + x, -iy + y, iz + z);
                        this.BuildChunkAt(ix + x, -iy + y, -iz + z);

                        this.BuildChunkAt(-ix + x, iy + y, iz + z);
                        this.BuildChunkAt(-ix + x, iy + y, -iz + z);

                        this.BuildChunkAt(-ix + x, -iy + y, iz + z);
                        this.BuildChunkAt(-ix + x, -iy + y, -iz + z);
                    }
                    yield return null;
                }

            }
        }
    }

	IEnumerator DrawChunks()
	{

		foreach(KeyValuePair<string, Chunk> c in chunks)
		{
			if(c.Value.status == Chunk.ChunkStatus.DRAW) 
			{
				c.Value.DrawChunk();
			}

			var bPos = this.lastbuildPos;
            var chPos = c.Value.chunk.transform.position;

			if (c.Value.chunk && c.Value.status == Chunk.ChunkStatus.DONE &&
                Mathf.Pow(bPos.x - chPos.x, 2) + Mathf.Pow(bPos.y - chPos.y, 2) + Mathf.Pow(bPos.z - chPos.z, 2) > Mathf.Pow((radius + 1) * World.chunkSize, 2))
                toRemove.Add(c.Key);

			yield return null;
		}
	}

	IEnumerator RemoveOldChunks()
	{
		for(int i = 0; i < toRemove.Count; i++)
		{
			string n = toRemove[i];

		    toRemove.Remove(n);

            if (chunks.TryGetValue(n, out Chunk c))
			{
			    
                Destroy(c.chunk);
				chunks.TryRemove(n, out c);
				yield return null;
			}
		}
	}

	public void BuildNearPlayer()
	{
		StopCoroutine("BuildRecursiveWorld");
		queue.Run(BuildRecursiveWorld((int)(player.transform.position.x/chunkSize),
											(int)(player.transform.position.y/chunkSize),
											(int)(player.transform.position.z/chunkSize),
											radius));
	}

	// Use this for initialization
	void Start () {
		Vector3 ppos = player.transform.position;
		player.transform.position = new Vector3(ppos.x,
											Utils.GenerateHeight(ppos.x,ppos.z) + 1,
											ppos.z);

		lastbuildPos = player.transform.position;
		player.SetActive(false);

		firstbuild = true;
		chunks = new ConcurrentDictionary<string, Chunk>();
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;	
		queue = new CoroutineQueue(maxCoroutines, StartCoroutine);
		
		//build starting chunk
		BuildChunkAt((int)(player.transform.position.x/chunkSize),
											(int)(player.transform.position.y/chunkSize),
											(int)(player.transform.position.z/chunkSize));
		//draw it
		queue.Run(DrawChunks());

		//create a bigger world
		queue.Run(BuildRecursiveWorld((int)(player.transform.position.x/chunkSize),
											(int)(player.transform.position.y/chunkSize),
											(int)(player.transform.position.z/chunkSize),radius));
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = lastbuildPos - player.transform.position;

		if(movement.magnitude > chunkSize )
		{
			lastbuildPos = player.transform.position;
			BuildNearPlayer();
		}


		if(!player.activeSelf)
		{
			player.SetActive(true);	
			firstbuild = false;
		}

		queue.Run(DrawChunks());
		queue.Run(RemoveOldChunks());
	}
}
