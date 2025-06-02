using UnityEngine;
using UnityEngine.UI;

public class PressKeyToClickButton : MonoBehaviour
{
    [SerializeField] private Button targetButton;

    void Update()
    {
        // Kiểm tra nếu phím Z được nhấn xuống
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Kích hoạt sự kiện onClick của button
            targetButton.onClick.Invoke();
        }
    }
}
