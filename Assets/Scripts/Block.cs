using System;
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
        public BlockType Type { get; set; }

        public BlockKind Kind { get; set; }

        public Chunk Chunk { get; set; }

        private Transform ParentTransform => this.Chunk.ChunkGameObject.transform;

        public Position Position { get;}
        public Position GlobalPosition { get; }

        /// <summary>
        ///  Quads of block
        /// </summary>
        public List<BlockQuad> BlockQuads { get; }

        /// <summary>
        /// Additional GM of Block for example light source
        /// </summary>
        public GameObject[] BlockObjects { get; set; }

        public Block(BlockType blockType, Vector3 pos, Chunk chunk)
        {
            this.Chunk = chunk;
            this.Position = pos;
            this.GlobalPosition = this.Position + this.Chunk.Position;
            this.BlockQuads = new List<BlockQuad>(6);
            this.SetType(blockType);
        }

        public void SetType(BlockType b)
        {
            BlockType oldType = this.Type;
            this.Type = b;

            this.Kind = StaticWorld.Instance.CubeDescriptions[this.Type].blockKind;

            // CUBE GAMEOBJECTS
            GameObject[] tempGameObjects = null;

            if (this.Type != BlockType.AIR)
            {
                CubeDescription cubeDescription = StaticWorld.Instance.CubeDescriptions[this.Type];

                tempGameObjects = cubeDescription.cubeGameObjects;
            }
            else
            {
                tempGameObjects = new GameObject[0];
            }

            // ADDITIONAL OBJECTS
            this.BlockObjects?.ToList().ForEach(Object.Destroy);

            this.BlockObjects = new GameObject[tempGameObjects.Length];

            for (int index = 0; index < tempGameObjects.Length; index++)
            {
                GameObject cubeGameObject = tempGameObjects[index];

                GameObject obj = Object.Instantiate(cubeGameObject);
                obj.transform.position = this.Position;
                obj.AddComponent<ObjectPosition>().Position = this.Position;

                obj.transform.position *= StaticWorld.K;
                obj.transform.localScale *= StaticWorld.K;

                obj.transform.SetParent(this.ParentTransform, false);

                this.BlockObjects[index] = obj;
            }

        }

        public bool HasSolidNeighbour(int x, int y, int z)
        {
            Block[,,] blocks;

            if (x < 0 || x >= World.chunkSize ||
                y < 0 || y >= World.chunkSize ||
                z < 0 || z >= World.chunkSize)
            {
                //block in a neighbouring chunkGameObject
                //Vector3 neighbourChunkPos = this.Chunk.Position +
                //                            new Position((x - (int)this.Position.X) * World.chunkSize,
                //                                (y - (int)this.Position.Y) * World.chunkSize,
                //                                (z - (int)this.Position.Z) * World.chunkSize);
                Position pos = new Position(x, y, z);

                ////block in a neighbouring chunkGameObject
                Position neighbourChunkPos = this.Chunk.Position + ((pos - this.Position) * World.chunkSize);

                x = World.ConvertBlockIndexToLocal(x);
                y = World.ConvertBlockIndexToLocal(y);
                z = World.ConvertBlockIndexToLocal(z);

                if (StaticWorld.Instance.Chunks.TryGetValue(neighbourChunkPos, out Chunk nChunk))
                {
                    blocks = nChunk.ChunkData.chunkData;
                }
                else
                    return false;
            } //block in this chunkGameObject
            else
            {
                blocks = this.Chunk.ChunkData.chunkData;
            }

            try
            {
                BlockKind neighbourKind = blocks[x, y, z].Kind;

                return neighbourKind == BlockKind.Solid ||
                      
                       (this.Kind == BlockKind.Transparent && neighbourKind == BlockKind.Transparent)||
                       (this.Kind == BlockKind.Glowing && neighbourKind == BlockKind.Glowing)
                    ;
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogWarning("sOME OUT OF RANGE IN block");
            }

            return false;
        }

        public bool HasSolidNeighbour(Position pos)
        {
            return this.HasSolidNeighbour(pos.X, pos.Y, pos.Z);
        }

        public void Draw()
        {
            this.BlockQuads.ForEach(blockQuad =>
            {
                this.Chunk.MeshFiltersBlock.Remove(blockQuad);
                Object.Destroy(blockQuad.MeshFilter);
                Object.Destroy(blockQuad.QuadGameObject);
            });
            this.BlockQuads.Clear();

            if (this.Kind == BlockKind.Invisible) return;

            if (!this.HasSolidNeighbour(this.Position.X, this.Position.Y, this.Position.Z + 1))
                this.BlockQuads.Add(new BlockQuad(Cubeside.FRONT, this.Type, this.ParentTransform, this.Position*StaticWorld.K));

            if (!this.HasSolidNeighbour(this.Position.X, this.Position.Y, this.Position.Z - 1))
                this.BlockQuads.Add(new BlockQuad(Cubeside.BACK, this.Type, this.ParentTransform,this.Position * StaticWorld.K));

            if (!this.HasSolidNeighbour(this.Position.X, this.Position.Y + 1, this.Position.Z))
                this.BlockQuads.Add(new BlockQuad(Cubeside.TOP, this.Type, this.ParentTransform, this.Position * StaticWorld.K));

            if (!this.HasSolidNeighbour(this.Position.X, this.Position.Y - 1, this.Position.Z))
                this.BlockQuads.Add(new BlockQuad(Cubeside.BOTTOM, this.Type, this.ParentTransform, this.Position * StaticWorld.K));

            if (!this.HasSolidNeighbour(this.Position.X - 1, this.Position.Y, this.Position.Z))
                this.BlockQuads.Add(new BlockQuad(Cubeside.LEFT, this.Type, this.ParentTransform, this.Position * StaticWorld.K));

            if (!this.HasSolidNeighbour(this.Position.X + 1, this.Position.Y, this.Position.Z))
                this.BlockQuads.Add(new BlockQuad(Cubeside.RIGHT, this.Type, this.ParentTransform, this.Position * StaticWorld.K));

            this.Chunk.MeshFiltersBlock.AddRange(this.BlockQuads);
        }
    }
}
