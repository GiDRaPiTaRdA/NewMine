﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class World : MonoBehaviour
{

    public GameObject player;
    public Material textureAtlas;
    public static int columnHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 1;
    public static int radius = 4;
    public static ConcurrentDictionary<Vector3, Chunk> chunks;
    public static bool firstbuild = true;
    public static ConcurrentQueue<Vector3> toRemove = new ConcurrentQueue<Vector3>();

    CoroutineQueue queue;
    public static uint maxCoroutines = 2000;

    public Vector3 lastbuildPos;

    void BuildChunkAt(int x, int y, int z)
    {
        if (y >= 0)
        {
            Vector3 chunkPosition = new Vector3(x * chunkSize,
                y * chunkSize,
                z * chunkSize);

            if (!chunks.TryGetValue(chunkPosition, out Chunk c))
            {
                c = new Chunk(chunkPosition, this.textureAtlas);
                c.chunk.transform.parent = this.transform;
                chunks.TryAdd(c.chunk.transform.position, c);
            }
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

        foreach (KeyValuePair<Vector3, Chunk> c in chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
            }

            var bPos = this.lastbuildPos;
            var chPos = c.Value.chunk.transform.position;

            if (c.Value.chunk && c.Value.status == Chunk.ChunkStatus.DONE &&
                Mathf.Pow(bPos.x - chPos.x, 2) + Mathf.Pow(bPos.y - chPos.y, 2) + Mathf.Pow(bPos.z - chPos.z, 2) > Mathf.Pow((radius + 1) * chunkSize, 2))
                toRemove.Enqueue(c.Key);

            yield return null;
        }
    }

    IEnumerator RemoveOldChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            toRemove.TryDequeue(out Vector3 n);

            if (chunks.TryGetValue(n, out Chunk c))
            {

                Destroy(c.chunk);
                c.Save();
                chunks.TryRemove(n, out c);
                yield return null;
            }
        }
    }

    public void BuildNearPlayer()
    {
        this.StopCoroutine("BuildRecursiveWorld");
        this.queue.Run(this.BuildRecursiveWorld((int)(this.player.transform.position.x / chunkSize),
                                            (int)(this.player.transform.position.y / chunkSize),
                                            (int)(this.player.transform.position.z / chunkSize),
                                            radius));
    }

    // Use this for initialization
    void Start()
    {
        Vector3 ppos = this.player.transform.position;
        this.player.transform.position = new Vector3(ppos.x,
                                            Utils.GenerateHeight(ppos.x, ppos.z) + 1,
                                            ppos.z);

        this.lastbuildPos = this.player.transform.position;
        this.player.SetActive(false);

        firstbuild = true;
        chunks = new ConcurrentDictionary<Vector3, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.queue = new CoroutineQueue(maxCoroutines, this.StartCoroutine);

        //build starting chunk
        this.BuildChunkAt((int)(this.player.transform.position.x / chunkSize),
                                            (int)(this.player.transform.position.y / chunkSize),
                                            (int)(this.player.transform.position.z / chunkSize));
        //draw it
        this.queue.Run(this.DrawChunks());

        //create a bigger world
        this.queue.Run(this.BuildRecursiveWorld((int)(this.player.transform.position.x / chunkSize),
                                            (int)(this.player.transform.position.y / chunkSize),
                                            (int)(this.player.transform.position.z / chunkSize), radius));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = this.lastbuildPos - this.player.transform.position;

        if (movement.magnitude > chunkSize)
        {
            this.lastbuildPos = this.player.transform.position;
            this.BuildNearPlayer();
        }


        if (!this.player.activeSelf)
        {
            this.player.SetActive(true);
            firstbuild = false;
        }

        this.queue.Run(this.DrawChunks());
        this.queue.Run(this.RemoveOldChunks());
    }

    public static Block GetWorldBlock(Vector3 pos)
    {
        int cx, cy, cz;

        if (pos.x < 0)
            cx = (int)(Mathf.Round(pos.x - chunkSize) / (float)chunkSize) * chunkSize;
        else
            cx = (int)(Mathf.Round(pos.x) / (float)chunkSize) * chunkSize;

        if (pos.y < 0)
            cy = (int)(Mathf.Round(pos.y - chunkSize) / (float)chunkSize) * chunkSize;
        else
            cy = (int)(Mathf.Round(pos.y) / (float)chunkSize) * chunkSize;


        if (pos.z < 0)
            cz = (int)(Mathf.Round(pos.z - chunkSize) / (float)chunkSize) * chunkSize;
        else
            cz = (int)(Mathf.Round(pos.z) / (float)chunkSize) * chunkSize;

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
