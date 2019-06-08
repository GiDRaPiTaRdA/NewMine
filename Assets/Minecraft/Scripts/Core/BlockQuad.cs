using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockQuad
    {
        public BlockType BlockType { get; set; }
        public Cubeside Cubeside { get; set; }

        public GameObject QuadGameObject { get; set; }

        public Position Position { get; set; }

        public Transform ParrentTransform { get; set; }

        public MeshFilter MeshFilter { get; set; }

        public Mesh MeshCollider { get; set; }

        private Transform Buffer => StaticWorld.Instance.WorldMeshBufferGameObject.transform;

        #region MyRegion

        private static readonly Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        private static readonly Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        private static readonly Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        private static readonly Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        private static readonly Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        private static readonly Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        private static readonly Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        private static readonly Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        private static readonly Dictionary<Cubeside, object> dictionary = new Dictionary<Cubeside, object>
        {
            {Cubeside.BOTTOM,
                new
                {
                    vertices = new [] { p0, p1, p2, p3 },
                    normals =new [] {Vector3.down, Vector3.down,Vector3.down, Vector3.down},
                }},

            {Cubeside.TOP,
                new
                {
                    vertices = new[] { p7, p6, p5, p4 },
                    normals =new [] {Vector3.up, Vector3.up, Vector3.up, Vector3.up},
                }},
            {Cubeside.LEFT,
                new
                {
                    vertices = new[] { p7, p4, p0, p3 },
                    normals =new [] {Vector3.left, Vector3.left, Vector3.left, Vector3.left},
                }},
            {Cubeside.RIGHT, new
                {
                    vertices = new[] { p5, p6, p2, p1 },
                    normals = new[] {Vector3.right, Vector3.right,Vector3.right, Vector3.right}
                    }
                },
            {Cubeside.FRONT, new
                {
                    vertices = new[] { p4, p5, p1, p0 },
                    normals = new[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward}
                }
            },

            {Cubeside.BACK, new
                {
                    vertices = new[]  { p6, p7, p3, p2 },
                    normals = new[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back }
                }
            },
        };


        #endregion

        public BlockQuad(Cubeside cubeside, BlockType blockType, Transform parrentTransform, Position position)
        {
            this.BlockType = blockType;
            this.Cubeside = cubeside;
            this.ParrentTransform = parrentTransform;
            this.Position = position;

            Mesh mesh = this.CreateQuad(cubeside);

            this.MeshCollider = mesh;

            this.QuadGameObject = new GameObject("BlockQuad");

            this.QuadGameObject.transform.localPosition = this.Position;
            this.QuadGameObject.transform.parent = this.Buffer;
            this.QuadGameObject.transform.localScale*=StaticWorld.K;
           

            this.MeshFilter = (MeshFilter)this.QuadGameObject.AddComponent(typeof(MeshFilter));
            
            this.MeshFilter.mesh = mesh;
        }

        public Mesh CreateQuad(Cubeside side)
        {
            Mesh mesh = new Mesh
            {
                name = "ScriptedMesh" + side
            };

            //all possible UVs
            Vector2 uv00 = new Vector2(0,0);
            Vector2 uv10 = new Vector2(1,0);
            Vector2 uv01 =new Vector2(0,1);
            Vector2 uv11 = new Vector2(1,1);


            mesh.vertices = ((dynamic)dictionary[side]).vertices;
            mesh.normals = ((dynamic)dictionary[side]).normals;
            mesh.uv = new[] { uv11, uv01, uv00, uv10 };
            mesh.triangles = new[] {3, 1, 0, 3, 2, 1};

            mesh.RecalculateBounds();

            

            return mesh;
        }
    }
}