using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    [CustomPropertyDrawer(typeof(DropdownField))]
    public class DropdownFieldDrawer : PropertyDrawer
    {
        static Color linkColor = new Color(0, 0.6f, 0.8f);
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) 
                   + (!((DropdownField)attribute).IsHideInfo ? EditorGUIUtility.singleLineHeight + 3f : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            Rect detailRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
            var field = (DropdownField)attribute;
            if (field == null)
            {
                DrawFallbackField("Attribute is invalid");
                return;
            }

            var path = field.Path;
            if (string.IsNullOrEmpty(path))
            {
                DrawFallbackField("Path is null or empty");
                return;
            }

            var type = GetFieldType(property);
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
                type = typeof(GameObject);
            
            var finder = CreateFinder(field, type);
            var results = finder.SearchForItems();
            var objectNames = results
                .Select(result => result.Split('/').LastOrDefault())
                .ToArray();
            
            var currentIndex = -1;
            var currentObject = property.objectReferenceValue;
            if (currentObject)
            {
                //Get index by name first
                currentIndex = Array.IndexOf(objectNames, currentObject.name);
                
                //Recheck if the object reference actually belong to source
                if (currentIndex != -1 && !finder.IsBelongToSource(currentObject))
                    currentIndex = -1;
            }

            //Index 0 is NULL option
            currentIndex++;
            if (currentIndex == 0 && property.objectReferenceValue)
            {
                DrawFallbackField("This Object does not belong to path: " + path);
                return;
            }

            var baseOptions = new[] { new GUIContent("NULL", QuickDropdownEditorUtils.GetWarningIcon()) };
            var icon = QuickDropdownEditorUtils.GetIconForType(type);
            var options = baseOptions
                .Concat(results.Select(s => new GUIContent(s, icon)))
                .ToArray();

            var buttonWidth = 25f;
            var allButtonWidth = 0f;
            var isShouldDrawCreateButton = type.IsSubclassOf(typeof(ScriptableObject)) &&
                                           field is IHasCreateSOButton { IsHideCreateSOButton: false };
            if (isShouldDrawCreateButton)
                allButtonWidth += buttonWidth;
            if (!field.IsHideInspectButton)
                allButtonWidth += buttonWidth;

            Rect fieldRect = new Rect(position.x, position.y, position.width - allButtonWidth, lineHeight);
            Rect inspectButtonRect = new Rect(position.x + position.width - allButtonWidth, position.y, buttonWidth,
                lineHeight);
            Rect createButtonRect =
                new Rect(position.x + position.width - allButtonWidth + (field.IsHideInspectButton ? 0 : buttonWidth),
                    position.y, buttonWidth, lineHeight);

            var selectedIndex = EditorGUI.Popup(fieldRect, new GUIContent(label.text), currentIndex, options);
            if (selectedIndex != currentIndex)
            {
                if (selectedIndex != 0)
                {
                    var targetObject = finder.GetResultAtIndex(selectedIndex - 1); 
                    property.objectReferenceValue = targetObject;
                }
                else
                {
                    property.objectReferenceValue = null;
                }
                property.serializedObject.ApplyModifiedProperties();
            }

            if (!field.IsHideInspectButton)
                DrawInspectButton(property, inspectButtonRect, field);

            if (isShouldDrawCreateButton && GUI.Button(createButtonRect, "+"))
                finder.CreateNewScriptableObject();

            if (!field.IsHideInfo)
                DrawGroupInfo();

            void DrawGroupInfo()
            {
                var image = finder.GetSourceIcon();
                Rect iconRect = new Rect(detailRect.x + 10, detailRect.y + 3, 12, 12);
                Rect buttonRect = new Rect(detailRect.x + 30, detailRect.y, path.Length * 6f, detailRect.height);
                GUI.DrawTexture(iconRect, image, ScaleMode.ScaleToFit);
                var oldColor = GUI.color;
                GUI.color = finder.IsSourceValid()? linkColor : Color.yellow;
                if (GUI.Button(buttonRect, new GUIContent(path), EditorStyles.miniLabel))
                    finder.SelectAndPingSource();
                GUI.color = oldColor;
            }

            void DrawFallbackField(string reason)
            {
                var oldColor = GUI.color;
                GUI.color = Color.red;
                if (field is { IsHideInfo: false })
                {
                    var rect = position;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, property, label);
                    GUI.Label(detailRect, reason, EditorStyles.miniLabel);
                }
                GUI.color = oldColor;
            }
        }

        static void DrawInspectButton(SerializedProperty property, Rect inspectButtonRect, DropdownField field)
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
                        QuickDropdownEditorUtils.SelectAndPing(property.objectReferenceValue);
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
            var fieldInfo = targetType.GetField(property.propertyPath,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            return fieldInfo?.FieldType;
        }

        static ObjectFinder CreateFinder(DropdownField field, Type type)
        {
            if (field is FromFolder)
                return new FolderObjectFinder(field, type);
            
            if (field is FromGroup)
                return new ScriptableGroupFinder(field, type);
            
            throw new NotImplementedException($"Don't know how to get finder for {type}");
        }
    }
}