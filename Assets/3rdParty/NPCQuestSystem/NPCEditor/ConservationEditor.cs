using Interaction;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class ConservationEditor : NodeEditor
{
	public static NPCConservationSO modifyConservation;

	bool creatingAnswer = false;
	Rect temporaryAnswer;
	bool blocking = false;

    private void OnEnable()
    {
		EditorUtility.SetDirty(modifyConservation);
    }

    private void OnDisable()
    {
		EditorUtility.ClearDirty(modifyConservation);
    }

    public static void ShowEditor(NPCConservationSO conservation)
	{
		modifyConservation = conservation;
		ConservationEditor editor = GetWindow<ConservationEditor>(conservation.name);
		editor.Init();
	}

	public override void Init()
	{
		base.Init();
		SetupNodeData();
	}

	private void SetupNodeData()
    {
		nodes.Clear();
		for (int i = 0; i < modifyConservation.dialogs.Count; i++)
		{
			Vector2 nodePos = modifyConservation.dialogs[i].nodePosition;
			nodes.Add(new Rect(nodePos.x, nodePos.y, 200, 100));
		}
		names = GetDialogIds();
		blocking = false;
	}

    public override void OnGUI()
    {
		if (blocking) return;

        base.OnGUI();
		//
		switch (Event.current.keyCode)
		{
			case KeyCode.Delete:
				// ...
				DeleteDialog(selectedNode);
				return;
			default:
				break;
		}
	}

    public override void CreateMenu(ref GenericMenu menu, Vector2 mousePosition)
    {
		menu.AddItem(new GUIContent("Dialog"), false, () => OnCreateDialog(mousePosition));
		menu.AddItem(new GUIContent("Answer"), false, () => OnCreateAnswer(mousePosition));
	}

	private void OnCreateAnswer(Vector2 mousePosition)
    {
		creatingAnswer = true;
		temporaryAnswer = new Rect(mousePosition.x, mousePosition.y, 200, 100);
		//enable line
		isConnect = true;
		selectedRect = temporaryAnswer;

	}

	private void OnCreateDialog(Vector2 mousePosition)
	{
		DialogConservation dialog = new DialogConservation();
		dialog.dialogId = GetNotCreatedId();
		dialog.nodePosition = mousePosition;
		//
		modifyConservation.dialogs.Add(dialog);
		//
		SetupNodeData();
	}

    public override void DrawNodes()
    {
        base.DrawNodes();
		//
		if(creatingAnswer)
        {
			temporaryAnswer = GUILayout.Window(nodes.Count + 1, temporaryAnswer, DrawAnswer, "Answer");
		}
		
	}

	void DrawAnswer(int id)
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown)
        {
			HideTemporaryAnswer();
		}
        //
        EditorGUILayout.Separator();
		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField(" ");
		EditorStyles.label.wordWrap = false;
		//
		GUI.DragWindow();
	}

	void HideTemporaryAnswer()
    {
		creatingAnswer = false;
		isConnect = false;
		Repaint();
	}

    public override void DrawNodeContent(int i)
    {
		if (i >= modifyConservation.dialogs.Count) return;
		EditorGUILayout.Separator();
		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField(modifyConservation.dialogs[i].message);
		EditorStyles.label.wordWrap = false;
	}

	public override void OnDragNode(int i)
    {
		if (i >= modifyConservation.dialogs.Count) return;

        base.OnDragNode(i);
		modifyConservation.dialogs[i].nodePosition = new Vector2(nodes[i].x, nodes[i].y);
	}

    public override void OnMouseDownNode(int id)
    {
        base.OnMouseDownNode(id);
		HideTemporaryAnswer();
	}

    public override void OnMouseDownMenu()
    {
        base.OnMouseDownMenu();
		HideTemporaryAnswer();
	}

    private string[] GetDialogIds()
    {
		string[] ids = new string[modifyConservation.dialogs.Count];
		for(int i = 0; i < modifyConservation.dialogs.Count; i ++)
        {
			ids[i] = modifyConservation.dialogs[i].dialogId;
		}

		return ids;
    }

	public string GetNotCreatedId()
    {
		int id = 0;
		while(true)
        {
			bool exist = false;
			for(int i = 0; i < modifyConservation.dialogs.Count; i ++)
            {
				if(modifyConservation.dialogs[i].dialogId.Equals("dialog" + (id + 1)))
                {
					id++;
					exist = true;
					break;
				}
            }

			if(!exist)
            {
				break;
			}
		}

		return "dialog" + (id + 1);

	}

	private void DeleteDialog(int id)
    {
		blocking = true;

		if (id >= 0 && id < modifyConservation.dialogs.Count)
        {
			selectedNode = -1;
			modifyConservation.dialogs.RemoveAt(id);
		}
		SetupNodeData();
		Repaint();
	}
}

#endif