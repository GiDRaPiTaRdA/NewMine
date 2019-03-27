using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeshCombines: List<CombineInstance>
    {
        public float SortIndex { get; set; }

        public List<Material> Materials { get; set; }
    }
}