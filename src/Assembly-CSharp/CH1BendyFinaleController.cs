using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class CH1BendyFinaleController : BaseController
{
	[Serializable]
	private class CollapseSection
	{
		public AudioClip AudioClip;

		public GameObject Collapse;

		public CollapsableCeiling[] Ceilings;
	}

	private const string CUTOUT = "_Cutout";

	[Header("<Initializers>")]
	[SerializeField]
	private GameObject m_Barricade;

	[SerializeField]
	private GenericDoorController m_HenryGateDoor;

	[Header("<Bendy Jumpscare>")]
	[SerializeField]
	private Transform m_BendyLookAt;

	[SerializeField]
	private EventTrigger m_BnedyEventTrigger;

	[SerializeField]
	private GameObject m_BENDY;

	[SerializeField]
	private MeshRenderer m_InkMachineMeshRenderer;

	[SerializeField]
	private Transform m_HenryFallLocation;

	[SerializeField]
	private Transform m_InkRaise;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_InkFinale;

	[SerializeField]
	private GameObject m_StobeLight;

	[Header("Sammys Room")]
	[SerializeField]
	private CH1GeneralController m_ProjectorController;

	[Header("<Lighting>")]
	[SerializeField]
	private List<GameObject> m_Lights;

	[SerializeField]
	private List<GameObject> m_BasementLights;

	[Header("Door")]
	[SerializeField]
	private CH1SammysRoomController m_SammyRoomController;

	[SerializeField]
	private BaseDoorController m_SammysDoor;

	[SerializeField]
	private GenericDoorController m_HallwayDoor;

	[Header("Exit")]
	[SerializeField]
	private EventTrigger m_ExitEventTrigger;

	[SerializeField]
	private CH1EntranceDoor m_EntranceDoor;

	[Header("Materials")]
	[SerializeField]
	private List<Material> m_CH1FinaleMaterials;

	[Header("<Fake Floor>")]
	[SerializeField]
	private InkMachineController m_InkMachineController;

	[Header("GameObjects")]
	[SerializeField]
	private List<GameObject> m_GameObjectsToActive;

	[SerializeField]
	private List<GameObject> m_GameObjectsToDisable;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_BrokenPlanksParent;

	[SerializeField]
	private Transform m_ExplosionForcePosition;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_FallingEventTrigger;

	[Header("Collapse Triggers")]
	[SerializeField]
	private List<EventTrigger> m_CollapseTriggers;

	[Header("Collapse Objects")]
	[SerializeField]
	private GameObject m_BendyCollapseObject;

	[SerializeField]
	private List<CollapseSection> m_CollapseSections;

	[Header("GateDoor")]
	[SerializeField]
	private EventTrigger m_GateEventTrigger;

	[SerializeField]
	private GameObject m_GateBent_01;

	[SerializeField]
	private GameObject m_GateBent_02;

	[SerializeField]
	private GameObject m_GateSpew_01;

	[SerializeField]
	private GameObject m_GateSpew_02;

	[SerializeField]
	private Transform m_FallPoint;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private Transform m_FreeRoamCam;

	private List<Rigidbody> m_BrokenPlanks = new List<Rigidbody>();

	private LightFixtureController[] m_LightFixtureControllers;

	private LightController[] m_LightControllers;

	private AudioClip m_JumpscareClip;

	private AudioClip m_MusicClip;

	private AudioClip m_HenryBClip;

	private AudioClip m_TumbleDownClip;

	private AudioClip m_BodyFallClip;

	private AudioClip m_SplashClip;

	private AudioClip m_SketchesMusic;

	private AudioClip m_HenryBodyFallClip;

	private AudioObject m_Music;

	private AudioObject m_InkFlowLoop;

	private Sequence m_FinaleSequence;

	private bool m_CanScare;

	private bool m_HasEffects;

	private bool m_ShowRunTutorial;

	private int m_CurrentColapseIndex;

	public override void Init()
	{
		base.Init();
		m_LightFixtureControllers = Object.FindObjectsOfType<LightFixtureController>();
		m_LightControllers = Object.FindObjectsOfType<LightController>();
	}

	public override void InitOnComplete()
	{
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		base.InitOnComplete();
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/MUS_Little_Devil_Darling_Remastered");
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_ChapterOneBendyAppears");
		m_HenryBClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_LAND");
		m_TumbleDownClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Tumble_Down_Shaft_01");
		m_BodyFallClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Tumble_Down_Shaft_Body_Fall_01");
		m_SplashClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Splash_01");
		m_SketchesMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH1/MUS_DownWhereMonstersLive");
		m_HenryBodyFallClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Henry_Body_Fall_01");
		m_InkFinale.SetActive(false);
		m_Barricade.SetActive(false);
		m_BnedyEventTrigger.SetActive(active: false);
		m_ExitEventTrigger.SetActive(active: false);
		m_GateEventTrigger.SetActive(active: false);
		m_HenryGateDoor.ForceOpen();
		m_GateBent_01.SetActive(false);
		m_GateBent_02.SetActive(false);
		for (int i = 0; i < m_BasementLights.Count; i++)
		{
			m_BasementLights[i].SetActive(false);
		}
		for (int j = 0; j < m_CH1FinaleMaterials.Count; j++)
		{
			Material val = m_CH1FinaleMaterials[j];
			val.SetFloat("_Cutout", 0f);
		}
		for (int k = 0; k < m_GameObjectsToActive.Count; k++)
		{
			m_GameObjectsToActive[k].SetActive(false);
		}
		for (int l = 0; l < m_GameObjectsToDisable.Count; l++)
		{
			m_GameObjectsToDisable[l].SetActive(true);
		}
		foreach (Transform item in m_BrokenPlanksParent)
		{
			Transform val2 = item;
			Rigidbody component = ((Component)val2).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				m_BrokenPlanks.Add(component);
			}
		}
		m_FallingEventTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BendyChaseObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		DOTweenUtil.DOAmbientLightColor(0.75f, 2f);
		m_Barricade.SetActive(true);
		m_BnedyEventTrigger.OnEnter += HandleBendyEventTriggerOnEnter;
		m_BnedyEventTrigger.OnExit += HandleBendyEventTriggerOnExit;
		m_BnedyEventTrigger.SetActive(active: true);
	}

	private void Update()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Expected O, but got Unknown
		if (m_ShowRunTutorial && PlayerInput.Run())
		{
			GameManager.Instance.HideTutorial();
			m_ShowRunTutorial = false;
		}
		if (m_CanScare && ((Renderer)m_InkMachineMeshRenderer).isVisible)
		{
			m_BnedyEventTrigger.OnEnter -= HandleBendyEventTriggerOnEnter;
			m_BnedyEventTrigger.OnExit -= HandleBendyEventTriggerOnExit;
			m_BnedyEventTrigger.Dispose();
			((Renderer)m_InkMachineMeshRenderer).enabled = false;
			m_CanScare = false;
			m_HasEffects = true;
			Sequence val = DOTween.Sequence();
			float num = 0f;
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(ActualActivate));
			TweenSettingsExtensions.InsertCallback(val, num + 2f, (TweenCallback)delegate
			{
				m_Music = GameManager.Instance.AudioManager.Play(m_MusicClip, AudioObjectType.MUSIC, -1);
			});
			Transform val2 = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
			GameManager.Instance.Player.GoToAndLookAt(m_HenryFallLocation);
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 3f, 0.6f, 15, 90f, false, true);
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(val2, m_BendyLookAt.position, 0.2f, (AxisConstraint)0, (Vector3?)null), (Ease)1));
			num += 0.4f;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(val2, m_HenryFallLocation.position.x, 0.5f, false), (Ease)1));
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveZ(val2, m_HenryFallLocation.position.z, 0.5f, false), (Ease)1));
			num += 0.35f;
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(val2, m_HenryFallLocation.position.y - 2f, 0.15f, false), (Ease)1));
			TweenSettingsExtensions.InsertCallback(val, num + 0.15f, (TweenCallback)delegate
			{
				GameManager.Instance.AudioManager.Play(m_HenryBodyFallClip);
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val2, new Vector3(-70f, -15f, 20f), 0.4f, (RotateMode)0), (Ease)1));
			num += 0.75f;
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				m_BENDY.SetActive(false);
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val2, m_HenryFallLocation.eulerAngles, 1f, (RotateMode)0), (Ease)7));
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val2, GameManager.Instance.Player.HeadContainer.position, 1f, false), (Ease)7));
			TweenSettingsExtensions.InsertCallback(val, num + 1f, (TweenCallback)delegate
			{
				GameManager.Instance.GameCamera.ExitFreeRoamCam();
				GameManager.Instance.Player.SetLock(active: false);
				GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_RUN"));
				m_ShowRunTutorial = true;
			});
			num += 1.5f;
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_06", string.Empty, 2f, isCurrentObjective: false, 1.75f));
			});
		}
	}

	private void LateUpdate()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (m_HasEffects)
		{
			GameManager.Instance.InkEffectManager.SetPosition(GameManager.Instance.Player.transform.position);
		}
	}

	private void HandleBendyEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_CanScare = true;
	}

	private void HandleBendyEventTriggerOnExit(object sender, EventArgs e)
	{
		m_CanScare = false;
	}

	private void ShakeCamera()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)GameManager.Instance.GameCamera.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 0.4f, 1.2f, 18, 90f, false, true), (Ease)1), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
	}

	private void ActualActivate()
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		S13AudioManager.Instance.InvokeEvent("evt_bendy_appears");
		GameManager.Instance.AudioManager.Play(m_JumpscareClip, AudioObjectType.MUSIC);
		m_EntranceDoor.Activate();
		m_HenryGateDoor.ForceClose();
		m_BENDY.SetActive(true);
		m_StobeLight.SetActive(false);
		m_InkFinale.SetActive(true);
		if ((Object)(object)m_SammyRoomController != (Object)null)
		{
			m_SammyRoomController.ForceClose();
		}
		TurnOffLights();
		m_ProjectorController.ShutDown();
		m_SammysDoor.ForceClose();
		m_SammysDoor.Lock();
		ResetSequence();
		float num = 0.25f;
		for (int i = 0; i < m_CH1FinaleMaterials.Count; i++)
		{
			Material val = m_CH1FinaleMaterials[i];
			val.SetFloat("_Cutout", 0f);
			TweenSettingsExtensions.Insert(m_FinaleSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(val, 0.5f, "_Cutout", 3f), (Ease)7));
		}
		num += 3.5f;
		TweenSettingsExtensions.Insert(m_FinaleSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_InkRaise, 2.15f, 30f, false), (Ease)7));
		num += 10f;
		TweenSettingsExtensions.InsertCallback(m_FinaleSequence, num, (TweenCallback)delegate
		{
			((Component)m_InkRaise.GetChild(0)).gameObject.tag = "DeepInk";
		});
		m_BendyCollapseObject.SetActive(true);
		for (int num2 = 0; num2 < m_CollapseTriggers.Count; num2++)
		{
			m_CollapseTriggers[num2].OnEnter += HandleColllapseTriggerEnter;
			m_CollapseTriggers[num2].SetActive(active: true);
		}
		m_ExitEventTrigger.OnEnter += HandleExitEventTriggerOnEnter;
		m_ExitEventTrigger.SetActive(active: true);
		m_GateEventTrigger.OnEnter += HandleGateBenderOnEnter;
		m_GateEventTrigger.SetActive(active: true);
	}

	private void HandleGateBenderOnEnter(object sender, EventArgs e)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		m_GateEventTrigger.OnEnter -= HandleGateBenderOnEnter;
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 0f, (TweenCallback)delegate
		{
			m_HallwayDoor.Close();
		});
		TweenSettingsExtensions.InsertCallback(val, 0.8f, (TweenCallback)delegate
		{
			ShakeCamera();
			S13AudioManager.Instance.PlayAudio("sfx_ink_bursts");
			m_HallwayDoor.gameObject.SetActive(false);
			m_GateBent_01.SetActive(true);
			m_GateSpew_01.SetActive(true);
		});
		TweenSettingsExtensions.InsertCallback(val, 2f, (TweenCallback)delegate
		{
			ShakeCamera();
			S13AudioManager.Instance.PlayAudio("sfx_ink_bursts");
			m_GateBent_01.SetActive(false);
			m_GateBent_02.SetActive(true);
			m_GateSpew_02.SetActive(true);
		});
	}

	private void HandleColllapseTriggerEnter(object sender, EventArgs e)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		EventTrigger eventTrigger = m_CollapseTriggers[m_CurrentColapseIndex];
		eventTrigger.OnEnter -= HandleColllapseTriggerEnter;
		eventTrigger.SetActive(active: false);
		ShakeCamera();
		CollapseSection collapseSection = m_CollapseSections[m_CurrentColapseIndex];
		S13AudioManager.Instance.PlayAudio("sfx_ink_bursts");
		if (collapseSection != null)
		{
			if (Object.op_Implicit((Object)(object)collapseSection.Collapse))
			{
				collapseSection.Collapse.SetActive(true);
			}
			if ((Object)(object)collapseSection.AudioClip != (Object)null)
			{
				GameManager.Instance.AudioManager.PlayAtPosition(collapseSection.AudioClip, eventTrigger.transform.position + Vector3.up * 2f);
			}
			if (collapseSection.Ceilings != null && collapseSection.Ceilings.Length > 0)
			{
				for (int i = 0; i < collapseSection.Ceilings.Length; i++)
				{
					collapseSection.Ceilings[i].Activate();
				}
			}
		}
		m_CurrentColapseIndex++;
	}

	private void HandleExitEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		if ((Object)(object)m_Music != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_Music.AudioSource.DOFade(0f, 1.5f), (TweenCallback)delegate
			{
				m_Music.Clear();
				m_Music = null;
			});
		}
		if ((Object)(object)m_InkFlowLoop != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_InkFlowLoop.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
			{
				m_InkFlowLoop.Clear();
				m_InkFlowLoop = null;
			});
		}
		m_HasEffects = false;
		BreakFloor();
		m_FallingEventTrigger.SetActive(active: true);
		m_FallingEventTrigger.OnEnter += HandleOnFallingEventTriggerOnEnter;
	}

	private void BreakFloor()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_floor_caves_in");
		for (int i = 0; i < m_GameObjectsToActive.Count; i++)
		{
			m_GameObjectsToActive[i].SetActive(true);
		}
		for (int j = 0; j < m_GameObjectsToDisable.Count; j++)
		{
			m_GameObjectsToDisable[j].SetActive(false);
		}
		for (int k = 0; k < m_BrokenPlanks.Count; k++)
		{
			m_BrokenPlanks[k].AddExplosionForce(1000f, m_ExplosionForcePosition.position, 10f);
		}
	}

	private void HandleOnFallingEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		m_FallingEventTrigger.OnEnter -= HandleOnFallingEventTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_TumbleDownClip);
		m_InkMachineController.Dispose();
		TweenSettingsExtensions.OnComplete<Sequence>(DOFall(), new TweenCallback(Complete));
	}

	private void TurnOffLights()
	{
		for (int i = 0; i < m_LightFixtureControllers.Length; i++)
		{
			m_LightFixtureControllers[i].TurnOff();
		}
		for (int j = 0; j < m_LightControllers.Length; j++)
		{
			m_LightControllers[j].TurnOff();
		}
		for (int k = 0; k < m_Lights.Count; k++)
		{
			m_Lights[k].SetActive(false);
		}
		for (int l = 0; l < m_BasementLights.Count; l++)
		{
			m_BasementLights[l].SetActive(true);
		}
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
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		m_FreeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_FallPoint);
		Vector3 val2 = GameManager.Instance.Player.HeadContainer.position - Vector3.up * 0.25f;
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_FreeRoamCam, val2, 2f, false), (Ease)1));
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, new Vector3(-90f, 0f, 90f), 1f, (RotateMode)0), (Ease)26));
		TweenSettingsExtensions.Insert(val, 1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, new Vector3(-90f, 0f, 180f), 1f, (RotateMode)0), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val, 2f, new TweenCallback(OnLanding));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_FreeRoamCam, m_FallPoint.eulerAngles, 0.5f, (RotateMode)0), (Ease)24));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOShakePosition(m_FreeRoamCam, 0.5f, 5f, 15, 90f, false, true), (Ease)1));
		TweenSettingsExtensions.Insert(val, 2f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(m_FreeRoamCam, GameManager.Instance.Player.HeadContainer.position.y, 0.5f, false), (Ease)6));
		return val;
	}

	private void OnLanding()
	{
		GameManager.Instance.ShowHurtBorder(isSilent: true);
		GameManager.Instance.AudioManager.Play(m_HenryBClip);
		GameManager.Instance.AudioManager.Play(m_BodyFallClip);
		GameManager.Instance.AudioManager.Play(m_SplashClip);
		GameManager.Instance.AudioManager.Play(m_SketchesMusic, AudioObjectType.MUSIC);
		GameManager.Instance.Player.SetSlowed(active: false);
		GameManager.Instance.InkEffectManager.SetActive(active: false);
	}

	private void Complete()
	{
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.HELLO_BENDY);
		GameManager.Instance.HideTutorial();
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BendyChaseObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_FinaleSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_FinaleSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_FinaleSequence, false);
			m_FinaleSequence = null;
		}
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH1_save_point_05");
		TurnOffLights();
		m_HenryGateDoor.ForceClose();
		for (int i = 0; i < m_GameObjectsToActive.Count; i++)
		{
			m_GameObjectsToActive[i].SetActive(true);
		}
		for (int j = 0; j < m_GameObjectsToDisable.Count; j++)
		{
			m_GameObjectsToDisable[j].SetActive(false);
		}
		m_BENDY.SetActive(false);
		m_StobeLight.SetActive(false);
		m_InkFinale.SetActive(true);
		m_ProjectorController.ShutDown();
		for (int k = 0; k < m_CH1FinaleMaterials.Count; k++)
		{
			Material val = m_CH1FinaleMaterials[k];
			val.SetFloat("_Cutout", 0.5f);
		}
		GameManager.Instance.HideTutorial();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		KillSequence();
		if (Object.op_Implicit((Object)(object)m_BnedyEventTrigger))
		{
			m_BnedyEventTrigger.OnEnter -= HandleBendyEventTriggerOnEnter;
			m_BnedyEventTrigger.OnExit -= HandleBendyEventTriggerOnExit;
		}
		if (Object.op_Implicit((Object)(object)m_ExitEventTrigger))
		{
			m_ExitEventTrigger.OnEnter -= HandleExitEventTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_FallingEventTrigger))
		{
			m_FallingEventTrigger.OnEnter -= HandleOnFallingEventTriggerOnEnter;
		}
		if (m_BrokenPlanks != null)
		{
			m_BrokenPlanks.Clear();
			m_BrokenPlanks = null;
		}
		m_LightFixtureControllers = null;
		m_LightControllers = null;
		m_Music = null;
		m_InkFlowLoop = null;
		m_JumpscareClip = null;
		m_MusicClip = null;
		m_HenryBClip = null;
		m_TumbleDownClip = null;
		m_BodyFallClip = null;
		m_SplashClip = null;
		m_SketchesMusic = null;
		base.OnDisposed();
	}
}
