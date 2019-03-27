using System;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class BlockBuilder
    {
        private int[] materialIds;
        public int[] MaterialIds => this.materialIds??(this.materialIds=this.GetMaterialIds());

        public abstract int[] GetMaterialIds();

        private Material[] materials;
        //public Material[] Materials => this.materials??
        //                               (this.materials =

        //    new[]
        //    {
        //        this.TopMaterial,
        //        this.BottomMaterial,
        //        this.LeftMaterial,
        //        this.RigthMaterial,
        //        this.FrontMaterial,
        //        this.BackMaterial,
        //    });

        //private Material topMaterial;
        //private Material bottomMaterial;
        //private Material leftMaterial;
        //private Material rightMaterial;
        //private Material frontMaterial;
        //private Material backMaterial;

        //public Material TopMaterial => this.topMaterial ?? (this.topMaterial = StaticWorld.Instance.materials[this.MaterialIds[0]]);

        //public Material BottomMaterial => this.bottomMaterial ?? (this.bottomMaterial = StaticWorld.Instance.materials[this.MaterialIds[1]]);

        //public Material LeftMaterial => this.leftMaterial ?? (this.leftMaterial = StaticWorld.Instance.materials[this.MaterialIds[2]]);

        //public Material RigthMaterial => this.rightMaterial ?? (this.rightMaterial = StaticWorld.Instance.materials[this.MaterialIds[3]]);

        //public Material FrontMaterial => this.frontMaterial ?? (this.frontMaterial = StaticWorld.Instance.materials[this.MaterialIds[4]]);

        //public Material BackMaterial => this.backMaterial ?? (this.backMaterial = StaticWorld.Instance.materials[this.MaterialIds[5]]);

    }
}