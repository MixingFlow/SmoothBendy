using System;
using UnityEngine;
using UnityEngine.UI;

public class DangerRoom_Zapper : MonoBehaviour
{
	[Serializable]
	public class ActionInfo
	{
		public ActivationMode sendMode;

		public int animIndex;

		public ActionInfo(ActivationMode _Mode, int _animIndex)
		{
			sendMode = _Mode;
			animIndex = _animIndex;
		}
	}

	public enum ActivationMode
	{
		MOVE,
		FROZEN,
		STOP,
		COUNT
	}

	public ActivationMode currentZapper;

	public Transform MainCamera;

	public Animator ZapperAnim;

	public Image Crosshair;

	public Text AnimIndex;

	public Text AnimationList;

	private int currentAction = 1;

	private void Start()
	{
		PlayAinmation(fire: false);
		GameManager.Instance.Player.SetLock(active: false);
	}

	private void Update()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			if (Input.GetKeyDown(i.ToString()))
			{
				currentAction = i;
				if (currentAction == 0)
				{
					currentAction = 10;
				}
				AnimIndex.text = "ANIM: " + currentAction + "\n";
				Text animIndex = AnimIndex;
				animIndex.text = animIndex.text + "MODE: " + currentZapper;
			}
		}
		RaycastHit hitObj = default(RaycastHit);
		if (Physics.Raycast(MainCamera.position, MainCamera.forward, ref hitObj, 1000f))
		{
			if ((Object)(object)((RaycastHit)(ref hitObj)).transform != (Object)null && Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref hitObj)).transform).GetComponent<AI_DangerRoom_AI_Controller>()))
			{
				DisplayAnimationInfo(hitObj);
			}
			else
			{
				AnimationList.text = string.Empty;
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			ShootEnemy(hitObj);
		}
		else if (Input.GetMouseButtonDown(1))
		{
			SwitchZapper();
		}
	}

	private void DisplayAnimationInfo(RaycastHit _hitObj)
	{
		AI_DangerRoom_AI_Controller component = ((Component)((RaycastHit)(ref _hitObj)).transform).GetComponent<AI_DangerRoom_AI_Controller>();
		AnimationList.text = ((Object)((Component)component).gameObject).name + "\n\n";
		for (int i = 0; i < 10; i++)
		{
			Text animationList = AnimationList;
			string text = animationList.text;
			animationList.text = text + (i + 1) + " : " + component.speedLabels[i] + "\n";
		}
	}

	private void SwitchZapper()
	{
		currentZapper++;
		if (currentZapper == ActivationMode.COUNT)
		{
			currentZapper = ActivationMode.MOVE;
		}
		PlayAinmation(fire: false);
		AnimIndex.text = "ANIM: " + currentAction + "\n";
		Text animIndex = AnimIndex;
		animIndex.text = animIndex.text + "MODE: " + currentZapper;
	}

	private void ShootEnemy(RaycastHit _hitObj)
	{
		if ((Object)(object)((RaycastHit)(ref _hitObj)).transform != (Object)null && Object.op_Implicit((Object)(object)((Component)((RaycastHit)(ref _hitObj)).transform).GetComponent<AI_DangerRoom_AI_Controller>()))
		{
			if (currentZapper == ActivationMode.STOP)
			{
				((Component)((RaycastHit)(ref _hitObj)).transform).BroadcastMessage("SetActivationMode", (object)new ActionInfo(currentZapper, 1), (SendMessageOptions)1);
			}
			else
			{
				((Component)((RaycastHit)(ref _hitObj)).transform).BroadcastMessage("SetActivationMode", (object)new ActionInfo(currentZapper, currentAction), (SendMessageOptions)1);
			}
		}
		PlayAinmation(fire: true);
	}

	private void PlayAinmation(bool fire)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		string text = "_Idle";
		if (fire)
		{
			text = string.Empty;
		}
		switch (currentZapper)
		{
		case ActivationMode.MOVE:
			ZapperAnim.Play("DangerRoom_GreenRay" + text);
			((Graphic)Crosshair).color = Color.green;
			break;
		case ActivationMode.FROZEN:
			ZapperAnim.Play("DangerRoom_YellowRay" + text);
			((Graphic)Crosshair).color = Color.yellow;
			break;
		case ActivationMode.STOP:
			ZapperAnim.Play("DangerRoom_RedRay" + text);
			((Graphic)Crosshair).color = Color.red;
			break;
		}
	}
}
