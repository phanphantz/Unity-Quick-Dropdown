using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    [CustomPropertyDrawer(typeof(DropdownField))]
    public class DropdownFieldDrawer : PropertyDrawer
    {
        static readonly Color LinkColor = new Color(0, 0.6f, 0.8f);
        static readonly Color ErrorColor = new Color(0.9f, 0.3f, 0);

        DropdownField Field { get; set; }
        Rect Position { get; set; }
        Rect DetailRect { get; set; }
        SerializedProperty Property { get; set; }
        GUIContent Label { get; set; }
        Type Type { get; set; }
        ObjectFinder Finder { get; set; }
        
        bool IsUnityObject { get; set; }
        bool IsSourceValid { get; set; }

        string Path => Field.Path;
        float SingleLineHeight => EditorGUIUtility.singleLineHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label)
                   + (!((DropdownField)attribute).IsHideInfo ? EditorGUIUtility.singleLineHeight + 3f : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Position = position;
            Property = property;
            Label = label;

            DetailRect = new Rect(Position.x, Position.y + SingleLineHeight, Position.width, SingleLineHeight);

            //Get Type information
            Type = FieldUtils.GetFieldType(property);
            if (Type == null)
            {
                DrawDefaultField();
                return;
            }

            if (Type.IsSubclassOf(typeof(MonoBehaviour)))
                Type = typeof(GameObject);

            IsUnityObject = Type.IsSubclassOf(typeof(Object));

            //Get DropdownField
            Field = (DropdownField)attribute;
            if (Field == null)
            {
                DrawFallbackField("Attribute is invalid");
                return;
            }

            if (Field.CheckInvalid(out var error))
            {
                DrawFallbackField(error.Message);
                return;
            }

            //Get ObjectFinder
            Finder = ObjectFinderFactory.GetFinder(Field, Type);
            if (!Finder.IsTypeSupported(Type))
            {
                DrawFallbackField("This type is not supported by the attribute " + Field.GetType().Name);
                return;
            }

            IsSourceValid = Finder.CheckAndPrepareSource();
            if (!IsSourceValid)
            {
                DrawDefaultField();
            }
            else
            {
                if (!TryDrawContent())
                    return;
            }

            if (!Field.IsHideInfo)
                DrawGroupInfo();
        }

        void DrawDefaultField()
        {
            var rect = Position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, Property, Label);
        }

        void DrawFallbackField(string reason)
        {
            var oldColor = GUI.color;
            GUI.color = ErrorColor;
            if (Field is { IsHideInfo: false })
            {
                DrawDefaultField();
                GUI.Label(DetailRect, reason, EditorStyles.miniLabel);
            }

            GUI.color = oldColor;
        }

        bool TryDrawContent()
        {
            var isShouldDrawCreateButton =
                Type.IsSubclassOf(typeof(ScriptableObject)) && !Field.IsHideCreateSOButton;
            var isShouldDrawInspectButton = !Field.IsHideInspectButton && IsUnityObject;

            var buttonWidth = 25f;
            var allButtonWidth = 0f;
            if (isShouldDrawCreateButton)
                allButtonWidth += buttonWidth;
            if (isShouldDrawInspectButton)
                allButtonWidth += buttonWidth;

            Rect remainingRect = EditorGUI.PrefixLabel(Position, Label);
            if (GUI.Button(new Rect(remainingRect.x, Position.y, remainingRect.width - allButtonWidth, EditorGUIUtility.singleLineHeight),
                   GetCurrentItemName(), EditorStyles.popup))
            {
                if (!DrawDropdown(remainingRect.x,remainingRect.width - allButtonWidth))
                    return false;
            }

            if (isShouldDrawInspectButton)
                DrawInspectButton(allButtonWidth, buttonWidth);

            if (isShouldDrawCreateButton)
                DrawCreateButton(allButtonWidth, buttonWidth);

            return true;
        }

        GUIContent GetCurrentItemName()
        {
            return IsUnityObject ? 
                (Property.objectReferenceValue ? 
                    new GUIContent(Property.objectReferenceValue.name, IconUtils.GetIconForType(Type)) : GetNullItemContent()) : new GUIContent(Property.stringValue);
        }

        static GUIContent GetNullItemContent()
        {
            return new GUIContent("NULL", IconUtils.GetWarningIcon());
        }

        bool DrawDropdown(float positionX, float width)
        {
            var results = Finder.SearchForItems();
            var objectNames = results
                .Select(result => result.Split('/').LastOrDefault())
                .ToArray();
            
            var currentIndex = FindCurrentIndex(objectNames);
            var baseOptions = new[] { GetNullItemContent() };
            var icon = IconUtils.GetIconForType(Type);
            var options = baseOptions
                .Concat(results.Select(s => new GUIContent(s, icon)))
                .ToArray();
            
            //Draw the dropdown popup
            PopupWindow.Show(
                new Rect(positionX, Position.y + EditorGUIUtility.singleLineHeight, width, 0),
                new CustomDropdownPopup(options, width, currentIndex,selectedIndex =>
                {
                    if (selectedIndex == currentIndex) 
                        return;
                    
                    if (selectedIndex != 0)
                    {
                        var targetObject = Finder.GetResultAtIndex(selectedIndex - 1);
                        ApplyChangeToProperty(targetObject);
                    }
                    else
                    {
                        ApplyChangeToProperty(null);
                    }
                }));
            return true;
        }

        int FindCurrentIndex(string[] objectNames)
        {
            var currentIndex = -1;
            Object currentObject = null;
            string rawAddress = "";
            if (IsUnityObject)
            {
                currentObject = Property.objectReferenceValue;
            }
            else if (Type == typeof(string))
            {
                rawAddress = Property.stringValue;
            }

            if (currentObject || !string.IsNullOrEmpty(rawAddress))
            {
                currentIndex = Array.IndexOf(objectNames, IsUnityObject && currentObject ? currentObject.name : rawAddress);

                //Recheck if the object reference really belong to source
                if (currentIndex != -1 && !Finder.IsBelongToSource(IsUnityObject ? currentObject : rawAddress))
                    currentIndex = -1;
            }

            //Index 0 is NULL option
            currentIndex++;
            if (currentIndex == 0 && currentObject)
            {
                Debug.LogWarning($"The Object '{currentObject.name}' does not belong to path: {Path}");
                return -1;
            }
            return currentIndex;
        }

        void ApplyChangeToProperty(Object targetObject)
        {
            if (IsUnityObject)
                Property.objectReferenceValue = targetObject;
            else if (Type == typeof(string))
                Property.stringValue = targetObject ? targetObject.name : string.Empty;
            Property.serializedObject.ApplyModifiedProperties();
        }
        
        void DrawCreateButton(float allButtonWidth, float buttonWidth)
        {
            var createButtonRect = new Rect(
                Position.x + Position.width - allButtonWidth + (Field.IsHideInspectButton ? 0 : buttonWidth),
                Position.y, buttonWidth, SingleLineHeight);

            if (GUI.Button(createButtonRect, "+"))
                Finder.CreateNewScriptableObject();
        }

        void DrawInspectButton(float allButtonWidth, float buttonWidth)
        {
            var inspectButtonRect = new Rect(Position.x + Position.width - allButtonWidth, Position.y,
                buttonWidth,
                SingleLineHeight);
            DrawObjectInspectButton(Property, inspectButtonRect, Field);
        }

        void DrawGroupInfo()
        {
            var image = Finder.GetSourceIcon();

            Rect iconRect = new Rect(DetailRect.x + 10, DetailRect.y + 3, 12, 12);
            Rect buttonRect = new Rect(DetailRect.x + 30, DetailRect.y, Path.Length * 6f, DetailRect.height);
            GUI.DrawTexture(iconRect, image, ScaleMode.ScaleToFit);
            var oldColor = GUI.color;

            GUI.color = IsSourceValid ? LinkColor : Color.yellow;
            if (GUI.Button(buttonRect, new GUIContent(Path, IsSourceValid ? "Click to search for the source again" : "Click to Jump to the source"), EditorStyles.miniLabel))
            {
                if (IsSourceValid)
                    Finder.SelectAndPingSource();
                else
                    Finder.SearchAndCacheSource();
            }

            if (!IsSourceValid)
            {
                buttonRect.x += buttonRect.width;
                buttonRect.width = 30f;
                GUI.color = oldColor;
                if (GUI.Button(buttonRect, new GUIContent("Fix", "Search for a source with the specified name or Create it if not found."), EditorStyles.miniButton))
                    Finder.CreateOrGetSourceFromInspector();
            }

            GUI.color = oldColor;
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
                        AssetUtils.SelectAndPingInProjectTab(property.objectReferenceValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}