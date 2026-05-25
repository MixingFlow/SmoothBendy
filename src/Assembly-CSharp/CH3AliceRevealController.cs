using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3AliceRevealController : BaseController
{
	[Header("Alice Angel Intro")]
	[SerializeField]
	private Transform m_FinalPosition;

	[SerializeField]
	private EventTrigger m_IntroTrigger;

	[SerializeField]
	private GameObject m_ExitBlocker;

	[SerializeField]
	private BaseDoorController m_DoorController;

	[SerializeField]
	private DisposableObject m_AliceProps;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_CompleteDisable;

	[SerializeField]
	private GameObject m_CompleteEnable;

	[SerializeField]
	private GameObject m_AliceEnable;

	[Header("Lighting")]
	[SerializeField]
	private GameObject m_SignSpotlight;

	[SerializeField]
	private Light m_DoorSpotlight;

	[SerializeField]
	private GameObject m_EnvironmentLighting;

	[SerializeField]
	private GameObject m_TelevisionLighting;

	[Header("Televisions")]
	[SerializeField]
	private List<CH3Television> m_Televisions;

	[Header("Light Fixtures")]
	[SerializeField]
	private List<LightFixtureController> m_LightFixtures;

	[SerializeField]
	private MeshRenderer m_Glass;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_AliceMusic;

	private AudioClip m_AliceJumpscare;

	private AudioClip m_LightClip;

	private AudioClip m_AliceAmbienceClip;

	private AudioClip[] m_AliceRevealClips;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AliceMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_aaintroductionsong");
		m_AliceJumpscare = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_aajumpscare");
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_AliceAmbienceClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_angelicambience");
		m_AliceRevealClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueReveal/");
		m_AliceEnable.SetActive(false);
		m_ExitBlocker.SetActive(false);
		m_CompleteEnable.SetActive(false);
		m_TelevisionLighting.SetActive(false);
		((Behaviour)m_DoorSpotlight).enabled = false;
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceRevealObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			m_IntroTrigger.OnEnter += HandleIntroTriggerOnEnter;
		}
	}

	private void HandleIntroTriggerOnEnter(object sender, EventArgs e)
	{
		m_IntroTrigger.OnEnter -= HandleIntroTriggerOnEnter;
		m_DoorController.Close();
		m_DoorController.Lock();
		GameManager.Instance.AudioManager.Play(m_LightClip);
		m_EnvironmentLighting.SetActive(false);
		m_ExitBlocker.SetActive(true);
		RenderSettings.ambientIntensity = 0f;
		BeginJumpscare();
		S13AudioManager.Instance.InvokeEvent("evt_alice_reveal_start");
		GameManager.Instance.AudioManager.Play(m_AliceMusic).OnComplete += HandleAliceIntroMusicOnComplete;
	}

	private Sequence BeginJumpscare()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 31f;
		float num2 = 2f;
		for (int i = 0; i < m_Televisions.Count; i++)
		{
			CH3Television cH3Television = m_Televisions[i];
			TweenSettingsExtensions.InsertCallback(val, num2, new TweenCallback(cH3Television.Play));
			num2 += 0.15f;
		}
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			((Behaviour)m_DoorSpotlight).enabled = true;
			GameManager.Instance.AudioManager.Play(m_LightClip);
			for (int j = 0; j < m_LightFixtures.Count; j++)
			{
				m_LightFixtures[j].TurnOff();
			}
		});
		return val;
	}

	private void HandleAliceIntroMusicOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleAliceIntroMusicOnComplete;
		m_DoorSpotlight.intensity = 2f;
		m_AliceEnable.SetActive(true);
		m_TelevisionLighting.SetActive(true);
		m_SignSpotlight.SetActive(true);
		for (int i = 0; i < m_Televisions.Count; i++)
		{
			m_Televisions[i].Stop();
		}
		ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 3.5f, 0.1f, 10, 90f, false, true);
		GameManager.Instance.ShowScreenBlocker(0f, 3.65f, delegate
		{
			GameManager.Instance.Player.SetLock(active: true);
		});
		GameManager.Instance.AudioManager.Play(m_AliceJumpscare).OnComplete += HandleAliceJumpscareOnComplete;
	}

	private void HandleAliceJumpscareOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleAliceJumpscareOnComplete;
		GameManager.Instance.AudioManager.Play(m_AliceAmbienceClip);
		for (int i = 0; i < m_AliceRevealClips.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceRevealClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_REVEAL[i], isTrimmed: true));
			if (i == m_AliceRevealClips.Length - 1)
			{
				audioObject.OnComplete += HandleRevealDialogueOnComplete;
			}
		}
	}

	private void HandleRevealDialogueOnComplete(object sender, EventArgs e)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		(sender as AudioObject).OnComplete -= HandleRevealDialogueOnComplete;
		GameManager.Instance.HideScreenBlocker(1f);
		DOTweenUtil.DOAmbientLightColor(1f, 2f);
		GameManager.Instance.Player.transform.position = m_FinalPosition.position;
		GameManager.Instance.Player.LookRotation(m_FinalPosition.rotation, Quaternion.identity);
		GameManager.Instance.Player.SetLock(active: false);
		((Renderer)m_Glass).material.SetFloat("_IsBroken", 1f);
		m_AliceProps.Dispose();
		m_AliceEnable.SetActive(false);
		m_CompleteDisable.SetActive(false);
		m_CompleteEnable.SetActive(true);
		m_TelevisionLighting.SetActive(false);
		m_EnvironmentLighting.SetActive(true);
		for (int i = 0; i < m_LightFixtures.Count; i++)
		{
			m_LightFixtures[i].TurnOn();
		}
		m_DoorController.Unlock();
		m_DoorController.Open(1f, (Ease)6, 145f);
		((Behaviour)m_DoorSpotlight).enabled = false;
		m_ExitBlocker.SetActive(false);
		S13AudioManager.Instance.InvokeEvent("evt_alice_reveal_complete");
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty, 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceRevealObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_04");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		((Renderer)m_Glass).material.SetFloat("_IsBroken", 1f);
		m_AliceProps.Dispose();
		m_AliceEnable.SetActive(false);
		m_CompleteDisable.SetActive(false);
		m_CompleteEnable.SetActive(true);
		m_TelevisionLighting.SetActive(false);
		m_EnvironmentLighting.SetActive(true);
		for (int i = 0; i < m_LightFixtures.Count; i++)
		{
			m_LightFixtures[i].TurnOn();
		}
		m_DoorController.Unlock();
		m_DoorController.Open(1f, (Ease)6, 145f);
		((Behaviour)m_DoorSpotlight).enabled = false;
		m_ExitBlocker.SetActive(false);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_AliceMusic = null;
		m_AliceJumpscare = null;
		m_LightClip = null;
		m_AliceAmbienceClip = null;
		m_AliceRevealClips = null;
		base.OnDisposed();
	}
}
