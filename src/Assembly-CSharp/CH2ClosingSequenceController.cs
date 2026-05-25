using System;
using DG.Tweening;
using UnityEngine;

public class CH2ClosingSequenceController : BaseController
{
	[Header("< END >")]
	[SerializeField]
	private Transform m_FinalLookAt;

	[SerializeField]
	private Transform m_BorisEndPos;

	[SerializeField]
	private Transform m_FinalRot;

	[SerializeField]
	private Transform m_BorisNeck;

	[SerializeField]
	private Transform m_Can;

	[SerializeField]
	private Transform m_CanEndPos;

	[SerializeField]
	private EventTrigger m_FinalTrigger;

	[SerializeField]
	private BorisAi m_Boris;

	[SerializeField]
	private WaypointList m_WaypointList;

	private Sequence m_Sequence;

	private AudioClip[] m_BorisFootstepClips;

	private AudioClip m_CanKickClip;

	private AudioClip m_HorrorCueClip;

	private AudioClip m_HenryClip15;

	private AudioClip m_HenryClip16;

	private AudioClip m_WhooshClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BorisFootstepClips = GameManager.Instance.GetAudioClips("Audio/SFX/Footsteps/Boris/Wood");
		m_HenryClip15 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_15");
		m_HenryClip16 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_16");
		m_CanKickClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Can_Roll_slow_01");
		m_HorrorCueClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Horror_Cue_02");
		m_WhooshClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_HUD_whoosh_01");
		m_Boris = GameManager.Instance.CharacterManager.Boris;
		m_FinalTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		m_FinalTrigger.OnEnter += HandleFinalTriggerOnEnter;
		m_FinalTrigger.SetActive(active: true);
	}

	private void HandleFinalTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		m_FinalTrigger.OnEnter -= HandleFinalTriggerOnEnter;
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.LockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.SetLock(active: true);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(GameManager.Instance.Player.HeadContainer, m_FinalLookAt.position, 2f, (AxisConstraint)0, (Vector3?)null), (Ease)7);
		GameManager.Instance.AudioManager.Play(m_CanKickClip);
		TweenSettingsExtensions.OnComplete<Sequence>(DOSequence(), new TweenCallback(SequenceOnComplete));
	}

	private Sequence DOSequence()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Can, m_CanEndPos.position, 3.5f, false), (Ease)1));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Can, new Vector3(0f, 2160f, 0f), 3.5f, (RotateMode)3), (Ease)1));
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_HorrorCueClip);
		});
		return m_Sequence;
	}

	private void SequenceOnComplete()
	{
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip15, "DIACH2/DIA_CH2_HENRY_15")).OnComplete += HandleDialogueOnComplete;
	}

	private void HandleDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleDialogueOnComplete;
		m_Boris.EnableWaypointPathing();
		m_Boris.OnWaypointComplete += HandleBorisOnWaypointComplete;
		m_Boris.UpdateWaypointList(m_WaypointList.Waypoints);
		DOBorisSequence();
	}

	private void HandleBorisOnWaypointComplete(object sender, EventArgs e)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		m_Boris.OnWaypointComplete -= HandleBorisOnWaypointComplete;
		m_WaypointList.Dispose();
		m_Boris.StopWaypointPathing();
		TweenExtensions.Kill((Tween)(object)m_Sequence, false);
		TweenSettingsExtensions.OnComplete<Sequence>(DOFinalSequence(), new TweenCallback(base.SendOnComplete));
	}

	private Sequence DOBorisSequence()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		ResetSequence();
		for (int i = 0; i < 20; i++)
		{
			TweenSettingsExtensions.InsertCallback(m_Sequence, 2f / 3f * (float)i, new TweenCallback(PlayFootStepAudio));
		}
		return m_Sequence;
	}

	private Sequence DOFinalSequence()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		ResetSequence();
		float num = 1f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_WhooshClip);
		});
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0f);
		});
		num += 2f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip16, "DIACH2/DIA_CH2_HENRY_16"));
		});
		num += m_HenryClip16.length + 0.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
		});
		return m_Sequence;
	}

	private void PlayFootStepAudio()
	{
		if (m_BorisFootstepClips != null && m_BorisFootstepClips.Length > 0)
		{
			int num = Random.Range(0, m_BorisFootstepClips.Length);
			AudioClip val = m_BorisFootstepClips[num];
			GameManager.Instance.AudioManager.Play(val);
			m_BorisFootstepClips[num] = m_BorisFootstepClips[0];
			m_BorisFootstepClips[0] = val;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		if (Object.op_Implicit((Object)(object)m_FinalTrigger))
		{
			m_FinalTrigger.OnEnter -= HandleFinalTriggerOnEnter;
		}
		m_BorisFootstepClips = null;
		m_CanKickClip = null;
		m_HorrorCueClip = null;
		m_HenryClip15 = null;
		m_HenryClip16 = null;
		m_WhooshClip = null;
		base.OnDisposed();
	}
}
