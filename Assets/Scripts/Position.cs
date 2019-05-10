
using UnityEngine;

namespace Assets.Scripts
{
    public struct Position
    {
        public Position(Vector3 vector3) : this((int)vector3.x, (int)vector3.y, (int)vector3.z) { }

        public Position(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        // User-defined conversion from Digit to double
        public static implicit operator Vector3(Position pos)
        {
            return new Vector3(pos.X,pos.Y,pos.Z);
        }
        //  User-defined conversion from double to Digit
        public static implicit operator Position(Vector3 vector)
        {
            return new Position(vector);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
