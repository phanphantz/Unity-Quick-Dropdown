using System.IO;

namespace PhEngine.QuickDropdown.Editor
{
    public class AssetFileResult
    {
        public string name;
        public string assetPath;
        public AssetFileResult(string assetPath)
        {
            name = Path.GetFileNameWithoutExtension(assetPath);
            this.assetPath = assetPath;
        }
    }
}