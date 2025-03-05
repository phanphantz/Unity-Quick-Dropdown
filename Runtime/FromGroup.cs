namespace PhEngine.QuickDropdown
{
    public class FromGroup : DropdownField, IHasCreateSOButton
    {
        public bool IsHideCreateSOButton { get; }
        public FromGroup(string path, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null,  bool isHideCreateSOButton = false, bool isHideInfo = false,  bool isHideInspectButton = false) : base(path, inspectMode, defaultNewItemName, isHideInspectButton, isHideInfo)
        {
            IsHideCreateSOButton = isHideCreateSOButton;
        }
    }
}