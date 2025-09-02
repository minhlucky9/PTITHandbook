using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using PlayerController;

/// <summary>
/// Quản lý chuỗi UIAnimationController cho từng vật phẩm trong Trace Quest.
/// Tạo hàm OnClick_NextUI để gắn vào Button trong mỗi UI panel.
/// Khi người chơi nhấn E, UI đầu tiên sẽ hiện. Khi bấm button, UI hiện tại sẽ Deactivate,
/// sau 1 giây sẽ hiện UI tiếp theo. Với UI cuối, sau 1 giây sẽ gọi ConfirmTrace.
/// </summary>
public class TraceObjectInteraction : TalkInteraction
{
    [Header("UI Sequence Settings")]
    [Tooltip("Danh sách UIAnimationController sẽ hiển thị tuần tự")]
    public List<UIAnimationController> uiControllers;

    private int index;              // chỉ số vật phẩm trong quest
    private int sequenceIndex;      // chỉ số panel UI đang hiển thị

    /// <summary>
    /// Gọi sau khi Instantiate(prefab)
    /// </summary>
    public void Initialize(int index)
    {
        this.index = index;
        interactableText = "Nhấn E để tương tác";

        // Nếu chưa assign qua Inspector, tự động tìm trong children (bao gồm inactive)
        if (uiControllers == null || uiControllers.Count == 0)
        {
            uiControllers = new List<UIAnimationController>(
                GetComponentsInChildren<UIAnimationController>(true)
            );
        }

        // Đảm bảo mọi UI ban đầu đã deactivate
        foreach (var ui in uiControllers)
            ui.Deactivate();
    }

    public override void Interact()
    {
      //  base.Interact();              
        PlayerManager.instance.isInteract = true;

        StartCoroutine(SmoothTransitionToTraceMiniGame());
        sequenceIndex = 0;
       
        if (uiControllers.Count > 0)
            uiControllers[sequenceIndex].Activate();
    }


    public void OnClick_NextUI()
    {
        if (sequenceIndex < uiControllers.Count)
            uiControllers[sequenceIndex].Deactivate();

       
        StartCoroutine(ShowNextAfterDelay());
    }

    private IEnumerator ShowNextAfterDelay()
    {
      


        sequenceIndex++;

        if (sequenceIndex < uiControllers.Count)
        {
            yield return new WaitForSeconds(1f);
            uiControllers[sequenceIndex].Activate();
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
            PlayerManager.instance.ActivateController();
            TraceQuestManager.instance.ConfirmTrace(index);
           
        }
    }

    /// <summary>
    /// Tắt tất cả UI của object này (gọi từ TraceQuestManager khi hết thời gian)
    /// </summary>
    public void ForceCloseAllUI()
    {
        if (uiControllers != null)
        {
            foreach (UIAnimationController uiController in uiControllers)
            {
                if (uiController != null)
                {
                    uiController.Deactivate();
                }
            }
        }

        // Stop tất cả coroutines đang chạy
        StopAllCoroutines();

        // Reset sequence index
        sequenceIndex = 0;
    }

    public void StartPuzzle()
    {
        if (sequenceIndex < uiControllers.Count)
            uiControllers[sequenceIndex].Deactivate();

        StartCoroutine(ShowPuzzleAfterDelay());
    }

    public IEnumerator ShowPuzzleAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        TraceQuestManager.instance.InitPuzzle();
    }

}
