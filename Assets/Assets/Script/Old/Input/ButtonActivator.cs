using UnityEngine;
using UnityEngine.UI;

public class ButtonActivator : MonoBehaviour
{
    public Button myButton; // Gán button t? Inspector
    public Button myButton1; // Gán button t? Inspector
    public Button myButton2; // Gán button t? Inspector
    public Button myButton3; // Gán button t? Inspector

    public bool IsUIShow = false;

    public KeyCode keyToPress = KeyCode.Space; // Phím b?n mu?n s? d?ng ?? kích ho?t button

    void Update()
    {
        if (!IsUIShow)
        {
            // Ki?m tra xem phím có ???c nh?n hay không
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // G?i hàm OnClick c?a button khi phím ???c nh?n
                myButton.onClick.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // G?i hàm OnClick c?a button khi phím ???c nh?n
                myButton3.onClick.Invoke();
            }
        }
       
        if (Input.GetKeyDown(KeyCode.O))
        {
            // G?i hàm OnClick c?a button khi phím ???c nh?n
            myButton1.onClick.Invoke();

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // G?i hàm OnClick c?a button khi phím ???c nh?n
            myButton2.onClick.Invoke();
        }
       
    }

    public void ISShow()
    {
        IsUIShow = true;
    }
    public void ISShow2()
    {
        IsUIShow = false;
    }
}
