using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CH2SacrificeController : BaseController
{
	private const float LOCK_VERTICAL = 25f;

	private const float LOCK_HORIZONTAL = 50f;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_Sammy;

	[SerializeField]
	private Transform m_SammyEndPosition;

	[SerializeField]
	private Transform m_TiedUpPosition;

	[SerializeField]
	private GenericDoorController m_GateDoor;

	[SerializeField]
	private List<Transform> m_Speakers;

	[SerializeField]
	private Transform m_SammyBlood;

	[Header("Animator")]
	[SerializeField]
	private Animator m_SammyAnimator;

	[Header("Door")]
	[SerializeField]
	private BaseDoorController m_SammysDoor;

	[Header("Light Fixture")]
	[SerializeField]
	private LightFixtureController m_LightController;

	[Header("Event Trigger")]
	[SerializeField]
	private EventTrigger m_MusicEventTrigger;

	[SerializeField]
	private InkMachineLoopController m_InkMachineLoop;

	[Header("Spawners")]
	[SerializeField]
	private PlayerSpawnNode m_InitialSpawner;

	[SerializeField]
	private PlayerSpawnNode m_SacrificeSpawner;

	[Header("Animation Event Objects")]
	[SerializeField]
	private GameObject m_PickupAxe;

	[SerializeField]
	private GameObject m_SammyAxe;

	private AudioClip[] m_SammyFootstepClips;

	private AudioClip[] m_MonologueClips;

	private AudioClip[] m_SpeakerMonologueClips;

	private AudioClip m_MusicLittleDevilClip;

	private AudioClip m_SammyMonologueBGClip;

	private AudioClip m_DuctCrawlingClip;

	private AudioClip m_RopeStressClip;

	private AudioClip m_RopeStressSnapClip;

	private AudioClip m_SpeakerTapClip;

	private AudioClip m_DoorCloseClip;

	private AudioObject m_MusicAudioObject;

	public override void InitOnComplete()
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_SammyFootstepClips = GameManager.Instance.GetAudioClips("Audio/SFX/Footsteps/Sammy/Wood");
		m_MonologueClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/FinaleMonologue/");
		m_SpeakerMonologueClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH2/Sammy/FinaleMonologueSpeaker/");
		m_MusicLittleDevilClip = GameManager.Instance.GetAudioClip("Audio/MUS/MUS_Little_Devil_Darling_Remastered");
		m_SammyMonologueBGClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Sammy/DIA_Sammy_Finale_BG_Audio");
		m_DuctCrawlingClip = GameManager.Instance.GetAudioClip("Audio/SFX/FOL_Duct_Crawling_01");
		m_RopeStressClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rope_Stress_01");
		m_RopeStressSnapClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rope_Stress_Snap_02");
		m_SpeakerTapClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Speaker_Tap_Feedback");
		m_DoorCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Generic_Close_01");
		m_LightController.TurnOff();
		m_SammysDoor.ForceOpen(85f);
		m_SacrificeSpawner.gameObject.SetActive(false);
		m_SammyBlood.localScale = new Vector3(m_SammyBlood.localScale.x, -0.1f, m_SammyBlood.localScale.z);
		m_InkMachineLoop.gameObject.SetActive(false);
		m_GateDoor.Close();
	}

	public override void Activate()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Expected O, but got Unknown
		GameManager.Instance.Player.SetLock(active: false);
		GameManager.Instance.Player.SetLockedMovement(active: true);
		GameManager.Instance.Player.transform.SetParent(m_TiedUpPosition);
		GameManager.Instance.Player.LookRotation(Quaternion.identity, Quaternion.identity);
		GameManager.Instance.Player.LockRotation(50f, 25f);
		GameManager.Instance.Player.transform.localPosition = Vector3.zero;
		GameManager.Instance.Player.transform.localEulerAngles = Vector3.zero;
		((Component)GameManager.Instance.GameCamera.Camera).transform.localPosition = Vector3.zero;
		((Component)GameManager.Instance.GameCamera.Camera).transform.localEulerAngles = Vector3.zero;
		m_SammyAnimator.SetTrigger("Monologue");
		Sequence val = DOTween.Sequence();
		GameCamera gameCam = GameManager.Instance.GameCamera;
		TweenSettingsExtensions.InsertCallback(val, 0.75f, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(2f);
		});
		TweenSettingsExtensions.InsertCallback(val, 3.2f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(1f);
		});
		TweenSettingsExtensions.InsertCallback(val, 4.3f, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(1f);
		});
		TweenSettingsExtensions.InsertCallback(val, 24.14f, (TweenCallback)delegate
		{
			HandleAxeSwap();
		});
		m_InitialSpawner.gameObject.SetActive(false);
		m_SacrificeSpawner.gameObject.SetActive(true);
		if (gameCam.DoF)
		{
			float distance = gameCam.UnityDOF.focalDistance;
			TweenSettingsExtensions.Insert(val, 2.5f, (Tween)(object)TweenSettingsExtensions.OnUpdate<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To((DOGetter<float>)(() => distance), (DOSetter<float>)delegate(float value)
			{
				distance = value;
			}, 5f, 2f), (Ease)1), (TweenCallback)delegate
			{
				gameCam.UnityDOF.focalDistance = distance;
			}));
			TweenSettingsExtensions.InsertCallback(val, 4.5f, (TweenCallback)delegate
			{
				gameCam.UnityDOF.manualDOF = false;
			});
		}
		int num = m_MonologueClips.Length;
		int num2 = 7;
		for (int num3 = 0; num3 < num; num3++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MonologueClips[num3], SubtitleConstants.DIALOGUE_CHAPTER_TWO_SAMMY_FINALE_MONOLOGUE[num3], isTrimmed: true));
			if (num3 == num - 1)
			{
				audioObject.OnComplete += HandleMonologueOnComplete;
			}
			else if (num3 == num2)
			{
				audioObject.OnComplete += HandleDuctCrawlingAudio;
			}
		}
		GameManager.Instance.Player.OnDeath += HandlePlayerOnDeath;
	}

	private void HandleAxeSwap()
	{
		m_SammyAxe.SetActive(false);
		m_PickupAxe.SetActive(true);
	}

	private void HandleDuctCrawlingAudio(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleDuctCrawlingAudio;
		GameManager.Instance.AudioManager.Play(m_DuctCrawlingClip);
	}

	private void HandleMonologueOnComplete(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		(sender as AudioObject).OnComplete -= HandleMonologueOnComplete;
		TweenSettingsExtensions.OnComplete<Sequence>(DOWalkSequence(), new TweenCallback(HandleSammyExitOnComplete));
	}

	private Sequence DOWalkSequence()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		GameManager.Instance.AudioManager.Play(m_RopeStressClip);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 2f, 0.15f, 10, 90f, false), (TweenCallback)delegate
		{
			TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 2f, 0.15f, 10, 90f, false), 1f);
		});
		m_SammyAnimator.SetBool("Walk", true);
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)ShortcutExtensions.DOLookAt(m_Sammy, m_SammyEndPosition.position, 0.45f, (AxisConstraint)0, (Vector3?)null));
		for (int num = 0; num < 9; num++)
		{
			TweenSettingsExtensions.InsertCallback(val, (float)num * 0.72f, new TweenCallback(PlayFootStepAudio));
		}
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(m_Sammy, m_SammyEndPosition.position.x, 6.5f, false), (Ease)1));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveZ(m_Sammy, m_SammyEndPosition.position.z, 6.5f, false), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, 5.5f, new TweenCallback(m_SammysDoor.Close));
		TweenSettingsExtensions.InsertCallback(val, 5.5f, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_DoorCloseClip);
		});
		TweenSettingsExtensions.InsertCallback(val, 6f, new TweenCallback(m_SammysDoor.Lock));
		return val;
	}

	private void HandleSammyExitOnComplete()
	{
		GameManager.Instance.AudioManager.Play(m_SpeakerTapClip).OnComplete += HandleSpeakersOnComplete;
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOScaleZ(m_Speakers[i], Random.Range(1.03f, 1.05f), 0.25f), Random.Range(0.05f, 0.15f)), (Ease)7), -1, (LoopType)1);
		}
	}

	private void HandleSpeakersOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.AudioManager.Play(m_SammyMonologueBGClip);
		int num = m_SpeakerMonologueClips.Length;
		int num2 = 1;
		int num3 = num - 3;
		for (int i = 0; i < num; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_SpeakerMonologueClips[i], SubtitleConstants.DIALOGUE_CHAPTER_TWO_SAMMY_FINALE_SPEAKER_MONOLOGUE[i], isTrimmed: true));
			if (i == num2)
			{
				audioObject.OnComplete += HandleGateDoorOpen;
			}
			else if (i == num3)
			{
				audioObject.OnComplete += HandleMusicTrigger;
			}
			if (i == num - 1)
			{
				audioObject.OnComplete += HandleSpeakerMonologueOnComplete;
			}
		}
	}

	private void HandleGateDoorOpen(object sender, EventArgs e)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		(sender as AudioObject).OnComplete -= HandleGateDoorOpen;
		GameManager.Instance.AudioManager.Play(m_RopeStressClip);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 2f, 0.15f, 10, 90f, false), (TweenCallback)delegate
		{
			TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 2f, 0.15f, 10, 90f, false), 1f);
		});
		m_LightController.TurnOn();
		m_GateDoor.Open();
	}

	private void HandleMusicTrigger(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMusicTrigger;
		m_MusicAudioObject = GameManager.Instance.AudioManager.Play(m_MusicLittleDevilClip, AudioObjectType.MUSIC, -1);
		m_MusicAudioObject.AudioSource.volume = 0.1f;
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_MusicAudioObject.AudioSource.DOFade(1f, 4f), (Ease)1), 0.2f);
	}

	private void HandleSpeakerMonologueOnComplete(object sender, EventArgs e)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		(sender as AudioObject).OnComplete -= HandleSpeakerMonologueOnComplete;
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Speakers[i], false);
		}
		TweenSettingsExtensions.Insert(DOTween.Sequence(), 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_SammyBlood, 20f, 8f), (Ease)7));
		GameManager.Instance.AudioManager.Play(m_RopeStressSnapClip);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.Camera, 3f, 0.25f, 10, 90f, false), (TweenCallback)delegate
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_ESCAPE_BENDY", "OBJECTIVES/CH2_OBJECTIVE_ESCAPE_BENDY_TIP", 4f));
			GameManager.Instance.ShowCrosshair();
			GameManager.Instance.Player.SetLockedMovement(active: false);
			GameManager.Instance.Player.UnlockRotation();
			Vector3 val = GameManager.Instance.Player.transform.forward * 2f;
			GameManager.Instance.Player.transform.SetParent((Transform)null);
			GameManager.Instance.Player.LookRotation(Quaternion.LookRotation(val));
		});
		m_MusicEventTrigger.OnEnter += HandleMusicEventTriggerOnEnter;
	}

	private void HandleMusicEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		m_MusicEventTrigger.OnEnter -= HandleMusicEventTriggerOnEnter;
		m_InkMachineLoop.gameObject.SetActive(true);
		m_InkMachineLoop.Activate();
		GameManager.Instance.AudioManager.Play(m_DuctCrawlingClip);
		if ((Object)(object)m_MusicAudioObject != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicAudioObject.AudioSource.DOFade(0f, 8f), (TweenCallback)delegate
			{
				if ((Object)(object)m_MusicAudioObject != (Object)null)
				{
					m_MusicAudioObject.Clear();
					m_MusicAudioObject = null;
				}
			});
		}
		SendOnComplete();
	}

	private void PlayFootStepAudio()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (m_SammyFootstepClips != null && m_SammyFootstepClips.Length > 0)
		{
			int num = Random.Range(0, m_SammyFootstepClips.Length);
			AudioClip val = m_SammyFootstepClips[num];
			GameManager.Instance.AudioManager.PlayAtPosition(val, m_Sammy.position);
			m_SammyFootstepClips[num] = m_SammyFootstepClips[0];
			m_SammyFootstepClips[0] = val;
		}
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if (!((Object)(object)m_MusicAudioObject != (Object)null))
		{
			return;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_MusicAudioObject.AudioSource.DOFade(0f, 3f), (TweenCallback)delegate
		{
			if ((Object)(object)m_MusicAudioObject != (Object)null)
			{
				m_MusicAudioObject.Clear();
				m_MusicAudioObject = null;
			}
		});
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			GameManager.Instance.Player.OnDeath -= HandlePlayerOnDeath;
		}
		for (int i = 0; i < m_Speakers.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_Speakers[i], false);
		}
		m_SammyFootstepClips = null;
		m_MonologueClips = null;
		m_SpeakerMonologueClips = null;
		m_MusicLittleDevilClip = null;
		m_SammyMonologueBGClip = null;
		m_DuctCrawlingClip = null;
		m_RopeStressClip = null;
		m_RopeStressSnapClip = null;
		m_SpeakerTapClip = null;
		m_DoorCloseClip = null;
		base.OnDisposed();
	}
}
