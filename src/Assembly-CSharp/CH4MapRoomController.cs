using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4MapRoomController : BaseController
{
	[Header("Objective: Open Door")]
	[SerializeField]
	private Interactable m_DoorLever;

	[SerializeField]
	private GenericDoorController m_WarehouseDoor;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[Header("Darkness")]
	[SerializeField]
	private GameObject m_Darkness;

	[SerializeField]
	private EventTrigger m_DarknessHideTrigger;

	[SerializeField]
	private EventTrigger m_DarknessShowTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_LeverClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_DoorLever.SetActive(active: false);
		m_ExitTrigger.SetActive(active: false);
		m_DarknessHideTrigger.SetActive(active: false);
		m_DarknessShowTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MapRoomObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_DarknessShowTrigger.OnEnter += HandleDarknessShowTriggerOnEnter;
		m_DarknessHideTrigger.OnEnter += HandleDarknessHideTriggerOnEnter;
		m_DarknessHideTrigger.SetActive(active: true);
		m_DoorLever.OnInteracted += HandleDoorLeverOnInteracted;
		m_DoorLever.SetActive(active: true);
	}

	private void ForceComplete()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_06");
		Transform obj = m_DoorLever.transform;
		obj.localEulerAngles += new Vector3(80f, 0f, 0f);
		m_DarknessHideTrigger.Dispose();
		m_DarknessShowTrigger.Dispose();
		m_Darkness.SetActive(false);
		m_WarehouseDoor.ForceOpen();
		SendOnComplete();
	}

	private void HandleDarknessHideTriggerOnEnter(object sender, EventArgs e)
	{
		m_DarknessHideTrigger.SetActive(active: false);
		m_DarknessShowTrigger.SetActive(active: true);
		m_Darkness.SetActive(false);
	}

	private void HandleDarknessShowTriggerOnEnter(object sender, EventArgs e)
	{
		m_DarknessShowTrigger.SetActive(active: false);
		m_DarknessHideTrigger.SetActive(active: true);
		m_Darkness.SetActive(true);
	}

	private void HandleDoorLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		m_DoorLever.OnInteracted -= HandleDoorLeverOnInteracted;
		m_DoorLever.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_LeverClip);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorLever.transform, new Vector3(-80f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)27);
		m_WarehouseDoor.Open();
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: true);
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_Darkness.SetActive(false);
		m_DarknessHideTrigger.OnEnter -= HandleDarknessHideTriggerOnEnter;
		m_DarknessHideTrigger.SetActive(active: false);
		m_DarknessShowTrigger.OnEnter -= HandleDarknessShowTriggerOnEnter;
		m_DarknessShowTrigger.SetActive(active: false);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MapRoomObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_DoorLever.OnInteracted -= HandleDoorLeverOnInteracted;
		m_LeverClip = null;
		base.OnDisposed();
	}
}
