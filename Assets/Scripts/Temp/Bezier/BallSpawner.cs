using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BallSpawner : NetworkBehaviour
{
    [Header("Ball Prefab")]
    [SerializeField] private GameObject pfBasicBall;

    [SerializeField] private Transform ballSpawnPosition;

    [Header("DebugBtn")] public GameObject debugBtn;

    public void OnSpawnBall()
    {
        if(!IsOwner)
            return;

        GameObject ngo = Instantiate(pfBasicBall, ballSpawnPosition);
        ngo.GetComponent<NetworkObject>().Spawn();
    }
}
