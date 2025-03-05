using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public class FolderObjectFinder : ObjectFinder
    {
        AssetFileResult[] pathResults;
        string AssetPath { get; }

        public FolderObjectFinder(DropdownField field, Type type) : base(field, type)
        {
            AssetPath = ObjectPath.StartsWith("Assets/") ? ObjectPath : "Assets/" + ObjectPath;
        }

        public override string[] SearchForItems()
        {
            pathResults = QuickDropdownEditorUtils.FindInFolder(Type.Name, AssetPath);
            return pathResults.Select(r =>
            {
                var path = r.assetPath.Replace(AssetPath, "").Split('.')[0];
                if (path.StartsWith("/"))
                    path = path.Substring(1);
                return path;
            }).ToArray();
        }

        public override Object GetResultAtIndex(int index)
        {
            return QuickDropdownEditorUtils.LoadAssetAtPath(pathResults[index].assetPath, Type);
        }
        
        public override void SelectAndPingSource()
        {
            var folderAsset = QuickDropdownEditorUtils.LoadAssetAtPath(AssetPath, typeof(Object));
            if (folderAsset != null)
                QuickDropdownEditorUtils.SelectAndPing(folderAsset);
        }

        public override void CreateNewScriptableObject()
        {
            if (string.IsNullOrEmpty(AssetPath))
                throw new InvalidOperationException("Directory path is empty.");
                
            if (!Directory.Exists(AssetPath))
                Directory.CreateDirectory(AssetPath);
                
            QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, AssetPath);
        }

        public override Texture GetSourceIcon()
        {
            return QuickDropdownEditorUtils.GetFolderIcon();
        }

        public override bool IsBelongToSource(Object currentObject)
        {
            var path = QuickDropdownEditorUtils.GetAssetPath(currentObject);
            return path != null && pathResults.Any(r => r.assetPath == path);
        }

        public override bool IsSourceValid()
        {
            return Directory.Exists(AssetPath);
        }
    }
}