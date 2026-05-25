using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4ResearchController : BaseController
{
	[Header("[Power Station]")]
	[SerializeField]
	private CH4WarehousePowerStation m_PowerStation;

	[Header("Entrance")]
	[SerializeField]
	private EventTrigger m_ResearchEnterTrigger;

	[SerializeField]
	private EventTrigger m_ResearchDialogueTrigger;

	[Header("Interactables")]
	[SerializeField]
	private Interactable m_InteractableCan;

	[Header("<Objective: Power Switch>")]
	[SerializeField]
	private CH3LeverLight m_ResearchLever;

	[Header("<Objective: Open Door>")]
	[SerializeField]
	private GenericDoorController m_ResearchDoor;

	[SerializeField]
	private InteractablePowerLever m_ResearchDoorLever;

	[Header("BACON!")]
	[SerializeField]
	private EventTrigger m_LoseBaconTrigger;

	[SerializeField]
	private GameObject m_Weapons;

	[SerializeField]
	private GameObject m_ActualWeapons;

	[SerializeField]
	private GameObject m_BaconSoupPhysicsPrefab;

	[SerializeField]
	private AudioClip m_CanDropClip;

	[Header("Enemies")]
	[SerializeField]
	private GameObject[] m_Ai;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip[] m_HenryDialogueResearch;

	private BaconSoupWeapon m_FakeSoupCan;

	private S13Switch m_BaconSoupAudioSwitch;

	private AudioClip m_Henry05Clip;

	private AudioClip m_LeverClip;

	private List<BaseAiController> m_ButcherGang = new List<BaseAiController>();

	public override void Init()
	{
		base.Init();
		for (int i = 0; i < m_Ai.Length; i++)
		{
			BaseAiController component = m_Ai[i].GetComponent<BaseAiController>();
			m_Ai[i].SetActive(false);
			if (Object.op_Implicit((Object)(object)component))
			{
				component.OnDeath += HandleButcherGangOnDeath;
				m_ButcherGang.Add(component);
			}
		}
	}

	private void HandleButcherGangOnDeath(object sender, EventArgs e)
	{
		BaseAiController baseAiController = sender as BaseAiController;
		baseAiController.OnDeath -= HandleButcherGangOnDeath;
		if (m_ButcherGang.Contains(baseAiController))
		{
			int num = m_ButcherGang.IndexOf(baseAiController);
			if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[num] != -1)
			{
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[num] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[num] * 414;
				GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
			}
		}
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryDialogueResearch = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Henry/Research/");
		m_Henry05Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_05");
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_FakeSoupCan = ((Component)m_InteractableCan).GetComponent<BaconSoupWeapon>();
		m_Weapons.SetActive(true);
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasDied)
		{
			if (Object.op_Implicit((Object)(object)m_ActualWeapons))
			{
				Object.Destroy((Object)(object)m_ActualWeapons);
			}
		}
		else if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			m_ActualWeapons.SetActive(false);
		}
		m_LoseBaconTrigger.SetActive(active: false);
		m_ResearchLever.Disable();
		if (Object.op_Implicit((Object)(object)m_ResearchDoorLever))
		{
			m_ResearchDoorLever.SetActive(active: false);
		}
		m_ResearchEnterTrigger.SetActive(active: false);
		m_ResearchDialogueTrigger.SetActive(active: false);
		m_BaconSoupAudioSwitch = ((Component)m_InteractableCan).GetComponentInChildren<S13Switch>();
	}

	public override void Activate()
	{
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
		for (int i = 0; i < m_Ai.Length; i++)
		{
			m_Ai[i].SetActive(true);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.ResearchObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.ResearchObjective.IsStarted)
		{
			if (Object.op_Implicit((Object)(object)m_FakeSoupCan))
			{
				m_FakeSoupCan.Dispose();
			}
			if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasDied)
			{
				if (Object.op_Implicit((Object)(object)m_ActualWeapons))
				{
					Object.Destroy((Object)(object)m_ActualWeapons);
				}
			}
			else if (Object.op_Implicit((Object)(object)m_ActualWeapons))
			{
				m_ActualWeapons.SetActive(true);
			}
			m_ResearchLever.ForceComplete();
			m_ResearchDoor.ForceOpen();
			m_ResearchDoorLever.ForceOpen();
			m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
			m_PowerStation.ActivatePower();
		}
		else
		{
			m_ResearchDoor.Open();
			m_ResearchEnterTrigger.OnEnter += HandleResearchEnterTriggerOnEnter;
			m_ResearchEnterTrigger.SetActive(active: true);
			m_ResearchDialogueTrigger.OnEnter += HandleResearchDialogueTriggerOnEnter;
			m_ResearchDialogueTrigger.SetActive(active: true);
			if (Object.op_Implicit((Object)(object)m_FakeSoupCan))
			{
				m_FakeSoupCan.OnHit += HandleFakeSoupCanOnHit;
			}
			m_InteractableCan.OnInteracted += HandleOnCanInteracted;
			m_InteractableCan.SetActive(active: true);
			m_ResearchLever.OnComplete += HandleResearchLeverOnComplete;
		}
	}

	private void HandleFakeSoupCanOnHit(object sender, EventArgs e)
	{
		m_FakeSoupCan.OnHit -= HandleFakeSoupCanOnHit;
		m_BaconSoupAudioSwitch.Play("land");
	}

	private void HandleOnCanInteracted(object sender, EventArgs e)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		m_InteractableCan.OnInteracted -= HandleOnCanInteracted;
		Rigidbody component = ((Component)m_InteractableCan).GetComponent<Rigidbody>();
		component.isKinematic = false;
		component.velocity = ((Component)component).transform.up * -10f + ((Component)component).transform.right * -5f;
	}

	private void HandleResearchDialogueTriggerOnEnter(object sender, EventArgs e)
	{
		m_ResearchDialogueTrigger.OnEnter -= HandleResearchDialogueTriggerOnEnter;
		for (int i = 0; i < m_HenryDialogueResearch.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDialogueResearch[i], SubtitleConstants.DIALOGUE_CH4_HENRY_RESEARCH[i], isTrimmed: true));
		}
	}

	private void HandleResearchEnterTriggerOnEnter(object sender, EventArgs e)
	{
		m_ResearchEnterTrigger.OnEnter -= HandleResearchEnterTriggerOnEnter;
		m_ResearchDoor.Close();
		m_ResearchLever.Activate();
	}

	private void HandleResearchLeverOnComplete(object sender, EventArgs e)
	{
		m_ResearchLever.OnComplete -= HandleResearchLeverOnComplete;
		m_ResearchDoorLever.OnInteracted += HandleResearchDoorLeverOnInteracted;
		m_ResearchDoorLever.SetActive(active: true);
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry05Clip, SubtitleConstants.DIA_CH4_HENRY_05));
		m_LoseBaconTrigger.OnEnter += HandleLoseBaconTriggerOnEnter;
		m_LoseBaconTrigger.SetActive(active: true);
		m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
		m_PowerStation.ActivatePower();
	}

	private void HandleLoseBaconTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		m_LoseBaconTrigger.OnEnter -= HandleLoseBaconTriggerOnEnter;
		Object.Destroy((Object)(object)m_Weapons);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.ResearchObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.4f, (TweenCallback)delegate
			{
				GameManager.Instance.AudioManager.Play(m_CanDropClip);
			});
			GameObject val = Object.Instantiate<GameObject>(m_BaconSoupPhysicsPrefab);
			val.transform.position = GameManager.Instance.Player.WeaponGameObject.transform.position;
			val.transform.eulerAngles = GameManager.Instance.Player.WeaponGameObject.transform.eulerAngles;
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
			GameManager.Instance.Player.UnEquipWeapon();
		}
	}

	private void HandleResearchDoorLeverOnInteracted(object sender, EventArgs e)
	{
		m_ResearchDoorLever.OnInteracted -= HandleResearchDoorLeverOnInteracted;
		m_ResearchDoorLever.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_LeverClip);
		m_ResearchDoor.Open();
	}

	private void HandlePowerStationOnPowerActivated(object sender, EventArgs e)
	{
		m_PowerStation.OnPowerActivated -= HandlePowerStationOnPowerActivated;
		if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			m_ActualWeapons.SetActive(true);
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.ResearchObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			Object.Destroy((Object)(object)m_ActualWeapons);
		}
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_09");
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.RideStorageObjective.IsStarted)
		{
			if (Object.op_Implicit((Object)(object)m_ActualWeapons))
			{
				Object.Destroy((Object)(object)m_ActualWeapons);
			}
		}
		else if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			m_ActualWeapons.SetActive(true);
		}
		m_ResearchLever.ForceComplete();
		m_ResearchDoorLever.ForceOpen();
		m_ResearchDoor.ForceOpen();
		m_PowerStation.ForceActivatePower();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		m_LeverClip = null;
		m_Henry05Clip = null;
		base.OnDisposed();
	}
}
