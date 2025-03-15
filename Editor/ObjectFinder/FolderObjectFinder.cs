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
            pathResults = FindInFolder(Type.Name, AssetPath);
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
            return AssetUtils.LoadAssetAtPath(pathResults[index].assetPath, Type);
        }

        public override void SelectAndPingSource()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = CachedSource;
            EditorApplication.delayCall += () => { EditorApplication.ExecuteMenuItem("Assets/Open"); };
        }

        public override void CreateNewScriptableObject()
        {
            if (string.IsNullOrEmpty(AssetPath))
                throw new InvalidOperationException("Directory path is empty.");
            
            CreateNewSource();
            AssetUtils.CreateScriptableObjectAndSelect(Field.DefaultNewItemName, Type, AssetPath);
        }

        public override Texture GetSourceIcon()
        {
            return IconUtils.GetFolderIcon();
        }

        public override bool IsBelongToSource(object currentObject)
        {
            var path = AssetUtils.GetAssetPath(currentObject as Object);
            return path != null && pathResults.Any(r => r.assetPath == path);
        }

        protected override Object SearchForSource()
        {
            if (!Directory.Exists(AssetPath))
                return null;
            
            return AssetUtils.LoadAssetAtPath(AssetPath, typeof(Object));
        }

        protected override Object CreateNewSource()
        {
            Directory.CreateDirectory(AssetPath);
            AssetDatabase.Refresh();
            return SearchForSource();
        }
        
        static AssetFileResult[] FindInFolder(string typeName, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return new AssetFileResult[] { };

            return AssetDatabase.FindAssets("t:" + typeName, new[] {folderPath})
                .Select(guid => new AssetFileResult(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}