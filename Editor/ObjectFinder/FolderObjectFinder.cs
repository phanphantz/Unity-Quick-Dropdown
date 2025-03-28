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
            var assetPath = ObjectPath.StartsWith("Assets/") ? ObjectPath : "Assets/" + ObjectPath;
            if (assetPath.EndsWith('/'))
                assetPath = assetPath.Substring(0, assetPath.Length - 1);

            AssetPath = assetPath;
        }
        
        protected override bool IsPathMatched()
        {
            return CachedSource.name == AssetPath.Split('/').LastOrDefault();
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

        public override object GetResultAtIndex(int index)
        {
            return AssetUtils.LoadAssetAtPath(pathResults[index].assetPath, Type);
        }

        public override void SelectAndPingSource()
        {
            Selection.activeObject = CachedSource;
            EditorApplication.delayCall += () => { EditorApplication.ExecuteMenuItem("Assets/Open"); };
        }

        public override void CreateNewScriptableObject()
        {
            if (string.IsNullOrEmpty(AssetPath))
                throw new InvalidOperationException("Directory path is empty.");
            
            if (CachedSource == null)
                PrepareSource();
            
            AssetUtils.CreateScriptableObjectAndOpen(Field.DefaultNewItemName, Type, AssetPath);
        }

        public override Texture GetSourceIcon()
        {
            return IconUtils.GetFolderIcon();
        }

        public override bool IsBelongToSource(object currentObject)
        {
            var path = AssetUtils.GetAssetPath(currentObject as Object);
            if (pathResults == null)
                SearchForItems();
            
            return path != null && pathResults != null && pathResults.Any(r => r.assetPath == path);
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