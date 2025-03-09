using System;

namespace PhEngine.QuickDropdown.Editor
{
    public class ScriptableGroupTypeFinder : ScriptableGroupFinder
    {
        Type ConfigType { get; }
        public ScriptableGroupTypeFinder(DropdownField field, Type configType, Type type) : base(field, type)
        {
            ConfigType = configType;
        }

        protected override ScriptableGroup FindMyGroup()
        {
            return QuickDropdownEditorUtils.FindConfigGroupOfType(ConfigType.Name);
        }
    }
}