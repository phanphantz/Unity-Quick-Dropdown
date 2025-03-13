using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public abstract class ScriptableContainerFinder : ObjectFinder
    {
        protected ScriptableContainer Container;
        protected ScriptableContainerFinder(DropdownField field, Type type) : base(field, type)
        {
        }
        
        public override string[] SearchForItems()
        {
            return Container ? Container.GetStringOptions(Type) : new string[] { };
        }

        public abstract ScriptableContainer FindContainer(string name);
        public override Object GetResultAtIndex(int index)
        {
            return Container.GetObjectFromFlatTree(Type, index);
        }
        
        public override void SelectAndPingSource()
        {
            QuickDropdownEditorUtils.SelectAndPingInProjectTab(Container);
        }

        public override void CreateNewScriptableObject()
        {
            Undo.IncrementCurrentGroup();
            var undoId = Undo.GetCurrentGroup();
            CreateSourceIfNotExists();
            var groupPath = QuickDropdownEditorUtils.GetAssetPath(Container);
            var newInstance = QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, Path.GetDirectoryName(groupPath));
            Undo.RegisterCompleteObjectUndo(Container, "Create new ScriptableObject");
            Container.AddObject(newInstance);
            EditorUtility.SetDirty(Container);
            Undo.CollapseUndoOperations(undoId);
        }

        public override Texture GetSourceIcon()
        {
            return QuickDropdownEditorUtils.GetScriptableObjectIcon();
        }

        public override bool IsBelongToSource(object currentObject)
        {
            return Container.ContainsObject(currentObject as Object);
        }

        public override bool CheckAndPrepareSource()
        {
            Container = FindContainer(ObjectPath);
            return Container;
        }

        public override void CreateSourceIfNotExists()
        {
            if (Container) 
                return;

            var groupPath = QuickDropdownEditorUtils.GetDefaultAssetPath(ObjectPath);
            var directory = Path.GetDirectoryName(groupPath);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("Directory path is empty.");
                
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Container = CreateNewContainer(groupPath);
        }

        protected abstract ScriptableContainer CreateNewContainer(string groupPath);
    }
}