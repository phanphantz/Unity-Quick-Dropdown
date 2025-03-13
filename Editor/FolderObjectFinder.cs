using System;
using System.IO;
using System.Linq;
using UnityEditor;
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
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = folderAsset;
                EditorApplication.delayCall += () => { EditorApplication.ExecuteMenuItem("Assets/Open"); };
            }
        }

        public override void CreateNewScriptableObject()
        {
            if (string.IsNullOrEmpty(AssetPath))
                throw new InvalidOperationException("Directory path is empty.");
            
            CreateSourceIfNotExists();
            QuickDropdownEditorUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, AssetPath);
        }

        public override Texture GetSourceIcon()
        {
            return QuickDropdownEditorUtils.GetFolderIcon();
        }

        public override bool IsBelongToSource(object currentObject)
        {
            var path = QuickDropdownEditorUtils.GetAssetPath(currentObject as Object);
            return path != null && pathResults.Any(r => r.assetPath == path);
        }

        public override bool CheckAndPrepareSource()
        {
            return Directory.Exists(AssetPath);
        }

        public override void CreateSourceIfNotExists()
        {
            if (!Directory.Exists(AssetPath))
            {
                Directory.CreateDirectory(AssetPath);
                AssetDatabase.Refresh();
            }
        }
    }
}