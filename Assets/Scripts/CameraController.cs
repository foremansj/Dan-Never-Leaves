using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] CinemachineVirtualCamera hardLookCamera;
    [SerializeField] float tableCameraOffsetheight;
    [SerializeField] GameObject player;

    public void SwitchCameras()
    {
        if(followCamera.Priority > hardLookCamera.Priority)
        {
            followCamera.Priority = 0;
            hardLookCamera.Priority = 1;
        }

        else
        {
            followCamera.Priority = 1;
            hardLookCamera.Priority = 0;
        }
    }

    public void HardLookAtObject(GameObject lookObject)
    {
        hardLookCamera.LookAt = lookObject.transform;
    }

    public void MoveHardLookCamera(Transform transform)
    {
        hardLookCamera.transform.position = transform.position - new Vector3(0, transform.position.y, 0) + 
                                            new Vector3(0, tableCameraOffsetheight, 0);
    }
}
