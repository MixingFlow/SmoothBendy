using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CH2MainPowerController : BaseController
{
	[Serializable]
	private class LobbyLights
	{
		public LightFixtureController LightFixture;

		public GameObject Lights;
	}

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_EntranceTrigger;

	[Header("Objective: Turn On Lobby Lights")]
	[SerializeField]
	private InteractablePowerLever m_Lever;

	[SerializeField]
	private List<GameObject> m_RecordingLbls;

	[SerializeField]
	private List<LobbyLights> m_LobbyLights;

	[SerializeField]
	private List<LightController> m_LobbyLongLights;

	[SerializeField]
	private List<LobbyLights> m_MusicRoomLights;

	[SerializeField]
	private EventTrigger m_InkStairwellEventTrigger;

	[SerializeField]
	private GameObject m_DarknessProjection;

	[SerializeField]
	private GameObject m_SearcherSpawnInkDrip;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_Henry07Clip;

	private AudioClip m_HenryClip08;

	private AudioClip m_PowerLeverClip;

	private AudioClip m_LightClip;

	private AudioClip m_LobbyEntranceMusicClip;

	public override void InitOnComplete()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		base.InitOnComplete();
		m_Henry07Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_07");
		m_HenryClip08 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_08");
		m_PowerLeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_LobbyEntranceMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_OldLightHead");
		m_EntranceTrigger.SetActive(active: false);
		for (int i = 0; i < m_LobbyLights.Count; i++)
		{
			GameObject lights = m_LobbyLights[i].Lights;
			if ((Object)(object)lights != (Object)null)
			{
				foreach (Transform item in lights.transform.parent)
				{
					Transform val = item;
					if (((Object)val).name == ((Object)lights).name + " REPLACEMENT")
					{
						m_LobbyLights[i].Lights = ((Component)val).gameObject;
						break;
					}
				}
			}
			TurnOffLobbyLight(m_LobbyLights[i]);
		}
		for (int j = 0; j < m_MusicRoomLights.Count; j++)
		{
			GameObject lights2 = m_MusicRoomLights[j].Lights;
			if ((Object)(object)lights2 != (Object)null)
			{
				foreach (Transform item2 in lights2.transform.parent)
				{
					Transform val2 = item2;
					if (((Object)val2).name == ((Object)lights2).name + " REPLACEMENT")
					{
						m_MusicRoomLights[j].Lights = ((Component)val2).gameObject;
						break;
					}
				}
			}
			TurnOffLobbyLight(m_MusicRoomLights[j]);
		}
		for (int k = 0; k < m_LobbyLongLights.Count; k++)
		{
			m_LobbyLongLights[k].TurnOff();
		}
		for (int l = 0; l < m_RecordingLbls.Count; l++)
		{
			m_RecordingLbls[l].SetActive(false);
		}
		m_InkStairwellEventTrigger.SetActive(active: false);
		m_DarknessProjection.SetActive(true);
		m_Lever.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicDepartmentObjective.IsStarted)
		{
			ForceComplete();
			return;
		}
		m_EntranceTrigger.OnEnter += HandleEntranceTriggerOnEnter;
		m_EntranceTrigger.SetActive(active: true);
		m_InkStairwellEventTrigger.OnEnter += HandleInkStairwellEventTriggerOnEnter;
		m_InkStairwellEventTrigger.SetActive(active: true);
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.OnComplete += HandleLeverOnComplete;
	}

	private void HandleEntranceTriggerOnEnter(object sender, EventArgs e)
	{
		m_EntranceTrigger.OnEnter -= HandleEntranceTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry07Clip, "DIACH2/DIA_CH2_HENRY_07"));
		GameManager.Instance.AudioManager.Play(m_LobbyEntranceMusicClip, AudioObjectType.MUSIC);
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		GameManager.Instance.AudioManager.PlayAtPosition(m_PowerLeverClip, m_Lever.transform.position);
		m_SearcherSpawnInkDrip.SetActive(true);
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		m_Lever.OnComplete -= HandleLeverOnComplete;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicDepartmentObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		TweenSettingsExtensions.OnComplete<Sequence>(DOPowerSequence(), new TweenCallback(base.SendOnComplete));
	}

	private void HandleInkStairwellEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_InkStairwellEventTrigger.OnEnter -= HandleInkStairwellEventTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip08, "DIACH2/DIA_CH2_HENRY_08"));
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_STAIRWELL", "OBJECTIVES/CH2_OBJECTIVE_STAIRWELL_TIP", 4f));
		m_Lever.SetActive(active: true);
	}

	private Sequence DOPowerSequence()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		for (int i = 0; i < m_LobbyLights.Count; i++)
		{
			LobbyLights lobbyLight = m_LobbyLights[i];
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				GameManager.Instance.AudioManager.Play(m_LightClip);
				TurnOnLobbyLight(lobbyLight);
			});
			num += 0.75f;
		}
		for (int num2 = 0; num2 < m_LobbyLongLights.Count; num2++)
		{
			LightController lightController = m_LobbyLongLights[num2];
			TweenSettingsExtensions.InsertCallback(val, num - 0.75f, (TweenCallback)delegate
			{
				lightController.TurnOff();
			});
		}
		for (int num3 = 0; num3 < m_MusicRoomLights.Count; num3++)
		{
			LobbyLights musicRoomLight = m_MusicRoomLights[num3];
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				TurnOnLobbyLight(musicRoomLight);
			});
		}
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(TurnOnRecordingLabels));
		return val;
	}

	private void TurnOnRecordingLabels()
	{
		m_DarknessProjection.SetActive(false);
		for (int i = 0; i < m_RecordingLbls.Count; i++)
		{
			m_RecordingLbls[i].SetActive(true);
		}
	}

	private void TurnOnLobbyLight(LobbyLights lobbyLight)
	{
		if (Object.op_Implicit((Object)(object)lobbyLight.LightFixture))
		{
			lobbyLight.LightFixture.TurnOn();
		}
		if (Object.op_Implicit((Object)(object)lobbyLight.Lights))
		{
			lobbyLight.Lights.SetActive(true);
		}
	}

	private void TurnOffLobbyLight(LobbyLights lobbyLight)
	{
		if (Object.op_Implicit((Object)(object)lobbyLight.LightFixture))
		{
			lobbyLight.LightFixture.TurnOff();
		}
		if (Object.op_Implicit((Object)(object)lobbyLight.Lights))
		{
			lobbyLight.Lights.SetActive(false);
		}
	}

	private void ForceComplete()
	{
		TurnOnRecordingLabels();
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_STAIRWELL", "OBJECTIVES/CH2_OBJECTIVE_STAIRWELL_TIP"));
		m_Lever.ForceOpen();
		m_SearcherSpawnInkDrip.SetActive(true);
		for (int i = 0; i < m_LobbyLights.Count; i++)
		{
			TurnOnLobbyLight(m_LobbyLights[i]);
		}
		for (int j = 0; j < m_LobbyLongLights.Count; j++)
		{
			m_LobbyLongLights[j].TurnOn();
		}
		for (int k = 0; k < m_MusicRoomLights.Count; k++)
		{
			TurnOnLobbyLight(m_MusicRoomLights[k]);
		}
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_Henry07Clip = null;
		m_HenryClip08 = null;
		m_LightClip = null;
		m_PowerLeverClip = null;
		m_LobbyEntranceMusicClip = null;
		if ((Object)(object)m_InkStairwellEventTrigger != (Object)null)
		{
			m_InkStairwellEventTrigger.OnEnter -= HandleInkStairwellEventTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
