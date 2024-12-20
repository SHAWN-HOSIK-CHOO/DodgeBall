using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
   public GameObject playerPrefab; // Inspector에서 플레이어 프리팹 할당

   private void Start()
   {
      // NetworkManager.Singleton이 null이 아닌 경우에만 콜백을 등록합니다.
      if (NetworkManager.Singleton != null)
      {
         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
      }
      else
      {
         Debug.LogError("NetworkManager.Singleton is not initialized.");
      }
   }

   private void OnClientConnected(ulong clientId)
   {
      if (IsServer)
      {
         // 클라이언트가 연결될 때마다 플레이어 오브젝트를 수동으로 생성
         if (playerPrefab != null)
         {
            var playerObject  = Instantiate(playerPrefab);
            var networkObject = playerObject.GetComponent<NetworkObject>();

            // playerObject를 클라이언트의 소유로 설정하여 네트워크에 스폰
            networkObject.SpawnAsPlayerObject(clientId);
         }
         else
         {
            Debug.LogWarning("Player Prefab is not assigned in CustomNetworkManager.");
         }
      }
   }

   private void OnDisable()
   {
      if (NetworkManager.Singleton != null)
      {
         NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
      }
   }
}