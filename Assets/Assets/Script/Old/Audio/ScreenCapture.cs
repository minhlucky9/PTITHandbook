using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotCapture : MonoBehaviour
{
    public GameObject Screenshot;

    [Header("MovementFreezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;

    public void CaptureScreenshot()
    {
        // Bắt đầu Coroutine để chụp màn hình và xử lý tiếp theo
        StartCoroutine(CaptureAndHandleScreenshot());
    }

    private IEnumerator CaptureAndHandleScreenshot()
    {
        // Đường dẫn tới thư mục Pictures/Screenshots
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Screenshots");

        // Tạo thư mục nếu chưa có
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Tên file với thời gian hiện tại để không bị trùng lặp
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, fileName);

        // Chụp ảnh màn hình và chờ 1 frame để đảm bảo quá trình chụp hoàn tất
        ScreenCapture.CaptureScreenshot(filePath);
        yield return new WaitForEndOfFrame();  // Chờ đến cuối frame hiện tại

        // Kiểm tra nếu đường dẫn hợp lệ và sau đó hiển thị Screenshot
        if (filePath != null)
        {
            Screenshot.SetActive(true);
        }

        // Xử lý trạng thái của các controller dựa trên character index
        if (character.index == 0)
        {
            MovementToggle.isCheck = false;
            CameraToggle.isCheck = false;
        }
        else
        {
            MovementToggle2.isCheck = false;
            CameraToggle2.isCheck = false;
        }

        Debug.Log("Screenshot saved to: " + filePath);
    }
}
