using System;
using System.Collections;
using System.Globalization;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance = null;
    public  static GameManager Instance => _instance == null ? null : _instance;
    
    private bool _isAllClientsConnected = false;
    private bool _isGameReady           = false;

    [Header("Local Player")] 
    public ThirdPersonController localPlayer;
    
    [Header("Game Start Info Panel")] 
    public GameObject startPanel;
    public TMP_Text startText;

    [Header("Counts Score")] 
    public Image scoreImage0;
    public Image scoreImage1;

    public int player0BlockCounts = 0;
    public int player1BlockCounts = 0;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer) 
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public bool IsAllClientsConnected()
    {
        return _isAllClientsConnected;
    }

    public bool IsGameReady()
    {
        return _isGameReady;
    }
    
    private void OnClientConnected(ulong clientId)
    {
        // 현재 연결된 클라이언트 수 확인
        int connectedClients = NetworkManager.ConnectedClientsList.Count;

        // 클라이언트가 2명 이상 연결되었는지 확인
        if (connectedClients >= 2)
        {
            SetReadyFlagServerRpc(true); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyFlagServerRpc(bool readyFlag)
    {
        _isAllClientsConnected = readyFlag;
        Debug.Log($"All clients ready: {_isAllClientsConnected}");
        
        UpdateReadyFlagClientRpc(_isAllClientsConnected);
    }

    [ClientRpc]
    private void UpdateReadyFlagClientRpc(bool readyFlag)
    {
        _isAllClientsConnected = readyFlag;
        Debug.Log($"isReady updated on client: {_isAllClientsConnected}");

        if (_isAllClientsConnected)
        {
            StartCoroutine(CoStartCountDown(5f));
        }
    }

    private IEnumerator CoStartCountDown(float time)
    {
        startPanel.SetActive(true);
        while (time > 0)
        {
            startText.text = time.ToString(CultureInfo.InvariantCulture);
            yield return new WaitForSeconds(1f);
            time--;
        }

        startText.text = "Game Start!";
        yield return new WaitForSeconds(1f);
        startPanel.SetActive(false);
        
        SkillManager.Instance.Initialize();
        _isGameReady = true;
    }

    public void RecalculateScoreBarImage(int hashSize)
    {
        scoreImage0.fillAmount = (float)player0BlockCounts / hashSize;
        scoreImage1.fillAmount = (float)player1BlockCounts / hashSize;
    }
}
