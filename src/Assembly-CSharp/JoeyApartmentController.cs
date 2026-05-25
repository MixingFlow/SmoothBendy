using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class JoeyApartmentController : BaseController
{
	[SerializeField]
	private TextMeshPro m_CalendarDate;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private EventTrigger m_AnimationTrigger;

	[SerializeField]
	private Animation m_JoeyAnimation;

	[SerializeField]
	private PlayerController m_PlayerPrefab;

	[SerializeField]
	private Transform m_CameraLook;

	[SerializeField]
	private Transform m_FinalDoor;

	[SerializeField]
	private Transform m_FinalLocationStart;

	[SerializeField]
	private Transform m_FinalLocationEnter;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	[SerializeField]
	private GameObject m_QuadTexture;

	[SerializeField]
	private VideoPlayer m_VideoPlayer;

	[SerializeField]
	private Interactable m_DoorInteractable;

	[SerializeField]
	private List<AudioClip> m_LockAudioClips;

	[SerializeField]
	private GameObject m_InkAudio;

	[Header("Post Credits")]
	[SerializeField]
	private Transform m_PostCreditStartLocation;

	[SerializeField]
	private Transform m_PostCreditEndLocation;

	private PlayerController m_Player;

	private Transform m_FreeRoamCam;

	private AudioClip[] m_JoeyClips;

	private AudioClip m_DoorClip;

	private AudioClip m_HenryClip;

	private AudioClip m_UncleJoeyClip;

	private AudioObject m_DoorAudioObject;

	private bool m_IsAtTalkLocation;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		GameManager.Instance.AudioManager.ListenerSetActive(active: false);
		m_JoeyClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/Joey");
		m_UncleJoeyClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/ad2");
		m_DoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Unlock_Open_Close_01");
		m_HenryClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_01");
		m_InkAudio.SetActive(false);
		m_Player = Object.Instantiate<PlayerController>(m_PlayerPrefab);
		m_Player.SetCameraSway(active: true);
		m_Player.GoToAndLookAt(m_StartLocation);
		m_Player.SetLock(active: true);
		m_QuadTexture.SetActive(false);
		if ((Object)(object)m_VideoPlayer != (Object)null)
		{
			m_VideoPlayer.Prepare();
		}
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.ShowWhiteScreenBlocker(0f);
		m_CalendarDate.text = Random.Range(1, 30).ToString();
		((MonoBehaviour)this).StartCoroutine(InitializeScene());
	}

	private IEnumerator InitializeScene()
	{
		yield return (object)new WaitForSeconds(1f);
		DOTweenUtil.DOAudioListenerVolume(1f, 1f);
		Activate();
	}

	public override void Activate()
	{
		m_Player.SetLock(active: false);
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.GameCamera.Dust = false;
		GameManager.Instance.HideWhiteScreenBlocker(3f);
		m_AnimationTrigger.OnEnter += HandleAnimationTriggerOnEnter;
		m_AnimationTrigger.SetActive(active: true);
		m_DoorInteractable.OnInteracted += HandleDoorInteractableOnInteracted;
		m_DoorInteractable.SetActive(active: true);
	}

	private void ActivatePostCredits()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		GameManager.Instance.HideCrosshair();
		GameManager.Instance.HideWhiteScreenBlocker(0f);
		GameManager.Instance.HideScreenBlocker(0f);
		m_FinalDoor.localEulerAngles = Vector3.zero;
		m_QuadTexture.SetActive(false);
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		val.position = m_PostCreditStartLocation.position;
		val.eulerAngles = m_PostCreditStartLocation.eulerAngles;
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_PostCreditEndLocation.position, 18f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_PostCreditEndLocation.eulerAngles, 16f, (RotateMode)0), (Ease)6));
		num += 18.5f;
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_UncleJoeyClip).OnComplete += HandleUncleJoeyOnComplete;
		});
		m_InkAudio.SetActive(true);
		S13AudioManager.Instance.InvokeEvent("evt_ch5_joey_ketchen_resume");
	}

	private void HandleDoorInteractableOnInteracted(object sender, EventArgs e)
	{
		if (Object.op_Implicit((Object)(object)m_DoorAudioObject))
		{
			return;
		}
		m_DoorAudioObject = PlayLockedAudio();
		m_DoorAudioObject.OnComplete += delegate
		{
			if (Object.op_Implicit((Object)(object)m_DoorAudioObject))
			{
				m_DoorAudioObject.Clear();
				m_DoorAudioObject = null;
			}
		};
	}

	private void HandleUncleJoeyOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowScreenBlocker(0f);
		GameManager.Instance.ShowCredits();
	}

	private void Update()
	{
		if (m_IsAtTalkLocation && !GameManager.Instance.isPaused)
		{
			m_CharacterLook.Rotation(m_CameraLook, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.GetInput();
		}
	}

	private void HandleAnimationTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		m_AnimationTrigger.OnEnter -= HandleAnimationTriggerOnEnter;
		GameManager.Instance.HideCrosshair();
		m_Player.SetCameraSway(active: true);
		m_Player.SetLockedMovement(active: true);
		m_Player.SetCollision(active: false);
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_CameraLook.position, 8f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_CameraLook.eulerAngles, 9f, (RotateMode)0), (Ease)6));
		num += 9f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_FreeRoamCam.SetParent(m_CameraLook);
			m_CharacterLook.Init(m_CameraLook, m_FreeRoamCam);
			m_CharacterLook.HorizontalClampSetActive(active: true);
			m_CharacterLook.SetHorizontalClamp(25f);
			m_IsAtTalkLocation = true;
		});
		for (int num2 = 0; num2 < m_JoeyClips.Length; num2++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_JoeyClips[num2], SubtitleConstants.DIA_CH5_JOEY[num2], isTrimmed: true));
			if (num2 == m_JoeyClips.Length - 2)
			{
				audioObject.OnComplete += HandleJoeyClipsOnComplete;
			}
		}
		m_JoeyAnimation.Play();
		S13AudioManager.Instance.InvokeEvent("evt_ch5_joey_speaks");
	}

	private void HandleJoeyClipsOnComplete(object sender, EventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		if (sender != null)
		{
			(sender as AudioObject).OnComplete -= HandleJoeyClipsOnComplete;
		}
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 0f, new TweenCallback(ExitLook));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_FinalLocationStart.position, 5f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_FinalLocationStart.eulerAngles, 3f, (RotateMode)0), (Ease)7));
		if ((Object)(object)m_VideoPlayer != (Object)null)
		{
			TweenSettingsExtensions.InsertCallback(val, 4f, (TweenCallback)delegate
			{
				m_QuadTexture.SetActive(true);
			});
			TweenSettingsExtensions.InsertCallback(val, 4.6f, (TweenCallback)delegate
			{
				m_VideoPlayer.Stop();
			});
			TweenSettingsExtensions.InsertCallback(val, 4.7f, (TweenCallback)delegate
			{
				m_VideoPlayer.Play();
			});
			TweenSettingsExtensions.InsertCallback(val, 4.8f, (TweenCallback)delegate
			{
				m_VideoPlayer.Pause();
			});
			TweenSettingsExtensions.InsertCallback(val, 4.9f, (TweenCallback)delegate
			{
				m_VideoPlayer.Play();
			});
		}
		TweenSettingsExtensions.InsertCallback(val, 5f, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_DoorClip);
		});
		TweenSettingsExtensions.Insert(val, 5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_FinalDoor, new Vector3(0f, 100f, 0f), 2f, (RotateMode)3), (Ease)7));
		TweenSettingsExtensions.Insert(val, 5.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_FinalLocationEnter.position, 2.75f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, 5.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_FinalLocationEnter.eulerAngles, 2.75f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.InsertCallback(val, 9f, (TweenCallback)delegate
		{
			if (!GameManager.Instance.GameData.CurrentSaveFile.HasDied && GameManager.Instance.GameData.CurrentSaveFile.Internecions[0] == 0 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[1] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[2] == 1 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[3] == 4 && GameManager.Instance.GameData.CurrentSaveFile.Internecions[4] == 0 && GameManager.Instance.GameData.CurrentSaveFile.CH5Data.InternecionValue == 414)
			{
				string text = "onef";
				string text2 = "our";
				string internecion = "f" + text2 + text + text2;
				GameManager.Instance.GameData.CurrentSaveFile.Internecion = internecion;
			}
			bool flag = GameManager.Instance.GameData.CurrentSaveFile.CH2Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue == 414;
			bool flag2 = GameManager.Instance.GameData.CurrentSaveFile.CH3Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue == 414;
			bool flag3 = GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[0] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[0] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[1] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[1] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[2] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[2] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[3] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[3] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414 && GameManager.Instance.GameData.CurrentSaveFile.CH4Data != null && GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[5] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[5] * 414;
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data = new CH1DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data = new CH2DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CH2Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data = new CH3DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data = new CH4DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH5Data = new CH5DataVO();
			GameManager.Instance.GameData.CurrentSaveFile.CH5Data.IsChapterComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CurrentChapter = 1;
			GameManager.Instance.GameData.CurrentSaveFile.IsNewGamePlus = true;
			GameManager.Instance.GameData.CurrentSaveFile.CompleteCount++;
			if (flag)
			{
				GameManager.Instance.GameData.CurrentSaveFile.CH2Data.InternecionValue = 414;
			}
			if (flag2)
			{
				GameManager.Instance.GameData.CurrentSaveFile.CH3Data.InternecionValue = 414;
			}
			if (flag3)
			{
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[0] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[0] * 414;
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[1] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[1] * 414;
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[2] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[2] * 414;
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[3] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[3] * 414;
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414;
				GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[5] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[5] * 414;
			}
			GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.TO_HELL_AND_BACK);
			GameManager.Instance.AudioManager.Play(m_HenryClip, AudioObjectType.DIALOGUE).OnComplete += HandleHenryClipOnComplete;
		});
		S13AudioManager.Instance.InvokeEvent("evt_ch5_joey_kitchen_exit");
	}

	private void ExitLook()
	{
		m_IsAtTalkLocation = false;
	}

	private AudioObject PlayLockedAudio()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (m_LockAudioClips.Count <= 0)
		{
			return null;
		}
		int index = Random.Range(0, m_LockAudioClips.Count);
		AudioClip val = m_LockAudioClips[index];
		AudioObject result = GameManager.Instance.AudioManager.PlayAtPosition(val, m_DoorInteractable.transform.position);
		m_LockAudioClips[index] = m_LockAudioClips[0];
		m_LockAudioClips[0] = val;
		return result;
	}

	private void HandleHenryClipOnComplete(object sender, EventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, new TweenCallback(ShowCredits));
	}

	public void ShowCredits()
	{
		CH5TheEndCredits cH5TheEndCredits = Object.Instantiate<CH5TheEndCredits>(GameManager.Instance.AssetManager.GetAsset<CH5TheEndCredits>("GamePlay/CH5/TheEnd"));
		cH5TheEndCredits.OnComplete += ShowCreditsOnComplete;
	}

	private void ShowCreditsOnComplete(object sender, EventArgs e)
	{
		ActivatePostCredits();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_AnimationTrigger))
		{
			m_AnimationTrigger.OnEnter -= HandleAnimationTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
