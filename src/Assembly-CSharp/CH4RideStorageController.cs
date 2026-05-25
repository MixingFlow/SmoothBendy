using System;
using S13Audio;
using UnityEngine;

public class CH4RideStorageController : BaseController
{
	[Header("[Power Station]")]
	[SerializeField]
	private CH4WarehousePowerStation m_PowerStation;

	[Header("Objective: Bert!")]
	[SerializeField]
	private CH3LeverLight m_StorageLever;

	[SerializeField]
	private EventTrigger m_StorageEnterTrigger;

	[SerializeField]
	private GenericDoorController m_StorageDoor;

	[SerializeField]
	private GenericDoorController m_StorageLeverDoor;

	[SerializeField]
	private GenericDoorController[] m_StorageGates;

	[SerializeField]
	private CH4BertrumController m_Bert;

	[SerializeField]
	private GameObject m_ActualWeapons;

	[Header("Spawners")]
	[SerializeField]
	private GameObject m_DeathSpawner;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_Henry06Clip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Henry06Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_06");
		m_StorageLever.Disable();
		m_StorageEnterTrigger.SetActive(active: false);
		for (int i = 0; i < m_StorageGates.Length; i++)
		{
			m_StorageGates[i].ForceOpen();
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.RideStorageObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.RideStorageObjective.IsStarted)
		{
			ForceDeath();
			return;
		}
		m_StorageDoor.Open();
		m_StorageEnterTrigger.OnEnter += HandleStorageEnterTriggerOnEnter;
		m_StorageEnterTrigger.SetActive(active: true);
	}

	private void ForceDeath()
	{
		if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			Object.Destroy((Object)(object)m_ActualWeapons);
		}
		m_StorageLever.ForceComplete();
		m_StorageLeverDoor.ForceOpen();
		m_StorageDoor.ForceOpen();
		m_DeathSpawner.SetActive(false);
		m_Bert.ForceComplete();
		m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
		m_PowerStation.ActivatePower();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_10");
		if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			Object.Destroy((Object)(object)m_ActualWeapons);
		}
		m_StorageLever.ForceComplete();
		m_StorageLeverDoor.ForceOpen();
		m_StorageDoor.ForceOpen();
		m_DeathSpawner.SetActive(false);
		m_Bert.ForceComplete();
		m_PowerStation.ForceActivatePower();
		SendOnComplete();
	}

	private void HandleStorageEnterTriggerOnEnter(object sender, EventArgs e)
	{
		m_StorageEnterTrigger.OnEnter -= HandleStorageEnterTriggerOnEnter;
		if (Object.op_Implicit((Object)(object)m_ActualWeapons))
		{
			Object.Destroy((Object)(object)m_ActualWeapons);
		}
		m_StorageDoor.Close();
		m_Bert.OnBegin += HandleBertOnBegin;
		m_Bert.OnComplete += HandleBertOnComplete;
		m_Bert.Activate();
	}

	private void HandleBertOnBegin(object sender, EventArgs e)
	{
		m_Bert.OnBegin -= HandleBertOnBegin;
		for (int i = 0; i < m_StorageGates.Length; i++)
		{
			m_StorageGates[i].Close();
		}
	}

	private void HandleBertOnComplete(object sender, EventArgs e)
	{
		m_Bert.OnComplete -= HandleBertOnComplete;
		for (int i = 0; i < m_StorageGates.Length; i++)
		{
			m_StorageGates[i].Open();
		}
		m_StorageLeverDoor.Open();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject) && !Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject.GetComponent<MeleeWeapon>()) && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[3] != -1)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[3] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[3] * 414;
		}
		m_StorageLever.Activate();
		m_StorageLever.OnComplete += HandleStorageLeverOnComplete;
	}

	private void HandleStorageLeverOnComplete(object sender, EventArgs e)
	{
		m_StorageLever.OnComplete -= HandleStorageLeverOnComplete;
		m_StorageDoor.Open();
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry06Clip, SubtitleConstants.DIA_CH4_HENRY_06));
		m_DeathSpawner.SetActive(false);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.RideStorageObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
		m_PowerStation.ActivatePower();
	}

	private void HandlePowerStationOnPowerActivated(object sender, EventArgs e)
	{
		m_PowerStation.OnPowerActivated -= HandlePowerStationOnPowerActivated;
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.RideStorageObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_Henry06Clip = null;
		base.OnDisposed();
	}
}
