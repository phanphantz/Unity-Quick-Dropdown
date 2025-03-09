using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PhEngine.QuickDropdown.Editor
{
    public class ScriptableGroupFinder : ScriptableContainerFinder
    {
        static Dictionary<string, ScriptableGroup> cachedGroups = new Dictionary<string, ScriptableGroup>();
        public ScriptableGroupFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        public override ScriptableContainer FindContainer(string name)
        {
            if (cachedGroups.TryGetValue(name, out var result) && result != null)
                return result;
            
            result = AssetDatabase
                .FindAssets("t:" + nameof(ScriptableGroup) + " " + name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableGroup>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(g => g);

            if (result)
            {
                if (!cachedGroups.TryAdd(name, result))
                    cachedGroups[name] = result;
            }
            
            return result;
        }

        protected override ScriptableContainer CreateNewContainer(string groupPath)
        {
            return QuickDropdownEditorUtils.CreateScriptableObject(typeof(ScriptableGroup), groupPath) as ScriptableGroup;
        }
    }
}