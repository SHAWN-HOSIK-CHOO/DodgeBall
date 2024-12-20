using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraSetup : NetworkBehaviour
{
    public GameObject followCamera;
    public GameObject aimCamera;

    public void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        
        followCamera = GameObject.FindWithTag("FollowCamera");
        aimCamera    = GameObject.FindWithTag("AimCamera");

        if (followCamera == null)
        {
            Debug.Log("followCamera Setup Error ");
        }
        else if (aimCamera == null)
        {
            Debug.Log("aimCamera Setup Error ");
        }
        else
        {
            followCamera.GetComponent<CinemachineCamera>().LookAt = this.transform;
            followCamera.GetComponent<CinemachineCamera>().Follow = this.transform;

            aimCamera.GetComponent<CinemachineCamera>().LookAt = this.transform;
            aimCamera.GetComponent<CinemachineCamera>().Follow = this.transform;
        }
    }
}

    
