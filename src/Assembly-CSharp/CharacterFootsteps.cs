using System;
using System.Collections.Generic;
using S13Audio;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

[Serializable]
public class CharacterFootsteps : TMGAbstractDisposable
{
	[Serializable]
	public class LandClips
	{
		public FootstepTypes Type;

		public List<AudioClip> Clips;

		public string ID;
	}

	[SerializeField]
	private Transform m_FootstepAudioPosition;

	[SerializeField]
	private bool m_Active;

	[SerializeField]
	private float m_StepInterval = 8f;

	[SerializeField]
	private float m_RunstepLength = 0.7f;

	[SerializeField]
	private List<LandClips> m_LandClips;

	private List<FootstepsDataVO> m_FootstepDataVOs;

	private FootstepTypes m_CurrentFootstepType;

	private AudioClip[] m_CurrentFootstepClips;

	private float m_StepCycle;

	private float m_NextStep;

	private bool m_IsMoving;

	private string m_AudioID;

	public float StepInterval => m_StepInterval;

	public float RunstepLength => m_RunstepLength;

	public FootstepTypes CurrentFootstepType => m_CurrentFootstepType;

	public S13Switch AudioSwitch { get; private set; }

	public void Init(List<FootstepsDataVO> dataVO)
	{
		if (m_Active)
		{
			m_FootstepDataVOs = dataVO;
			AudioSwitch = ((Component)m_FootstepAudioPosition).GetComponentInChildren<S13Switch>();
		}
	}

	public void SetFootstepType(FootstepTypes footstepType)
	{
		if (!m_Active || m_CurrentFootstepType == footstepType)
		{
			return;
		}
		int count = m_FootstepDataVOs.Count;
		for (int i = 0; i < count; i++)
		{
			FootstepsDataVO footstepsDataVO = m_FootstepDataVOs[i];
			if (footstepsDataVO.Type == footstepType)
			{
				m_CurrentFootstepType = footstepType;
				m_CurrentFootstepClips = footstepsDataVO.Clips;
				m_AudioID = footstepsDataVO.ID;
			}
		}
	}

	public void ProgressStepCycle(float magnitude, float speed)
	{
		if (m_Active)
		{
			if (magnitude > 0f && (PlayerInput.MoveX() != 0f || PlayerInput.MoveY() != 0f))
			{
				m_IsMoving = true;
				m_StepCycle += (magnitude + speed) * Time.deltaTime;
			}
			else
			{
				PlayFootStepScuffAudio();
			}
			if (!(m_StepCycle <= m_NextStep))
			{
				m_NextStep = m_StepCycle + m_StepInterval;
				AudioSwitch.Play(m_AudioID);
			}
		}
	}

	private void PlayFootStepScuffAudio()
	{
		if (m_IsMoving)
		{
			m_IsMoving = false;
		}
	}

	public void PlayJumpAudio()
	{
		AudioSwitch.Play("jump");
	}

	public void PlayLandAudio()
	{
		string id = "land_wood";
		if (m_CurrentFootstepType == FootstepTypes.DIRT)
		{
			id = "land_dirt";
		}
		else if (m_CurrentFootstepType == FootstepTypes.METAL)
		{
			id = "land_metal";
		}
		else if (m_CurrentFootstepType == FootstepTypes.TILE)
		{
			id = "land_tile";
		}
		AudioSwitch.Play(id);
	}

	protected override void OnDisposed()
	{
		if (m_FootstepDataVOs != null)
		{
			for (int i = 0; i < m_FootstepDataVOs.Count; i++)
			{
				m_FootstepDataVOs[i].Dispose();
			}
			m_FootstepDataVOs.Clear();
			m_FootstepDataVOs = null;
		}
		if (m_CurrentFootstepClips != null)
		{
			m_CurrentFootstepClips = null;
		}
		base.OnDisposed();
	}
}
