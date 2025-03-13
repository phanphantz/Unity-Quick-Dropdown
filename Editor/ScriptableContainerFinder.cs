using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public abstract class ScriptableContainerFinder : ObjectFinder
    {
        ScriptableContainer container;
        protected ScriptableContainerFinder(DropdownField field, Type type) : base(field, type)
        {
        }
        
        public override string[] SearchForItems()
        {
            return container ? container.GetStringOptions(Type) : new string[] { };
        }

        public abstract ScriptableContainer FindContainer(string name);
        public override Object GetResultAtIndex(int index)
        {
            return container.GetObjectFromFlatTree(Type, index);
        }
        
        public override void SelectAndPingSource()
        {
            QuickDropdownEditorUtils.SelectAndPingInProjectTab(container);
        }

        public override void CreateNewScriptableObject()
        {
            Undo.IncrementCurrentGroup();
            var undoId = Undo.GetCurrentGroup();
            CreateSourceIfNotExists();
            var groupPath = QuickDropdownEditorUtils.GetAssetPath(container);
            var newInstance = QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, Path.GetDirectoryName(groupPath));
            Undo.RegisterCompleteObjectUndo(container, "Create new ScriptableObject");
            container.AddObject(newInstance);
            EditorUtility.SetDirty(container);
            Undo.CollapseUndoOperations(undoId);
        }

        public override Texture GetSourceIcon()
        {
            return QuickDropdownEditorUtils.GetScriptableObjectIcon();
        }

        public override bool IsBelongToSource(Object currentObject)
        {
            return container.ContainsObject(currentObject);
        }

        public override bool CheckAndPrepareSource()
        {
            container = FindContainer(ObjectPath);
            return container;
        }

        public override void CreateSourceIfNotExists()
        {
            if (container) 
                return;

            var groupPath = QuickDropdownEditorUtils.GetDefaultAssetPath(ObjectPath);
            var directory = Path.GetDirectoryName(groupPath);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("Directory path is empty.");
                
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            container = CreateNewContainer(groupPath);
        }

        protected abstract ScriptableContainer CreateNewContainer(string groupPath);
    }
}