using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4AccountingController : BaseController
{
	[Serializable]
	public class BookPuzzle
	{
		public Interactable Book;

		public Transform Position;
	}

	[Header("Grants Office")]
	[SerializeField]
	private BaseDoorController m_GrantsDoor;

	[Header("Objective: Archive Enter")]
	[SerializeField]
	private Interactable m_HatchValve;

	[SerializeField]
	private Interactable m_FloorValve;

	[SerializeField]
	private Transform m_HatchDoor;

	[SerializeField]
	private EventTrigger m_ArchiveDoorTrigger;

	[SerializeField]
	private EventTrigger m_ArchiveEntranceTrigger;

	[SerializeField]
	private List<GameObject> m_StageLights;

	[SerializeField]
	private LightController m_EntranceLantern;

	[SerializeField]
	private GameObject m_LostOnesParent;

	[Header("Objective: Archive Exit")]
	[SerializeField]
	private GameObject m_Statues;

	[SerializeField]
	private List<LightBulbController> m_Lights;

	[SerializeField]
	private List<BookPuzzle> m_BookPuzzle;

	[SerializeField]
	private List<CH4SafeDoors> m_SafeDoors;

	[SerializeField]
	private List<SwayController> m_Lanterns;

	[SerializeField]
	private CH4CurvedDoor AccountingExitDoor;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[Header("Meatly Achievement")]
	[SerializeField]
	private BaseDoorController m_MeatlyDoor;

	[SerializeField]
	private MeatlyController m_MeatlyController;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private List<S13ObjectComplex> m_SpotlightAudio = new List<S13ObjectComplex>();

	private Sequence m_SwayDelaySequence;

	private Sequence m_HatchDoorSequence;

	private AudioObject m_TempSafes;

	private AudioClip m_TempSafesClip;

	private AudioClip m_tempDoor;

	private AudioClip m_ValvePlaceClip;

	private AudioClip m_HatchDoorClip;

	private AudioClip m_ValveTurnClip;

	private AudioClip m_HenryClip01;

	private AudioClip m_HenryClip02;

	private AudioClip m_SpotlightMusic;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ValveTurnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
		m_HatchDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_valvepaneldooropen");
		m_ValvePlaceClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/Valves/SFX_CH3_valvepuzzle_valvearray_02");
		m_TempSafesClip = GameManager.Instance.GetAudioClip("Audio/TEMP/sfx_safes_going_crazy_loop");
		m_tempDoor = GameManager.Instance.GetAudioClip("Audio/TEMP/temp_door");
		m_HenryClip01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_01");
		m_HenryClip02 = GameManager.Instance.GetAudioClip("Audio/DIA/CH4/Henry/DIA_CH4_HENRY_02");
		m_SpotlightMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_LongLongForgotten");
		for (int i = 0; i < m_Lights.Count; i++)
		{
			m_Lights[i].TurnOff();
		}
		m_HatchValve.SetActive(active: false);
		m_FloorValve.SetActive(active: false);
		m_ArchiveDoorTrigger.SetActive(active: false);
		m_ArchiveEntranceTrigger.SetActive(active: false);
		m_ExitTrigger.SetActive(active: false);
		m_EntranceLantern.TurnOff();
		m_MeatlyDoor.Lock();
		for (int j = 0; j < m_StageLights.Count; j++)
		{
			GameObject val = m_StageLights[j];
			S13ObjectComplex componentInChildren = val.GetComponentInChildren<S13ObjectComplex>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				m_SpotlightAudio.Add(componentInChildren);
			}
			val.SetActive(false);
		}
	}

	public override void Activate()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.AccountingObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.AccountingObjective.IsStarted)
		{
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_SECRET_PASSAGE", string.Empty));
			S13AudioManager.Instance.InvokeEvent("evt_starting_lift_exit");
			m_HatchDoor.localEulerAngles = Vector3.zero;
			for (int i = 0; i < m_StageLights.Count; i++)
			{
				m_StageLights[i].gameObject.SetActive(true);
			}
			m_BookPuzzle[0].Book.SetActive(active: true);
			m_BookPuzzle[0].Book.OnInteracted += HandleInitialBookOnInteracted;
		}
		else
		{
			m_GrantsDoor.OnInteracted += HandleGrantsDoorOnInteracted;
			m_ArchiveDoorTrigger.OnEnter += HandleArchiveDoorTriggerOnEnter;
			m_ArchiveDoorTrigger.SetActive(active: true);
		}
	}

	private void ForceComplete()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_01");
		S13AudioManager.Instance.InvokeEvent("evt_accounting_exit_door_open");
		m_Statues.SetActive(false);
		for (int i = 0; i < m_Lights.Count; i++)
		{
			m_Lights[i].TurnOn();
		}
		for (int j = 0; j < m_BookPuzzle.Count; j++)
		{
			BookPuzzle bookPuzzle = m_BookPuzzle[j];
			bookPuzzle.Book.transform.position = bookPuzzle.Position.position;
		}
		AccountingExitDoor.ForceOpen();
		m_LostOnesParent.SetActive(false);
		SendOnComplete();
	}

	private void HandleGrantsDoorOnInteracted(object sender, EventArgs e)
	{
		m_GrantsDoor.OnInteracted -= HandleGrantsDoorOnInteracted;
		S13AudioManager.Instance.InvokeEvent("evt_startinglift_sideroom_opened");
	}

	private void HandleArchiveDoorTriggerOnEnter(object sender, EventArgs e)
	{
		m_ArchiveDoorTrigger.OnEnter -= HandleArchiveDoorTriggerOnEnter;
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip01, "DIACH4/DIA_CH4_HENRY_01"));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_ENTER_THE_ARCHIVES", "OBJECTIVES/CH4_OBJECTIVE_ENTER_THE_ARCHIVES_TIP", 4f));
		};
		((Component)m_HatchValve).GetComponent<Collider>().enabled = false;
		m_HatchValve.SetActive(active: true);
		m_FloorValve.OnInteracted += HandleFloorValveOnInteracted;
		m_FloorValve.SetActive(active: true);
	}

	private void HandleFloorValveOnInteracted(object sender, EventArgs e)
	{
		m_FloorValve.OnInteracted += HandleFloorValveOnInteracted;
		m_FloorValve.gameObject.SetActive(false);
		GameManager.Instance.Player.PlayPickUpSound();
		((Component)m_HatchValve).GetComponent<Collider>().enabled = true;
		m_HatchValve.OnInteracted += HandleHatchValveOnInteracted;
	}

	private void HandleHatchValveOnInteracted(object sender, EventArgs e)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		m_HatchValve.OnInteracted -= HandleHatchValveOnInteracted;
		m_HatchValve.SetActive(active: false);
		m_FloorValve.gameObject.SetActive(true);
		m_FloorValve.transform.SetParent(m_HatchValve.transform.parent);
		m_FloorValve.transform.localPosition = m_HatchValve.transform.localPosition;
		m_FloorValve.transform.localEulerAngles = m_HatchValve.transform.localEulerAngles;
		m_HatchValve.Dispose();
		GameManager.Instance.AudioManager.PlayAtPosition(m_ValvePlaceClip, m_HatchDoor.position);
		m_HatchDoorSequence = DOTween.Sequence();
		float num = 0.5f;
		TweenSettingsExtensions.InsertCallback(m_HatchDoorSequence, num, (TweenCallback)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.AudioManager.PlayAtPosition(m_ValveTurnClip, m_HatchDoor.position);
		});
		TweenSettingsExtensions.Insert(m_HatchDoorSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_FloorValve.transform, new Vector3(720f, 0f, 0f), 2f, (RotateMode)3), (Ease)7));
		num += 2f;
		S13AudioManager.Instance.InvokeEvent("evt_stage_entry_door_open");
		TweenSettingsExtensions.InsertCallback(m_HatchDoorSequence, num, (TweenCallback)delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.AudioManager.PlayAtPosition(m_HatchDoorClip, m_HatchDoor.position);
		});
		TweenSettingsExtensions.Insert(m_HatchDoorSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_HatchDoor, new Vector3(0f, -105f, 0f), 1.5f, (RotateMode)3), (Ease)7));
		m_MeatlyController.Activate();
		m_MeatlyDoor.Unlock();
		m_ArchiveEntranceTrigger.OnEnter += HandleArchiveEntranceTriggerOnEnter;
		m_ArchiveEntranceTrigger.SetActive(active: true);
	}

	private void HandleArchiveEntranceTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		m_ArchiveEntranceTrigger.OnEnter -= HandleArchiveEntranceTriggerOnEnter;
		m_ArchiveEntranceTrigger.SetActive(active: false);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.AccountingObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_SECRET_PASSAGE", string.Empty, 4f));
		TweenExtensions.Kill((Tween)(object)m_HatchDoorSequence, false);
		m_HatchDoorSequence = DOTween.Sequence();
		TweenSettingsExtensions.Insert(m_HatchDoorSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_HatchDoor, Vector3.zero, 0.25f, (RotateMode)0), (Ease)1));
		Sequence val = DOTween.Sequence();
		float num = 0f;
		for (int i = 0; i < m_StageLights.Count; i++)
		{
			num += (float)i * 0.75f;
			GameObject go = m_StageLights[i];
			S13ObjectComplex s13 = null;
			if (i < m_SpotlightAudio.Count)
			{
				s13 = m_SpotlightAudio[i];
			}
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				go.SetActive(true);
				if ((Object)(object)s13 != (Object)null)
				{
					s13.Play();
				}
			});
		}
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_EntranceLantern.TurnOn();
		});
		m_BookPuzzle[0].Book.SetActive(active: true);
		m_BookPuzzle[0].Book.OnInteracted += HandleInitialBookOnInteracted;
		GameManager.Instance.AudioManager.Play(m_SpotlightMusic, AudioObjectType.MUSIC);
		S13AudioManager.Instance.InvokeEvent("evt_starting_lift_exit");
	}

	private void HandleInitialBookOnInteracted(object sender, EventArgs e)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		m_Statues.SetActive(false);
		BookPuzzle bookPuzzle = m_BookPuzzle[0];
		bookPuzzle.Book.OnInteracted -= HandleInitialBookOnInteracted;
		((Component)bookPuzzle.Book).GetComponentInChildren<S13ObjectComplex>().Play();
		Transform lightPosition = m_Lights[0].transform;
		m_LostOnesParent.SetActive(false);
		GameManager.Instance.Player.SetLockedMovement(active: true);
		Transform freeRoamCam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(bookPuzzle.Book.transform, bookPuzzle.Position.position, 0.5f, false), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val, 0.5f, (TweenCallback)delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(freeRoamCam, lightPosition.position, 1f, (AxisConstraint)0, (Vector3?)null), (Ease)7), (TweenCallback)delegate
			{
				GameManager.Instance.GameCamera.ExitFreeRoamCam();
				GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip02, "DIACH4/DIA_CH4_HENRY_02"));
			});
		});
		TweenSettingsExtensions.InsertCallback(val, 1.5f, (TweenCallback)delegate
		{
			CheckLights();
			for (int i = 1; i < m_BookPuzzle.Count; i++)
			{
				BookPuzzle bookPuzzle2 = m_BookPuzzle[i];
				bookPuzzle2.Book.SetActive(active: true);
				bookPuzzle2.Book.OnInteracted += HandleBookOnInteracted;
			}
		});
	}

	private void HandleBookOnInteracted(object sender, EventArgs e)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		Interactable interactable = (Interactable)sender;
		BookPuzzle bookPuzzle = null;
		for (int i = 0; i < m_BookPuzzle.Count; i++)
		{
			if ((Object)(object)m_BookPuzzle[i].Book == (Object)(object)interactable)
			{
				bookPuzzle = m_BookPuzzle[i];
				break;
			}
		}
		bookPuzzle.Book.OnInteracted -= HandleBookOnInteracted;
		((Component)interactable).GetComponentInChildren<S13ObjectComplex>().Play();
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(bookPuzzle.Book.transform, bookPuzzle.Position.position, 0.5f, false), (Ease)6), new TweenCallback(CheckLights));
	}

	private void CheckLights()
	{
		if (m_Lights.Count > 0)
		{
			LightBulbController lightBulbController = m_Lights[0];
			lightBulbController.OnTurnedOn += HandleLightTurnedOn;
			lightBulbController.TurnOn();
			S13AudioManager.Instance.PlayAudio("sfx_lightbulb_detail");
		}
	}

	private void HandleLightTurnedOn(object sender, EventArgs e)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		LightBulbController lightBulbController = (LightBulbController)sender;
		lightBulbController.OnTurnedOn -= HandleLightTurnedOn;
		if (m_Lights.Contains(lightBulbController))
		{
			m_Lights.Remove(lightBulbController);
		}
		if (m_Lights.Count <= 0)
		{
			m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
			m_ExitTrigger.SetActive(active: true);
		}
		else if (m_Lights.Count == 2)
		{
			GameManager.Instance.GameCamera.VisionEffect.OnStart += HandleVisionEffectOnStart;
			GameManager.Instance.GameCamera.VisionEffect.OnStop += HandleVisionEffectOnStop;
			GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 4f, (TweenCallback)delegate
			{
				GameManager.Instance.GameCamera.VisionEffect.EndEffect();
			});
		}
	}

	private void HandleVisionEffectOnStart(object sender, EventArgs e)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		GameManager.Instance.GameCamera.VisionEffect.OnStart -= HandleVisionEffectOnStart;
		m_TempSafes = GameManager.Instance.AudioManager.Play(m_TempSafesClip);
		for (int i = 0; i < m_SafeDoors.Count; i++)
		{
			m_SafeDoors[i].OpenAndShut();
		}
		for (int j = 0; j < m_Lanterns.Count; j++)
		{
			m_Lanterns[j].EnableHeavySway(20f);
		}
		m_SwayDelaySequence = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(m_SwayDelaySequence, 25f, (TweenCallback)delegate
		{
			for (int k = 0; k < m_Lanterns.Count; k++)
			{
				m_Lanterns[k].DisableSway();
			}
		});
	}

	private void HandleVisionEffectOnStop(object sender, EventArgs e)
	{
		GameManager.Instance.GameCamera.VisionEffect.OnStop -= HandleVisionEffectOnStop;
		m_TempSafes.Stop();
		for (int i = 0; i < m_SafeDoors.Count; i++)
		{
			m_SafeDoors[i].ResetDoor();
		}
		for (int j = 0; j < m_Lanterns.Count; j++)
		{
			m_Lanterns[j].ResetSway();
		}
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		GameManager.Instance.AudioManager.PlayAtPosition(m_tempDoor, AccountingExitDoor.transform.position);
		AccountingExitDoor.OnOpen += HandleAccountingExitDoorOnOpen;
		AccountingExitDoor.Open(1.6f);
	}

	private void HandleAccountingExitDoorOnOpen(object sender, EventArgs e)
	{
		AccountingExitDoor.OnOpen -= HandleAccountingExitDoorOnOpen;
		TweenExtensions.Kill((Tween)(object)m_SwayDelaySequence, false);
		for (int i = 0; i < m_Lanterns.Count; i++)
		{
			m_Lanterns[i].DisableSway();
		}
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_ENTER_THE_DARKNESS", string.Empty, 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.AccountingObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		TweenExtensions.Kill((Tween)(object)m_SwayDelaySequence, false);
		TweenExtensions.Kill((Tween)(object)m_HatchDoorSequence, false);
		m_TempSafes = null;
		m_TempSafesClip = null;
		m_tempDoor = null;
		m_ValvePlaceClip = null;
		m_HatchDoorClip = null;
		m_ValveTurnClip = null;
		m_HenryClip01 = null;
		m_HenryClip02 = null;
		base.OnDisposed();
	}
}
