using DS.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.windows
{
    using System;
    
    public class DSGraphView : GraphView
    {
       public DSGraphView()
        {
            AddManimulator();
            AddGridBackGround();

           
            AddStyles();
        }

        private DSNode CreateNode(Vector2 position)
        {
            DSNode node = new DSNode();

            node.Initialize(position);
            node.Draw();

            return node;
        }

        private void AddGridBackGround()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);  
        }
        
        
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogSystem/DSGraphViewStyles.uss");

            styleSheets.Add(styleSheet);

        }

        private void AddManimulator()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            this.AddManipulator(CreateNodeContextualMenu());
            


        }

        private IManipulator CreateNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                   menuEvent => menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)))
                );  

            return contextualMenuManipulator;
        }
    }
}

