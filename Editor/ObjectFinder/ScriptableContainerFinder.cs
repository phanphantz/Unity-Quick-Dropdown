using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public abstract class ScriptableContainerFinder : ObjectFinder
    {
        ScriptableContainer Container => CachedSource as ScriptableContainer;
        protected ScriptableContainerFinder(DropdownField field, Type type) : base(field, type)
        {
        }
        
        public override string[] SearchForItems()
        {
            return Container ? Container.GetStringOptions(Type) : new string[] { };
        }
        
        public override Object GetResultAtIndex(int index)
        {
            return Container.GetObjectFromFlatTree(Type, index);
        }
        
        public override void SelectAndPingSource()
        {
            AssetUtils.SelectAndPingInProjectTab(Container);
        }

        public override void CreateNewScriptableObject()
        {
            Undo.IncrementCurrentGroup();
            var undoId = Undo.GetCurrentGroup();
            CreateNewSource();
            var groupPath = AssetUtils.GetAssetPath(Container);
            var newInstance = AssetUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, Path.GetDirectoryName(groupPath));
            Undo.RegisterCompleteObjectUndo(Container, "Create new ScriptableObject");
            Container.AddObject(newInstance);
            EditorUtility.SetDirty(Container);
            Undo.CollapseUndoOperations(undoId);
        }

        public override Texture GetSourceIcon()
        {
            return IconUtils.GetScriptableObjectIcon();
        }

        public override bool IsBelongToSource(object currentObject)
        {
            return Container.ContainsObject(currentObject as Object);
        }

        protected override Object CreateNewSource()
        {
            var groupPath = AssetUtils.GetDefaultAssetPath(ObjectPath);
            var directory = Path.GetDirectoryName(groupPath);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("Directory path is empty.");
                
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            return CreateNewContainer(groupPath);
        }

        protected abstract ScriptableContainer CreateNewContainer(string groupPath);
    }
}