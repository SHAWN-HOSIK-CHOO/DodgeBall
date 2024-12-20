using System;
using UnityEngine;

public class BallScript : MonoBehaviour
{
   private Vector3 _startPosition;
   private Vector3 _targetPosition;
   private float   _throwSpeed;
   private float   _maxHeight;
   private float   _timeElapsed;
   private bool    _isInitialized;

   public EBallThrowMode currentThrowMode;

   public const int CurveMaxCollideCount = 1;
   public const int ShootMaxCollideCount = 1;
   public const int RollMaxCollideCount  = 10;

   public int currentCollideCount = 0;
   private int _targetCollideCount  = 0;

   private Rigidbody rb;

   [Header("Shoot mode")] public float shootForce = 10f;

   void Awake()
   {
      rb               = GetComponent<Rigidbody>();
      currentThrowMode = EBallThrowMode.Curve;
      rb.isKinematic   = true; 
   }

   public void Initialize(Vector3 start, Vector3 target, float speed, float height, EBallThrowMode throwMode = EBallThrowMode.Curve)
   {
      _startPosition    = start;
      _targetPosition   = target;
      _throwSpeed       = speed;
      _maxHeight        = height;
      _timeElapsed      = 0f;
      currentThrowMode = throwMode;

      switch (currentThrowMode)
      {
         case EBallThrowMode.Curve:
            _targetCollideCount = CurveMaxCollideCount;
            break;
         case EBallThrowMode.Shoot:
            _targetCollideCount = ShootMaxCollideCount;
            rb.isKinematic      = false;
            break;
         case EBallThrowMode.Roll:
            _targetCollideCount = RollMaxCollideCount;
            break;
         default:
            Debug.Log("Error");
            break;
      }
      
      _isInitialized    = true;
   }

   void Update()
   {
      if (_isInitialized)
      {
         if (currentThrowMode == EBallThrowMode.Curve)
         {
            _timeElapsed += Time.deltaTime;
            float travelDuration = Vector3.Distance(_startPosition, _targetPosition) / _throwSpeed;
            float t              = _timeElapsed                                     / travelDuration;

            // 포물선 경로 계산
            transform.position = CalculateParabolicPosition(_startPosition, _targetPosition, _maxHeight, t);

            // 목표 위치에 도달하면 Rigidbody 활성화
            if (t >= 0.8f)
            {
               _isInitialized  = false;
               ActivateRigidbodyWithVelocity(t, travelDuration);
            }
         }
         else if (currentThrowMode == EBallThrowMode.Shoot)
         {
            Vector3 shootDir = ( _targetPosition - _startPosition ).normalized;
            rb.linearVelocity = shootDir * shootForce;
         }
      }
   }

   Vector3 CalculateParabolicPosition(Vector3 start, Vector3 target, float maxHeight, float t)
   {
      Vector3 horizontalPosition = Vector3.Lerp(start, target, t); // 수평 이동
      float   parabolicHeight    = 4 * maxHeight * t * (1 - t);    // 포물선 Y축 보정값

      return new Vector3(horizontalPosition.x, start.y + parabolicHeight, horizontalPosition.z);
   }
   
   void ActivateRigidbodyWithVelocity(float t, float travelDuration)
   {
      // 현재 순간의 포물선 속도 계산
      float   nextT      = t + 0.01f; // t보다 약간 큰 값으로 다음 위치를 예측
      Vector3 currentPos = transform.position;
      Vector3 nextPos    = CalculateParabolicPosition(_startPosition, _targetPosition, _maxHeight, nextT);
      Vector3 direction  = (nextPos - currentPos).normalized;

      // Rigidbody를 활성화하고 현재 이동 방향으로 속도를 설정
      rb.isKinematic = false;
      rb.linearVelocity    = direction * _throwSpeed;
   }

   private void OnTriggerEnter(Collider other)
   {
      Debug.Log(other.tag + " is collided with this ball");
      
      currentCollideCount++;
      
      if (currentCollideCount >= _targetCollideCount)
      {
         Destroy(this.gameObject);
      }
   }

   private void OnCollisionEnter(Collision other)
   {
      if (other.transform.CompareTag("Player"))
      {
         Debug.Log("Player Collided with this ball");
         currentCollideCount++;
      
         if (currentCollideCount >= _targetCollideCount)
         {
            Destroy(this.gameObject);
         }
      }
   }
}