using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
//using Microsoft.Unity.VisualStudio.Editor;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] CinemachineVirtualCamera hardLookCamera;
    [SerializeField] GameObject cameraReticle;
    [SerializeField] float tableCameraOffsetheight;
    [SerializeField] GameObject player;
    Image reticleImage;

    private void Start()
    {
        reticleImage = cameraReticle.GetComponent<Image>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void SwitchCameras()
    {
        if(followCamera.Priority > hardLookCamera.Priority)
        {
            followCamera.Priority = 0;
            hardLookCamera.Priority = 1;
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            cameraReticle.SetActive(false);
        }

        else
        {
            followCamera.Priority = 1;
            hardLookCamera.Priority = 0;
            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            cameraReticle.SetActive(true);
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

    public void ChangeReticleColor(Color color)
    {
        reticleImage.color = color;
    }
}
