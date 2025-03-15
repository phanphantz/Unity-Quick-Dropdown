namespace PhEngine.QuickDropdown.Editor
{
    public class AssetRenameDetector : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            ObjectFinderFactory.Dispose();
            return paths;
        }
    }
}