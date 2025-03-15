#if ADDRESSABLES_DROPDOWN
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor.Addressables
{
    public class AddressableEntryFinder : ObjectFinder
    {
        AddressableAssetGroup group;
        
        static readonly Dictionary<string, AddressableAssetGroup> CachedGroups = new Dictionary<string, AddressableAssetGroup>();
        
        static Texture unsafeAddressableIcon;
        public AddressableEntryFinder(DropdownField field, Type type) : base(field, type)
        {
        }

        public override bool IsTypeSupported(Type type)
        {
            return base.IsTypeSupported(type) || type == typeof(string);
        }

        public override string[] SearchForItems()
        {
            return group.entries.Select(e => e.address).ToArray();
        }

        public override Object GetResultAtIndex(int index)
        {
            var entry = group.entries.ElementAt(index);
            var path = AssetDatabase.GUIDToAssetPath(entry.guid);
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        public override void SelectAndPingSource()
        {
            Selection.activeObject = group;
            EditorGUIUtility.PingObject(group);
        }

        public override void CreateNewScriptableObject()
        {
            var folderPath = "Assets/Resources/QuickDropdown/AddressableAssets/" + Type.Name;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            
            Undo.IncrementCurrentGroup();
            var id = Undo.GetCurrentGroup();
            var createdItem = AssetUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, folderPath);
            var actualPath = AssetDatabase.GetAssetPath(createdItem);
            AddressableUtils.AddToAddressableGroup(AssetDatabase.AssetPathToGUID(actualPath), ObjectPath, createdItem.name);
            Undo.CollapseUndoOperations(id);
        }

        public override Texture GetSourceIcon()
        {
            if (unsafeAddressableIcon == null)
                unsafeAddressableIcon = EditorGUIUtility.IconContent("d_UnityLogo").image;

            return unsafeAddressableIcon;
        }

        public override bool IsBelongToSource(object currentObject)
        {
            var targetObject = currentObject as Object;
            if (currentObject is string address)
                return group.entries.Any(e => e.address == address);   
            
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetObject, out var guid, out _) 
                   && group.entries.Any(e => e.guid == guid);
        }

        public override bool CheckAndPrepareSource()
        {
            if (CachedGroups.TryGetValue(ObjectPath, out group) && group != null)
                return true;
            
            group = AddressableUtils.GetFirstFoundGroup(ObjectPath);
            if (group != null)
            {
                if (CachedGroups.TryAdd(ObjectPath, group))
                    CachedGroups[ObjectPath] = group;
            }
            return group;
        }

        public override void CreateSourceIfNotExists()
        {
            AddressableUtils.GetOrCreateGroup(ObjectPath);
        }
    }
}
#endif