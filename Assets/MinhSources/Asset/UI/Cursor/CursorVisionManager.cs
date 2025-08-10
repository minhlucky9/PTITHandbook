using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorVisionManager : MonoBehaviour
{
    public Texture2D cursorNormal;
    public Texture2D cursorClick;
    float offset = 25f;
    void Awake()
    {
        //DontDestroyOnLoad(this);
        Cursor.SetCursor(cursorNormal, Vector3.zero, CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorClick, Vector3.one * offset, CursorMode.ForceSoftware);
        } else if(Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursorNormal, Vector3.one * offset, CursorMode.ForceSoftware);
        }
    }
}
