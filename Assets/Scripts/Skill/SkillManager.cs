using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StarterAssets;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static  SkillManager Instance => _instance == null ? null : _instance;

    public readonly object LockObject = new object();
    
    public List<Skill> equipSkill             = new List<Skill>();
    public List<int>   currentBlockConditions = new List<int>();
    
    public int currentSkillIndex = 0;

    public List<Image> slotImages;

    public List<GameObject> pfFakeSkillGlobalStorage = new List<GameObject>();
    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSkill(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSkill(2);
        }
    }

    public void Initialize()
    {
        foreach (var skill in equipSkill)
        {
            skill.isActivated = false;
        }
    }
    
    public void SelectSkill(int index)
    {
        if (index >= equipSkill.Count)
        {
            Debug.Log("index overflow");
            return;
        }
        
        Color currentColor1 = slotImages[currentSkillIndex].color;
        slotImages[currentSkillIndex].color = new Color(1f, 1f, 1f, currentColor1.a);
        
        currentSkillIndex                   = index;
        
        Color currentColor2 = slotImages[currentSkillIndex].color;
        slotImages[currentSkillIndex].color = new Color(0f, 0f, 1f, currentColor2.a);
    }

    public void EquipSkill(Skill newSkill)
    {
        equipSkill.Add(newSkill);
    }

    public void RemoveBlockIndex(int removeVal)
    {
        currentBlockConditions.Remove(removeVal); 
        InformBlockRemoval();
    }

    private void InformBlockRemoval()
    {
        foreach (var skill in equipSkill)
        {
            bool canActivate = skill.CheckCanThisSkillActivated();
            
            if (!canActivate)
            {
                Debug.Log("Removed skill index : " + skill.slotIndex);
                skill.isActivated = false;
                
                Color c = slotImages[skill.slotIndex].color;
                c.a                   = 0.3f;
                slotImages[skill.slotIndex].color = c;
            }
        }
    }

    private List<int> ReturnsActivateSkillIndex()
    {
        List<int> retList = new List<int>();
        
        foreach (var skill in equipSkill)
        {
            bool res = skill.CheckCanThisSkillActivated();
            if (res)
            {
                retList.Add(skill.slotIndex);
            }
        }

        return retList;
    }

    public void UpdateSkillState(int newVal)
    {
        //Debug.Log("UpdateSkillCalled");
        
        currentBlockConditions.Add(newVal);
        
        List<int> skillIndexList = ReturnsActivateSkillIndex();
        if (skillIndexList.Count == 0)
        {
            //Debug.Log("Skill empty");
        }
        else
        {
            foreach (var idx in skillIndexList)
            {
                lock (LockObject)
                {
                    equipSkill[idx].isActivated = true;
                    
                    Color c = slotImages[idx].color;
                    c.a                   = 1f;
                    slotImages[idx].color = c;
                }
            }
        }
    }

    public bool CanUseCurrentSkill()
    {
        if (equipSkill[currentSkillIndex].isActivated)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void OnSkillReadyEvent(object sender, EventArgs e)
    {
        if (equipSkill[currentSkillIndex].skillType == ESkillType.DefenseCatch)
        {
            
        }
    }

    public void OnSkillActionEvent(object sender, EventArgs e)
    {
        if (equipSkill[currentSkillIndex].skillType == ESkillType.AttackShoot)
        {
            AttackShoot();
        }
        else if (equipSkill[currentSkillIndex].skillType == ESkillType.AttackTransform)
        {
            
        }
        else if (equipSkill[currentSkillIndex].skillType == ESkillType.DefenseCatch)
        {
            
        }
    }


    private void AttackShoot()
    {
        Vector3 playerPosition = MyPlayerInputManager.Instance.localPlayer.transform.position;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector3 targetPosition    = MyPlayerInputManager.Instance.GetMouseWorldPosition(screenCenterPoint);

        Vector3 spawnTargetForwardVector = ( targetPosition - playerPosition ).normalized;
        spawnTargetForwardVector.y = 0f;
        
        //스킬을 사용하는 클라이언트는 skill tag 오브젝트를 던진다.
        GameObject skillObject = Instantiate(equipSkill[currentSkillIndex].pfSkillObject,
                                             MyPlayerInputManager.Instance.localPlayer.shootSkillSpawnPosition);
        skillObject.transform.forward = spawnTargetForwardVector;
        skillObject.transform.SetParent(null);
        skillObject.GetComponent<SkillScript>().Initialize(ESkillType.AttackShoot,5f,10f);
        
        //회전
        MyPlayerInputManager.Instance.localPlayer.transform.forward = spawnTargetForwardVector;

        MyPlayerInputManager.Instance.localPlayer.GetComponent<PlayerActionController>().AttackShootActionServerRPC(MyPlayerInputManager.Instance.localPlayer.shootSkillSpawnPosition.position,
            spawnTargetForwardVector, equipSkill[currentSkillIndex].globalIndex);
    }

    public void AttackShootOtherClients(Vector3 attackerSpawnPosition, Vector3 spawnForwardVector, int globalInt)
    {
        GameObject skillObject = Instantiate(pfFakeSkillGlobalStorage[globalInt], attackerSpawnPosition, Quaternion.identity);
        skillObject.transform.forward = spawnForwardVector;
        //TODO: 스킬 정보 넘겨주기 최적화
        skillObject.GetComponent<SkillScript>().Initialize(ESkillType.AttackShoot,5f,10f);
    }
}
