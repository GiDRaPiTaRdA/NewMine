namespace Assets.Scripts
{
    public class StoneBlock : BlockBuilder
    {
        public override int[] GetMaterialIds()
        {
            return new []{3,3,3,3,3,3};
        }
    }
}