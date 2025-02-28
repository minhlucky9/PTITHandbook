using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements
{
    using Enumerations;
    using UnityEngine.UIElements;

    public class DSNode : Node
    {
        public string DialogName { get; set; }

        public List<string> Choices { get; set; }

        public string Text { get; set; }

        public DSDialogType DialogueType { get; set; }

        public void Initialize(Vector2 position)
        {
            DialogName = "DialogName";
            Choices = new List<string>();
            Text = "Dialogue text.";

            SetPosition(new Rect(position,Vector2.zero));
        }

        public void Draw()
        {
            /* TITLE CONTAINER */
            TextField dialogNameTexField = new TextField()
            {
                value = DialogName
            };


            /* INPUT CONTAINER */
            titleContainer.Insert(0, dialogNameTexField);
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,Port.Capacity.Multi,typeof(bool));
            inputPort.portName = "Dialog Connection";

            inputContainer.Add(inputPort);

            /* EXTENSION CONTAINER */

            VisualElement customdatacontainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "dialogText"
            };

            TextField textTextField = new TextField()
            {
                value = Text
            };

            textFoldout.Add(textTextField);

            customdatacontainer.Add(textFoldout);

            extensionContainer.Add(customdatacontainer);

            RefreshExpandedState();
        }
    }
}

