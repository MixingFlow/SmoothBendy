using System;
using DG.Tweening;
using TMG.Controls;
using UnityEngine;

public class CH5OpeningSequenceController : BaseController
{
	[SerializeField]
	private CH5OpeningScenes m_OpeningScenes;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	[Header("Scene 01")]
	[SerializeField]
	private Transform m_S1StartLocation;

	[SerializeField]
	private Transform m_S1UpLocation;

	[SerializeField]
	private Transform m_S1DoorwayLocation;

	[SerializeField]
	private Transform m_S1BackupLocation;

	[Header("Scene 02")]
	[SerializeField]
	private Transform m_S2Location;

	[Header("Scene 03")]
	[SerializeField]
	private Transform m_S3Location;

	[Header("Scene 04")]
	[SerializeField]
	private Transform m_S4KneelLocation;

	[SerializeField]
	private Transform m_S4DoorwayLocation;

	[SerializeField]
	private GameObject[] m_S4Activate;

	[Header("Scene 05")]
	[SerializeField]
	private Transform m_S5StartLocation;

	[SerializeField]
	private Transform m_S5EndLocation;

	[Header("Scene 06")]
	[SerializeField]
	private Transform m_S6Location;

	[Header("Scene 07")]
	[SerializeField]
	private Transform m_S7StartLocation;

	[SerializeField]
	private Transform m_S7StandLocation;

	private Transform m_FreeRoamCam;

	private Sequence m_Sequence;

	private AudioClip m_IntroMusic;

	private AudioObject m_StoriesMusicObject;

	private AudioClip m_StoriesMusic;

	private AudioObject m_CirclesMusicObject;

	private AudioClip m_CirclesMusic;

	private AudioObject m_SceneSevenClipObject;

	private AudioClip m_SceneSevenClip;

	private Action m_Skip;

	private bool m_CanSkip;

	private int m_SkipCount;

	private bool m_CanMoveCamera;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_S4Activate.Length; i++)
		{
			if (Object.op_Implicit((Object)(object)m_S4Activate[i]))
			{
				m_S4Activate[i].SetActive(false);
			}
		}
		m_CanMoveCamera = false;
		m_IntroMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheLastReel");
		m_StoriesMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_Stories");
		m_CirclesMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_Circles");
		m_SceneSevenClip = GameManager.Instance.GetAudioClip("Audio/SFX/sfx_sceneseven_rumble");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.SafehouseObjective.IsStarted)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void ForceComplete()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.Player.transform.position = GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PlayerPosition.GetVector();
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PlayerRotation.GetVector()));
		GameManager.Instance.HideScreenBlocker(0.1f, 0.5f);
		m_OpeningScenes.ForceComplete();
		GameManager.Instance.Player.AllowSeeingTool(active: true);
		for (int i = 0; i < m_S4Activate.Length; i++)
		{
			if (Object.op_Implicit((Object)(object)m_S4Activate[i]))
			{
				m_S4Activate[i].SetActive(true);
			}
		}
		UnlockPlayer();
		SendOnComplete();
	}

	private void InternalActivate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.SetLock(active: true);
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_FreeRoamCam.position = m_S1StartLocation.position;
		m_FreeRoamCam.eulerAngles = m_S1StartLocation.eulerAngles;
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_IntroMusic, AudioObjectType.MUSIC);
		});
		num += 4f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH5_LABEL", "MENU/CH5_TITLE");
		});
		num += 10f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(2f);
			GameManager.Instance.UnlockPause();
			m_OpeningScenes.Scene01Event01 += HandleScene01Event01;
			m_OpeningScenes.Scene01OnComplete += OpeningScene01OnComplete;
			m_OpeningScenes.ActivateScene01();
			SetSkip(OpeningScene01ForceComplete);
		});
		num += 3.5f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S1UpLocation.position, 4f, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_S1UpLocation.eulerAngles, 4f, (RotateMode)0), (Ease)7));
		num += 5f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S1DoorwayLocation.position, 10f, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_S1DoorwayLocation.eulerAngles, 12f, (RotateMode)0), (Ease)7));
	}

	private void HandleScene01Event01(object sender, EventArgs e)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene01Event01 -= HandleScene01Event01;
		float num = 2f;
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S1BackupLocation.position, 3f, false), (Ease)6), num);
		num += 2.5f;
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(m_FreeRoamCam, m_S1StartLocation.position, 3f, (AxisConstraint)0, (Vector3?)null), (Ease)7), num);
		num += 1.5f;
		GameManager.Instance.ShowScreenBlocker(1f, num);
	}

	private void OpeningScene01ForceComplete()
	{
		KillCutsceneSequence(delegate
		{
			OpeningScene01OnComplete(null, null);
		});
	}

	private void OpeningScene01OnComplete(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene01OnComplete -= OpeningScene01OnComplete;
		m_FreeRoamCam.position = m_S2Location.position;
		m_FreeRoamCam.eulerAngles = m_S2Location.eulerAngles;
		GameManager.Instance.HideScreenBlocker(2f, 0.25f);
		m_OpeningScenes.Scene02Event01 += HandleScene02Event01;
		m_OpeningScenes.Scene02OnComplete += OpeningScene02OnComplete;
		m_OpeningScenes.ActivateScene02();
		SetSkip(OpeningScene02ForceComplete);
	}

	private void HandleScene02Event01(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene02Event01 -= HandleScene02Event01;
		GameManager.Instance.ShowScreenBlocker(1f, 3f);
	}

	private void OpeningScene02ForceComplete()
	{
		KillCutsceneSequence(delegate
		{
			OpeningScene02OnComplete(null, null);
		});
	}

	private void OpeningScene02OnComplete(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene02OnComplete -= OpeningScene02OnComplete;
		m_FreeRoamCam.position = m_S3Location.position;
		m_FreeRoamCam.eulerAngles = m_S3Location.eulerAngles;
		GameManager.Instance.HideScreenBlocker(2f, 0.25f);
		m_OpeningScenes.Scene03Event01 += HandleScene03Event01;
		m_OpeningScenes.Scene03OnComplete += OpeningScene03OnComplete;
		m_OpeningScenes.ActivateScene03();
		SetSkip(OpeningScene03ForceComplete);
	}

	private void HandleScene03Event01(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene03Event01 -= HandleScene03Event01;
		GameManager.Instance.ShowScreenBlocker(1f, 0.25f);
	}

	private void OpeningScene03ForceComplete()
	{
		KillCutsceneSequence(delegate
		{
			OpeningScene03OnComplete(null, null);
		});
	}

	private void OpeningScene03OnComplete(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene03OnComplete -= OpeningScene03OnComplete;
		m_FreeRoamCam.position = m_S4KneelLocation.position;
		m_FreeRoamCam.eulerAngles = m_S4KneelLocation.eulerAngles;
		GameManager.Instance.HideScreenBlocker(1f, 0.1f);
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S4DoorwayLocation.position, 2f, false), (Ease)7), 3.5f);
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_S4DoorwayLocation.eulerAngles, 3f, (RotateMode)0), (Ease)7), 3.75f);
		for (int i = 0; i < m_S4Activate.Length; i++)
		{
			if (Object.op_Implicit((Object)(object)m_S4Activate[i]))
			{
				m_S4Activate[i].SetActive(true);
			}
		}
		m_OpeningScenes.Scene04OnComplete += OpeningScene04OnComplete;
		m_OpeningScenes.ActivateScene04();
		SetSkip(OpeningScene04ForceComplete);
	}

	private void OpeningScene04ForceComplete()
	{
		KillCutsceneSequence(delegate
		{
			OpeningScene04OnComplete(null, null);
		});
	}

	private void OpeningScene04OnComplete(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		m_OpeningScenes.Scene04OnComplete -= OpeningScene04OnComplete;
		GameManager.Instance.ShowScreenBlocker(1f, 0.25f);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			m_FreeRoamCam.position = m_S5StartLocation.position;
			m_FreeRoamCam.eulerAngles = m_S5StartLocation.eulerAngles;
			GameManager.Instance.HideScreenBlocker(1f, 0.5f);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S5EndLocation.position, 5f, false), (Ease)7), (TweenCallback)delegate
			{
				m_FreeRoamCam.SetParent(m_S5EndLocation);
				m_CharacterLook.Init(m_S5EndLocation, m_FreeRoamCam);
				m_CharacterLook.HorizontalClampSetActive(active: true);
				m_CharacterLook.SetHorizontalClamp(25f);
				m_CanMoveCamera = true;
			});
			m_OpeningScenes.Scene05Event01 += HandleScene05Event01;
			m_OpeningScenes.Scene05Event02 += HandleScene05Event02;
			m_OpeningScenes.Scene05Event03 += HandleScene05Event03;
			m_OpeningScenes.Scene05Event04 += HandleScene05Event04;
			m_OpeningScenes.Scene05Event05 += HandleScene05Event05;
			m_OpeningScenes.Scene05OnComplete += OpeningScene05OnComplete;
			m_OpeningScenes.ActivateScene05();
			SetSkip(OpeningScene05ForceComplete);
		});
	}

	private void Update()
	{
		if (m_CanSkip && PlayerInput.InteractOnPressed())
		{
			m_SkipCount++;
			if (m_SkipCount >= 2)
			{
				m_SkipCount = 0;
				if (m_Skip != null)
				{
					m_Skip();
				}
			}
		}
		if (m_CanMoveCamera && !GameManager.Instance.isPaused)
		{
			m_CharacterLook.Rotation(m_S5EndLocation, m_FreeRoamCam);
			m_CharacterLook.GetInput();
		}
	}

	private void HandleScene05Event01(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene05Event01 -= HandleScene05Event01;
		m_StoriesMusicObject = GameManager.Instance.AudioManager.Play(m_StoriesMusic, AudioObjectType.MUSIC);
	}

	private void HandleScene05Event02(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene05Event02 -= HandleScene05Event02;
		GameManager.Instance.Player.transform.SetParent(m_S5EndLocation);
		GameManager.Instance.Player.LockRotation(25f, 20f);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.Player.UseSeeingTool(active: true);
		GameManager.Instance.Player.SetLock(active: false, ignoreSeeingTool: true);
		GameManager.Instance.Player.SetLockedMovement(active: true);
	}

	private void HandleScene05Event03(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene05Event03 -= HandleScene05Event03;
		m_CirclesMusicObject = GameManager.Instance.AudioManager.Play(m_CirclesMusic, AudioObjectType.MUSIC);
	}

	private void HandleScene05Event04(object sender, EventArgs e)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene05Event04 -= HandleScene05Event03;
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.SetLockedMovement(active: false);
		GameManager.Instance.Player.SetLock(active: true);
		GameManager.Instance.Player.UseSeeingTool(active: false);
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_FreeRoamCam.SetParent(m_S5EndLocation);
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		Vector3 position = GameManager.Instance.Player.transform.position;
		GameManager.Instance.Player.GoToAndLookAt(m_S5EndLocation);
		GameManager.Instance.Player.transform.position = position;
		GameManager.Instance.HideCrosshair();
	}

	private void HandleScene05Event05(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene05Event05 -= HandleScene05Event03;
		GameManager.Instance.ShowScreenBlocker(1f);
	}

	private void OpeningScene05ForceComplete()
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_StoriesMusicObject != (Object)null)
		{
			m_StoriesMusicObject.Clear();
			m_StoriesMusicObject = null;
		}
		if ((Object)(object)m_CirclesMusicObject != (Object)null)
		{
			m_CirclesMusicObject.Clear();
			m_CirclesMusicObject = null;
		}
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.SetLockedMovement(active: false);
		GameManager.Instance.Player.SetLock(active: true);
		GameManager.Instance.Player.UseSeeingTool(active: false);
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		m_FreeRoamCam.SetParent(m_S5EndLocation);
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		Vector3 position = GameManager.Instance.Player.transform.position;
		GameManager.Instance.Player.GoToAndLookAt(m_S5EndLocation);
		GameManager.Instance.Player.transform.position = position;
		GameManager.Instance.HideCrosshair();
		KillCutsceneSequence(delegate
		{
			OpeningScene05OnComplete(null, null);
		});
	}

	private void OpeningScene05OnComplete(object sender, EventArgs e)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		m_OpeningScenes.Scene05OnComplete -= OpeningScene05OnComplete;
		m_CanMoveCamera = false;
		GameManager.Instance.HideCrosshair();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			m_FreeRoamCam.SetParent((Transform)null);
			m_FreeRoamCam.position = m_S6Location.position;
			m_FreeRoamCam.eulerAngles = m_S6Location.eulerAngles;
			GameManager.Instance.HideScreenBlocker(1f, 0.1f);
			m_OpeningScenes.Scene06Event01 += HandleScene06Event01;
			m_OpeningScenes.Scene06OnComplete += OpeningScene06OnComplete;
			m_OpeningScenes.ActivateScene06();
			SetSkip(OpeningScene06ForceComplete);
		});
	}

	private void HandleScene06Event01(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene06Event01 -= HandleScene06Event01;
		GameManager.Instance.ShowScreenBlocker(1f, 3.25f);
	}

	private void OpeningScene06ForceComplete()
	{
		KillCutsceneSequence(delegate
		{
			OpeningScene06OnComplete(null, null);
		});
	}

	private void OpeningScene06OnComplete(object sender, EventArgs e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		m_OpeningScenes.Scene06OnComplete -= OpeningScene06OnComplete;
		m_FreeRoamCam.position = m_S7StartLocation.position;
		m_FreeRoamCam.eulerAngles = m_S7StartLocation.eulerAngles;
		Sequence val = DOTween.Sequence();
		float num = 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(0.5f, 0.1f);
			m_SceneSevenClipObject = GameManager.Instance.AudioManager.Play(m_SceneSevenClip);
			m_OpeningScenes.Scene07OnComplete += OpeningScene07OnComplete;
			m_OpeningScenes.ActivateScene07();
			SetSkip(OpeningScene07ForceComplete);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, m_S7StandLocation.position, 1f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_S7StandLocation.eulerAngles, 1.1f, (RotateMode)0), (Ease)7));
		num += 1.2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, GameManager.Instance.Player.HeadContainer.position, 1.5f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_S5EndLocation.parent.eulerAngles, 1.6f, (RotateMode)0), (Ease)7));
	}

	private void OpeningScene07ForceComplete()
	{
		if ((Object)(object)m_SceneSevenClipObject != (Object)null)
		{
			m_SceneSevenClipObject.Clear();
			m_SceneSevenClipObject = null;
		}
		KillCutsceneSequence(delegate
		{
			m_OpeningScenes.SkipFinal();
			OpeningScene07OnComplete(null, null);
		});
	}

	private void OpeningScene07OnComplete(object sender, EventArgs e)
	{
		m_OpeningScenes.Scene07OnComplete -= OpeningScene07OnComplete;
		m_CanSkip = false;
		GameManager.Instance.Player.AllowSeeingTool(active: true);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		UnlockPlayer();
		GameManager.Instance.HideScreenBlocker();
		SendOnComplete();
	}

	private void SetSkip(Action skip)
	{
		m_CanSkip = true;
		m_Skip = skip;
	}

	private void KillCutsceneSequence(Action onComplete)
	{
		KillSequence();
		m_CanSkip = false;
		GameManager.Instance.KillScreenBlockerFadeTween();
		GameManager.Instance.ShowScreenBlocker(0.5f, 0f, delegate
		{
			ShortcutExtensions.DOComplete((Component)(object)m_FreeRoamCam, false);
			ShortcutExtensions.DOKill((Component)(object)m_FreeRoamCam, false);
			GameManager.Instance.KillSubtitles();
			GameManager.Instance.AudioManager.ClearCurrentDialogueQueue();
			if (onComplete != null)
			{
				onComplete();
			}
		});
	}

	private Sequence DOSequence()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(0.1f);
		});
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowChapterTitle("MENU/CH5_LABEL", "MENU/CH5_TITLE");
		});
		num += 4f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(UnlockPlayer));
		return m_Sequence;
	}

	private void UnlockPlayer()
	{
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.UnlockPause();
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.ShowCrosshair();
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
		m_Skip = null;
		m_IntroMusic = null;
		m_StoriesMusic = null;
		m_CirclesMusic = null;
		m_SceneSevenClip = null;
		if ((Object)(object)m_StoriesMusicObject != (Object)null)
		{
			m_StoriesMusicObject.Clear();
			m_StoriesMusicObject = null;
		}
		if ((Object)(object)m_CirclesMusicObject != (Object)null)
		{
			m_CirclesMusicObject.Clear();
			m_CirclesMusicObject = null;
		}
		if ((Object)(object)m_SceneSevenClipObject != (Object)null)
		{
			m_SceneSevenClipObject.Clear();
			m_SceneSevenClipObject = null;
		}
		base.OnDisposed();
	}
}
