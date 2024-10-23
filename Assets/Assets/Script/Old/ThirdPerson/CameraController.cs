using System.Collections;
using System.Collections.Generic;
using static Models;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public JackController PlayerController;

    public CameraSettingModel settings;

    private Vector3 targetRotation;

    public GameObject yGymbal;
    private Vector3 yGymbalRotation;

    public float movementSmoothTime = 0.1f;
    private Vector3 movementVelocity = Vector3.zero;

    public Transform targetTransform;



   
    private void Update()
    {
        FollowCameraTarget();
        CameraRotation();
    }

    private void CameraRotation()
    {
        var viewInput = PlayerController.input_View;

        targetRotation.y += (settings.InvertedX ? -(viewInput.x * settings.SensitivityX) : (viewInput.x * settings.SensitivityX)) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(targetRotation);




        yGymbalRotation.x += (settings.InvertedY ? (viewInput.y * settings.SensitivityY) : -(viewInput.y * settings.SensitivityY)) * Time.deltaTime;
        yGymbalRotation.x = Mathf.Clamp(yGymbalRotation.x, settings.YClampMin, settings.YClampMax);
        yGymbal.transform.localRotation = Quaternion.Euler(yGymbalRotation);




        if (PlayerController.isTargetMode)
        {
            var currentRotation = PlayerController.transform.rotation;



            var newRotation = currentRotation.eulerAngles;
            newRotation.y = targetRotation.y;

            currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), settings.CharaterRotationSmoothDamp);

            PlayerController.transform.rotation = currentRotation;
        }
    }
    public void FollowCameraTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref movementVelocity, movementSmoothTime);
        transform.position = targetPosition;
    }
}
