using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

public enum ESkillType
{
   AttackShoot,
   //Ex. Ball Scale Up
   AttackTransform,
   DefenseCatch,
   Count
}

public enum ESkillHitEffect
{
   Damage,
   //공 타이머 쿨밀
   CoolExtension,
   //타일 배치,
   PlaceTile
}

[CreateAssetMenu(fileName = "SkillSO", menuName = "ScriptableObjects/Skill")]
public class Skill : ScriptableObject
{
   [Header("Skill Name")] 
   public string skillName = null;

   [Header("Global number")] 
   public int globalIndex;
   
   [Space(5)]
   [Header("Skill Activate Conditions Each For Clients")]
   public List<int> skillReadyConditionClient0 = new List<int>();
   [Header("For Client 1 -> Client 0 Value minus 8")]
   public List<int> skillReadyConditionClient1 = new List<int>();

   [Space(5)] 
   [Header("Skill CoolDown")] 
   public float coolDown = 5f;
   [Header(           "Skill Slot Index")]
   public int       slotIndex;

   [Space(5)]
   [Header("Skill Type")] 
   public ESkillType skillType;
   [Header("Skill Hit Effects")]
   public List<ESkillHitEffect> skillHitEffects;
   public float      damage;
   public float      coolExtensionValue;
   public bool       isPlacingTiles;
   
   [Space(5)]
   [Header("Skill Prefab")]
   public GameObject pfSkillObject;
   [Header("Fake Prefab")] 
   public GameObject pfFakeSkillObject;

   public bool isActivated = false;
   
   
   public bool CheckCanThisSkillActivated()
   {
      List<int> selectedConditionList = ThirdPersonController.SClientID == 0
                                           ? skillReadyConditionClient0
                                           : skillReadyConditionClient1;

      bool flag = true;
      
      foreach (var condition in selectedConditionList)
      {
         if (!SkillManager.Instance.currentBlockConditions.Contains(condition))
         {
            flag = false;
         }
         
      }

      return flag;
   }
   
   
}
