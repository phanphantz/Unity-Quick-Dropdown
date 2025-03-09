using UnityEngine;

namespace PhEngine.QuickDropdown
{
    public abstract class DropdownField : PropertyAttribute
    {
        public string Path { get; }
        public InspectMode InspectMode { get; }
        public string DefaultNewItemName { get; }
        public bool IsHideInspectButton { get; }
        public bool IsHideInfo { get; }
        public bool IsHideCreateSOButton { get; }

        protected DropdownField(string path, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null, bool isHideInspectButton = false, bool isHideInfo = false, bool isHideCreateSOButton = false)
        {
            Path = path;
            InspectMode = inspectMode;
            DefaultNewItemName = defaultNewItemName;
            IsHideInspectButton = isHideInspectButton;
            IsHideInfo = isHideInfo;
            IsHideCreateSOButton = isHideCreateSOButton;
        }
    }

    public enum InspectMode
    {
        OpenPropertyWindow, Select
    }
}