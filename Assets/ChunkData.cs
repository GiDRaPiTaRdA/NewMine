
using Assets.Scripts;
using UnityEngine;

namespace Assets
{
    public class ChunkData
    {
        public Block[,,] chunkData;

        public ChunkData(Block [,,] chunkData)
        {
            this.chunkData = chunkData;
        }

        public Block this[int x,int y,int z]
        {
            get => this.chunkData[x, y, z];
            set => this.chunkData[x, y, z] = value;
        }

        public Block this[Position pos]
        {
            get => this[pos.X,pos.Y,pos.Z];
            set => this[pos.X, pos.Y, pos.Z] = value;
        }

        public Block this[Vector3 vector3]
        {
            get => this[new Position(vector3)];
            set => this[new Position(vector3)] = value;
        }

    }
}