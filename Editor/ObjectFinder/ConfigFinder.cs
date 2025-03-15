using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    public class ConfigFinder : ScriptableContainerFinder
    {
        FromConfig FromConfig => Field as FromConfig;
        static Dictionary<Type, ScriptableContainer> cachedContainers = new Dictionary<Type, ScriptableContainer>();
        
        public ConfigFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        protected override ScriptableContainer FindContainer(string name)
        {
            return Container;
        }

        public override bool CheckAndPrepareSource()
        {
            if (cachedContainers.TryGetValue(FromConfig.ConfigType, out  Container) &&  Container)
                return true;
            
            var possibleItems = AssetDatabase
                .FindAssets("t:" + FromConfig.ConfigType.Name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableContainer>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            var result = possibleItems.FirstOrDefault(g => g);
            if (possibleItems.Length > 1 && result != null)
            {
                Debug.LogWarning($"There are more than one Config Group of type: {FromConfig.ConfigType.Name} in the project. The first match '{result.name}' will be used. Please make sure there is only one instance of this type.");
                return false;
            }
            
            if (result)
                cachedContainers.TryAdd(FromConfig.ConfigType, result);

            Container = result;
            return  Container;
        }

        protected override ScriptableContainer CreateNewContainer(string groupPath)
        {
            return AssetUtils.CreateScriptableObject(FromConfig.ConfigType, groupPath) as ScriptableContainer;
        }
    }
}