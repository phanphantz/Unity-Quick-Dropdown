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

        public CustomDropdownPopup(GUIContent[] options, float width, int currentIndex, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            this.currentIndex = currentIndex;
            this.width = width;
            
            itemStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
            };
        }

        public override Vector2 GetWindowSize()
        {
            var optionCount = options.Length;
            var height = (optionCount == 1 ? 1 : optionCount + 1) * EditorGUIUtility.singleLineHeight;
            height += 3f;

            if (optionCount > 0)
            {
                var longestItem = options
                    .OrderByDescending(o => o.text.Length)
                    .FirstOrDefault();

                width = Mathf.Max(width, GUI.skin.label.CalcSize(longestItem).x);
            }
            return new Vector2(width, Mathf.Min(5000, height));
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
                if (GUILayout.Button(options[i], itemStyle,GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    onSelect?.Invoke(i); // Invoke the callback with the selected option
                    editorWindow.Close(); // Close the popup after selection
                }
                
                if (isSelected)
                {
                    GUIContent checkmarkContent = EditorGUIUtility.IconContent("d_FilterSelectedOnly@2x"); // This is one of the checkmark icons available in Unity. The name might change based on Unity version and skin.
        
                    // Calculate the position for the checkmark icon
                    Rect lastRect = GUILayoutUtility.GetLastRect(); // Gets the rect of the last drawn layout element, which in this case is the button.
                    float checkmarkSize = EditorGUIUtility.singleLineHeight; // Use the single line height as the size for the checkmark for consistency.
                    Rect checkmarkRect = new Rect(lastRect.xMax - checkmarkSize, lastRect.y, checkmarkSize, checkmarkSize);
        
                    // Draw the checkmark icon at the end of the line
                    GUI.Label(checkmarkRect, checkmarkContent);
                }
                
                if (i < options.Length - 1) // Avoid drawing after the last item
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float lineHeight = 1; // Thickness of the line
                    Rect lineRect = new Rect(lastRect.x, lastRect.yMax, lastRect.width, lineHeight);
                    EditorGUI.DrawRect(lineRect, new Color(1,1,1,0.2f)); // Draw a grey line
                }
            }
        }
    }
}