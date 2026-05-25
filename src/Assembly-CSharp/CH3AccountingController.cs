using System;
using TMG.Core;
using UnityEngine;

public class CH3AccountingController : TMGMonoBehaviour
{
	[SerializeField]
	private MeleeWeapon m_HiddenAxe;

	[SerializeField]
	private BlockedDoorController m_AccountingDoor;

	private Vector3 m_AxeOriginPosition;

	private Vector3 m_AxeOriginRotation;

	private bool m_IsUnlocked;

	public override void InitOnComplete()
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AccountingRoom.IsComplete)
		{
			m_IsUnlocked = true;
			m_AccountingDoor.ForceOpen();
			m_AccountingDoor.Door.ForceOpen(140f);
			m_AccountingDoor.Door.Lock();
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.UsedAxe && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ButcherGangTask.IsStarted)
			{
				if (Object.op_Implicit((Object)(object)m_HiddenAxe))
				{
					m_HiddenAxe.Interaction.SetActive(active: true);
				}
			}
			else if (Object.op_Implicit((Object)(object)m_HiddenAxe))
			{
				m_HiddenAxe.Dispose();
			}
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AccountingRoom.IsStarted)
		{
			m_AccountingDoor.ForceOpen();
			m_AccountingDoor.Door.OnInteracted += HandleDoorOnInteracted;
		}
		else if (Object.op_Implicit((Object)(object)m_HiddenAxe))
		{
			m_HiddenAxe.Interaction.SetActive(active: false);
		}
		m_AxeOriginPosition = m_HiddenAxe.transform.position;
		m_AxeOriginRotation = m_HiddenAxe.transform.eulerAngles;
	}

	public void Activate()
	{
		if (!m_IsUnlocked)
		{
			m_IsUnlocked = true;
			m_AccountingDoor.OnUnlocked += HandleAccountingDoorOnUnlocked;
		}
	}

	public void ActivateAxe()
	{
		if (Object.op_Implicit((Object)(object)m_HiddenAxe))
		{
			m_HiddenAxe.Interaction.OnInteracted += HandleAxeOnInteracted;
			m_HiddenAxe.Interaction.SetActive(active: true);
		}
	}

	public void DisableAxe()
	{
		if (Object.op_Implicit((Object)(object)m_HiddenAxe) && Object.op_Implicit((Object)(object)m_HiddenAxe.Interaction))
		{
			m_HiddenAxe.Interaction.SetActive(active: false);
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.UsedAxe = false;
	}

	private void HandleAxeOnInteracted(object sender, EventArgs e)
	{
		if (Object.op_Implicit((Object)(object)m_HiddenAxe))
		{
			if (Object.op_Implicit((Object)(object)m_HiddenAxe.Interaction))
			{
				m_HiddenAxe.Interaction.OnInteracted -= HandleAxeOnInteracted;
			}
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.UsedAxe = true;
		}
	}

	public void Reset()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_HiddenAxe))
		{
			m_HiddenAxe.Dispose();
		}
		m_HiddenAxe = GameManager.Instance.AssetManager.CreateAsset<MeleeWeapon>("GamePlay/Weapons/Weapon_Axe");
		m_HiddenAxe.transform.position = m_AxeOriginPosition;
		m_HiddenAxe.transform.eulerAngles = m_AxeOriginRotation;
		m_HiddenAxe.Interaction.SetActive(active: false);
	}

	private void HandleAccountingDoorOnUnlocked(object sender, EventArgs e)
	{
		m_AccountingDoor.OnUnlocked -= HandleAccountingDoorOnUnlocked;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AccountingRoom.IsStarted = true;
		m_AccountingDoor.Door.OnInteracted += HandleDoorOnInteracted;
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		m_AccountingDoor.Door.OnInteracted -= HandleDoorOnInteracted;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AccountingRoom.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
