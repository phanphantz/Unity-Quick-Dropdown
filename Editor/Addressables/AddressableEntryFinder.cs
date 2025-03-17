#if ADDRESSABLES_DROPDOWN
using System;
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
        AddressableAssetGroup Group => CachedSource as AddressableAssetGroup;
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
            return Group.entries.Select(e => e.address).ToArray();
        }

        public override Object GetResultAtIndex(int index)
        {
            var entry = Group.entries.ElementAt(index);
            var path = AssetDatabase.GUIDToAssetPath(entry.guid);
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        public override void SelectAndPingSource()
        {
            Selection.activeObject = Group;
            EditorGUIUtility.PingObject(Group);
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
            var createdItem = AssetUtils.CreateScriptableObjectAndOpen(Field.DefaultNewItemName, Type, folderPath);
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
            {
                return Group.entries.Any(e => e.address == address);   
            }

            if (targetObject == null)
                return false;
            
            return AssetDatabase.TryGetGUIDAndLocalFileIdentifier(targetObject, out var guid, out long _) 
                   && Group.entries.Any(e => e.guid == guid);
        }

        protected override Object SearchForSource()
        {
            return AddressableUtils.GetFirstFoundGroup(ObjectPath);
        }

        protected override Object CreateNewSource()
        {
            return AddressableUtils.GetOrCreateGroup(ObjectPath);
        }
    }
}
#endif