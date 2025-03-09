using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public static class QuickDropdownEditorUtils
    {
        static Texture GetObjectIcon()
        {
            if (unsafeObjectIcon)
                return unsafeObjectIcon;
            
            unsafeObjectIcon =  EditorGUIUtility.IconContent("d_Prefab On Icon").image;
            return unsafeObjectIcon;
        }

        static Texture GetPrefabIcon()
        {
            if (unsafePrefabIcon)
                return unsafePrefabIcon;
            
            unsafePrefabIcon =  EditorGUIUtility.IconContent("d_Prefab Icon").image;
            return unsafePrefabIcon;
        }
        
        public static Texture GetScriptableObjectIcon()
        {
            if (unsafeScriptableObjectIcon)
                return unsafeScriptableObjectIcon;
            
            unsafeScriptableObjectIcon =  EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            return unsafeScriptableObjectIcon;
        }

        public static Texture GetWarningIcon()
        {
            if (unsafeWarningIcon)
                return unsafeWarningIcon;

            unsafeWarningIcon = EditorGUIUtility.IconContent("console.warnicon.inactive.sml@2x").image;
            return unsafeWarningIcon;
        }
        
        public static Texture GetFolderIcon()
        {
            if (unsafeFolderIcon)
                return unsafeFolderIcon;
            
            unsafeFolderIcon =  EditorGUIUtility.IconContent("d_FolderOpened Icon").image;
            return unsafeFolderIcon;
        }

        static Texture unsafeScriptableObjectIcon;
        static Texture unsafeWarningIcon;
        static Texture unsafeFolderIcon;
        static Texture unsafePrefabIcon;
        static Texture unsafeObjectIcon;
        
        static Dictionary<string, ScriptableGroup> cachedGroups = new Dictionary<string, ScriptableGroup>();
        public static AssetFileResult[] FindInFolder(string typeName, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return new AssetFileResult[] { };
            
            return AssetDatabase.FindAssets("t:" + typeName, new []{ folderPath })
                .Select(guid => new AssetFileResult(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        public static ScriptableGroup FindGroup(string name)
        {
            if (cachedGroups.TryGetValue(name, out var result))
                return result;
            
            result = AssetDatabase
                .FindAssets("t:" + nameof(ScriptableGroup) + " " + name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableGroup>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault(g => g);
            
            if (result)
                cachedGroups.Add(name, result);
            
            return result;
        }
        
        public static ScriptableObject CreateScriptableObjectAndSelect(string name, Type type, string folderPath)
        {
            name = string.IsNullOrEmpty(name) ? type.Name : name;
            var assetPath = GetUniqueAssetFilePath(name, folderPath);
            var loadedInstance = CreateScriptableObject(type, assetPath);
            SelectAndPing(loadedInstance);
            return loadedInstance;
        }

        static ScriptableObject CreateScriptableObject(Type type, string assetPath)
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
                
            var loadedInstance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
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

        public static void SelectAndPing(Object obj)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public static ScriptableGroup PrepareGroup(string groupName)
        {
            var existingGroup = FindGroup(groupName);
            if (existingGroup)
                return existingGroup;

            var groupPath = $"Assets/Resources/SOQuick/{groupName}.asset";
            var directory = Path.GetDirectoryName(groupPath);
            if (string.IsNullOrEmpty(directory))
                throw new InvalidOperationException("Directory path is empty.");
                
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return CreateScriptableObject(typeof(ScriptableGroup), groupPath) as ScriptableGroup;
        }

        public static Texture GetIconForType(Type type)
        {
           if (type.IsSubclassOf(typeof(ScriptableObject)))
               return GetScriptableObjectIcon();
           if (type.IsSubclassOf(typeof(GameObject)) || type.IsSubclassOf(typeof(Component)) || type.IsSubclassOf(typeof(MonoBehaviour)))
               return GetPrefabIcon();

           var typeName = type.Name;
           var commonAssetTypes = new string[] { "Sprite" , "AudioClip", "Texture", "Texture2D", "Material" , "TextAsset", "VideoClip", "AnimationClip", "Mesh", "Animator", "AnimatorController" , "AnimatorOverrideController", "Avatar"};
           var mightBeCommonAssetType = commonAssetTypes.Contains(typeName) ? EditorGUIUtility.IconContent(typeName + " Icon").image : null;
           return mightBeCommonAssetType != null ? mightBeCommonAssetType : GetObjectIcon();
        }

        public static Object LoadAssetAtPath(string path, Type type)
        {
            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        public static string GetAssetPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }
    }
}