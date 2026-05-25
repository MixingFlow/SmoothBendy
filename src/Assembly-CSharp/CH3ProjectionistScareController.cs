using System;
using Ai;
using UnityEngine;

public class CH3ProjectionistScareController : BaseController
{
	[Header("Ai")]
	[SerializeField]
	private BaseAiController m_Projectionist;

	[Header("Event Trigger")]
	[SerializeField]
	private EventTrigger m_ProjectionistTrigger;

	[Header("Door")]
	[SerializeField]
	private BaseDoorController m_Door;

	private AudioClip m_HorrorCue;

	private bool m_IsPreppingDispose;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HorrorCue = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Horror_Cue_02");
		m_ProjectionistTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsSpeakeasyComplete)
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
		m_Door.Lock();
		m_ProjectionistTrigger.SetActive(active: true);
		m_ProjectionistTrigger.OnEnter += HandleProjectionistTriggerOnEnter;
	}

	private void ForceComplete()
	{
		m_Door.Unlock();
		DisposeController();
	}

	private void HandleProjectionistTriggerOnEnter(object sender, EventArgs e)
	{
		m_ProjectionistTrigger.OnEnter -= HandleProjectionistTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_HorrorCue);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsSpeakeasyComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_Door.Unlock();
		m_Door.OnInteracted += HandleDoorOnInteracted;
		m_Projectionist.gameObject.SetActive(true);
		m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointOnComplete;
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		DisposeController();
	}

	private void HandleProjectionistWaypointOnComplete(object sender, EventArgs e)
	{
		DisposeController();
	}

	private void DisposeController()
	{
		if (!m_IsPreppingDispose)
		{
			m_IsPreppingDispose = true;
			KillProjectionist();
			Dispose();
		}
	}

	private void KillProjectionist()
	{
		if ((Object)(object)m_Projectionist != (Object)null)
		{
			m_Projectionist.OnWaypointComplete -= HandleProjectionistWaypointOnComplete;
			if (!m_Projectionist.IsDisposed)
			{
				m_Projectionist.Dispose();
			}
			m_Projectionist = null;
		}
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ProjectionistTrigger))
		{
			m_ProjectionistTrigger.OnEnter -= HandleProjectionistTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_Door))
		{
			m_Door.OnInteracted -= HandleDoorOnInteracted;
		}
		m_HorrorCue = null;
		KillProjectionist();
		base.OnDisposed();
	}
}
