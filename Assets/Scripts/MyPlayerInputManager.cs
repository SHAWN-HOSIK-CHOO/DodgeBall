using System;
using System.Collections;
using StarterAssets;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyPlayerInputManager : MonoBehaviour
{
	private static MyPlayerInputManager _instance = null;
	public static  MyPlayerInputManager Instance => _instance == null ? null : _instance;
	
	public CinemachineCamera     followVirtualCamera;

	public GameObject fillImageHolder;
	public Image      fillImage;
	public float      throwBallCooldown = 5f;

	public ThirdPersonController localPlayer;
	
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

		Application.targetFrameRate = 60;
		fillImageHolder.SetActive(false);
	}
	
#if ENABLE_INPUT_SYSTEM 
	private PlayerInput _playerInput;
#endif
	
	public bool IsCurrentDeviceMouse
	{
		get
		{
#if ENABLE_INPUT_SYSTEM
			return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
		}
	}

	private void Start()
	{
#if ENABLE_INPUT_SYSTEM 
		_playerInput                           =  GetComponent<PlayerInput>();
		_playerInput.actions["Ready"].started  += OnThrowRightClickStarted;
		_playerInput.actions["Ready"].canceled += OnThrowRightClickReleased;
		_playerInput.actions["Skill"].started  += OnSkillLeftClickStarted;
		_playerInput.actions["Skill"].canceled += OnSkillLeftClickReleased;
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
	}

	public void RegisterLocalPlayer(ThirdPersonController player)
	{
		if (player.IsOwner)
		{
			localPlayer = player;
			Debug.Log(player.OwnerClientId);
		}
	}
	
	public Vector3 GetMouseWorldPosition(Vector3 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			//Debug.Log(hit.point);
			return hit.point;
		}
		Debug.Log("Zero!!");
		return Vector3.zero;
	}

	[Header("Character Input Values")]
    public Vector2 move;
	public Vector2 look;
	public bool    jump;
	public bool    sprint;
    
	[Header("Movement Settings")]
	public bool analogMovement;
    
	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	[Header("Throw Input")] 
	public bool isRightClicked = false;
	public bool isThrowReady = false;

	[Header("Skill Input")] 
	public bool skill = false;

	public bool  isCoolDown    = false;
	public float throwCoolDown = 3.0f;

	[Header("Aim Camera")] 
	public bool isZoomed = false;

	public bool shouldJamRotation = false;
	
    #if ENABLE_INPUT_SYSTEM
	public void OnMove(InputValue value)
	{
		if(!GameManager.Instance.IsGameReady())
			return;

		MoveInput(value.Get<Vector2>());
	}
    
	public void OnLook(InputValue value)
	{ 
		if(!GameManager.Instance.IsGameReady())
			return;

		if(cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}
    
	public void OnJump(InputValue value)
	{
		if(!GameManager.Instance.IsGameReady())
			return;

		JumpInput(value.isPressed);
	}
    
	public void OnSprint(InputValue value)
	{
		if(!GameManager.Instance.IsGameReady())
			return;
		
		SprintInput(value.isPressed);
	}
	
	public void OnThrowRightClickStarted(InputAction.CallbackContext context)
	{
		if(!GameManager.Instance.IsGameReady())
			return;
		
		if (isCoolDown)
		{
			return;
		}
		
		isRightClicked = true;
		isThrowReady   = true;
		Debug.Log("Shoot Ready");
		localPlayer.InvokeThrowReadyEvent();
		Cursor.lockState = CursorLockMode.Confined;
	}
	
	public void OnThrowRightClickReleased(InputAction.CallbackContext context)
	{
		if(!GameManager.Instance.IsGameReady())
			return;
		
		if (isThrowReady && !isCoolDown)
		{
			localPlayer.InvokeThrowBallEvent();
			isThrowReady = false;

			StartCoroutine(CoFillImageThrowCoolDown());
		}
		isRightClicked   = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void OnSkillLeftClickStarted(InputAction.CallbackContext context)
	{
		if(!GameManager.Instance.IsGameReady())
			return;
		
		if(!SkillManager.Instance.CanUseCurrentSkill())
			return;

		if (SkillManager.Instance.equipSkill[SkillManager.Instance.currentSkillIndex].skillType == ESkillType.DefenseCatch)
		{
			
		}
		else 
		{
			followVirtualCamera.gameObject.SetActive(false);
			shouldJamRotation = true;
			localPlayer.InvokeSkillReadyEvent();
			isZoomed = true;
		}
	}

	public void OnSkillLeftClickReleased(InputAction.CallbackContext context)
	{
		if(!GameManager.Instance.IsGameReady())
			return;
		
		if (SkillManager.Instance.equipSkill[SkillManager.Instance.currentSkillIndex].skillType == ESkillType.DefenseCatch)
		{
			
		}
		else 
		{
			if (!isZoomed)
			{
				return;
			}
		
			StartCoroutine(CoVirtualCameraSwapTimer(0.3f));
			localPlayer.InvokeSkillActionEvent();
			isZoomed = false;
		}
	}
	
	private IEnumerator CoFillImageThrowCoolDown()
	{
		float elapsedTime = 0f;

		isCoolDown = true; 
		fillImageHolder.SetActive(true);
		
		while (elapsedTime <= throwBallCooldown)
		{
			elapsedTime          += Time.deltaTime;
			fillImage.fillAmount =  Mathf.Clamp01(elapsedTime / throwBallCooldown);
			yield return null;
		}

		fillImage.fillAmount = 1f;
		
		fillImageHolder.SetActive(false);
		isCoolDown = false; 
	}

	private IEnumerator CoVirtualCameraSwapTimer(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		followVirtualCamera.gameObject.SetActive(true);
		shouldJamRotation = false;
	}
	
    #endif
    
    
	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	} 
    
	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}
    
	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}
    
	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}
	
	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}
    
	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
