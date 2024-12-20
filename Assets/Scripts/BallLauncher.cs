using System;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public enum EBallThrowMode
{
    Curve,
    Shoot,
    Roll,
    Count
}
public class BallLauncher : MonoBehaviour
{
    public GameObject ballPrefab;
    [Header("다른 클라이언트 실행 전용, tag 다름")]
    public GameObject fakeballPrefab;        // 가짜 공 프리팹
    public Transform  launchPoint;     // 공을 던질 시작 위치
    public float      throwSpeed = 5f; // 던질 속도
    public float      maxHeight  = 5f; // 포물선 최대 높이
    public float      groundY    = 0f;
    public Vector3    targetPosition;// 바닥의 높이 (공이 떨어질 높이)
    
    public Vector3 ThrowBall(EBallThrowMode throwMode = EBallThrowMode.Curve)
    {
        targetPosition = MyPlayerInputManager.Instance.GetMouseWorldPosition(Input.mousePosition);
        targetPosition.y = groundY; // 목표 위치의 y 값을 바닥 높이로 고정
        LaunchBall(launchPoint.position, targetPosition, throwSpeed, maxHeight, throwMode);

        return targetPosition;
    }

    public void ThrowBall_OtherClients(Vector3 targetPos, EBallThrowMode throwMode = EBallThrowMode.Curve)
    {
        targetPos.y = groundY;
        LaunchFakeBall(launchPoint.position, targetPos, throwSpeed, maxHeight, throwMode);
    }
    
    void LaunchBall(Vector3 startPosition, Vector3 target, float speed, float height, EBallThrowMode throwMode = EBallThrowMode.Curve)
    {
        GameObject ballInstance = Instantiate(ballPrefab, startPosition, Quaternion.identity);
        BallScript ballScript   = ballInstance.GetComponent<BallScript>();

        // BallScript에 초기 설정 전달
        ballScript.Initialize(startPosition, target, speed, height, throwMode);
    }
    
    void LaunchFakeBall(Vector3 startPosition, Vector3 target, float speed, float height, EBallThrowMode throwMode = EBallThrowMode.Curve)
    {
        GameObject ballInstance = Instantiate(fakeballPrefab, startPosition, Quaternion.identity);
        BallScript ballScript   = ballInstance.GetComponent<BallScript>();

        // BallScript에 초기 설정 전달
        ballScript.Initialize(startPosition, target, speed, height, throwMode);
    }
}

