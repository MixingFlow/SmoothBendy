using System;
using UnityEngine;

public class CH5Caves : BaseController
{
	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[SerializeField]
	private GameObject m_Searchers;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ExitTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.CavesObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		m_ExitTrigger.Dispose();
		m_Searchers.SetActive(false);
		SendOnComplete();
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.CavesObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ExitTrigger))
		{
			m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
