using UnityEngine;

namespace Assets.Scripts
{
    public class GrassBlock : BlockBuilder
    {
        public override int[] GetMaterialIds() => new[] { 0, 2, 1, 1, 1, 1 };
    }
}