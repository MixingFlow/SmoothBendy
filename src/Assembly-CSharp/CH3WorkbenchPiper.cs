using System;
using TMG.Core;
using UnityEngine;

public class CH3WorkbenchPiper : TMGMonoBehaviour
{
	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private GameObject m_Piper;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PowerCoreTask.Object[0].IsComplete)
		{
			m_Door.ForceOpen(145f);
			m_Piper.SetActive(false);
		}
		else
		{
			m_Door.OnInteracted += HandleDoorOnInteracted;
		}
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		m_Piper.SetActive(false);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
