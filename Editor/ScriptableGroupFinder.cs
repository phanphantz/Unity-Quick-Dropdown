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
            group = FindMyGroup();
            return group ? group.GetStringOptions(Type) : new string[] { };
        }

        protected virtual ScriptableGroup FindMyGroup()
        {
            return QuickDropdownEditorUtils.FindGroup(ObjectPath);
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
            Undo.IncrementCurrentGroup();
            var undoId = Undo.GetCurrentGroup();
            CreateSourceIfNotExists();
            var groupPath = QuickDropdownEditorUtils.GetAssetPath(group);
            var newInstance = QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, Path.GetDirectoryName(groupPath));
            Undo.RegisterCompleteObjectUndo(group, "Create new ScriptableObject");
            group.Add(newInstance);
            EditorUtility.SetDirty(group);
            Undo.CollapseUndoOperations(undoId);
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
            return FindMyGroup();
        }

        public override void CreateSourceIfNotExists()
        {
            if (!group)
                group = QuickDropdownEditorUtils.PrepareGroup(ObjectPath);
        }
    }
}