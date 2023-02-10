using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라 정면에 팝업 되어야 하는 애한테 붙이세요
public class PopupUI : MonoBehaviour
{
    public bool isFixOnCameraRotation; //카메라 x,z회전에 따름
    float distance = 0.4f;

    void OnEnable()
    {
        SetCanvasPosition();
    }

    void SetCanvasPosition()
    {
        if (!isFixOnCameraRotation)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            transform.position = Camera.main.transform.position + forward * distance;
            transform.LookAt(Camera.main.transform);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + 180f, 0f);
        }
        else
        {
            Vector3 cameraForwardPos = Camera.main.transform.position;
            cameraForwardPos.z += distance;
            transform.position = cameraForwardPos;
            transform.rotation = Camera.main.transform.rotation;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
