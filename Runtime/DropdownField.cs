using System;
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

        public virtual bool CheckInvalid(out Exception exception)
        {
            exception = null;
            if (!string.IsNullOrEmpty(Path)) 
                return false;
            
            exception = new Exception("Path is null or empty.");
            return true;
        }
    }

    public enum InspectMode
    {
        OpenPropertyWindow, Select
    }
}