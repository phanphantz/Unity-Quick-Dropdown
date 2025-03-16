using System;
using System.Linq;
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
        float lineHeight => EditorGUIUtility.singleLineHeight + 4;

        public CustomDropdownPopup(GUIContent[] options, float width, int currentIndex, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            this.currentIndex = currentIndex;
            this.width = width;
            
            itemStyle = new GUIStyle(EditorStyles.label)
            {
                fixedHeight = lineHeight,
                alignment = TextAnchor.MiddleLeft,
            };
        }

        public override Vector2 GetWindowSize()
        {
            var optionCount = options.Length;
            if (optionCount == 0)
                return Vector2.zero;
            
            var longestItem = options
                .OrderByDescending(o => o.text.Length)
                .FirstOrDefault();

            var longestSize = GUI.skin.label.CalcSize(longestItem);
            width = Mathf.Max(width, longestSize.x);
            var height = (optionCount * (lineHeight)) + (EditorGUIUtility.standardVerticalSpacing * (options.Length - 1));
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect rect)
        {
            if (options == null || options.Length == 0)
            {
                EditorGUILayout.LabelField("No options available.");
                return;
            }
            
            for (int i = 0; i < options.Length; i++)
            {
                var isSelected = i == currentIndex;
                itemStyle.fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal;
                if (GUILayout.Button(options[i], itemStyle,GUILayout.Height(lineHeight)))
                {
                    onSelect?.Invoke(i); // Invoke the callback with the selected option
                    editorWindow.Close(); // Close the popup after selection
                }
                if (isSelected)
                {
                    GUIContent checkmarkContent = EditorGUIUtility.IconContent("d_FilterSelectedOnly@2x"); // This is one of the checkmark icons available in Unity. The name might change based on Unity version and skin.
        
                    // Calculate the position for the checkmark icon
                    Rect lastRect = GUILayoutUtility.GetLastRect(); // Gets the rect of the last drawn layout element, which in this case is the button.
                    float checkmarkSize = lineHeight; // Use the single line height as the size for the checkmark for consistency.
                    Rect checkmarkRect = new Rect(lastRect.xMax - checkmarkSize, lastRect.y, checkmarkSize, checkmarkSize);
        
                    // Draw the checkmark icon at the end of the line
                    GUI.Label(checkmarkRect, checkmarkContent);
                }
                
                if (i < options.Length - 1) // Avoid drawing after the last item
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float lineHeight = 1; // Thickness of the line
                    Rect lineRect = new Rect(lastRect.x, lastRect.yMax, lastRect.width, lineHeight);
                    EditorGUI.DrawRect(lineRect, new Color(1,1,1,0.1f)); // Draw a grey line
                }
            }
        }
    }
}