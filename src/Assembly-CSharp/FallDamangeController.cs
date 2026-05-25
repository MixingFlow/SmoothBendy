using System;
using TMG.Core;
using UnityEngine;

public class FallDamangeController : TMGMonoBehaviour
{
	[SerializeField]
	private EventTrigger m_FallChecker;

	[SerializeField]
	private EventTrigger m_FallTrigger;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_FallTrigger.SetActive(active: false);
		AddChecker();
	}

	private void HandleFallCheckerOnEnter(object sender, EventArgs e)
	{
		m_FallChecker.OnEnter -= HandleFallCheckerOnEnter;
		AddTrigger();
	}

	private void HandleFallTriggerOnEnter(object sender, EventArgs e)
	{
		m_FallTrigger.OnEnter -= HandleFallTriggerOnEnter;
		for (int i = 0; i < 3; i++)
		{
			GameManager.Instance.ShowHurtBorder();
		}
		AddChecker();
	}

	private void AddChecker()
	{
		m_FallChecker.ResetTrigger();
		m_FallChecker.OnEnter += HandleFallCheckerOnEnter;
		m_FallChecker.SetActive(active: true);
	}

	private void AddTrigger()
	{
		m_FallTrigger.ResetTrigger();
		m_FallTrigger.OnEnter += HandleFallTriggerOnEnter;
		m_FallTrigger.SetActive(active: true);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
