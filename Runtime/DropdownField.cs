using UnityEngine;

namespace PhEngine.QuickDropdown
{
    public interface IHasCreateSOButton
    {
        bool IsHideCreateSOButton { get; }
    }

    public abstract class DropdownField : PropertyAttribute
    {
        public string Path { get; }
        public InspectMode InspectMode { get; }
        public string DefaultNewItemName { get; }
        public bool IsHideInspectButton { get; }
        public bool IsHideInfo { get; }

        protected DropdownField(string path, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null, bool isHideInspectButton = false, bool isHideInfo = false)
        {
            Path = path;
            InspectMode = inspectMode;
            DefaultNewItemName = defaultNewItemName;
            IsHideInspectButton = isHideInspectButton;
            IsHideInfo = isHideInfo;
        }
    }

    public enum InspectMode
    {
        OpenPropertyWindow, Select
    }
}