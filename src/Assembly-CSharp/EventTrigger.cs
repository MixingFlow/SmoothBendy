using System;
using TMG.Core;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : TMGMonoBehaviour
{
	[Header("Trigger Type")]
	[SerializeField]
	private bool m_OnStay = true;

	[SerializeField]
	private bool m_OnEnter;

	[Header("Trigger Options")]
	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private bool m_IsSingleTrigger = true;

	[SerializeField]
	private bool m_OnExit;

	[Header("Collider Type")]
	[SerializeField]
	private bool m_UseAi;

	private Collider m_Collider;

	public bool IsTriggered { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public override void Init()
	{
		base.Init();
		base.gameObject.layer = LayerMask.NameToLayer("EventTrigger");
		ActualGetCollider();
	}

	private void GetCollider()
	{
		ActualGetCollider();
		if (Object.op_Implicit((Object)(object)m_Collider))
		{
			m_Collider.isTrigger = true;
		}
	}

	private void ActualGetCollider()
	{
		m_Collider = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)m_Collider))
		{
			m_Collider.enabled = m_IsActive;
		}
	}

	public void SetActive(bool active)
	{
		if (Object.op_Implicit((Object)(object)m_Collider))
		{
			m_Collider.enabled = (m_IsActive = active);
		}
	}

	public void ResetTrigger()
	{
		IsTriggered = false;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (m_OnEnter && m_IsActive && !IsTriggered)
		{
			ActualTriggerEnter(col);
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (m_OnStay && m_IsActive && !IsTriggered)
		{
			ActualTriggerEnter(col);
		}
	}

	private void ActualTriggerEnter(Collider col)
	{
		string text = ((!m_UseAi) ? "Player" : "Ai");
		if (((Component)col).CompareTag(text))
		{
			IsTriggered = true;
			this.OnEnter.Send(this);
			if (m_IsSingleTrigger)
			{
				Dispose();
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (m_OnExit && m_IsActive && IsTriggered)
		{
			string text = ((!m_UseAi) ? "Player" : "Ai");
			if (((Component)col).CompareTag(text))
			{
				IsTriggered = false;
				this.OnExit.Send(this);
			}
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
	}
}
