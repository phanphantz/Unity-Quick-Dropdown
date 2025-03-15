using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public class ScriptableGroupFinder : ScriptableContainerFinder
    {
        public ScriptableGroupFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        protected override Object SearchForSource()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(ScriptableGroup) + " " + ObjectPath);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) != ObjectPath)
                    continue;

                var obj = AssetDatabase.LoadAssetAtPath<ScriptableGroup>(path);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        protected override ScriptableContainer CreateNewContainer(string groupPath)
        {
            return AssetUtils.CreateScriptableObject(typeof(ScriptableGroup), groupPath) as ScriptableGroup;
        }
    }
}