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
        public CubesContainer container;

        private Dictionary<BlockType, CubeDescription> cubeDescriptions;

       

        public Dictionary<BlockType, CubeDescription> CubeDescriptions => this.cubeDescriptions ??
                                                            (this.cubeDescriptions = this.container.cubeDescriptions.ToDictionary(d => d.blockType, d => d));

        public bool grassTemp = true;

        public int columnHeight = 4;
        public int worldSize = 1;

        public float k = 1f;
        public static float K => Instance.k;

        public Dictionary<Position, Chunk> Chunks { get; set; }

        public GameObject WorldMeshBufferGameObject { get; private set; }

        public static StaticWorld Instance { get; set; }

        public int loadingPercent = 0;

        private StaticWorld()
        {
            Instance = this;
        }

        public IEnumerator BuildWorld(WorldData save =null)
        {
            yield return null;
            Stopwatch s = Stopwatch.StartNew();

            int count = 0;
            int totalChunks = this.worldSize * this.worldSize * this.columnHeight;

            if (save != null)
            {
                // Load
                for (int z = 0; z < this.worldSize; z++)
                    for (int x = 0; x < this.worldSize; x++)
                        for (int y = this.columnHeight-1; y >= 0; y--)
                        {
                            Position chunkPosition = new Position(x, y, z) * World.chunkSize;

                            Chunk c = new Chunk(chunkPosition, this.transform, save[x,y,z]);

                            this.Chunks.Add(chunkPosition, c);
                        }
            }
            else
            {
                // Create
                for (int z = 0; z < this.worldSize; z++)
                    for (int x = 0; x < this.worldSize; x++)
                        for (int y = this.columnHeight-1; y >= 0; y--)
                        {
                            Position chunkPosition = new Position(x, y, z) * World.chunkSize;

                            Chunk c = new Chunk(chunkPosition, this.transform);

                            this.Chunks.Add(chunkPosition, c);
                        }
            }

            // Render
            foreach (KeyValuePair<Position, Chunk> chunk in this.Chunks)
            {
                chunk.Value.DrawChunk();
                count++;
                this.loadingPercent = (int)(((float)count / (float)totalChunks) * 100);
                yield return null;
            }

            s.Stop();
            Debug.Log(s.ElapsedMilliseconds);
        }

        // Use this for initialization
        void Start()
        {
            this.WorldMeshBufferGameObject = new GameObject("WorldMeshBuffer");

            this.Chunks = new Dictionary<Position, Chunk>();
            this.transform.rotation = Quaternion.identity;

            this.StartCoroutine(Load());

            IEnumerator Load()
            {
                Stopwatch a = Stopwatch.StartNew();
                WorldData data = null;

                data = SaveManager.Instance.LoadData("Default");

                Debug.Log("LoadData "+ a.ElapsedMilliseconds);

                return this.BuildWorld(data);
            }
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

            if (Instance.Chunks.TryGetValue(new Vector3(cx, cy, cz), out Chunk c))
            {
                return c.ChunkData[blx, bly, blz];
            }
            else
            {
                return null;
            }
        }

        public static Vector3 GetChunkPosition(Vector3 blockPosition)
        {
            Vector3 chunkPosition = blockPosition / World.chunkSize;
            chunkPosition.x = (int)chunkPosition.x;
            chunkPosition.y = (int)chunkPosition.y;
            chunkPosition.z = (int)chunkPosition.z;
            chunkPosition *= World.chunkSize;

            return chunkPosition;
        }
    }
}
