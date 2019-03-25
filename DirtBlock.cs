using Assets.Scripts;

namespace DefaultNamespace
{
    public class DirtBlock : BlockBuilder
    {
        public override int[] GetMaterialIds()
        {
            return new[] {2, 2, 2, 2, 2, 2};
        }
    }
}