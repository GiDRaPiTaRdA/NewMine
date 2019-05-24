
using UnityEngine;

namespace Assets.Scripts
{
    public struct Position
    {
        private Vector3 Vector { get; }

        public Position(int x, int y, int z) : this(new Vector3(x, y, z)){}

        public Position(Vector3 vector3)=> this.Vector = vector3;

        // User-defined conversion from Digit to double
        public static implicit operator Vector3(Position pos)
        {
            return pos.Vector;
        }
        //  User-defined conversion from double to Digit
        public static implicit operator Position(Vector3 vector)
        {
            return new Position(vector);
        }


        public static Position operator *(Position p1, float m) => new Position(p1.Vector * m);
        public static Position operator /(Position p1, float d) => new Position(p1.Vector / d);
        public static Position operator +(Position p1, Position p2) => new Position(p1.Vector + p2.Vector);
        public static Position operator -(Position p1, Position p2) => new Position(p1.Vector - p2.Vector);


        public int X => (int)this.Vector.x;
        public int Y => (int)this.Vector.y;
        public int Z => (int)this.Vector.z;

        public override string ToString() => this.Vector.ToString();
    }
}
