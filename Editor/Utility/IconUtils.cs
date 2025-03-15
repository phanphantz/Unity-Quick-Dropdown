using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    public static class IconUtils
    {
        static Texture GetObjectIcon()
        {
            if (unsafeObjectIcon)
                return unsafeObjectIcon;

            unsafeObjectIcon = EditorGUIUtility.IconContent("d_Prefab On Icon").image;
            return unsafeObjectIcon;
        }

        static Texture GetPrefabIcon()
        {
            if (unsafePrefabIcon)
                return unsafePrefabIcon;

            unsafePrefabIcon = EditorGUIUtility.IconContent("d_Prefab Icon").image;
            return unsafePrefabIcon;
        }

        public static Texture GetScriptableObjectIcon()
        {
            if (unsafeScriptableObjectIcon)
                return unsafeScriptableObjectIcon;

            unsafeScriptableObjectIcon = EditorGUIUtility.IconContent("ScriptableObject Icon").image;
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

            unsafeFolderIcon = EditorGUIUtility.IconContent("d_FolderOpened Icon").image;
            return unsafeFolderIcon;
        }

        static Texture unsafeScriptableObjectIcon;
        static Texture unsafeWarningIcon;
        static Texture unsafeFolderIcon;
        static Texture unsafePrefabIcon;
        static Texture unsafeObjectIcon;

        public static Texture GetIconForType(Type type)
        {
            if (type == typeof(string))
                return null;
            if (type.IsSubclassOf(typeof(ScriptableObject)))
                return GetScriptableObjectIcon();
            if (type.IsSubclassOf(typeof(GameObject)) || type.IsSubclassOf(typeof(Component)) ||
                type.IsSubclassOf(typeof(MonoBehaviour)))
                return GetPrefabIcon();

            var typeName = type.Name;
            var commonAssetTypes = new string[]
            {
                "Sprite", "AudioClip", "Texture", "Texture2D", "Material", "TextAsset", "VideoClip", "AnimationClip",
                "Mesh", "Animator", "AnimatorController", "AnimatorOverrideController", "Avatar"
            };
            var mightBeCommonAssetType = commonAssetTypes.Contains(typeName)
                ? EditorGUIUtility.IconContent(typeName + " Icon").image
                : null;
            return mightBeCommonAssetType != null ? mightBeCommonAssetType : GetObjectIcon();
        }
    }
}