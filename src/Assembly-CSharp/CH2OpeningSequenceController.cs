using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CH2OpeningSequenceController : BaseController
{
	[Header("< START >")]
	[SerializeField]
	private Transform m_StartPosition;

	[SerializeField]
	private Transform m_KneelPosition;

	[SerializeField]
	private AnimationCurve m_KneelCurve;

	private GameCamera m_GameCam;

	private Transform m_GameCamTransform;

	private AudioClip m_TitleMusicClip;

	private AudioClip m_RingingEarsClip;

	private AudioClip m_StandUpClip;

	private AudioClip m_HenryClip01;

	private AudioClip m_HenryClip02;

	private AudioClip m_HenryClipGetUp;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_TitleMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Chapter_Two_Title");
		m_RingingEarsClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Ringing_Ears_01");
		m_StandUpClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Stand_Up_Player_01");
		m_HenryClip01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_01");
		m_HenryClip02 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_02");
		m_HenryClipGetUp = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_GET_UP");
	}

	public override void Activate()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.RitualObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_GameCam = GameManager.Instance.GameCamera;
		m_GameCamTransform = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_GameCamTransform.position = m_StartPosition.position;
		m_GameCamTransform.eulerAngles = m_StartPosition.eulerAngles;
		if (m_GameCam.DoF)
		{
			m_GameCam.UnityDOF.manualDOF = true;
			m_GameCam.UnityDOF.focalDistance = 0f;
		}
		DOOpeningSequence();
	}

	private void ForceComplete()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.transform.position = GameManager.Instance.GameData.CurrentSaveFile.CH2Data.PlayerPosition.GetVector();
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(GameManager.Instance.GameData.CurrentSaveFile.CH2Data.PlayerRotation.GetVector()));
		GameManager.Instance.HideScreenBlocker(0.1f, 0.5f);
		GameManager.Instance.ShowCrosshair();
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetInteraction(active: true);
		GameManager.Instance.Player.SetCameraSway(active: true);
		SendOnComplete();
	}

	private Sequence DOOpeningSequence()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 1f;
		float duration = 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_TitleMusicClip, AudioObjectType.MUSIC);
			GameManager.Instance.ShowHurtBorder(isSilent: true);
		});
		num += 5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH2_LABEL", "MENU/CH2_TITLE", showBlocker: false);
		});
		num += 2f;
		for (int num2 = 0; num2 < 4; num2++)
		{
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				GameManager.Instance.ShowHurtBorder(isSilent: true);
			});
		}
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(PlayIntroDialogue_01));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(duration);
		});
		num += duration;
		duration = 0.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(duration);
		});
		num += duration + 0.75f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(duration);
		});
		num += duration + 0.25f;
		if (m_GameCam.DoF)
		{
			m_GameCam.UnityDOF.manualDOF = true;
			float distance = m_GameCam.UnityDOF.focalDistance;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => distance), (DOSetter<float>)delegate(float value)
			{
				distance = value;
			}, 5f, 2f), (Ease)1), (TweenCallback)delegate
			{
				m_GameCam.UnityDOF.focalDistance = distance;
			}));
		}
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker();
		});
		num += 0.6f;
		if (m_GameCam.DoF)
		{
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				m_GameCam.UnityDOF.manualDOF = false;
			});
		}
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker();
		});
		return val;
	}

	private Sequence DOGetUpSequence()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		float num = 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			((Component)m_GameCam.Camera).transform.SetParent(GameManager.Instance.Player.CameraParent);
		});
		TweenSettingsExtensions.InsertCallback(val, num += 0.5f, new TweenCallback(PlayIntroDialogue_02));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_GameCamTransform, m_KneelPosition.position, 1.5f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_GameCamTransform, m_KneelPosition.eulerAngles, 1.5f, (RotateMode)0), (Ease)7));
		num += 2.1f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_GameCamTransform, GameManager.Instance.Player.HeadContainer.position, 2f, false), m_KneelCurve));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_GameCamTransform, GameManager.Instance.Player.transform.eulerAngles, 2f, (RotateMode)0), m_KneelCurve));
		return val;
	}

	private void PlayIntroDialogue_01()
	{
		GameManager.Instance.AudioManager.Play(m_RingingEarsClip);
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip01, "DIACH2/DIA_CH2_HENRY_01", isTrimmed: true));
		audioObject.OnComplete += delegate
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			TweenSettingsExtensions.OnComplete<Sequence>(DOGetUpSequence(), new TweenCallback(GetUpSequenceOnComplete));
		};
	}

	private void GetUpSequenceOnComplete()
	{
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		PlayIntroDialogue_03();
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetInteraction(active: true);
		GameManager.Instance.Player.SetCameraSway(active: true);
	}

	private void PlayIntroDialogue_02()
	{
		GameManager.Instance.AudioManager.Play(m_StandUpClip);
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClipGetUp, string.Empty, isTrimmed: true));
	}

	private void PlayIntroDialogue_03()
	{
		GameManager.Instance.ShowCrosshair();
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip02, "DIACH2/DIA_CH2_HENRY_02", isTrimmed: true));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_NEW_EXIT", "OBJECTIVES/CH2_OBJECTIVE_NEW_EXIT_TIP", 4f));
		};
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_GameCam = null;
		m_GameCamTransform = null;
		m_TitleMusicClip = null;
		m_RingingEarsClip = null;
		m_StandUpClip = null;
		m_HenryClip01 = null;
		m_HenryClip02 = null;
		m_HenryClipGetUp = null;
		base.OnDisposed();
	}
}
