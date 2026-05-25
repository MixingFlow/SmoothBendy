using System;
using DG.Tweening;
using UnityEngine;

public class CH5BendyArena : BaseController
{
	[SerializeField]
	private EventTrigger m_MazeExitTrigger;

	[SerializeField]
	private Ch5BeastBendyChargeController m_BendyChargeController;

	[SerializeField]
	private GenericDoorController m_PillarRoomEntranceDoor;

	[SerializeField]
	private GameObject m_PillarRoomEntranceBrokenDoor;

	[SerializeField]
	private CH5ArenaPillar[] m_Pillars;

	[SerializeField]
	private GameObject m_Lights;

	[SerializeField]
	private LightFixtureController[] m_LightFixtures;

	[SerializeField]
	private GenericDoorController m_PillarRoomExitDoor;

	[SerializeField]
	private GenericDoorController[] m_HallwayDoors;

	[SerializeField]
	private InteractablePowerLever[] m_HallwayLevers;

	[Header("Final Bendy")]
	[SerializeField]
	private BeastBendy_Ai m_Bendy;

	[SerializeField]
	private CH5PowerStation m_PowerStation;

	private AudioClip m_LeverClip;

	private AudioClip m_InkDemonMusic;

	private AudioObject m_MusicObject;

	private int m_DoorIndex;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_PillarRoomEntranceBrokenDoor.SetActive(false);
		m_Bendy.gameObject.SetActive(false);
		m_MazeExitTrigger.SetActive(active: false);
		for (int i = 0; i < m_HallwayLevers.Length; i++)
		{
			m_HallwayLevers[i].SetActive(active: false);
		}
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_InkDemonMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheInkDemon");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.BendyArenaObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.BendyArenaObjective.IsStarted)
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		for (int i = 0; i < m_HallwayLevers.Length; i++)
		{
			InteractablePowerLever interactablePowerLever = m_HallwayLevers[i];
			interactablePowerLever.OnInteracted += HandleLeverOnInteracted;
			interactablePowerLever.OnComplete += HandleLeverOnComplete;
			interactablePowerLever.SetActive(active: true);
		}
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_InkDemonMusic, AudioObjectType.MUSIC, -1);
		m_MusicObject.AudioSource.volume = 0f;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_MusicObject.AudioSource.DOFade(1f, 1f), (Ease)1), new TweenCallback(m_BendyChargeController.Activate));
	}

	private void ForceStart()
	{
	}

	private void ForceComplete()
	{
		SendOnComplete();
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		InteractablePowerLever interactablePowerLever = sender as InteractablePowerLever;
		interactablePowerLever.OnInteracted -= HandleLeverOnInteracted;
		GameManager.Instance.AudioManager.PlayAtPosition(m_LeverClip, interactablePowerLever.transform.position);
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		InteractablePowerLever interactablePowerLever = sender as InteractablePowerLever;
		interactablePowerLever.OnComplete -= HandleLeverOnComplete;
		m_HallwayDoors[m_DoorIndex].Open();
		m_DoorIndex++;
		if (m_DoorIndex >= m_HallwayDoors.Length)
		{
			m_MazeExitTrigger.OnEnter += HandleMazeExitTriggerOnEnter;
			m_MazeExitTrigger.SetActive(active: true);
		}
	}

	private void HandleMazeExitTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		m_MazeExitTrigger.OnEnter -= HandleMazeExitTriggerOnEnter;
		Sequence val = DOTween.Sequence();
		float num = 0.5f;
		for (int i = 0; i < m_HallwayDoors.Length; i++)
		{
			TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_HallwayDoors[i].Close));
			num += 0.15f;
		}
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_PillarRoomEntranceDoor.Open));
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)m_MusicObject))
				{
					m_MusicObject.Clear();
					m_MusicObject = null;
				}
			});
		}
		m_BendyChargeController.Deactivate();
		m_PowerStation.OnStartUpComplete += HandleOnStartUpComplete;
		m_PowerStation.OnPowerShutDown += HandlePowerStationOnPowerShutDown;
		m_PowerStation.Activate();
	}

	private void HandleOnStartUpComplete(object sender, EventArgs e)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		m_PowerStation.OnStartUpComplete -= HandleOnStartUpComplete;
		m_PillarRoomEntranceDoor.Close();
		for (int i = 0; i < m_Pillars.Length; i++)
		{
			m_Pillars[i].OnHit += HandlePillarOnHit;
			m_Pillars[i].TurnOn();
		}
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_InkDemonMusic, AudioObjectType.MUSIC, -1);
		m_MusicObject.AudioSource.volume = 0f;
		TweenSettingsExtensions.SetEase<Tweener>(m_MusicObject.AudioSource.DOFade(1f, 0.5f), (Ease)1);
		m_Bendy.gameObject.SetActive(true);
		Vector3 val = m_Bendy.transform.position + Vector3.up * 3f;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(GameManager.Instance.GameCamera.FreeRoamCam, val, 0.5f, (AxisConstraint)0, (Vector3?)null), (Ease)7), new TweenCallback(GameManager.Instance.GameCamera.ExitFreeRoamCam));
	}

	private void HandlePillarOnHit(object sender, EventArgs e)
	{
		(sender as CH5ArenaPillar).OnHit -= HandlePillarOnHit;
		m_PowerStation.PowerDown();
	}

	private void HandlePowerStationOnPowerShutDown(object sender, EventArgs e)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		m_PowerStation.OnPowerShutDown -= HandlePowerStationOnPowerShutDown;
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			m_MusicObject.Clear();
			m_MusicObject = null;
		}
		RenderSettings.ambientIntensity = 0f;
		m_Bendy.gameObject.SetActive(false);
		m_Lights.SetActive(false);
		for (int i = 0; i < m_LightFixtures.Length; i++)
		{
			m_LightFixtures[i].TurnOff();
		}
		m_PillarRoomEntranceDoor.gameObject.SetActive(false);
		m_PillarRoomEntranceBrokenDoor.SetActive(true);
		m_PillarRoomExitDoor.Open();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 4f, (TweenCallback)delegate
		{
			DOTweenUtil.DOAmbientLightColor(1f, 9f);
		});
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_LeverClip = null;
		base.OnDisposed();
	}
}
