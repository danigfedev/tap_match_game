using UnityEditor;
using UnityEngine;

namespace _AppAssets.Code.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var gameManager = (GameManager)target;
            
            DrawDefaultInspector();
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Reset Board"))
            {
                gameManager.ResetBoard();
            }
        }
    }
}