namespace PhEngine.QuickDropdown
{
    public class FromFolder : DropdownField, IHasCreateSOButton
    {
        public bool IsHideCreateSOButton { get; }
        public FromFolder(string path, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null, bool isHideInspectButton = false, bool isHideInfo = false, bool isHideCreateSOButton = false) : base(path, inspectMode, defaultNewItemName, isHideInspectButton, isHideInfo)
        {
            IsHideCreateSOButton = isHideCreateSOButton;
        }
    }
}