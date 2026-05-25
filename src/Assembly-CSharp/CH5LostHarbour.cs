using System;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5LostHarbour : BaseController
{
	[Header("Exit Boat")]
	[SerializeField]
	private GameObject m_Spawner;

	[SerializeField]
	private GameObject m_BoatBackBlocker;

	[SerializeField]
	private EventTrigger m_BoatExitTrigger;

	[SerializeField]
	private GameObject m_BoatBlocker;

	[SerializeField]
	private EventTrigger m_PlankBreakTrigger;

	[SerializeField]
	private GameObject m_PlankBlocker;

	[SerializeField]
	private Breakable m_BreakablePlank;

	[Header("Objective: Sammy!")]
	[SerializeField]
	private CH5LostHarborSammyController m_SammyController;

	[SerializeField]
	private Renderer m_SammyBody;

	[SerializeField]
	private GameObject m_SammyBodyColliders;

	[Header("Objective: Battle!")]
	[SerializeField]
	private CH5LostHarbourBattleController m_BattleController;

	[SerializeField]
	private AllyAiController m_Tom;

	[SerializeField]
	private AllyAiController m_Allison;

	[SerializeField]
	private WaypointList m_AllisonWaypoints;

	[SerializeField]
	private CH5FenceDoor m_FenceDoor;

	[Header("Objective: Fall Down!")]
	[SerializeField]
	private EventTrigger m_FallTrigger;

	[SerializeField]
	private Breakable m_WeakPlank;

	[SerializeField]
	private Transform m_LandLocation;

	private Transform m_FreeRoamCam;

	private AudioClip m_RumbleClip;

	private AudioClip m_PlankBreakClip;

	private AudioClip m_HenryBClip;

	private AudioClip m_BodyFallClip;

	private AudioClip m_SplashClip;

	private AudioClip m_AllisonHenryClip;

	private AudioClip m_FallingAgainMusic;

	private AudioClip[] m_AllisonBattleIntro;

	private AudioClip[] m_AllisonBattleEnding;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Spawner.SetActive(false);
		m_BoatBackBlocker.SetActive(false);
		m_BoatBlocker.SetActive(false);
		m_PlankBlocker.SetActive(false);
		m_RumbleClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Rumble_Loop_01");
		m_PlankBreakClip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/Axe/SFX_Axe_Wood_Hit_Crack_06");
		m_HenryBClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_LAND");
		m_BodyFallClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Tumble_Down_Shaft_Body_Fall_01");
		m_SplashClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Splash_01");
		m_AllisonHenryClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/AliceA/DIA_CH5_ALICEA_HENRY");
		m_FallingAgainMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_FallingAgain");
		m_AllisonBattleIntro = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/BattleIntro");
		m_AllisonBattleEnding = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/BattleEnding");
	}

	public override void Activate()
	{
		m_Spawner.SetActive(true);
		m_BoatBackBlocker.SetActive(true);
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.LostHarbourObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.LostHarbourObjective.IsStarted)
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
		m_BoatExitTrigger.OnEnter += HandleBoatExitTriggerOnEnter;
		m_BoatExitTrigger.SetActive(active: true);
		m_SammyController.OnComplete += HandleSammyControllerOnComplete;
		m_SammyController.Activate();
	}

	private void ForceStart()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		m_WeakPlank.Dispose();
		m_Allison.transform.position = m_AllisonWaypoints.Waypoints[0].transform.position;
		m_Allison.transform.eulerAngles = m_AllisonWaypoints.Waypoints[0].transform.eulerAngles;
		m_SammyController.ForceComplete();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1.5f, (TweenCallback)delegate
		{
			int num = m_AllisonBattleIntro.Length - 1;
			m_Allison.DoSpeak(m_AllisonBattleIntro[num].length);
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonBattleIntro[num], SubtitleConstants.DIA_CH5_ALISONA_BATTLE_INTRO[num]));
			m_BattleController.EnableMusic();
			m_BattleController.OnComplete += HandleBattleControllerOnComplete;
			m_BattleController.Activate();
		});
	}

	private void ForceComplete()
	{
		m_WeakPlank.Dispose();
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
		}
		GameManager.Instance.Player.UnEquipWeapon();
		SendOnComplete();
	}

	private void HandleBoatExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_BoatExitTrigger.OnEnter -= HandleBoatExitTriggerOnEnter;
		m_BoatBlocker.SetActive(true);
		m_PlankBreakTrigger.OnEnter += HandlePlankBreakTriggerOnEnter;
		m_PlankBreakTrigger.SetActive(active: true);
	}

	private void HandlePlankBreakTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		m_PlankBreakTrigger.OnEnter -= HandlePlankBreakTriggerOnEnter;
		GameManager.Instance.AudioManager.PlayAtPosition(m_PlankBreakClip, m_BreakablePlank.transform.position);
		m_BreakablePlank.Destroy(m_BreakablePlank.transform.position + Vector3.up * 2f);
		m_PlankBlocker.SetActive(true);
	}

	private void HandleSammyControllerOnComplete(object sender, EventArgs e)
	{
		m_SammyController.OnComplete -= HandleSammyControllerOnComplete;
		m_Tom.SetThought(AiThought.Idle);
		m_Allison.UpdateWaypointList(m_AllisonWaypoints.Waypoints);
		m_Allison.DoSpeak(m_AllisonBattleIntro[0].length + m_AllisonBattleIntro[1].length);
		for (int i = 0; i < 2; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonBattleIntro[i], SubtitleConstants.DIA_CH5_ALISONA_BATTLE_INTRO[i], isTrimmed: true));
			if (i != 1)
			{
				continue;
			}
			audioObject.OnComplete += delegate
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Expected O, but got Unknown
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Expected O, but got Unknown
				m_Allison.SetTrigger("LookAround");
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOFloat(m_SammyBody.materials[0], 1f, "_Dissolve", 4f), 1f), (Ease)1), (TweenCallback)delegate
				{
					m_SammyBodyColliders.SetActive(false);
				});
				GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
				ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
				ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 3f, 0.25f, 15, 90f, false, false);
				AudioObject rumble = GameManager.Instance.AudioManager.Play(m_RumbleClip);
				TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(rumble.AudioSource.DOFade(0f, 1f), 3f), (TweenCallback)delegate
				{
					if (Object.op_Implicit((Object)(object)rumble))
					{
						rumble.Clear();
					}
					RumbleOnComplete();
				});
			};
		}
	}

	private void RumbleOnComplete()
	{
		for (int i = 2; i < m_AllisonBattleIntro.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonBattleIntro[i], SubtitleConstants.DIA_CH5_ALISONA_BATTLE_INTRO[i], isTrimmed: true));
			if (i == m_AllisonBattleIntro.Length - 3)
			{
				audioObject.OnComplete += delegate
				{
					m_BattleController.EnableMusic();
				};
			}
			else if (i == m_AllisonBattleIntro.Length - 2)
			{
				audioObject.OnComplete += delegate
				{
					m_BattleController.OnComplete += HandleBattleControllerOnComplete;
					m_BattleController.Activate();
				};
			}
			else if (i == 3)
			{
				audioObject.OnComplete += delegate
				{
					m_Allison.DoSpeak(m_AllisonBattleIntro[3].length + m_AllisonBattleIntro[4].length + m_AllisonBattleIntro[5].length + m_AllisonBattleIntro[6].length);
				};
			}
		}
	}

	private void HandleBattleControllerOnComplete(object sender, EventArgs e)
	{
		m_BattleController.OnComplete -= HandleBattleControllerOnComplete;
		m_Allison.SetTarget(m_FenceDoor.AttackLocation);
		float num = 0f;
		for (int i = 0; i < m_AllisonBattleEnding.Length; i++)
		{
			num += m_AllisonBattleEnding[i].length;
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonBattleEnding[i], SubtitleConstants.DIA_CH5_ALISONA_BATTLE_ENDING[i], isTrimmed: true));
			if (i == m_AllisonBattleEnding.Length - 1)
			{
				audioObject.OnComplete += delegate
				{
					m_Tom.ExitCombat();
					m_Tom.SetThought(AiThought.Idle);
					m_Tom.SetTarget(GameManager.Instance.Player.transform);
					m_Allison.ExitCombat();
					m_Allison.SetThought(AiThought.Idle);
					m_Allison.SetTarget(GameManager.Instance.Player.transform);
				};
			}
		}
		m_FallTrigger.OnEnter += HandleFallTriggerOnEnter;
		m_FallTrigger.SetActive(active: true);
		m_Allison.DoSpeak(num);
	}

	private void HandleFallTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		m_FallTrigger.OnEnter -= HandleFallTriggerOnEnter;
		if (Object.op_Implicit((Object)(object)m_WeakPlank))
		{
			m_WeakPlank.Destroy(m_WeakPlank.transform.position + Vector3.up * 0.5f);
		}
		TweenSettingsExtensions.OnComplete<Sequence>(DOFall(), new TweenCallback(Complete));
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonHenryClip, "DIACH5/DIA_CH5_ALLISON_HENRY"));
		GameManager.Instance.AudioManager.Play(m_FallingAgainMusic, AudioObjectType.MUSIC);
		S13AudioManager.Instance.InvokeEvent("evt_ch5_abyss_fall");
	}

	private Sequence DOFall()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_LandLocation);
		Vector3 val2 = GameManager.Instance.Player.HeadContainer.position - Vector3.up * 0.25f;
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, val2, 2f, false), (Ease)1));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, new Vector3(-90f, 0f, 90f), 1f, (RotateMode)0), (Ease)26));
		TweenSettingsExtensions.InsertCallback(val, 0.5f, new TweenCallback(OnFalling));
		TweenSettingsExtensions.Insert(val, 1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, new Vector3(-90f, 0f, 180f), 1f, (RotateMode)0), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val, 2f, new TweenCallback(OnLanding));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_LandLocation.eulerAngles, 0.5f, (RotateMode)0), (Ease)24));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOShakePosition(m_FreeRoamCam, 0.5f, 5f, 15, 90f, false, true), (Ease)1));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(m_FreeRoamCam, GameManager.Instance.Player.HeadContainer.position.y, 0.5f, false), (Ease)6));
		return val;
	}

	private void OnFalling()
	{
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.InactiveWeapon))
		{
			Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
		}
		GameManager.Instance.Player.UnEquipWeapon();
	}

	private void OnLanding()
	{
		for (int i = 0; i < 3; i++)
		{
			GameManager.Instance.ShowHurtBorder(isSilent: true);
		}
		GameManager.Instance.AudioManager.Play(m_HenryBClip);
		GameManager.Instance.AudioManager.Play(m_BodyFallClip);
		GameManager.Instance.AudioManager.Play(m_SplashClip);
	}

	private void Complete()
	{
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.LostHarbourObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_RumbleClip = null;
		m_PlankBreakClip = null;
		m_HenryBClip = null;
		m_BodyFallClip = null;
		m_SplashClip = null;
		m_AllisonHenryClip = null;
		m_FallingAgainMusic = null;
		base.OnDisposed();
	}
}
