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

        public CustomDropdownPopup(GUIContent[] options, int currentIndex, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            this.currentIndex = currentIndex;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, Mathf.Min(300, options.Length * EditorGUIUtility.singleLineHeight));
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
                if (GUILayout.Button(options[i], GUILayout.Height(EditorGUIUtility.singleLineHeight)))
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