using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH3ClosingSequenceController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[SerializeField]
	private CH3ServiceController m_ServiceController;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_FinalTrigger;

	[Header("Alice")]
	[SerializeField]
	private BaseAiController m_Alice;

	[SerializeField]
	private GameObject m_AliceHum;

	[SerializeField]
	private Animator m_BorisAnimationController;

	[Header("Lift")]
	[SerializeField]
	private Transform m_BorisEndPosition;

	[SerializeField]
	private Transform m_Lift;

	[SerializeField]
	private Transform m_LiftPosition;

	[SerializeField]
	private Transform m_EndPosition;

	[SerializeField]
	private Transform m_EndCameraPosition;

	[Header("Shafts")]
	[SerializeField]
	private List<DisposableObject> m_Shafts;

	[Header("Lighting")]
	[SerializeField]
	private GameObject m_Sparks;

	[SerializeField]
	private List<LightFlicker> m_LightFlicker;

	[SerializeField]
	private List<Light> m_Lights;

	[Header("Disable")]
	[SerializeField]
	private List<GameObject> m_ToDisable;

	private AudioObject m_LoopObject;

	private AudioClip[] m_CompleteTaskClips;

	private AudioClip[] m_MonologueAClips;

	private AudioClip[] m_MonologueBClips;

	private AudioClip[] m_MonologueCClips;

	private AudioObject m_Ambience;

	private AudioClip m_HorrorAmbienceClip;

	private AudioClip m_FinalClip_01;

	private AudioClip m_FinalClip_02;

	private AudioClip m_FinalClip_03;

	private AudioClip m_LiftDepartClip;

	private AudioClip m_LiftLoopClip;

	private AudioClip m_LiftFallLoopClip;

	private AudioClip m_SafehouseDoorClip;

	private Sequence m_DropSequence;

	private float Speed = 0.15f;

	private bool m_IsLanded;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_CompleteTaskClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/DIA/CH3/Alice/TasksComplete/");
		m_MonologueAClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueFinaleA/");
		m_MonologueBClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueFinaleB/");
		m_MonologueCClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueFinaleC/");
		m_HorrorAmbienceClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Horror_Ambience_Loop_01");
		m_FinalClip_01 = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_finaleending");
		m_FinalClip_02 = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_finaleending2");
		m_FinalClip_03 = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_finaleending3");
		m_LiftDepartClip = GameManager.Instance.GetAudioClip("Audio/SFX/Lift/SFX_Lift_Depart");
		m_LiftLoopClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_Lift_End_Loop");
		m_LiftFallLoopClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_elevatorfallingloop");
		m_SafehouseDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_safehousedoor");
		m_Sparks.SetActive(false);
		m_FinalTrigger.SetActive(active: false);
		m_AliceHum.SetActive(false);
	}

	public override void Activate()
	{
		m_ServiceController.Activate();
		m_LiftController.ActivateFinale();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceTasksObjective.IsStarted)
		{
			m_LiftController.DisableLift();
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_COMPLETE_B", string.Empty));
			m_FinalTrigger.SetActive(active: true);
			m_FinalTrigger.OnEnter += HandleFinalTriggerOnEnter;
			return;
		}
		for (int i = 0; i < m_CompleteTaskClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_CompleteTaskClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_TASKS_COMPLETE[i], isTrimmed: true));
			if (i >= m_CompleteTaskClips.Length - 1)
			{
				audioObject.OnComplete += HandleFinalTaskOnComplete;
			}
		}
	}

	private void HandleFinalTaskOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleFinalTaskOnComplete;
		m_LiftController.DisableLift();
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceTasksObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_COMPLETE_B", string.Empty, 4f));
		m_FinalTrigger.SetActive(active: true);
		m_FinalTrigger.OnEnter += HandleFinalTriggerOnEnter;
	}

	private void HandleFinalTriggerOnEnter(object sender, EventArgs e)
	{
		m_FinalTrigger.OnEnter -= HandleFinalTriggerOnEnter;
		m_LiftController.OnAliceDoorClosed += HandleAliceDoorClosed;
		m_LiftController.FinaleCloseAliceFloor();
	}

	private void HandleAliceDoorClosed(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_LiftController.OnAliceDoorClosed -= HandleAliceDoorClosed;
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftAscend(), new TweenCallback(AscendOnComplete));
	}

	private Sequence DOLiftAscend()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0f;
		GameManager.Instance.Player.transform.SetParent(m_Lift);
		for (int i = 0; i < m_MonologueAClips.Length; i++)
		{
			AudioClip val2 = m_MonologueAClips[i];
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(val2, SubtitleConstants.DIALOGUE_CH3_ALICE_FINALE_MONOLOGUE_A[i], isTrimmed: true));
			num2 += val2.length;
		}
		GameManager.Instance.AudioManager.Play(m_LiftDepartClip).OnComplete += delegate
		{
			m_LoopObject = GameManager.Instance.AudioManager.Play(m_LiftLoopClip, AudioObjectType.SOUND_EFFECT, -1);
		};
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(m_Lift, m_EndPosition.position.y, num2, false), (Ease)1));
		return val;
	}

	private void AscendOnComplete()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		Vector3 position = m_Lift.position;
		position.y = m_EndPosition.position.y;
		position.z = m_EndPosition.position.z;
		m_Lift.position = position;
		GameManager.Instance.AiGlobalNetwork.ClearAllAi();
		if ((Object)(object)m_LoopObject != (Object)null)
		{
			m_LoopObject.Clear();
			m_LoopObject = null;
		}
		GameManager.Instance.AudioManager.Play(m_SafehouseDoorClip);
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftDrop(), new TweenCallback(LiftDropOnComplete));
	}

	private Sequence DOLiftDrop()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_Sparks.SetActive(true);
			m_Boris.SetCower(active: true);
			m_LoopObject = GameManager.Instance.AudioManager.Play(m_LiftFallLoopClip, AudioObjectType.SOUND_EFFECT, -1);
			DoShake();
			Go(1f, (Ease)26);
		});
		for (int num2 = 0; num2 < m_MonologueBClips.Length; num2++)
		{
			AudioClip val2 = m_MonologueBClips[num2];
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(val2, SubtitleConstants.DIALOGUE_CH3_ALICE_FINALE_MONOLOGUE_B[num2], isTrimmed: true));
			num += val2.length;
		}
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_FinalClip_01);
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MonologueCClips[0], SubtitleConstants.DIALOGUE_CH3_ALICE_FINALE_MONOLOGUE_C[0], isTrimmed: true));
		});
		num += m_MonologueCClips[0].length;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0f);
		});
		return val;
	}

	private void LiftDropOnComplete()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_LoopObject != (Object)null)
		{
			m_LoopObject.Clear();
			m_LoopObject = null;
		}
		m_IsLanded = true;
		KillDropSequence();
		KillShake();
		m_Boris.gameObject.SetActive(false);
		m_Alice.OnWaypointComplete += HandleAliceOnWaypointComplete;
		m_Lift.position = m_LiftPosition.position;
		m_Sparks.SetActive(false);
		GameManager.Instance.Player.SetCameraSway(active: true);
		GameManager.Instance.Player.SetLock(active: true);
		GameManager.Instance.LockPause();
		GameManager.Instance.HideCrosshair();
		DOAliceSeuence();
	}

	private Sequence DOAliceSeuence()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_FinalClip_02);
		});
		num += 4f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_Ambience = GameManager.Instance.AudioManager.Play(m_HorrorAmbienceClip, AudioObjectType.SOUND_EFFECT, -1);
		});
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			m_Alice.gameObject.SetActive(true);
			m_AliceHum.SetActive(true);
			GameManager.Instance.Player.HeadContainer.SetParent(m_EndCameraPosition);
			GameManager.Instance.Player.HeadContainer.localPosition = Vector3.zero;
			GameManager.Instance.Player.HeadContainer.localEulerAngles = Vector3.zero;
			GameManager.Instance.HideScreenBlocker(4f);
			for (int i = 0; i < m_Lights.Count; i++)
			{
				TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOIntensity(m_Lights[i], 0f, 5f), 2f), (Ease)1);
			}
		});
		return val;
	}

	private void HandleAliceOnWaypointComplete(object sender, EventArgs e)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		m_Alice.OnWaypointComplete -= HandleAliceOnWaypointComplete;
		GameManager.Instance.ShowScreenBlocker(0f);
		m_Alice.gameObject.SetActive(false);
		GameManager.Instance.AudioManager.Play(m_FinalClip_02);
		for (int i = 0; i < m_LightFlicker.Count; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_LightFlicker[i].Light, false);
			m_LightFlicker[i].Light.intensity = 1.5f;
			((Behaviour)m_LightFlicker[i]).enabled = true;
		}
		for (int j = 0; j < m_ToDisable.Count; j++)
		{
			m_ToDisable[j].SetActive(false);
		}
		RenderSettings.ambientIntensity = 0f;
		TweenSettingsExtensions.OnComplete<Sequence>(DOBorisPull(), new TweenCallback(BorisPullOnComplete));
	}

	private Sequence DOBorisPull()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		if (Object.op_Implicit((Object)(object)m_Ambience))
		{
			m_Ambience.Stop();
			m_Ambience = null;
		}
		Sequence val = DOTween.Sequence();
		float num = 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			m_BorisAnimationController.SetTrigger("Pull");
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(((Component)m_BorisAnimationController).transform, m_BorisEndPosition.position, 0.5f, false), (Ease)1);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(((Component)m_BorisAnimationController).transform, Vector3.zero, 0.3f, (RotateMode)0), (Ease)1);
			GameManager.Instance.HideScreenBlocker(0f);
			GameManager.Instance.AudioManager.Play(m_FinalClip_03);
		});
		num += 0.45f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(0f);
			GameManager.Instance.Player.transform.SetParent((Transform)null);
			GameManager.Instance.Player.gameObject.SetActive(false);
			GameManager.Instance.GameCamera.gameObject.SetActive(false);
			GameManager.Instance.AudioManager.ListenerSetActive(active: true);
		});
		num += 3.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
		});
		return val;
	}

	private void BorisPullOnComplete()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_PATH_OF_THE_DEMON);
		}
		else
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.THE_PATH_OF_THE_ANGEL);
		}
		SendOnComplete();
	}

	private void DoShake()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		if (!m_IsLanded)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(((Component)GameManager.Instance.GameCamera.Camera).transform, 0.1f, 0.25f, 1, 90f, false, true), new TweenCallback(DoShake));
		}
	}

	private void KillShake()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)((Component)GameManager.Instance.GameCamera.Camera).transform, false);
		((Component)GameManager.Instance.GameCamera.Camera).transform.localPosition = Vector3.zero;
	}

	private void Go(float speed, Ease ease = (Ease)1)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		ResetDropSequence();
		for (int i = 0; i < m_Shafts.Count; i++)
		{
			DisposableObject disposableObject = m_Shafts[i];
			float num = disposableObject.transform.localPosition.y + 60f;
			TweenSettingsExtensions.Insert(m_DropSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(disposableObject.transform, num, speed, false), ease));
		}
		TweenSettingsExtensions.InsertCallback(m_DropSequence, speed, (TweenCallback)delegate
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			DisposableObject disposableObject2 = m_Shafts[m_Shafts.Count - 1];
			Vector3 localPosition = disposableObject2.transform.localPosition;
			localPosition.y -= 240f;
			disposableObject2.transform.localPosition = localPosition;
			m_Shafts.Insert(0, disposableObject2);
			m_Shafts.RemoveAt(m_Shafts.Count - 1);
		});
		TweenSettingsExtensions.OnComplete<Sequence>(m_DropSequence, (TweenCallback)delegate
		{
			Go(Speed, (Ease)1);
		});
	}

	private void ResetDropSequence()
	{
		KillDropSequence();
		m_DropSequence = DOTween.Sequence();
	}

	private void KillDropSequence()
	{
		if (m_DropSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_DropSequence, false);
			m_DropSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillDropSequence();
		m_Ambience = null;
		m_LoopObject = null;
		m_MonologueAClips = null;
		m_MonologueBClips = null;
		m_MonologueCClips = null;
		m_HorrorAmbienceClip = null;
		m_FinalClip_01 = null;
		m_FinalClip_02 = null;
		m_FinalClip_03 = null;
		m_LiftDepartClip = null;
		m_LiftLoopClip = null;
		m_LiftFallLoopClip = null;
		m_SafehouseDoorClip = null;
		base.OnDisposed();
	}
}
