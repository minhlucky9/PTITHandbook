using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Assign 8 Panels ở đây (theo thứ tự)")]
    public GameObject[] panels;  // mảng 8 panel tương ứng với 8 button

    [Header("Assign 8 Button ở đây (theo thứ tự)")]
    public Button[] buttons;     // mảng 8 button

    private void Awake()
    {
        // Đảm bảo mảng đã được set đủ phần tử
        if (panels.Length != buttons.Length)
        {
            Debug.LogError("Số lượng panels và buttons phải bằng nhau!");
            return;
        }

        // Gán listener cho từng button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // bắt giá trị i vào biến cục bộ để tránh lỗi closure
            buttons[i].onClick.AddListener(() => ShowOnlyPanel(index));
        }

        // [Tuỳ chọn] Hiển thị panel đầu tiên khi start
        ShowOnlyPanel(0);
    }

    /// <summary>
    /// Hiển thị đúng panel tại vị trí index, ẩn tất cả panel còn lại.
    /// </summary>
    public void ShowOnlyPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
    }
}
