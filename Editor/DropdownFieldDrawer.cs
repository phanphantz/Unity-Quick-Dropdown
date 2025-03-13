using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    [CustomPropertyDrawer(typeof(DropdownField))]
    public class DropdownFieldDrawer : PropertyDrawer
    {
        static Color linkColor = new Color(0, 0.6f, 0.8f);
        static Color errorColor = new Color(0.9f, 0.3f,0);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label)
                   + (!((DropdownField) attribute).IsHideInfo && GetFieldType(property) != null ? EditorGUIUtility.singleLineHeight + 3f : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = GetFieldType(property);
            if (type == null)
            {
                DrawDefaultField(position, property, label);
                return;
            }
            
            var lineHeight = EditorGUIUtility.singleLineHeight;
            Rect detailRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
            var field = (DropdownField) attribute;
            if (field == null)
            {
                DrawFallbackField("Attribute is invalid");
                return;
            }
            
            if (field.CheckInvalid(out var error))
            {
                DrawFallbackField(error.Message);
                return;
            }

            var path = field.Path;
            var isUnityObject = type.IsSubclassOf(typeof(Object));
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
                type = typeof(GameObject);

            var finder = CreateFinder(field, type);
            if (!finder.IsTypeSupported(type))
            {
                DrawFallbackField("This type is not supported by the attribute " + field.GetType().Name);
                return;
            }
            
            var isSourceValid = finder.CheckAndPrepareSource();
            if (!isSourceValid)
            {
                var rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property, label);
            }
            else
            {
                var results = finder.SearchForItems();
                var objectNames = results
                    .Select(result => result.Split('/').LastOrDefault())
                    .ToArray();

                var currentIndex = -1;
                var indexSearchResult = FindCurrentIndexResult(currentIndex, objectNames);
                if (!indexSearchResult.isValid)
                    return;
                
                currentIndex = indexSearchResult.index;
                var baseOptions = new[] {new GUIContent("NULL", QuickDropdownEditorUtils.GetWarningIcon())};
                var icon = QuickDropdownEditorUtils.GetIconForType(type);
                var options = baseOptions
                    .Concat(results.Select(s => new GUIContent(s, icon)))
                    .ToArray();

                var buttonWidth = 25f;
                var allButtonWidth = 0f;
                var isShouldDrawCreateButton = type.IsSubclassOf(typeof(ScriptableObject)) && !field.IsHideCreateSOButton;
                var isShouldDrawInspectButton = !field.IsHideInspectButton && isUnityObject;
                if (isShouldDrawCreateButton)
                    allButtonWidth += buttonWidth;
                if (isShouldDrawInspectButton)
                    allButtonWidth += buttonWidth;

                Rect fieldRect = new Rect(position.x, position.y, position.width - allButtonWidth, lineHeight);
                Rect inspectButtonRect = new Rect(position.x + position.width - allButtonWidth, position.y, buttonWidth,
                    lineHeight);
                Rect createButtonRect =
                    new Rect(
                        position.x + position.width - allButtonWidth + (field.IsHideInspectButton ? 0 : buttonWidth),
                        position.y, buttonWidth, lineHeight);

                var selectedIndex = EditorGUI.Popup(fieldRect, new GUIContent(label.text), currentIndex, options);
                if (selectedIndex != currentIndex)
                {
                    if (selectedIndex != 0)
                    {
                        var targetObject = finder.GetResultAtIndex(selectedIndex - 1);
                        ApplyChangeToProperty(targetObject);
                    }
                    else
                    {
                        ApplyChangeToProperty(null);
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }

                if (isShouldDrawInspectButton)
                    DrawObjectInspectButton(property, inspectButtonRect, field);

                if (isShouldDrawCreateButton && GUI.Button(createButtonRect, "+"))
                    finder.CreateNewScriptableObject();
            }
            
            if (!field.IsHideInfo)
                DrawGroupInfo();

            void DrawGroupInfo()
            {
                var image = finder.GetSourceIcon();
                Rect iconRect = new Rect(detailRect.x + 10, detailRect.y + 3, 12, 12);
                Rect buttonRect = new Rect(detailRect.x + 30, detailRect.y, path.Length * 6f, detailRect.height);
                GUI.DrawTexture(iconRect, image, ScaleMode.ScaleToFit);
                var oldColor = GUI.color;

                GUI.color = isSourceValid ? linkColor : Color.yellow;
                if (GUI.Button(buttonRect, new GUIContent(path), EditorStyles.miniLabel))
                    finder.SelectAndPingSource();

                if (!isSourceValid)
                {
                    buttonRect.x += buttonRect.width;
                    buttonRect.width = 52f;
                    GUI.color = oldColor;
                    if (GUI.Button(buttonRect, new GUIContent("Create"), EditorStyles.miniButton))
                        finder.CreateSourceIfNotExists();
                }

                GUI.color = oldColor;
            }
            
            (bool isValid, int index) FindCurrentIndexResult(int currentIndex, string[] objectNames)
            {
                Object currentObject = null;
                string rawAddress = "";
                if (isUnityObject)
                {
                    currentObject = property.objectReferenceValue;
                }
                else if (type == typeof(string))
                {
                    rawAddress = property.stringValue;
                }
                if (currentObject || !string.IsNullOrEmpty(rawAddress))
                {
                    //Get index by name first
                    currentIndex = Array.IndexOf(objectNames, isUnityObject ? currentObject.name : rawAddress);

                    //Recheck if the object reference actually belong to source
                    if (currentIndex != -1 && !finder.IsBelongToSource(isUnityObject ? currentObject : rawAddress))
                        currentIndex = -1;
                }
                //Index 0 is NULL option
                currentIndex++;
                if (currentIndex == 0 &&currentObject)
                {
                    DrawFallbackField("This Object does not belong to path: " + path);
                    return (false, -1);
                }
                return (true, currentIndex);
            }

            void DrawFallbackField(string reason)
            {
                var oldColor = GUI.color;
                GUI.color = errorColor;
                if (field is {IsHideInfo: false})
                {
                    DrawDefaultField(position, property, label);
                    GUI.Label(detailRect, reason, EditorStyles.miniLabel);
                }

                GUI.color = oldColor;
            }
            
            void ApplyChangeToProperty(Object targetObject)
            {
                if (isUnityObject)
                    property.objectReferenceValue = targetObject;
                else if (type == typeof(string))
                    property.stringValue = targetObject ? targetObject.name : string.Empty;
            }
        }

        static void DrawDefaultField(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property, label);
        }

        static void DrawObjectInspectButton(SerializedProperty property, Rect inspectButtonRect, DropdownField field)
        {
            EditorGUI.BeginDisabledGroup(property.objectReferenceValue == null);
            if (GUI.Button(inspectButtonRect, new GUIContent(EditorGUIUtility.IconContent("d_Search Icon"))))
            {
                switch (field.InspectMode)
                {
                    case InspectMode.OpenPropertyWindow:
                        EditorGUIUtility.PingObject(property.objectReferenceValue);
                        EditorUtility.OpenPropertyEditor(property.objectReferenceValue);
                        break;
                    case InspectMode.Select:
                        QuickDropdownEditorUtils.SelectAndPingInProjectTab(property.objectReferenceValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        static Type GetFieldType(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();
            return GetFieldViaPath(targetType, property.propertyPath)?.FieldType;
        }
        
        /// <summary>
        /// Taken from: https://discussions.unity.com/t/a-smarter-way-to-get-the-type-of-serializedproperty/186674/6
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldViaPath(Type type, string path)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var parent = type;
            var fi = parent.GetField(path, flags);
            var paths = path.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                fi = parent?.GetField(paths[i], flags);
                if (fi != null)
                {
                    // there are only two container field type that can be serialized:
                    // Array and List<T>
                    if (fi.FieldType.IsArray)
                    {
                            parent = fi.FieldType.GetElementType();
                            i += 2;
                        continue;
                    }
                    if (fi.FieldType.IsGenericType)
                    {
                            parent = fi.FieldType.GetGenericArguments()[0];
                            i += 2;
                        continue;
                    }
                    parent = fi.FieldType;
                }
                else
                {
                    break;
                }
            }
            if (fi == null)
                return type.BaseType != null ? GetFieldViaPath(type.BaseType, path) : null;
            
            return fi;
        }

        static ObjectFinder CreateFinder(DropdownField field, Type type)
        {
            return field switch
            {
                FromConfig => new ConfigFinder(field, type),
                FromFolder => new FolderObjectFinder(field, type),
                FromGroup => new ScriptableGroupFinder(field, type),
#if ADDRESSABLES_DROPDOWN
                PhEngine.QuickDropdown.Addressables.FromAddressable => new Addressables.AddressableEntryFinder(field, type),
#endif
                _ => throw new NotImplementedException($"Don't know how to get finder for {type}")
            };
        }
    }
}