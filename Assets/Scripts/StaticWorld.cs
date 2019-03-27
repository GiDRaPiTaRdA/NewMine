using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    public class StaticWorld : MonoBehaviour
    {
        public GameObject[] cubes;

        private Dictionary<BlockType, CubeDescription> cubeDescriptions;

        public Dictionary<BlockType, CubeDescription> CubeDescriptions => this.cubeDescriptions ??
                                                     (this.cubeDescriptions = this.cubes
                                                         .Select(c => c.GetComponent<CubeDescription>()).ToDictionary(d => d.blockType,d=>d));



        public static int columnHeight = 4;
        public int worldSize = 1;
        public static Dictionary<Vector3, Chunk> chunks;
        public static StaticWorld Instance;

        public StaticWorld()
        {
            StaticWorld.Instance = this;
        }

        IEnumerator BuildWorld()
        {
            Stopwatch s = Stopwatch.StartNew();

            for (int z = 0; z < this.worldSize; z++)
                for (int x = 0; x < this.worldSize; x++)
                    for (int y = columnHeight; y >= 0; y--)
                    {
                        Vector3 chunkPosition = new Vector3(x * World.chunkSize, y * World.chunkSize, z * World.chunkSize);
                        Chunk c = new Chunk(chunkPosition);
                        c.chunk.transform.parent = this.transform;
                        chunks.Add(c.chunk.transform.position, c);
                    }

            foreach (KeyValuePair<Vector3, Chunk> c in chunks)
            {
                c.Value.DrawChunk();
                yield return null;
            }

            s.Stop();
            Debug.Log(s.ElapsedMilliseconds);
        }

        // Use this for initialization
        void Start()
        {
            chunks = new Dictionary<Vector3, Chunk>();
            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;
            this.StartCoroutine(this.BuildWorld());

           // this.textureAtlas.mainTexture.
            //this.textureAtlas.mainTexture.mipMapBias = +0;
        }

        public static Block GetWorldBlock(Vector3 pos)
        {
            int cx, cy, cz;

            if (pos.x < 0)
                cx = (int)(Mathf.Round(pos.x - World.chunkSize) / (float)World.chunkSize) * World.chunkSize;
            else
                cx = (int)(Mathf.Round(pos.x) / (float)World.chunkSize) * World.chunkSize;

            if (pos.y < 0)
                cy = (int)(Mathf.Round(pos.y - World.chunkSize) / (float)World.chunkSize) * World.chunkSize;
            else
                cy = (int)(Mathf.Round(pos.y) / (float)World.chunkSize) * World.chunkSize;


            if (pos.z < 0)
                cz = (int)(Mathf.Round(pos.z - World.chunkSize) / (float)World.chunkSize) * World.chunkSize;
            else
                cz = (int)(Mathf.Round(pos.z) / (float)World.chunkSize) * World.chunkSize;

            int blx = (int)Mathf.Abs((float)Math.Round(pos.x) - cx);
            int bly = (int)Mathf.Abs((float)Math.Round(pos.y) - cy);
            int blz = (int)Mathf.Abs((float)Math.Round(pos.z) - cz);

            if (chunks.TryGetValue(new Vector3(cx, cy, cz), out Chunk c))
            {
                return c.chunkData[blx, bly, blz];
            }
            else
            {
                return null;
            }
        }
    }
}
