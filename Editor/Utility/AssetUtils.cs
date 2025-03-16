using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public static class AssetUtils
    {
        public static Object LoadAssetAtPath(string path, Type type)
        {
            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        public static string GetAssetPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        public static string GetDefaultAssetPath(string assetName)
        {
            return $"Assets/Resources/QuickDropdown/{assetName}.asset";
        }

        public static void SelectAndPingInProjectTab(Object obj)
        {
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public static ScriptableObject CreateScriptableObjectAndSelect(string name, Type type, string folderPath,
            bool isSelectAndPing = true)
        {
            name = string.IsNullOrEmpty(name) ? type.Name : name;
            var assetPath = GetUniqueAssetFilePath(name, folderPath);
            var loadedInstance = CreateScriptableObject(type, assetPath);
            if (isSelectAndPing)
                SelectAndPingInProjectTab(loadedInstance);
            return loadedInstance;
        }

        public static ScriptableObject CreateScriptableObject(Type type, string assetPath)
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var loadedInstance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            Undo.RegisterCreatedObjectUndo(loadedInstance, "Create ScriptableObject");
            return loadedInstance;
        }

        static string GetUniqueAssetFilePath(string name, string path)
        {
            string assetPath = $"{path}/{name}.asset";
            var index = 0;
            while (File.Exists(assetPath))
            {
                assetPath = $"{path}/{name}_{index}.asset";
                index++;
            }

            return assetPath;
        }
    }
}