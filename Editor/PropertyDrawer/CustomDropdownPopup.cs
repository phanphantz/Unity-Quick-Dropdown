using System;
using UnityEditor;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    public class CustomDropdownPopup : PopupWindowContent
    {
        GUIContent[] options; 
        Action<int> onSelect; 
        Vector2 scrollPosition;

        int currentIndex;
        float width;
        GUIStyle itemStyle;

        public CustomDropdownPopup(GUIContent[] options, float width, int currentIndex, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            this.currentIndex = currentIndex;
            this.width = width;
            
            itemStyle = new GUIStyle(EditorStyles.helpBox);
            itemStyle.alignment = TextAnchor.MiddleLeft;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(width, Mathf.Min(5000, (options.Length + 1) * EditorGUIUtility.singleLineHeight));
        }

        public override void OnGUI(Rect rect)
        {
            if (options == null || options.Length == 0)
            {
                EditorGUILayout.LabelField("No options available.");
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var oldColor = GUI.color;
            for (int i = 0; i < options.Length; i++)
            {
                GUI.color = i == currentIndex ? Color.green : oldColor;
                if (GUILayout.Button(options[i], itemStyle,GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    onSelect?.Invoke(i); // Invoke the callback with the selected option
                    editorWindow.Close(); // Close the popup after selection
                }
            }
            GUI.color = oldColor;
            EditorGUILayout.EndScrollView();
        }
    }
}