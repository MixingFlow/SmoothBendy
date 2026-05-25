using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5ThroneRoom : BaseController
{
	[SerializeField]
	private GenericDoorController m_EntranceDoor;

	[SerializeField]
	private GenericDoorController m_SideDoor;

	[SerializeField]
	private AudioLog m_AudioLog;

	[SerializeField]
	private Transform m_Reel;

	[SerializeField]
	private GameObject m_Bendy;

	[SerializeField]
	private Transform m_LookLocation;

	[SerializeField]
	private Transform m_CameraParent;

	[SerializeField]
	private AnimationClip m_CameraAnimation;

	[SerializeField]
	private Animator m_CameraAnimator;

	[SerializeField]
	private Transform m_CameraEndLocation;

	[SerializeField]
	private Transform m_PlayerEndLocation;

	[SerializeField]
	private GameObject m_Hole;

	private AudioClip m_MusicClip;

	private AudioClip m_BendyMusicClip;

	private AudioClip m_HenryClip;

	private AudioClip m_WallSmashClip;

	private AudioClip[] m_JoeyClips;

	private AudioClip m_OnClip;

	private AudioClip m_OffClip;

	private AudioClip m_RunningClip;

	private AudioObject m_InkBeatObject;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		AnimationEventUtil.AddAnimationEvent(ref m_CameraAnimator, ((Object)m_CameraAnimation).name, "OnComplete", 920);
		m_Bendy.SetActive(false);
		m_JoeyClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/JoeyAudioLog");
		m_HenryClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Henry/DIA_CH5_HENRY_THE_END");
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_LegacyAndShame");
		m_BendyMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheBeastRevealed");
		m_WallSmashClip = GameManager.Instance.GetAudioClip("Audio/SFX/sfx_wall_smash");
		m_OnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Turn_On_01");
		m_OffClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Turn_Off_01");
		m_RunningClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Run_01");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.ThroneRoomObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.ThroneRoomObjective.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		m_AudioLog.OnInteracted += HandleAudioLogOnInteracted;
		m_AudioLog.SetActive(active: true);
	}

	private void HandleAudioLogOnInteracted(object sender, EventArgs e)
	{
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Expected O, but got Unknown
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Expected O, but got Unknown
		m_AudioLog.OnInteracted -= HandleAudioLogOnInteracted;
		m_AudioLog.SetActive(active: false);
		m_AudioLog.ForceRemoveEffects();
		m_EntranceDoor.ForceClose();
		m_SideDoor.ForceOpen();
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.AudioManager.Play(m_MusicClip, AudioObjectType.MUSIC);
		GameManager.Instance.AudioManager.Play(m_OnClip);
		AudioObject runningClip = GameManager.Instance.AudioManager.Play(m_RunningClip, AudioObjectType.DIALOGUE, -1);
		float num = 0f;
		int num2 = m_JoeyClips.Length - 1;
		for (int i = 0; i < m_JoeyClips.Length; i++)
		{
			AudioClip val = m_JoeyClips[i];
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(val, SubtitleConstants.DIA_CH5_JOEY_AUDIO_LOG[i], isTrimmed: true));
			if (i == num2 - 1)
			{
				audioObject.OnComplete += delegate
				{
					GameManager.Instance.AudioManager.Play(m_HenryClip, AudioObjectType.DIALOGUE);
				};
			}
			if (i != 0 && i != num2 && i != num2 - 1 && i != num2 - 2 && i != num2 - 3)
			{
				num += val.length;
			}
		}
		Transform freeRoamCamera = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.GoToAndLookAt(m_PlayerEndLocation);
		GameManager.Instance.Player.SetLock(active: true);
		Sequence val2 = DOTween.Sequence();
		float num3 = 0f;
		float length = m_JoeyClips[0].length;
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(freeRoamCamera, m_LookLocation.position, length, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(freeRoamCamera, m_LookLocation.eulerAngles, length, (RotateMode)0), (Ease)7));
		num3 += length;
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(freeRoamCamera, new Vector3(0f, 360f, 0f), num, (RotateMode)2), (Ease)7));
		num3 += num;
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(freeRoamCamera, m_Reel.position, m_JoeyClips[num2 - 3].length, (AxisConstraint)0, (Vector3?)null), (Ease)7));
		num3 += m_JoeyClips[num2 - 3].length;
		TweenSettingsExtensions.InsertCallback(val2, num3 - 0.01f, (TweenCallback)delegate
		{
			m_Reel.SetParent(freeRoamCamera);
		});
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Reel, new Vector3(0f, -0.2f, 2f), m_JoeyClips[num2 - 2].length, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Reel, new Vector3(0f, -90f, -15f), m_JoeyClips[num2 - 2].length, (RotateMode)0), (Ease)7));
		num3 += m_JoeyClips[num2 - 2].length;
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Reel, new Vector3(0f, 90f, 15f), m_JoeyClips[num2 - 1].length, (RotateMode)0), (Ease)7));
		num3 += m_JoeyClips[num2 - 1].length + m_JoeyClips[num2].length;
		TweenSettingsExtensions.Insert(val2, num3, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Reel, new Vector3(0f, -3f, 0f), 0.5f, false), (Ease)7));
		TweenSettingsExtensions.InsertCallback(val2, num3, (TweenCallback)delegate
		{
			runningClip.Stop();
			m_Bendy.SetActive(true);
			GameManager.Instance.AudioManager.Play(m_OffClip);
			GameManager.Instance.AudioManager.Play(m_BendyMusicClip, AudioObjectType.MUSIC);
			S13AudioManager.Instance.PlayAudio("sfx_beast_reveal_anim");
		});
		num3 += 0.25f;
		TweenSettingsExtensions.InsertCallback(val2, num3, (TweenCallback)delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			freeRoamCamera.SetParent(m_CameraParent);
			ShortcutExtensions.DOLocalMove(freeRoamCamera, Vector3.zero, 0.5f, false);
			ShortcutExtensions.DOLocalRotate(freeRoamCamera, new Vector3(0f, 90f, 90f), 0.5f, (RotateMode)0);
		});
	}

	private void ForceStart()
	{
	}

	private void ForceComplete()
	{
		m_Hole.SetActive(false);
		SendOnComplete();
	}

	public void RevealOnComplete()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		((Component)m_Reel).gameObject.SetActive(false);
		m_Hole.SetActive(false);
		m_Bendy.SetActive(false);
		RenderSettings.ambientIntensity = 0.85f;
		GameManager.Instance.AudioManager.Play(m_WallSmashClip);
		GameManager.Instance.GameCamera.FreeRoamCam.SetParent((Transform)null);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(GameManager.Instance.GameCamera.FreeRoamCam, m_CameraEndLocation.position, 0.5f, false), (Ease)1);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(GameManager.Instance.GameCamera.FreeRoamCam, m_CameraEndLocation.eulerAngles, 0.5f, (RotateMode)0), (Ease)1);
		Sequence val = DOTween.Sequence();
		float num = 1.5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(GameManager.Instance.GameCamera.FreeRoamCam, GameManager.Instance.Player.HeadContainer.position, 1.5f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(GameManager.Instance.GameCamera.FreeRoamCam, GameManager.Instance.Player.HeadContainer.eulerAngles, 1.5f, (RotateMode)0), (Ease)7));
		num += 2.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			GameManager.Instance.Player.SetLock(active: false);
			GameManager.Instance.ShowCrosshair();
		});
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_JoeyClips = null;
		m_HenryClip = null;
		m_MusicClip = null;
		m_BendyMusicClip = null;
		if (Object.op_Implicit((Object)(object)m_InkBeatObject))
		{
			m_InkBeatObject.Clear();
			m_InkBeatObject = null;
		}
		base.OnDisposed();
	}
}
