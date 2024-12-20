using UnityEngine;
using Unity.Netcode;

public class MyNetworkManager : NetworkManager
{
   private bool   isCountingFrames = false;
   private int    frameCount       = 0;
   private double targetStartTime  = 0;

   private void Start()
   {
      if (IsHost)
      {
         // 호스트가 시작할 때 프레임 동기화 설정
         SetTargetStartTime();
         isCountingFrames = false;
         frameCount       = 0;

         // 추가 클라이언트 접속 시 콜백 등록
         NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
      }
   }

   private void SetTargetStartTime()
   {
      // 현재 시간에 5초를 더해 모든 클라이언트에 전달할 타임스탬프 설정
      targetStartTime = NetworkManager.Singleton.ServerTime.Time + 5.0;
      StartFrameCountingClientRpc(targetStartTime);
   }

   private void OnClientConnected(ulong clientId)
   {
      // 추가 클라이언트가 연결될 때 동기화 시간 전달
      Debug.Log(clientId + " is now connected");
      if (IsHost)
      {
         StartFrameCountingClientRpc(targetStartTime);
      }
   }

   [ClientRpc]
   private void StartFrameCountingClientRpc(double startTime)
   {
      targetStartTime  = startTime;
      isCountingFrames = false; // 시작 시간에 도달할 때까지 대기
   }

   private void Update()
   {
      double currentTime = NetworkManager.Singleton.ServerTime.Time;

      // 설정된 시작 시간에 도달한 경우 카운트 시작
      if (!isCountingFrames && currentTime >= targetStartTime)
      {
         isCountingFrames = true;
         frameCount       = 0; // 프레임 카운트를 0으로 초기화
      }

      if (isCountingFrames)
      {
         frameCount++;
         // 게임 기능에 frameCount 활용
      }
   }
}
