using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.windows
{
    public class DSEditorWindow : EditorWindow
    {
        [MenuItem("Window/DS/Dialog Graph")]
        public static void ShowExample()
        {
             GetWindow<DSEditorWindow>("Dialog Graph");
       
        }

      private void OnEnable()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView();

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogSystem/DSVariables.uss");

            rootVisualElement.styleSheets.Add(styleSheet);

        }
    }
}

