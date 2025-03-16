using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    public class CustomDropdownPopup : PopupWindowContent
    {
        DropdownOption[] options; 
        Action<int> onSelect; 
        Vector2 scrollPosition;

        int currentIndex;
        GUIStyle itemStyle;
        Texture icon;
        Rect rect;
        
        float lineHeight => EditorGUIUtility.singleLineHeight + 4;

        public CustomDropdownPopup(Texture icon, DropdownOption[] options, int currentIndex, Action<int> onSelect)
        {
            this.options = options;
            this.onSelect = onSelect;
            this.currentIndex = currentIndex;
            this.icon = icon;
            
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

            var longestSize = GUI.skin.label.CalcSize(new GUIContent(longestItem.text, icon));
            var height = (optionCount * (lineHeight)) + (EditorGUIUtility.standardVerticalSpacing * (options.Length - 1));
            return new Vector2(longestSize.x, height);
        }

        public override void OnGUI(Rect rect)
        {
            this.rect = rect;
            if (options == null || options.Length == 0)
            {
                EditorGUILayout.LabelField("No options available.");
                return;
            }
            DrawOptions(options);
        }

        void DrawOptions(DropdownOption[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                var isSelected = i == currentIndex;
                itemStyle.fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal;
                var text = options[i].text;
                
                Texture itemIcon = null;
                var isGroup = options[i].options.Count != 0;
                if (!isGroup)
                    itemIcon = text == "NULL" ? IconUtils.GetWarningIcon() : icon;
                
                if (GUILayout.Button(new GUIContent(text, itemIcon), itemStyle,GUILayout.Height(lineHeight)))
                {
                    if (isGroup)
                    {
                        PopupWindow.Show(GUILayoutUtility.GetLastRect(), new CustomDropdownPopup(icon, options[i].options.ToArray(), currentIndex,onSelect));
                    }
                    else
                    {
                        onSelect?.Invoke(i);
                        editorWindow.Close(); 
                    }
                }
                
                if (isSelected && !isGroup)
                {
                    GUIContent checkmarkContent = EditorGUIUtility.IconContent("d_FilterSelectedOnly@2x"); 
                    
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float checkmarkSize = lineHeight; 
                    Rect checkmarkRect = new Rect(lastRect.xMax - checkmarkSize, lastRect.y, checkmarkSize, checkmarkSize);
                    GUI.Label(checkmarkRect, checkmarkContent);
                }
                if (i < options.Length - 1) 
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    float lineHeight = 1;
                    Rect lineRect = new Rect(lastRect.x, lastRect.yMax, lastRect.width, lineHeight);
                    EditorGUI.DrawRect(lineRect, new Color(1,1,1,0.1f));
                }
            }
        }
    }
}