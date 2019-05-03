﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using LightType = UnityEngine.LightType;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class Block
    {
        public BlockType bType;

        public BlockKind bKind;

        public GameObject[] blockObjects;

        public readonly Chunk owner;
        readonly GameObject parent;
        public Vector3 position;

        public List<BlockQuad> MeshFilters { get; private set; }



        //public static readonly Vector2[,] blockUVs =
        //{
        //    /*GRASS TOP*/
        //    {
        //        new Vector2(0.5f, 0.8125f),new Vector2(0.5625f,0.8125f),
        //        new Vector2(0.5f,0.875f),new Vector2(0.5625f,0.875f)
        //    },
        //    /*GRASS SIDE*/
        //    {
        //        new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f),
        //        new Vector2(0.1875f, 1.0f), new Vector2(0.25f, 1.0f)
        //    },
        //    /*DIRT*/
        //    {
        //        new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f),
        //        new Vector2(0.125f, 1.0f), new Vector2(0.1875f, 1.0f)
        //    },
        //    /*STONE*/
        //    {
        //        new Vector2(0, 0.875f), new Vector2(0.0625f, 0.875f),
        //        new Vector2(0, 0.9375f), new Vector2(0.0625f, 0.9375f)
        //    },
        //    /*BEDROCK*/
        //    {
        //        new Vector2(0.3125f, 0.8125f), new Vector2(0.375f, 0.8125f),
        //        new Vector2(0.3125f, 0.875f), new Vector2(0.375f, 0.875f)
        //    },
        //    /*REDSTONE*/
        //    {
        //        new Vector2(0.1875f, 0.75f), new Vector2(0.25f, 0.75f),
        //        new Vector2(0.1875f, 0.8125f), new Vector2(0.25f, 0.8125f)
        //    },
        //    /*DIAMOND*/
        //    {
        //        new Vector2(0.125f, 0.75f), new Vector2(0.1875f, 0.75f),
        //        new Vector2(0.125f, 0.8125f), new Vector2(0.1875f, 0.8125f)
        //    }
        //};

        public Block(BlockType b, Vector3 pos, GameObject p, Chunk o)
        {
            this.SetType(b);
            this.owner = o;
            this.parent = p;
            this.position = pos;
            this.MeshFilters = new List<BlockQuad>();


        }

        public void SetType(BlockType b)
        {
            BlockType oldType = this.bType;
            this.bType = b;


            this.bKind = b == BlockType.AIR ?
                BlockKind.Invisible : StaticWorld.Instance.CubeDescriptions[this.bType].blockKind;




            GameObject[] tempGameObjects = null;

            if (this.bType != BlockType.AIR)
            {
                CubeDescription cubeDescription = StaticWorld.Instance.CubeDescriptions[this.bType];

                tempGameObjects = cubeDescription.cubeGameObjects;
            }
            else
            {
                tempGameObjects = new GameObject[0];
            }

            this.blockObjects?.ToList().ForEach(Object.Destroy);

            this.blockObjects = new GameObject[tempGameObjects.Length];

            for (int index = 0; index < tempGameObjects.Length; index++)
            {
                GameObject cubeGameObject = tempGameObjects[index];

                GameObject obj = Object.Instantiate(cubeGameObject);
                obj.transform.position = this.position;
                obj.transform.SetParent(this.parent.transform, false);
               
                this.blockObjects[index] = obj;
            }

        }

        public bool HasSolidNeighbour(int x, int y, int z)
        {
            Block[,,] chunks;

            if (x < 0 || x >= World.chunkSize ||
                y < 0 || y >= World.chunkSize ||
                z < 0 || z >= World.chunkSize)
            {
                //block in a neighbouring chunk

                Vector3 neighbourChunkPos = this.parent.transform.position +
                                            new Vector3((x - (int)this.position.x) * World.chunkSize,
                                                (y - (int)this.position.y) * World.chunkSize,
                                                (z - (int)this.position.z) * World.chunkSize);

                x = World.ConvertBlockIndexToLocal(x);
                y = World.ConvertBlockIndexToLocal(y);
                z = World.ConvertBlockIndexToLocal(z);

                if (StaticWorld.chunks.TryGetValue(neighbourChunkPos, out Chunk nChunk))
                {
                    chunks = nChunk.chunkData.chunkData;
                }
                else
                    return false;
            } //block in this chunk
            else
                chunks = this.owner.chunkData.chunkData;

            try
            {
                var kind = chunks[x, y, z].bKind;

                return kind == BlockKind.Solid || 
                       (this.bKind == BlockKind.Transparent && kind == BlockKind.Transparent)
                       ||
                       (this.bKind == BlockKind.Glowing && kind == BlockKind.Glowing)
                    ;
            }
            catch (IndexOutOfRangeException)
            {
            }

            return false;
        }

        public bool HasSolidNeighbour(Vector3 pos)
        {
            return this.HasSolidNeighbour(new Position((int)pos.x, (int)pos.y, (int)pos.z));
        }

        public bool HasSolidNeighbour(Position pos)
        {
            return this.HasSolidNeighbour(pos.X, pos.Y, pos.Z);
        }

        public void Draw()
        {
            this.MeshFilters.ForEach(mf =>
            {
                this.owner.MeshFiltersBlock.Remove(mf);
                //this.owner.MeshFilters.Remove(mf.MeshFilter);
                Object.Destroy(mf.QuadGameObject);
            });
            this.MeshFilters.Clear();

            if (this.bType == BlockType.AIR) return;



            if (!this.HasSolidNeighbour((int)this.position.x, (int)this.position.y, (int)this.position.z + 1))
                this.MeshFilters.Add(new BlockQuad(Cubeside.FRONT, this.bType, this.parent.transform, this.position));

            if (!this.HasSolidNeighbour((int)this.position.x, (int)this.position.y, (int)this.position.z - 1))
                this.MeshFilters.Add(new BlockQuad(Cubeside.BACK, this.bType, this.parent.transform, this.position));

            if (!this.HasSolidNeighbour((int)this.position.x, (int)this.position.y + 1, (int)this.position.z))
                this.MeshFilters.Add(new BlockQuad(Cubeside.TOP, this.bType, this.parent.transform, this.position));

            if (!this.HasSolidNeighbour((int)this.position.x, (int)this.position.y - 1, (int)this.position.z))
                this.MeshFilters.Add(new BlockQuad(Cubeside.BOTTOM, this.bType, this.parent.transform, this.position));

            if (!this.HasSolidNeighbour((int)this.position.x - 1, (int)this.position.y, (int)this.position.z))
                this.MeshFilters.Add(new BlockQuad(Cubeside.LEFT, this.bType, this.parent.transform, this.position));

            if (!this.HasSolidNeighbour((int)this.position.x + 1, (int)this.position.y, (int)this.position.z))
                this.MeshFilters.Add(new BlockQuad(Cubeside.RIGHT, this.bType, this.parent.transform, this.position));

            //this.MeshFilters.ForEach(mf =>
            //{
            //    this.owner.MeshFilters.Add(mf.MeshFilter);
            //});

            this.owner.MeshFiltersBlock.AddRange(this.MeshFilters);
        }
    }
}
