using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2MainGateSwitchController : BaseController
{
	[Header("Objective: Open Gate")]
	[SerializeField]
	private GenericDoorController m_GateDoor;

	[SerializeField]
	private Interactable m_GateSwitchInteract;

	[SerializeField]
	private InteractableSwitchBox m_BaconSoupSwitchBox;

	[SerializeField]
	private List<InteractableSwitchBox> m_SwitchBoxes;

	[SerializeField]
	private List<LightFlicker> m_GateSwitchLights;

	[SerializeField]
	private LightFixtureController m_LightFixture;

	[SerializeField]
	private Transform m_GateSwitch;

	[SerializeField]
	private Transform m_GateSwitchEndPos;

	[SerializeField]
	private GameObject m_LightObjects;

	[SerializeField]
	private EventTrigger m_GateObjective;

	[Header("Objective: Enter Music Lobby")]
	[SerializeField]
	private Transform m_EntranceAudioLocation;

	[SerializeField]
	private GameObject m_PlankLight;

	[SerializeField]
	private List<GameObject> m_PlanksAtDoor;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip[] m_GenericButtonClips;

	private AudioClip m_HenryClip06;

	private AudioClip m_SearcherClip;

	private AudioClip m_LightClip;

	private AudioClip m_PowerLeverClip;

	private int m_ActiveSwitches;

	private int m_MaxSwitches = 3;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_GenericButtonClips = GameManager.Instance.GetAudioClips("Audio/SFX/GenericButtons/");
		m_HenryClip06 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_06");
		m_SearcherClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/Searchers/Idle/SFX_Searchers_Voiice_Mouth_Open_02");
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_PowerLeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_BaconSoupSwitchBox.ForceDoorOpen();
		m_GateObjective.SetActive(active: false);
		m_GateSwitchInteract.SetActive(active: false);
		m_PlankLight.SetActive(false);
		m_LightFixture.TurnOn();
		for (int i = 0; i < m_GateSwitchLights.Count; i++)
		{
			m_GateSwitchLights[i].TurnOn();
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.GateObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.GateObjective.IsStarted)
		{
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_GATE", "OBJECTIVES/CH2_OBJECTIVE_GATE_TIP"));
			ActivateSwitchBoxes();
		}
		else
		{
			m_GateObjective.OnEnter += HandleObjectiveOnEnter;
			m_GateObjective.SetActive(active: true);
		}
	}

	private void HandleObjectiveOnEnter(object sender, EventArgs e)
	{
		m_GateObjective.OnEnter -= HandleObjectiveOnEnter;
		ActivateSwitchBoxes();
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.GateObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip06, "DIACH2/DIA_CH2_HENRY_06", isTrimmed: true));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_GATE", "OBJECTIVES/CH2_OBJECTIVE_GATE_TIP", 4f));
		};
	}

	private void ActivateSwitchBoxes()
	{
		m_BaconSoupSwitchBox.ActivateSwitch();
		m_BaconSoupSwitchBox.OnComplete += HandleSwitchBoxOnComplete;
		for (int i = 0; i < m_SwitchBoxes.Count; i++)
		{
			m_SwitchBoxes[i].OnComplete += HandleSwitchBoxOnComplete;
			m_SwitchBoxes[i].ActivateDoor();
		}
	}

	private void HandleSwitchBoxOnComplete(object sender, EventArgs e)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		InteractableSwitchBox interactableSwitchBox = sender as InteractableSwitchBox;
		interactableSwitchBox.OnComplete -= HandleSwitchBoxOnComplete;
		PlayGenericButtonSound(interactableSwitchBox.transform.position);
		m_GateSwitchLights[m_ActiveSwitches].TurnOff();
		m_ActiveSwitches++;
		if (m_ActiveSwitches >= m_MaxSwitches)
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_RAISE_GATE", string.Empty, 4f));
			m_GateSwitchInteract.SetActive(active: true);
			m_GateSwitchInteract.OnInteracted += HandleGateSwitchOnInteract;
		}
	}

	private void HandleGateSwitchOnInteract(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_GateSwitchInteract.OnInteracted -= HandleGateSwitchOnInteract;
		TweenSettingsExtensions.OnComplete<Sequence>(DOGateRise(), new TweenCallback(HandleGateRiseOnComplete));
	}

	private Sequence DOGateRise()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0.5f;
		GameManager.Instance.AudioManager.PlayAtPosition(m_PowerLeverClip, m_GateSwitch.position);
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_GateSwitch, m_GateSwitchEndPos.localPosition, num2, false), (Ease)6));
		num += num2;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(TurnOffLights));
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_GateDoor.Open));
		S13AudioManager.Instance.InvokeEvent("evt_enter_music_department");
		num += 2f;
		for (int i = 0; i < 7; i++)
		{
			TweenSettingsExtensions.Insert(val, num + (float)i * 1f, (Tween)(object)ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 1f, 0.05f, 10, 90f, false));
		}
		num += 7f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.AudioManager.PlayAtPosition(m_LightClip, m_EntranceAudioLocation.position);
			GameManager.Instance.AudioManager.PlayAtPosition(m_SearcherClip, m_EntranceAudioLocation.position);
			m_PlankLight.SetActive(true);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 1f, 0.05f, 10, 90f, true));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			((Component)GameManager.Instance.GameCamera.Camera).transform.localPosition = Vector3.zero;
		});
		return val;
	}

	private void HandleGateRiseOnComplete()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.GateObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void TurnOffLights()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.PlayAtPosition(m_LightClip, m_LightFixture.transform.position);
		m_LightFixture.TurnOff();
		m_LightObjects.SetActive(false);
	}

	private void PlayGenericButtonSound(Vector3 position)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (m_GenericButtonClips != null && m_GenericButtonClips.Length > 0)
		{
			int num = Random.Range(0, m_GenericButtonClips.Length);
			AudioClip val = m_GenericButtonClips[num];
			GameManager.Instance.AudioManager.PlayAtPosition(val, position);
			m_GenericButtonClips[num] = m_GenericButtonClips[0];
			m_GenericButtonClips[0] = val;
		}
	}

	private void RemoveListeners()
	{
		m_BaconSoupSwitchBox.OnComplete -= HandleSwitchBoxOnComplete;
		for (int i = 0; i < m_SwitchBoxes.Count; i++)
		{
			m_SwitchBoxes[i].OnComplete -= HandleSwitchBoxOnComplete;
		}
		m_GateSwitchInteract.OnInteracted -= HandleGateSwitchOnInteract;
		m_GateObjective.OnEnter -= HandleObjectiveOnEnter;
	}

	private void ForceComplete()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		for (int i = 0; i < m_GateSwitchLights.Count; i++)
		{
			m_GateSwitchLights[i].TurnOff();
		}
		m_GateSwitch.localPosition = m_GateSwitchEndPos.localPosition;
		m_GateDoor.ForceOpen();
		m_BaconSoupSwitchBox.ForceDoorOpen();
		m_BaconSoupSwitchBox.ForceSwitchOn();
		for (int j = 0; j < m_SwitchBoxes.Count; j++)
		{
			m_SwitchBoxes[j].ForceDoorOpen();
			m_SwitchBoxes[j].ForceSwitchOn();
		}
		m_LightFixture.TurnOff();
		m_PlankLight.SetActive(true);
		for (int k = 0; k < m_PlanksAtDoor.Count; k++)
		{
			m_PlanksAtDoor[k].SetActive(false);
		}
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		m_GenericButtonClips = null;
		m_HenryClip06 = null;
		m_SearcherClip = null;
		m_LightClip = null;
		m_PowerLeverClip = null;
		base.OnDisposed();
	}
}
