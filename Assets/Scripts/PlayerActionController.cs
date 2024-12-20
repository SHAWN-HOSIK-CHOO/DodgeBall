using System;
using StarterAssets;
using UnityEngine;
using Unity.Netcode;

public class PlayerActionController : NetworkBehaviour
{
    private ThirdPersonController _thirdPersonController;
    private GameObject            _spawnedBall;
    private BallLauncher          _ballLauncher;

    private EBallThrowMode _throwMode;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _thirdPersonController = this.GetComponent<ThirdPersonController>();
        _ballLauncher          = GetComponent<BallLauncher>();
        _throwMode              = EBallThrowMode.Curve;

        _thirdPersonController.ThrowReadyEventHandler  += ThrowReadyInstantiateBall;
        _thirdPersonController.ThrowActionEventHandler += ThrowActionBall;
        _thirdPersonController.SkillReadyEventHandler  += SkillManager.Instance.OnSkillReadyEvent;
        _thirdPersonController.SkillActionEventHandler += SkillManager.Instance.OnSkillActionEvent;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _throwMode = EBallThrowMode.Curve;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _throwMode = EBallThrowMode.Shoot;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _throwMode = EBallThrowMode.Roll;
        }
    }

    public void ThrowReadyInstantiateBall(object sender, EventArgs e)
    {
        if(IsOwner)
            ThrowReadyCallServerRPC();
    }

    [ServerRpc]
    public void ThrowReadyCallServerRPC()
    {
        ThrowReadyCallClientRPC();
    }
    [ClientRpc]
    public void ThrowReadyCallClientRPC()
    {
        Debug.Log("ThrowReady None NetBall State!");
        //TODO: 공 상대에게 보여주기
    }
    
    
    public void ThrowActionBall(object sender, EventArgs e)
    {
        Vector3 targetPosition = _ballLauncher.ThrowBall(_throwMode);
        ThrowBallServerRPC(targetPosition, _throwMode);
    }
    
    [ServerRpc]
    public void ThrowBallServerRPC(Vector3 clientTargetPos, EBallThrowMode curMode)
    {
        ThrowBallClientRPC(clientTargetPos, curMode);
    }
    [ClientRpc]
    public void ThrowBallClientRPC(Vector3 clientTargetPos, EBallThrowMode curMode)
    {
        if(IsOwner)
            return;
        
        _ballLauncher.ThrowBall_OtherClients(clientTargetPos, curMode);
    }
    
    //Skill
    [ServerRpc]
    public void AttackShootActionServerRPC(Vector3 spawnPos, Vector3 targetForward, int globalIndex)
    {
        AttackShootActionClientRPC(spawnPos,targetForward,globalIndex);
    }

    [ClientRpc]
    public void AttackShootActionClientRPC(Vector3 pos, Vector3 vec, int index)
    {
        if(IsOwner)
            return;
        
        SkillManager.Instance.AttackShootOtherClients(pos,vec,index);
    }
}
