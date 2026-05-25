using System;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using S13Audio;
using TMPro;
using UnityEngine;

public class CH1MainPowerController : BaseController
{
	[Header("<Controller>")]
	[SerializeField]
	private MeatlyController m_MeatlyController;

	[SerializeField]
	private List<CH1Pedestal> m_Pedestals;

	[Header("Objective: Turn On Ink Machine")]
	[SerializeField]
	private InkMachineController m_InkMachineController;

	[SerializeField]
	private List<GameObject> m_ActiveGameObjects;

	[SerializeField]
	private TextMeshPro m_ScreenLbl;

	[SerializeField]
	private InteractablePowerLever m_Lever;

	[SerializeField]
	private List<InkPipeController> m_InkPipes;

	[SerializeField]
	private LightController m_MainPowerLight;

	[SerializeField]
	private LightFixtureController m_HallwayLightController;

	[SerializeField]
	private GameObject m_HallwayLights;

	[SerializeField]
	private BaseDoorController m_BreakRoomDoor;

	private AudioClip m_LeverClip;

	private AudioClip m_LightClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_Lever.SetActive(active: false);
		for (int i = 0; i < m_ActiveGameObjects.Count; i++)
		{
			m_ActiveGameObjects[i].SetActive(false);
		}
		m_MainPowerLight.TurnOff();
		SetScreenLabel("INWORLD/LOW PRESSURE");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineObjective.IsStarted)
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
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_05", string.Empty, 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		InternalInitialize();
	}

	private void InternalInitialize()
	{
		SetScreenLabel("INWORLD/PRESSURE_READY");
		m_Lever.OnComplete += HandleLeverOnComplete;
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.SetActive(active: true);
	}

	private void ForceStart()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_05", string.Empty));
		InternalInitialize();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH1_save_point_04");
		SetScreenLabel("INWORLD/PRESSURE_RUNNING", 0.25f, 0.15f);
		for (int i = 0; i < m_ActiveGameObjects.Count; i++)
		{
			m_ActiveGameObjects[i].SetActive(true);
		}
		for (int j = 0; j < m_InkPipes.Count; j++)
		{
			m_InkPipes[j].TurnOn();
		}
		for (int k = 0; k < m_Pedestals.Count; k++)
		{
			m_Pedestals[k].TurnLightOff();
		}
		m_InkMachineController.TurnOn();
		m_MainPowerLight.TurnOn();
		m_HallwayLightController.TurnOff();
		m_HallwayLights.SetActive(false);
		m_MeatlyController.Activate();
		m_BreakRoomDoor.ForceClose();
		m_BreakRoomDoor.Lock();
		m_Lever.ForceOpen();
		SendOnComplete();
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_LeverClip);
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		m_Lever.OnComplete -= HandleLeverOnComplete;
		GameManager.Instance.AudioManager.Play(m_LightClip);
		S13AudioManager.Instance.InvokeEvent("evt_main_power_switch_activated");
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		ForceComplete();
	}

	private void SetScreenLabel(string label, float duration = 0.5f, float delay = 0.25f, bool toUpper = true)
	{
		ShortcutExtensions.DOKill((Component)(object)m_ScreenLbl, false);
		string Translation = string.Empty;
		if (LocalizationManager.TryGetTranslation(label, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			label = Translation;
		}
		if (toUpper)
		{
			label = label.ToUpper();
		}
		m_ScreenLbl.text = label;
		m_ScreenLbl.alpha = 1f;
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(m_ScreenLbl.DOFade(0.25f, duration), delay), -1, (LoopType)1), (Ease)7);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_ScreenLbl, false);
		m_LeverClip = null;
		m_LightClip = null;
		base.OnDisposed();
	}
}
