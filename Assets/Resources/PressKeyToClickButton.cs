using PlayerController;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PressKeyToClickButton : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    public Transform Safezone;
    public Transform Player;
    public Transform Cam;

    void Update()
    {
        // Kiểm tra nếu phím Z được nhấn xuống
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Kích hoạt sự kiện onClick của button
            targetButton.onClick.Invoke();
        }
    }


    public void ReturnToSafeZone()
    {
        var player = PlayerManager.instance;
        var camHandle = CameraHandle.singleton;
        var locomotion = player.GetComponent<PlayerLocomotion>();
        if (player == null || Safezone == null || camHandle == null || locomotion == null)
            return;

        // 1) Tính offset camera (3m sau + 2m cao)
        Vector3 camOffset = -player.transform.forward * 3f + Vector3.up * 2f;

        // 2) Tắt scripts ngay lập tức
        PlayerManager.instance.DeactivateController();

        // 3) Start coroutine: đợi 3s → teleport → bật lại
        StartCoroutine(TeleportAndReenable(player, camHandle, locomotion, camOffset));
    }

    private IEnumerator TeleportAndReenable(
        PlayerManager player,
        CameraHandle camHandle,
        PlayerLocomotion locomotion,
        Vector3 camOffset)
    {
        // đợi 3 giây trước khi dịch chuyển
        yield return new WaitForSeconds(1f);

        // 4) Teleport player và rig‐root camera
        player.transform.position = Safezone.position;
        camHandle.transform.position = player.transform.position + camOffset;
      //  camHandle.ClearLockOnTarget();

        // 5) Bật lại scripts để resume behaviour
        PlayerManager.instance.ActivateController();
    }
}
