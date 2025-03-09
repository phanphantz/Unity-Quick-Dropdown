using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public class ScriptableGroupFinder : ObjectFinder
    {
        ScriptableGroup group;

        public ScriptableGroupFinder(DropdownField field, Type type) : base(field, type)
        {
        }
        
        public override string[] SearchForItems()
        {
            group = QuickDropdownEditorUtils.FindGroup(ObjectPath);
            return group ? group.GetStringOptions(Type) : new string[] { };
        }
        
        public override Object GetResultAtIndex(int index)
        {
            return group.GetElementFromFlatTree(Type, index);
        }
        
        public override void SelectAndPingSource()
        {
            QuickDropdownEditorUtils.SelectAndPing(group);
        }

        public override void CreateNewScriptableObject()
        {
            CreateSourceIfNotExists();
            var groupPath = QuickDropdownEditorUtils.GetAssetPath(group);
            var newInstance = QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, Path.GetDirectoryName(groupPath));
            group.Add(newInstance);
            EditorUtility.SetDirty(group);
        }

        public override Texture GetSourceIcon()
        {
            return QuickDropdownEditorUtils.GetScriptableObjectIcon();
        }

        public override bool IsBelongToSource(Object currentObject)
        {
            return group.Contains(currentObject);
        }

        public override bool IsSourceValid()
        {
            return QuickDropdownEditorUtils.FindGroup(ObjectPath);
        }

        public override void CreateSourceIfNotExists()
        {
            if (!group)
                group = QuickDropdownEditorUtils.PrepareGroup(ObjectPath);
        }
    }
}