using System;
using UnityEngine;

public class CH2PipeOrganController : BaseController
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_AudioPosition;

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_PipeOrganInteract;

	private AudioObject m_PipeOrganAudioObject;

	private AudioClip[] m_PipeOrganAudioClips;

	private int m_InteractCount;

	private int m_InteractMax = 5;

	private bool m_HasAchievement;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_PipeOrganAudioClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Instruments/PipeOrgan/");
		m_PipeOrganInteract.SetActive(active: true);
		m_PipeOrganInteract.OnInteracted += HandlePipeOrganOnInteracted;
	}

	private void HandlePipeOrganOnInteracted(object sender, EventArgs e)
	{
		m_PipeOrganInteract.SetActive(active: false);
		m_PipeOrganInteract.OnInteracted -= HandlePipeOrganOnInteracted;
		PlayPipeOrganAudio();
		if (!m_HasAchievement)
		{
			m_InteractCount++;
			if (m_InteractCount >= m_InteractMax)
			{
				m_HasAchievement = true;
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.JOHNNYS_BROKEN_HEART);
			}
		}
	}

	private void HandlePipeOrganAudioOnComplete(object sender, EventArgs e)
	{
		m_PipeOrganAudioObject.OnComplete -= HandlePipeOrganAudioOnComplete;
		m_PipeOrganInteract.SetActive(active: true);
		m_PipeOrganInteract.OnInteracted += HandlePipeOrganOnInteracted;
	}

	private void PlayPipeOrganAudio()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (m_PipeOrganAudioClips != null && m_PipeOrganAudioClips.Length > 0)
		{
			int num = Random.Range(0, m_PipeOrganAudioClips.Length);
			AudioClip val = m_PipeOrganAudioClips[num];
			m_PipeOrganAudioObject = null;
			m_PipeOrganAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(val, m_AudioPosition.position);
			m_PipeOrganAudioObject.OnComplete += HandlePipeOrganAudioOnComplete;
			m_PipeOrganAudioClips[num] = m_PipeOrganAudioClips[0];
			m_PipeOrganAudioClips[0] = val;
		}
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_PipeOrganInteract != (Object)null)
		{
			m_PipeOrganInteract.OnInteracted -= HandlePipeOrganOnInteracted;
		}
		if ((Object)(object)m_PipeOrganAudioObject != (Object)null)
		{
			m_PipeOrganAudioObject.OnComplete -= HandlePipeOrganAudioOnComplete;
			m_PipeOrganAudioObject.Clear();
			m_PipeOrganAudioObject = null;
		}
		m_PipeOrganAudioClips = null;
		base.OnDisposed();
	}
}
