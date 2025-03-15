using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PhEngine.QuickDropdown.Editor
{
    public class ScriptableGroupFinder : ScriptableContainerFinder
    {
        static readonly Dictionary<string, ScriptableGroup> CachedGroups = new Dictionary<string, ScriptableGroup>();
        public ScriptableGroupFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        protected override ScriptableContainer FindContainer(string name)
        {
            if (CachedGroups.TryGetValue(name, out var result) && result != null)
                return result;
            
            result = AssetDatabase
                .FindAssets("t:" + nameof(ScriptableGroup) + " " + name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableGroup>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(g => g);

            if (result)
            {
                if (!CachedGroups.TryAdd(name, result))
                    CachedGroups[name] = result;
            }
            
            return result;
        }

        protected override ScriptableContainer CreateNewContainer(string groupPath)
        {
            return AssetUtils.CreateScriptableObject(typeof(ScriptableGroup), groupPath) as ScriptableGroup;
        }
    }
}