namespace Assets.Minecraft.Scripts.Data
{
    public class GameData
    {
        public static LoadMode? LoadMode { get; set; } = null;
        public static string SelectedSave { get; set; } = null;
    }

    public enum LoadMode
    {
        Create, Load
    } 
}