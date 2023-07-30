using _AppAssets.Code.Settings;
using UnityEditor;
using UnityEngine;

namespace _AppAssets.Code.Editor
{
    [CustomEditor(typeof(DisplaySettings))]
    public class DisplaySettingsEditor : UnityEditor.Editor
    {
        private int _uiRectangleTotalHeight = 400;
        private float _aspectRatio = (float) 9 / 16;
        private DisplaySettings _settings;

        public override void OnInspectorGUI()
        {
            _settings = (DisplaySettings)target;
            DrawDefaultInspector();
            DrawSpace(10);
            DrawUIPreview();
        }

        private void DrawUIPreview()
        {
            var headerHeightPercentage = _settings.HeaderHeightScreenPercentage;
            var boardHeightPercentage = _settings.BoardHeightScreenPercentage;
            var footerHeightPercentage = _settings.FooterHeightScreenPercentage;
            
            float headerHeight = _uiRectangleTotalHeight * headerHeightPercentage;
            float boardHeight = _uiRectangleTotalHeight * boardHeightPercentage;
            float footerHeight = _uiRectangleTotalHeight * footerHeightPercentage;

            float rectangleWidth = _uiRectangleTotalHeight * _aspectRatio;
            
            DrawRectangleWithLabel(rectangleWidth, 15, Color.gray, "Preview");
            DrawSpace(2);
            DrawRectangleWithLabel(rectangleWidth, headerHeight, Color.red, $"Header: {headerHeightPercentage}");
            DrawRectangleWithLabel(rectangleWidth, boardHeight, Color.green, $"Board area: {boardHeightPercentage}");
            DrawRectangleWithLabel(rectangleWidth, footerHeight, Color.blue, $"Footer: {footerHeightPercentage}");
        }

        private void DrawRectangleWithLabel(float rectangleWidth, float headerHeight, Color rectangleColor, string label)
        {
            float horizontalSpace = (EditorGUIUtility.currentViewWidth - rectangleWidth) / 2f;
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(rectangleWidth), GUILayout.Height(headerHeight));
            rect.x += horizontalSpace;
            
            EditorGUI.DrawRect(rect, rectangleColor);
            DrawLabel(rect, label, Color.black);
        }

        private void DrawLabel(Rect rect, string label, Color labelColor)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = labelColor;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(rect, label, labelStyle);
        }

        private void DrawSpace(int size)
        {
            EditorGUILayout.Space(size);
        } 
    }
}