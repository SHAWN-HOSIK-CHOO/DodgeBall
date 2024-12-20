using System;
using Unity.Netcode;
using UnityEngine;

public class BallPhysicsController : NetworkBehaviour
{
   private Rigidbody rb;

   private void Start()
   {
      rb = GetComponent<Rigidbody>();
        
      // 서버와 클라이언트마다 다른 설정 적용
      if (IsServer || IsHost)
      {
         // 서버에서는 Rigidbody가 물리 엔진에 의해 영향을 받도록 설정
         rb.isKinematic = true;
      }
      else
      {
         // 클라이언트에서는 위치를 서버로부터 받기만 하도록 설정
         rb.isKinematic = true;
      }
   }
   
   private void FixedUpdate()
   {
      if (!IsServer || !IsHost)
      {
         // 클라이언트에서는 물리 업데이트를 실행하지 않음
         return;
      }

      // 서버에서만 물리 연산을 처리
      // 물리적 움직임이나 힘을 적용하는 코드가 여기에 위치
   }
}