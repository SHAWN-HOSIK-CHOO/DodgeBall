using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIManager : MonoBehaviour
{
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost();});
        _clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient();});
    }
}
