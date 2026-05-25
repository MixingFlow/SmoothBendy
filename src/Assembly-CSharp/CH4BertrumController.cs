using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class CH4BertrumController : TMGMonoBehaviour
{
	private enum ActionType
	{
		SWITCH_DIRECTION,
		QUARTER_SPIN,
		HALF_SPIN,
		THREE_QUARTER_SPIN,
		FULL_SPIN,
		ATTACK,
		PAUSE
	}

	[SerializeField]
	private Animator m_HeadAnimator;

	[Header("Sit Location")]
	[SerializeField]
	private Transform m_SitLocation;

	[SerializeField]
	private Interactable m_SitInteraction;

	[Header("Rotations")]
	[SerializeField]
	private Transform m_Top;

	[SerializeField]
	private Transform m_Center;

	[SerializeField]
	private Transform m_Bottom;

	[SerializeField]
	private Transform m_Head;

	[Header("Arms")]
	[SerializeField]
	private CH4BertrumArm m_Arm1;

	[SerializeField]
	private CH4BertrumArm m_Arm2;

	[SerializeField]
	private CH4BertrumArm m_Arm3;

	[SerializeField]
	private CH4BertrumArm m_Arm4;

	[Header("Doors")]
	[SerializeField]
	private List<CH4BertrumDoor> m_Doors;

	[Header("Lights")]
	[SerializeField]
	private List<LightBulbController> m_BottomLights;

	[SerializeField]
	private List<LightBulbController> m_TopLights;

	[SerializeField]
	private GameObject m_DoorLights;

	[Header("SceneProps")]
	[SerializeField]
	private Transform m_WorkbenchHitPoint;

	[SerializeField]
	private DestructibleObject m_Workbench;

	[SerializeField]
	private Transform m_Axe;

	[SerializeField]
	private Transform m_AxeStartPoint;

	[SerializeField]
	private Transform m_AxeLandPoint;

	[SerializeField]
	private BrokenWeapon m_BrokenAxe;

	[Header("AudioLog")]
	[SerializeField]
	private AudioLog m_AudioLog;

	[SerializeField]
	private GameObject m_CableFull;

	[SerializeField]
	private GameObject m_CableBroken;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject m_BaconSoupPhysicsPrefab;

	[SerializeField]
	private AudioClip m_CanDropClip;

	private bool m_IsActive;

	private int m_TurnDirection = 1;

	private Sequence m_PhaseSequence;

	private Sequence m_SpinSequence;

	private List<int[]> m_PhaseSequences = new List<int[]>();

	private List<CH4BertrumArm> m_Arms = new List<CH4BertrumArm>();

	private int m_ActiveArm;

	private int m_CurrentPhase;

	private int m_CurrentPhaseSequence;

	private AudioClip[] m_BertIntro;

	private AudioClip m_IntroMusic;

	private AudioClip m_BattleMusic;

	private AudioClip m_BattleFinishMusic;

	private AudioClip m_OnClip;

	private AudioObject m_BattleAudio;

	private bool m_IsRidingBert;

	public event EventHandler OnBegin;

	public event EventHandler OnComplete;

	public override void Init()
	{
		base.Init();
		m_CableBroken.SetActive(false);
		m_Arm1.OnDestroyed += HandleArmOnDestroyed;
		m_Arm2.OnDestroyed += HandleArmOnDestroyed;
		m_Arm3.OnDestroyed += HandleArmOnDestroyed;
		m_Arm4.OnDestroyed += HandleArmOnDestroyed;
		m_Arms.Add(m_Arm1);
		m_Arms.Add(m_Arm2);
		m_Arms.Add(m_Arm3);
		m_Arms.Add(m_Arm4);
		m_PhaseSequences.Add(new int[14]
		{
			4, 0, 4, 5, 0, 1, 0, 3, 5, 0,
			2, 0, 1, 0
		});
		m_PhaseSequences.Add(new int[18]
		{
			2, 0, 2, 0, 1, 5, 0, 4, 0, 4,
			0, 3, 0, 2, 0, 1, 5, 0
		});
		m_PhaseSequences.Add(new int[17]
		{
			4, 0, 4, 5, 0, 1, 5, 0, 4, 0,
			1, 0, 5, 0, 1, 5, 0
		});
		m_PhaseSequences.Add(new int[16]
		{
			1, 5, 0, 1, 5, 0, 2, 0, 2, 5,
			0, 3, 5, 0, 4, 0
		});
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BertIntro = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Bert/Boss");
		m_OnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Cassette_Player_Turn_On_01");
		m_IntroMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_HelloBertie");
		m_BattleMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_ColossalWonders");
		m_BattleFinishMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_ColossalWondersFinisher");
		for (int i = 0; i < m_BottomLights.Count; i++)
		{
			m_BottomLights[i].TurnOff();
		}
		for (int j = 0; j < m_TopLights.Count; j++)
		{
			m_TopLights[j].TurnOff();
		}
		m_DoorLights.SetActive(false);
		((Component)m_Axe).gameObject.SetActive(false);
		m_AudioLog.SetActive(active: false);
		m_AudioLog.SetSingleInteraction(_isSingle: true);
		m_SitInteraction.SetActive(active: false);
	}

	public void Activate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Arms.Count; i++)
		{
			m_Arms[i].transform.localPosition = Vector3.zero;
		}
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			((Component)m_Axe).GetComponent<MeleeWeapon>().OnEquipped += HandleAxeOnEquipped;
		}
		m_AudioLog.OnInteracted += HandleAudioLogOnInteracted;
		m_AudioLog.SetActive(active: true);
	}

	private void HandleAxeOnEquipped(object sender, EventArgs e)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		(sender as MeleeWeapon).OnEquipped -= HandleAxeOnEquipped;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.4f, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_CanDropClip);
		});
		GameObject val = Object.Instantiate<GameObject>(m_BaconSoupPhysicsPrefab);
		val.transform.position = GameManager.Instance.Player.WeaponGameObject.transform.position;
		val.transform.eulerAngles = GameManager.Instance.Player.WeaponGameObject.transform.eulerAngles;
		Object.Destroy((Object)(object)GameManager.Instance.Player.InactiveWeapon);
	}

	private void HandleAudioLogOnInteracted(object sender, EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLog.OnInteracted -= HandleAudioLogOnInteracted;
		m_AudioLog.SetActive(active: false);
		this.OnBegin.Send(this);
		GameManager.Instance.AudioManager.PlayAtPosition(m_OnClip, m_AudioLog.transform.position);
		int num = m_BertIntro.Length;
		int num2 = 4;
		int num3 = 6;
		int num4 = num - 1;
		for (int i = 0; i < m_BertIntro.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_BertIntro[i], SubtitleConstants.DIA_CH4_BERT_REVEAL[i], isTrimmed: true));
			if (i == num2)
			{
				audioObject.OnComplete += delegate
				{
					GameManager.Instance.AudioManager.Play(m_IntroMusic, AudioObjectType.MUSIC);
				};
			}
			else if (i == num3)
			{
				audioObject.OnComplete += delegate
				{
					DOStartUp();
					S13AudioManager.Instance.InvokeEvent("evt_bert_boss_startup");
				};
			}
			else if (i == num4)
			{
				audioObject.OnComplete += delegate
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Expected O, but got Unknown
					TweenSettingsExtensions.OnComplete<Sequence>(DOReavealHead(), new TweenCallback(PhaseSequence));
				};
			}
		}
	}

	private void HandleArmOnDestroyed(object sender, EventArgs e)
	{
		KillPhaseSequence();
		KillSpinSequence();
		m_HeadAnimator.SetTrigger("Hit");
		S13AudioManager.Instance.InvokeEvent("evt_bert_arm" + (5 - m_Arms.Count) + "_dead");
		CH4BertrumArm cH4BertrumArm = sender as CH4BertrumArm;
		cH4BertrumArm.OnDestroyed -= HandleArmOnDestroyed;
		m_Arms.Remove(cH4BertrumArm);
		if (m_ActiveArm >= m_Arms.Count)
		{
			m_ActiveArm = 0;
		}
		cH4BertrumArm.Dispose();
		GetNextPhase();
	}

	private Sequence DOStartUp()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		m_SitInteraction.OnInteracted += HandleSitInteractionOnInteracted;
		m_SitInteraction.SetActive(active: true);
		Sequence val = DOTween.Sequence();
		float num = 2f;
		for (int i = 0; i < m_BottomLights.Count; i++)
		{
			LightBulbController lightBulbController = m_BottomLights[i];
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(lightBulbController.TurnOn));
			num += 0.1f;
		}
		for (int j = 0; j < m_TopLights.Count; j++)
		{
			LightBulbController lightBulbController2 = m_TopLights[j];
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(lightBulbController2.TurnOn));
			num += 0.1f;
		}
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Arm1.Activate));
		num += 3f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Arm2.Activate));
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Arm3.Activate));
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Arm4.Activate));
		return val;
	}

	private void HandleSitInteractionOnInteracted(object sender, EventArgs e)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		m_SitInteraction.OnInteracted -= HandleSitInteractionOnInteracted;
		GameManager.Instance.Player.SetCollision(active: false);
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			GameManager.Instance.Player.WeaponGameObject.SetActive(false);
		}
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		val.SetParent(m_SitLocation);
		val.localPosition = Vector3.zero;
		val.localEulerAngles = Vector3.zero;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
		{
			m_IsRidingBert = true;
		});
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.GOING_TO_BE_SICK);
	}

	private void Update()
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsRidingBert && PlayerInput.InteractOnPressed())
		{
			m_IsRidingBert = false;
			GameManager.Instance.Player.SetCollision(active: true);
			GameManager.Instance.Player.GoToAndLookAt(GameManager.Instance.GameCamera.transform);
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
			{
				GameManager.Instance.Player.WeaponGameObject.SetActive(true);
			}
			Vector3 val = GameManager.Instance.Player.transform.position - base.transform.position;
			GameManager.Instance.Player.AddForce(((Vector3)(ref val)).normalized * 20f + Vector3.up * 1.5f);
		}
	}

	private void Go()
	{
		m_IsActive = true;
	}

	private void FixedUpdate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsActive)
		{
			m_Head.LookAt(GameManager.Instance.Player.transform);
			m_Head.eulerAngles = new Vector3(0f, m_Head.eulerAngles.y, 0f);
		}
	}

	private void PhaseSequence()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		switch (m_CurrentPhase)
		{
		case 0:
			SetAttackArm(0, active: true);
			SetAttackArm(1, active: false);
			SetAttackArm(2, active: true);
			SetAttackArm(3, active: false);
			break;
		case 1:
			SetAttackArm(0, active: true);
			SetAttackArm(1, active: false);
			SetAttackArm(2, active: true);
			break;
		case 2:
			SetAttackArm(0, active: true);
			SetAttackArm(1, active: false);
			break;
		case 3:
			SetAttackArm(0, active: true);
			break;
		}
		ResetPhaseSequence();
		int actionType = m_PhaseSequences[m_CurrentPhase][m_CurrentPhaseSequence];
		TweenSettingsExtensions.InsertCallback(m_PhaseSequence, 0f, (TweenCallback)delegate
		{
			CheckActionType(actionType);
		});
		TweenSettingsExtensions.InsertCallback(m_PhaseSequence, GetNextActionTime(actionType, isPostRotation: true), new TweenCallback(CheckPhaseSequence));
	}

	private void SetAttackArm(int armIndex, bool active)
	{
		CH4BertrumArm cH4BertrumArm = m_Arms[armIndex];
		cH4BertrumArm.SetAttackArm(active);
		cH4BertrumArm.SetRandomSpeed();
	}

	private void CheckPhaseSequence()
	{
		m_CurrentPhaseSequence++;
		if (m_CurrentPhaseSequence >= m_PhaseSequences[m_CurrentPhase].Length)
		{
			m_CurrentPhaseSequence = 0;
		}
		PhaseSequence();
	}

	private void GetNextPhase()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		m_CurrentPhase++;
		m_CurrentPhaseSequence = 0;
		if (m_CurrentPhase >= 4)
		{
			TweenSettingsExtensions.OnComplete<Sequence>(DODeath(), new TweenCallback(HandleDeathOnComplete));
		}
		else
		{
			TweenSettingsExtensions.OnComplete<Sequence>(DONextPhase(), new TweenCallback(PhaseSequence));
		}
	}

	private void HandleDeathOnComplete()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		m_BattleAudio.Stop();
		GameManager.Instance.AudioManager.Play(m_BattleFinishMusic, AudioObjectType.MUSIC);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 3f, (TweenCallback)delegate
		{
			for (int i = 0; i < m_Doors.Count; i++)
			{
				m_Doors[i].Close(6f, (Ease)7);
			}
		});
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.AROUND_AND_AROUND);
		SendOnComplete();
	}

	private Sequence DONextPhase()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		for (int i = 0; i < m_Doors.Count; i++)
		{
			m_Doors[i].Close(0.5f, (Ease)30);
		}
		float num = 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(1080f, 5f);
			for (int j = 0; j < m_Arms.Count; j++)
			{
				m_Arms[j].PhaseAttack();
			}
		});
		num += 3f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			for (int j = 0; j < m_Arms.Count; j++)
			{
				m_Arms[j].PhaseAttack();
			}
		});
		num += 3f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			SwitchDirections();
			for (int j = 0; j < m_Arms.Count; j++)
			{
				m_Arms[j].MakeVulnerable();
			}
			for (int k = 0; k < m_Doors.Count; k++)
			{
				m_Doors[k].Open();
			}
		});
		return val;
	}

	private void DOSpinSequence(float rotation = 360f, float speed = 4f)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		ResetSpinSequence();
		S13AudioManager.Instance.InvokeEvent("evt_bert_hub_turn_start");
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Center, new Vector3(0f, rotation * (float)m_TurnDirection, 0f), speed, (RotateMode)3), (Ease)1));
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Top, new Vector3(0f, rotation * (float)(-m_TurnDirection), 0f), speed, (RotateMode)3), (Ease)1));
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Bottom, new Vector3(0f, (rotation + 60f) * (float)(-m_TurnDirection), 0f), speed, (RotateMode)3), (Ease)1));
		TweenSettingsExtensions.OnComplete<Sequence>(m_SpinSequence, (TweenCallback)delegate
		{
			DOSpinInteruption();
		});
	}

	private Sequence DOSpinInteruption()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		ResetSpinSequence();
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Center, new Vector3(0f, 5f * (float)m_TurnDirection, 0f), 1f, (RotateMode)3), (Ease)24));
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Top, new Vector3(0f, 5f * (float)(-m_TurnDirection), 0f), 1f, (RotateMode)3), (Ease)24));
		TweenSettingsExtensions.Insert(m_SpinSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Bottom, new Vector3(0f, 5f * (float)(-m_TurnDirection), 0f), 1f, (RotateMode)3), (Ease)24));
		return m_SpinSequence;
	}

	private Sequence DODeath()
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject) && Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject.GetComponent<MeleeWeapon>()))
		{
			BrokenWeapon brokenWeapon = Object.Instantiate<BrokenWeapon>(m_BrokenAxe);
			brokenWeapon.Break(GameManager.Instance.Player.WeaponGameObject.transform, GameManager.Instance.Player.transform.position);
			Object.Destroy((Object)(object)GameManager.Instance.Player.WeaponGameObject);
			GameManager.Instance.Player.UnEquipWeapon();
		}
		if (Object.op_Implicit((Object)(object)m_Axe))
		{
			Object.Destroy((Object)(object)((Component)m_Axe).gameObject);
		}
		Sequence val = DOTween.Sequence();
		float num = 2f;
		S13AudioManager.Instance.InvokeEvent("evt_bert_boss_final_freakout");
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(90f, 0.5f);
		});
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(90f, 0.5f);
		});
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(90f, 0.5f);
		});
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(90f, 0.5f);
		});
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			SwitchDirections();
			DOSpinSequence(1080f, 3f);
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
			ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 3f, 2f, 15, 90f, false, true), (TweenCallback)delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
			});
		});
		num += 3.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_HeadAnimator.SetTrigger("Death");
			S13AudioManager.Instance.InvokeEvent("evt_bert_boss_defeated");
			ParticleRandomizedEmitController[] componentsInChildren = ((Component)this).GetComponentsInChildren<ParticleRandomizedEmitController>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetActive(_IsActive: false);
			}
			ParticleSystem[] componentsInChildren2 = ((Component)this).GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].Stop();
				componentsInChildren2[j].Emit(5);
			}
			m_IsActive = false;
		});
		return val;
	}

	private void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	private void SwitchDirections()
	{
		m_TurnDirection *= -1;
	}

	public void DestroyWorkbench()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		m_AudioLog.gameObject.SetActive(false);
		m_CableFull.SetActive(false);
		m_CableBroken.SetActive(true);
		((Component)m_Axe).GetComponent<BaseWeapon>().Interaction.SetActive(active: false);
		((Component)m_Axe).gameObject.SetActive(true);
		m_Axe.position = m_AxeStartPoint.position;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Axe, m_AxeLandPoint.position, 0.5f, false), (Ease)30), (TweenCallback)delegate
		{
			((Component)m_Axe).GetComponent<BaseWeapon>().Interaction.SetActive(active: true);
		});
		m_Workbench.Destroy(m_WorkbenchHitPoint.position);
		S13AudioManager.Instance.PlayAudio("sfx_workbench_destroyed");
	}

	private Sequence DOReavealHead()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		Go();
		OpenDoors();
		S13AudioManager.Instance.InvokeEvent("evt_bert_boss_head_reveal");
		m_DoorLights.SetActive(true);
		float num = 4f;
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_BattleAudio = GameManager.Instance.AudioManager.Play(m_BattleMusic, AudioObjectType.MUSIC, -1);
			DOSpinSequence(765f, 6.5f);
		});
		num += 7.5f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Arm2.AttackWorkbench));
		num += 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			DOSpinSequence(35f, 0.5f);
		});
		num += 1.5f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			for (int i = 0; i < m_Arms.Count; i++)
			{
				m_Arms[i].MakeVulnerable();
			}
			SwitchDirections();
		});
		return val;
	}

	private void OpenDoors()
	{
		for (int i = 0; i < m_Doors.Count; i++)
		{
			m_Doors[i].Open();
		}
	}

	private void CloseDoors()
	{
		for (int i = 0; i < m_Doors.Count; i++)
		{
			m_Doors[i].Close(0.5f, (Ease)30);
		}
	}

	private void DOAttack()
	{
		m_HeadAnimator.SetTrigger("Attack");
		switch (m_CurrentPhase)
		{
		case 0:
		case 1:
			ArmAttack(0);
			ArmAttack(2);
			break;
		case 2:
		case 3:
			ArmAttack(0);
			break;
		}
	}

	private void ArmAttack(int armIndex)
	{
		m_Arms[armIndex].ResetSpeed();
		m_Arms[armIndex].Attack();
	}

	private float GetPhaseSpeed()
	{
		return (float)m_CurrentPhase * 0.175f;
	}

	private void CheckActionType(int actionType)
	{
		switch (actionType)
		{
		case 0:
			SwitchDirections();
			break;
		case 1:
			DOSpinSequence(90f, GetNextActionTime(actionType));
			break;
		case 2:
			DOSpinSequence(180f, GetNextActionTime(actionType));
			break;
		case 3:
			DOSpinSequence(270f, GetNextActionTime(actionType));
			break;
		case 4:
			DOSpinSequence(360f, GetNextActionTime(actionType));
			break;
		case 5:
			DOAttack();
			break;
		case 6:
			DOReavealHead();
			break;
		}
	}

	private float GetNextActionTime(int actionType, bool isPostRotation = false)
	{
		return actionType switch
		{
			0 => 0.01f, 
			1 => 1f - GetPhaseSpeed() + ((!isPostRotation) ? 0f : 1f), 
			2 => 2f - GetPhaseSpeed() + ((!isPostRotation) ? 0f : 1f), 
			3 => 3f - GetPhaseSpeed() + ((!isPostRotation) ? 0f : 1f), 
			4 => 4f - GetPhaseSpeed() + ((!isPostRotation) ? 0f : 1f), 
			5 => 11f, 
			6 => 4f, 
			_ => 0f, 
		};
	}

	public void ForceComplete()
	{
		for (int i = 0; i < m_Arms.Count; i++)
		{
			Object.Destroy((Object)(object)m_Arms[i].gameObject);
		}
		m_AudioLog.gameObject.SetActive(false);
		m_CableFull.SetActive(false);
		m_CableBroken.SetActive(true);
		if (Object.op_Implicit((Object)(object)m_Axe))
		{
			Object.Destroy((Object)(object)((Component)m_Axe).gameObject);
		}
		m_Workbench.Dispose();
	}

	private void KillPhaseSequence()
	{
		if (m_PhaseSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_PhaseSequence, false);
		}
	}

	private void ResetPhaseSequence()
	{
		KillPhaseSequence();
		m_PhaseSequence = DOTween.Sequence();
	}

	private void KillSpinSequence()
	{
		if (m_SpinSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_SpinSequence, false);
			S13AudioManager.Instance.InvokeEvent("evt_bert_hub_turn_stop");
		}
	}

	private void ResetSpinSequence()
	{
		KillSpinSequence();
		m_SpinSequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		KillPhaseSequence();
		KillSpinSequence();
		if (m_PhaseSequences != null)
		{
			m_PhaseSequences.Clear();
			m_PhaseSequences = null;
		}
		if (m_Arms != null)
		{
			m_Arms.Clear();
			m_Arms = null;
		}
		m_BertIntro = null;
		m_IntroMusic = null;
		m_BattleMusic = null;
		m_BattleFinishMusic = null;
		m_OnClip = null;
		m_BattleAudio = null;
		base.OnDisposed();
	}
}
