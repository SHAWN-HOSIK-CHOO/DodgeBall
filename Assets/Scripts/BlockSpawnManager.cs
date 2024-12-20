using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawnManager : NetworkBehaviour
{
   public GameObject       blockPrefab;   
   public float            gridSize               = 1f;
   public List<GameObject> spawnedFloorPrefabList = new List<GameObject>();

   public GameObject[] pfMockBlockPrefab = new GameObject[2];

   public int side = 0;

   public          HashSet<int> GridSpawnPositions { get; } = new HashSet<int>();
   public readonly object       LockObject = new object();

   public override void OnNetworkSpawn()
   {
      if (!IsOwner)
      {
         return;
      }
      
      base.OnNetworkSpawn();
      
      GameObject go = GameObject.FindWithTag("InvisibleFloor");
      go.GetComponent<BlockSpawner>().blockSpawnManager = this;
      
      //어느 진영에 있는지
      if (this.transform.position.z <= 10)
      {
         side        = 0;
      }
      else
      {
         side        = 1;
      }
      
      //Debug.Log(OwnerClientId + " side : " + side);
   }

   public void InformBlockPositionWrapper(int posVal)
   {
      if(!IsOwner)
         return;
      InformNewBlockPositionServerRPC(posVal);
   }

   [ServerRpc]
   public void InformNewBlockPositionServerRPC(int posVal, ServerRpcParams serverRpcParams =default)
   {
      InformNewBlockPositionClientRPC(posVal);
   }

   [ClientRpc]
   public void InformNewBlockPositionClientRPC(int posVal)
   {
      lock (LockObject)
      {
         if (GridSpawnPositions.Contains(posVal))
         {
            Debug.Log("____________________________________");
            return;
         }
         
         if (GridSpawnPositions.Add(posVal) )
         {
            //Debug.Log("Added : " + posVal);
            Vector3 spawnPosition = new Vector3(Mathf.Floor(posVal / 100), 0, posVal % 100);
            GameObject go =Instantiate(pfMockBlockPrefab[(int)OwnerClientId], spawnPosition, Quaternion.identity);

            if ((int)OwnerClientId == 0)
            {
               go.transform.rotation = Quaternion.Euler(0f,180f,0f);
            }
            
            go.GetComponent<FloorBlockMetaData>().positionValue = posVal;
            
            //TODO:우아하게--------------------------------------------------------------
            go.GetComponent<FloorBlockMetaData>().side          = (int)OwnerClientId;
            if ((int)OwnerClientId == 0)
            {
               GameManager.Instance.player0BlockCounts++;
            }
            else if((int)OwnerClientId == 1)
            {
               GameManager.Instance.player1BlockCounts++;
            }
            else
            {
               Debug.Log("Error");
            }
            GameManager.Instance.RecalculateScoreBarImage(GridSpawnPositions.Count);
            //----------------------------------------------------------------------
            
            spawnedFloorPrefabList.Add(go);
         }
      }
   }

   public void RemoveBlockRandomly(int randomValue)
   {
      lock (LockObject)
      { 
         if (GridSpawnPositions.Contains(randomValue))
         {
            GridSpawnPositions.Remove(randomValue);
            for (int i = spawnedFloorPrefabList.Count - 1 ; i >= 0; i--)
            {
               if (spawnedFloorPrefabList[i].GetComponent<FloorBlockMetaData>().positionValue == randomValue)
               {
                  GameObject tmp = spawnedFloorPrefabList[i];
                  spawnedFloorPrefabList.RemoveAt(i);
                  
                  //TODO:우아하게-------------------------------------------------------------------------
                  if (tmp.GetComponent<FloorBlockMetaData>().side == 0)
                  {
                     GameManager.Instance.player0BlockCounts--;
                  }
                  else if (tmp.GetComponent<FloorBlockMetaData>().side == 1)
                  {
                     GameManager.Instance.player1BlockCounts--;
                  }
                  else
                  {
                     Debug.Log("Error");
                  }
                  GameManager.Instance.RecalculateScoreBarImage(GridSpawnPositions.Count);
                  //---------------------------------------------------------------------------------------
                  
                  Destroy(tmp.gameObject);
                  Debug.Log("Destroyed : " + randomValue); 
               }
            }
         }
         else
         {
            Debug.Log("Grid Pos doesnt contain : " + randomValue);
            return;
         }
         
      }
   }
}