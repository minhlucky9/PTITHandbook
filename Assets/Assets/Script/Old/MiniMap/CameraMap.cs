using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMap : MonoBehaviour
{
    public Transform CameraTransform;

    public float normalSpeed;
    public float fastSpeed;

    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;

    public Vector3 ZoomAmount;
    public Vector3 NewZoom;
    public Vector3 newPosition;
    public Quaternion newRotation;

    public Vector3 DragStartPosition;
    public Vector3 DragCurrentPosition;
    public Vector3 RotateStartPosition;
    public Vector3 RotateCurrentPosition;

    // Giới hạn di chuyển
    public Vector3 minPosition;
    public Vector3 maxPosition;

    // Giới hạn zoom
    public float minZoom;
    public float maxZoom;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        NewZoom = CameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        /*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            NewZoom += ZoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            NewZoom -= ZoomAmount;
        }
        */
        // Giới hạn vị trí mới
        newPosition = new Vector3(
            Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y),
            Mathf.Clamp(newPosition.z, minPosition.z, maxPosition.z)
        );

        // Giới hạn zoom mới
        NewZoom = new Vector3(
            Mathf.Clamp(NewZoom.x, minZoom, maxZoom),
            Mathf.Clamp(NewZoom.y, minZoom, maxZoom),
            Mathf.Clamp(NewZoom.z, minZoom, maxZoom)
        );

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, NewZoom, Time.deltaTime * movementTime);
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            NewZoom += Input.mouseScrollDelta.y * ZoomAmount;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                DragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                DragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + DragStartPosition - DragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            RotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            RotateCurrentPosition = Input.mousePosition;

            Vector3 difference = RotateStartPosition - RotateCurrentPosition;

            RotateStartPosition = RotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }
}
