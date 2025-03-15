using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public class ConfigFinder : ScriptableContainerFinder
    {
        FromConfig FromConfig => Field as FromConfig;
        public ConfigFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        protected override Object SearchForSource()
        {
            var possibleItems = AssetDatabase
                .FindAssets("t:" + FromConfig.ConfigType.Name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableContainer>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            var result = possibleItems.FirstOrDefault(g => g);
            if (possibleItems.Length > 1 && result != null)
                Debug.LogWarning($"There are more than one Config Group of type: {FromConfig.ConfigType.Name} in the project. The first match '{result.name}' will be used. Please make sure there is only one instance of this type.");
            
            return result;
        }

        protected override ScriptableContainer CreateNewContainer(string groupPath)
        {
            return AssetUtils.CreateScriptableObject(FromConfig.ConfigType, groupPath) as ScriptableContainer;
        }
    }
}