using System;
using Unity.Netcode;
using UnityEngine;

public class FrameSyncManager : NetworkBehaviour
{
   public bool   isCountingFrames = false;
   public int    frameCount       = 0;
   public static double targetStartTime = 999;

   public int maxPlayerCount     = 2;
   public int currentPlayerCount = 0;

   public override void OnNetworkSpawn()
   {
      base.OnNetworkSpawn();
      
      if (IsHost)
      {
         // 호스트가 네트워크 활성화 후 동기화 시작 시간 설정
         isCountingFrames = false;
         frameCount       = 0;

         // 클라이언트 연결 시 콜백 등록 (네트워크 활성화 후에 등록됨)
         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
      }
   }

   private void OnClientConnected(ulong clientId)
   {
      Debug.Log(clientId + " is now connected");
      currentPlayerCount++;
      
      if(currentPlayerCount < maxPlayerCount)
         return;
      
      if (IsHost)
      {
         targetStartTime = NetworkManager.Singleton.ServerTime.Time + 10.0;
         StartFrameCountingClientRpc(targetStartTime);
      }
   }

   [ClientRpc]
   private void StartFrameCountingClientRpc(double startTime)
   {
      Debug.Log($"Client {NetworkManager.Singleton.LocalClientId} received StartFrameCountingClientRpc with startTime {startTime}");
      targetStartTime  = startTime;
      isCountingFrames = false; // 설정된 시간까지 대기
      //Debug.Log(NetworkManager.Singleton.LocalClientId + " 's target time is : " + _targetStartTime);
   }
   

   private void Update()
   {
      if(!IsOwner)
         return;
      
      // 서버와 동기화된 현재 시간 확인
      double currentTime = NetworkManager.Singleton.ServerTime.Time;
      
      //Debug.Log(this.GetInstanceID() + ": " + NetworkManager.Singleton.LocalClientId + " 's target time is : " + _targetStartTime);

      // 설정된 시작 시간에 도달하면 프레임 카운트 시작
      if (!isCountingFrames && currentTime >= targetStartTime)
      {
         isCountingFrames = true;
         frameCount       = 0; // 프레임 카운트를 0으로 초기화
         //Debug.Log(OwnerClientId + " target time : " + _targetStartTime + "  current time : " + currentTime);
      }

      if (isCountingFrames)
      {
         frameCount++;
         Debug.Log(OwnerClientId + " frame : ");
      }
   }

   public override void OnDestroy()
   {
      if (NetworkManager.Singleton != null)
      {
         NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
      }
   }
}