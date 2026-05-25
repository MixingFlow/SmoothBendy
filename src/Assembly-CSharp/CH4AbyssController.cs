using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4AbyssController : BaseController
{
	[Header("Entrance")]
	[SerializeField]
	private GameObject m_EntranceCurvedDoor;

	[SerializeField]
	private CH4CurvedDoor m_CurvedDoor;

	[SerializeField]
	private EventTrigger m_AbyssEntranceTrigger;

	[Header("Objective: Make A Gear")]
	[SerializeField]
	private Interactable m_PipeLeaver;

	[SerializeField]
	private CH1PipeValve m_PipeValve;

	[SerializeField]
	private Transform m_Pipe;

	[SerializeField]
	private Transform m_PipeStartPos;

	[SerializeField]
	private Transform m_PipeEndPos;

	[SerializeField]
	private List<GameObject> m_PipeObjects;

	[SerializeField]
	private List<MeshRenderer> m_InkMaterials;

	[Header("Objective: Cross the Abyss")]
	[SerializeField]
	private Animator m_SwollenSearcher;

	[SerializeField]
	private Transform m_ThickInkParent;

	[SerializeField]
	private Holdable m_ThickInkPrefab;

	[SerializeField]
	private InkMakerController m_InkMaker;

	[SerializeField]
	private CH4BridgeMachine m_BridgeMachine;

	[SerializeField]
	private Interactable m_ExitDoor;

	private AudioObject m_SwollenSearcherAudio;

	private AudioClip m_LeverClip;

	private AudioClip m_ValveClip;

	private AudioClip m_SwollenSearcherIdleClip;

	private AudioClip m_SwollenSearcherPop;

	private AudioClip m_SwollenSearcherDie;

	private AudioClip m_ExitDoorClip;

	private Holdable m_ThickInk;

	private OcclusionPortal m_EntrancePortal;

	public override void Init()
	{
		base.Init();
		m_EntrancePortal = m_EntranceCurvedDoor.GetComponent<OcclusionPortal>();
		if (Object.op_Implicit((Object)(object)m_EntrancePortal))
		{
			m_EntrancePortal.open = true;
		}
	}

	public override void InitOnComplete()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_ValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
		m_SwollenSearcherIdleClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_IDLE");
		m_SwollenSearcherPop = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_swollensearcherpop");
		m_SwollenSearcherDie = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_DIE");
		m_ExitDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_safehousedoor");
		m_EntranceCurvedDoor.SetActive(false);
		m_AbyssEntranceTrigger.SetActive(active: false);
		m_ExitDoor.SetActive(active: false);
		m_Pipe.localPosition = m_PipeStartPos.localPosition;
		for (int i = 0; i < m_PipeObjects.Count; i++)
		{
			m_PipeObjects[i].SetActive(false);
		}
		for (int j = 0; j < m_InkMaterials.Count; j++)
		{
			((Renderer)m_InkMaterials[j]).material.SetFloat("_Cutout", 0f);
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.BridgeMachineObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.BridgeMachineObjective.IsStarted)
		{
			m_BridgeMachine.OnReady += HandleBridgeMachineOnReady;
			m_BridgeMachine.TurnOffInitialLever();
		}
		else
		{
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_ENTER_THE_DARKNESS", string.Empty));
			m_AbyssEntranceTrigger.OnEnter += HandleAbyssEntranceTriggerOnEnter;
			m_AbyssEntranceTrigger.SetActive(active: true);
		}
	}

	private void ForceComplete()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_02");
		m_BridgeMachine.ForceComplete();
		Transform obj = m_PipeLeaver.transform;
		obj.localEulerAngles += new Vector3(-80f, 0f, 0f);
		((Component)m_Pipe).transform.localPosition = m_PipeEndPos.localPosition;
		for (int i = 0; i < m_PipeObjects.Count; i++)
		{
			m_PipeObjects[i].SetActive(true);
		}
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_RESCUE_BORIS", string.Empty));
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.LostOnesObjective.IsComplete)
		{
			SendOnComplete();
			return;
		}
		m_ExitDoor.OnInteracted += HandleExitDoorOnInteracted;
		m_ExitDoor.SetActive(active: true);
	}

	private void HandleAbyssEntranceTriggerOnEnter(object sender, EventArgs e)
	{
		m_AbyssEntranceTrigger.OnEnter -= HandleAbyssEntranceTriggerOnEnter;
		m_BridgeMachine.OnReady += HandleBridgeMachineOnReady;
		m_BridgeMachine.Activate();
	}

	private void HandleExitDoorOnInteracted(object sender, EventArgs e)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		m_ExitDoor.OnInteracted -= HandleExitDoorOnInteracted;
		if (Object.op_Implicit((Object)(object)m_ExitDoor))
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_ExitDoorClip, m_ExitDoor.transform.position);
		}
		ShortcutExtensions.DOKill((Component)(object)m_ExitDoor.transform, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_ExitDoor.transform, new Vector3(0f, 100f, 0f), 2.5f, (RotateMode)3), (Ease)7);
		SendOnComplete();
	}

	private void HandleBridgeMachineOnReady(object sender, EventArgs e)
	{
		m_BridgeMachine.OnReady -= HandleBridgeMachineOnReady;
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH4Data.BridgeMachineObjective.IsStarted)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.BridgeMachineObjective.IsStarted = true;
			GameManager.Instance.GameDataManager.Save();
		}
		m_PipeLeaver.OnInteracted += HandlePipeLeverOnInteracted;
		m_PipeLeaver.AllowShimmer(_active: true);
		m_PipeLeaver.SetActive(active: true);
	}

	private void HandleCurvedDoorOnClose(object sender, EventArgs e)
	{
		m_CurvedDoor.OnClose -= HandleCurvedDoorOnClose;
		m_EntranceCurvedDoor.SetActive(true);
		if (Object.op_Implicit((Object)(object)m_EntrancePortal))
		{
			m_EntrancePortal.open = false;
		}
	}

	private void HandlePipeLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		m_PipeLeaver.OnInteracted -= HandlePipeLeverOnInteracted;
		m_PipeLeaver.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_LeverClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_PipeLeaver.transform, new Vector3(-80f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)27), new TweenCallback(PipeLeverOnComplete));
	}

	private void PipeLeverOnComplete()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		S13AudioManager.Instance.InvokeEvent("evt_ink_pipe_opens");
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Pipe, m_PipeEndPos.localPosition, 10f, false), (Ease)7), new TweenCallback(PipeRiseOnComplete));
	}

	private void PipeRiseOnComplete()
	{
		for (int i = 0; i < m_PipeObjects.Count; i++)
		{
			m_PipeObjects[i].SetActive(true);
		}
		m_PipeValve.OnInteracted += HandlePipeValveOnInteracted;
		m_PipeValve.Activate();
	}

	private void HandlePipeValveOnInteracted(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		m_PipeValve.OnInteracted -= HandlePipeValveOnInteracted;
		GameManager.Instance.AudioManager.PlayAtPosition(m_ValveClip, m_PipeValve.transform.position);
		S13AudioManager.Instance.InvokeEvent("evt_swolen_searcher_appears");
		TweenSettingsExtensions.OnComplete<Tweener>(m_PipeValve.DORotate(2f), new TweenCallback(PipeValveRotateOnComplete));
	}

	private void PipeValveRotateOnComplete()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		for (int i = 0; i < m_InkMaterials.Count; i++)
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(((Renderer)m_InkMaterials[i]).material, 0.3f, "_Cutout", 5f), (Ease)6);
		}
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			m_ThickInk = Object.Instantiate<Holdable>(m_ThickInkPrefab);
			m_ThickInk.transform.SetParent(m_ThickInkParent);
			m_ThickInk.transform.localPosition = Vector3.zero;
			m_ThickInk.transform.localEulerAngles = Vector3.zero;
			m_ThickInk.transform.localScale = Vector3.one;
			m_SwollenSearcher.SetBool("IsHiding", false);
			m_SwollenSearcher.SetTrigger("IsActive");
			m_SwollenSearcherAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_SwollenSearcherIdleClip, m_ThickInkParent.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, base.transform);
			m_ThickInk.OnInteracted += HandleThickInkOnInteracted;
			m_ThickInk.SetActive(active: true);
		});
	}

	private void HandleThickInkOnInteracted(object sender, EventArgs e)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		m_ThickInk.OnInteracted -= HandleThickInkOnInteracted;
		GameManager.Instance.AudioManager.Play(m_SwollenSearcherPop);
		GameManager.Instance.AudioManager.PlayAtPosition(m_SwollenSearcherDie, ((Component)m_ThickInkParent).transform.position);
		if ((Object)(object)m_SwollenSearcherAudio != (Object)null)
		{
			m_SwollenSearcherAudio.Stop();
			m_SwollenSearcherAudio = null;
		}
		m_SwollenSearcher.SetBool("IsHiding", true);
		for (int i = 0; i < m_InkMaterials.Count; i++)
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(((Renderer)m_InkMaterials[i]).material, 0f, "_Cutout", 5f), (Ease)6);
		}
		m_InkMaker.OnTrayInteracted += HandleInkMakerOnTrayInteracted;
		m_InkMaker.OnGearInteracted += HandleOnGearInteracted;
		m_InkMaker.OnComplete += HandleInkMakerOnComplete;
		m_InkMaker.Activate();
	}

	private void HandleInkMakerOnTrayInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnTrayInteracted -= HandleInkMakerOnTrayInteracted;
		m_ThickInk.Remove();
		m_ThickInk.Dispose();
	}

	private void HandleInkMakerOnComplete(object sender, EventArgs e)
	{
		m_InkMaker.OnComplete -= HandleInkMakerOnComplete;
		m_InkMaker.Deactivate();
		m_PipeValve.OnInteracted += HandlePipeValveOnInteracted;
		m_PipeValve.Activate();
	}

	private void HandleOnGearInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnGearInteracted -= HandleOnGearInteracted;
		GameManager.Instance.Player.PlayPickUpSound();
		m_BridgeMachine.OnEnter += HandleBridgeCartOnEnter;
		m_BridgeMachine.OnComplete += HandleBridgeMachineOnComplete;
		m_BridgeMachine.FinalActivate();
	}

	private void HandleBridgeCartOnEnter(object sender, EventArgs e)
	{
		m_BridgeMachine.OnEnter -= HandleBridgeCartOnEnter;
		if (Object.op_Implicit((Object)(object)m_ThickInk))
		{
			m_ThickInk.Remove();
			m_ThickInk.Dispose();
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
		}
		GameManager.Instance.Player.UnEquipWeapon();
	}

	private void HandleBridgeMachineOnComplete(object sender, EventArgs e)
	{
		m_BridgeMachine.OnComplete -= HandleBridgeMachineOnComplete;
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_RESCUE_BORIS", string.Empty));
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.BridgeMachineObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_ExitDoor.OnInteracted += HandleExitDoorOnInteracted;
		m_ExitDoor.SetActive(active: true);
	}

	protected override void OnDisposed()
	{
		m_SwollenSearcherAudio = null;
		m_LeverClip = null;
		m_ValveClip = null;
		m_SwollenSearcherIdleClip = null;
		m_SwollenSearcherPop = null;
		m_SwollenSearcherDie = null;
		m_ExitDoorClip = null;
		base.OnDisposed();
	}
}
