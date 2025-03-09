using System;

namespace PhEngine.QuickDropdown
{
    public class FromConfig : DropdownField
    {
        public Type ConfigType { get; }
        public FromConfig(Type configType, InspectMode inspectMode = InspectMode.OpenPropertyWindow, string defaultNewItemName = null, bool isHideInspectButton = false, bool isHideInfo = false, bool isHideCreateSOButton = false) : base(configType.FullName, inspectMode, defaultNewItemName, isHideInspectButton, isHideInfo, isHideCreateSOButton)
        {
            ConfigType = configType;
        }

        public override bool CheckInvalid(out Exception exception)
        {
            exception = null;
            if (ConfigType == typeof(ScriptableGroup))
            {
                exception = new Exception("Config cannot be a type of ScriptableGroup");
                return true;
            }
            if (ConfigType.IsSubclassOf(typeof(ScriptableContainer))) 
                return base.CheckInvalid(out exception);
            
            exception = new Exception("The type must be a subclass of ScriptableContainer.");
            return true;
        }
    }
}