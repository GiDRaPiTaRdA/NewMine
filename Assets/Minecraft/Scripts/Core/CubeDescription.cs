using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class CubeDescription : MonoBehaviour
{
    public BlockType blockType;

    public BlockKind blockKind;

    public GameObject[] cubeGameObjects;

    public Material previewMaterial;

    private Dictionary<Cubeside, Material> cubeContent;

    public Dictionary<Cubeside, Material> CubeContent =>
        this.cubeContent ?? (this.cubeContent = this.GetCubeContents());

    public Dictionary<Cubeside, Material> GetCubeContents()
    {
        Material[] materials = this.GetComponentsInChildren<MeshRenderer>().Select(mr => mr.sharedMaterial).ToArray();

        Dictionary<Cubeside, Material> cont = null;

        if (materials.Length == 6)
        {
            cont = new Dictionary<Cubeside, Material>(6)
            {
                {Cubeside.TOP, materials[0]},
                {Cubeside.BOTTOM, materials[1]},
                {Cubeside.LEFT, materials[2]},
                {Cubeside.RIGHT, materials[3]},
                {Cubeside.FRONT, materials[4]},
                {Cubeside.BACK, materials[5]},
            };
        }


        return cont;
    }
}

