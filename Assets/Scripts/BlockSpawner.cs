using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    public BlockSpawnManager blockSpawnManager;

    public float blockSampleStep = 3f;
    
    private void OnTriggerStay(Collider other)
    {
        if (blockSpawnManager == null)
        {
            Debug.Log("BlockSpawnManager is not connected");
            return;
        }

        if (other.CompareTag("Skill"))
        {
            Debug.Log("Skill Entered");
            HandleBlockPlacements(other, blockSampleStep);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (blockSpawnManager == null)
        {
            Debug.Log("BlockSpawnManager is not connected");
            return;
        }
    
        if (other.CompareTag("Ball"))
        {
            if (other.gameObject.GetComponent<BallScript>().currentThrowMode == EBallThrowMode.Shoot)
            {
                return;
            }
    
            HandleBlockPlacements(other);
        }
    }

    private void HandleBlockPlacements(Collider other, float sampleStep = 1f)
    {
        // 콜라이더의 AABB(Box Bounds) 가져오기
        Bounds ballBounds = other.bounds;

        // 격자 크기
        float gridSize = blockSpawnManager.gridSize;

        // 바운드 내 격자 좌표 계산
        int minX = Mathf.FloorToInt(ballBounds.min.x / gridSize);
        int maxX = Mathf.CeilToInt(ballBounds.max.x  / gridSize);
        int minZ = Mathf.FloorToInt(ballBounds.min.z / gridSize);
        int maxZ = Mathf.CeilToInt(ballBounds.max.z  / gridSize);

       //sampleStep /= 2f;
        
        lock (blockSpawnManager.LockObject)
        {
            // //공이 맞은 중심은 무조건 배치
            // Vector3 centerPosition = new Vector3(
            //                                       Mathf.RoundToInt(other.transform.position.x),
            //                                       Mathf.RoundToInt(other.transform.position.y),
            //                                       Mathf.RoundToInt(other.transform.position.z)
            //                                      );
            //
            // float roundedSnappedX = Mathf.Round(centerPosition.x / gridSize) * gridSize;
            // float roundedSnappedZ = Mathf.Round(centerPosition.z / gridSize) * gridSize;
            //
            // int roundedSaveValue = Mathf.RoundToInt(roundedSnappedX * 100 + roundedSnappedZ);
            //
            // if (!blockSpawnManager.GridSpawnPositions.Contains(roundedSaveValue))
            // {
            //     blockSpawnManager.InformBlockPositionWrapper(roundedSaveValue);
            //     SkillManager.Instance.UpdateSkillState(roundedSaveValue);
            //
            //     // Debug 출력
            //     Debug.Log($"Rounded Position Block Added: {roundedSnappedX}, {roundedSnappedZ}");
            // }
            
            //나머지는 샘플링으로 배치
            for (int x = minX; x <= maxX; x += (int)sampleStep)
            {
                for (int z = minZ; z <= maxZ; z += (int)sampleStep)
                {
                    // 격자 중심 좌표 계산
                    float snappedX = x * gridSize;
                    float snappedZ = z * gridSize;
                    
                    if(snappedX < 0 || snappedX > 20 || snappedZ < 0 || snappedZ > 20)
                        continue;

                    // 상대편 체크: Side 조건 유지
                    if (blockSpawnManager.side == 0 && snappedZ <= 10)
                    {
                        continue;
                    }
                    else if (blockSpawnManager.side == 1 && snappedZ >= 10)
                    {
                        continue;
                    }


                    // 격자 좌표 저장 값 계산
                    int saveValue = Mathf.RoundToInt(snappedX * 100 + snappedZ);

                    // 이미 배치된 위치는 건너뜀
                    if (!blockSpawnManager.GridSpawnPositions.Contains(saveValue))
                    {
                        blockSpawnManager.InformBlockPositionWrapper(saveValue);
                        SkillManager.Instance.UpdateSkillState(saveValue);
                    }
                }
            }
        }
    }
}
