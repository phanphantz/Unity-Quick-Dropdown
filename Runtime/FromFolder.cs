namespace PhEngine.QuickDropdown
{
    public class FromFolder : DropdownField
    {
        public FromFolder(string path, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null, bool isHideInspectButton = false, bool isHideInfo = false, bool isHideCreateSOButton = false) : base(path, inspectMode, defaultNewItemName, isHideInspectButton, isHideInfo, isHideCreateSOButton)
        {
        }
    }
}