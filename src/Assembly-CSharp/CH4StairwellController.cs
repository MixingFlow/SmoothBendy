using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4StairwellController : BaseController
{
	[Header("Stairwell")]
	[SerializeField]
	private BaseDoorController m_DoorController;

	[SerializeField]
	private EventTrigger m_EntranceTrigger;

	[SerializeField]
	private Transform m_DoorEntrance;

	[SerializeField]
	private GameObject m_FunnelCollider;

	[SerializeField]
	private GameObject m_DoorBlockage;

	[SerializeField]
	private OcclusionPortal m_DoorPortal;

	[SerializeField]
	private GameObject m_ArmsTemp;

	[SerializeField]
	private List<Animator> m_Arms;

	[SerializeField]
	private EventTrigger m_AliceDialogueTrigger;

	[SerializeField]
	private InkMachineLoopController m_InkMachineLoop;

	[Header("Wandering Lost One")]
	[SerializeField]
	private CH4LostOneCrazy m_CrazyLostOne;

	[SerializeField]
	private EventTrigger m_LostOneTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip[] m_DialogueAliceSpiralStairClips;

	private AudioClip m_LostOneMusicClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_DialogueAliceSpiralStairClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Alice/SpiralStairs");
		m_LostOneMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_TheLostOnes");
		m_ArmsTemp.SetActive(false);
		m_DoorBlockage.SetActive(false);
		m_EntranceTrigger.SetActive(active: false);
		m_AliceDialogueTrigger.SetActive(active: false);
		m_LostOneTrigger.SetActive(active: false);
		m_InkMachineLoop.gameObject.SetActive(false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.LostOnesObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_DoorController.OnInteracted += HandleOnDoorOpen;
		m_EntranceTrigger.OnEnter += HandleEntranceTriggerOnEnter;
		m_EntranceTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_03");
		m_DoorController.ForceOpen(145f);
		m_DoorController.Lock();
		m_InkMachineLoop.gameObject.SetActive(true);
		m_InkMachineLoop.Activate();
		((Component)m_DoorEntrance).gameObject.SetActive(false);
		m_DoorBlockage.SetActive(true);
		m_DoorPortal.open = false;
		SendOnComplete();
	}

	private void HandleEntranceTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		m_EntranceTrigger.OnEnter += HandleEntranceTriggerOnEnter;
		GameManager.Instance.GameCamera.VisionEffect.OnStop += HandleVisionEffectOnStop;
		GameManager.Instance.GameCamera.VisionEffect.OnStart += HandleVisionEffectOnStart;
		GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 6f, (TweenCallback)delegate
		{
			GameManager.Instance.GameCamera.VisionEffect.EndEffect();
		});
		m_AliceDialogueTrigger.OnEnter += HandleAliceDialogueTriggerOnEnter;
		m_AliceDialogueTrigger.SetActive(active: true);
	}

	private void HandleVisionEffectOnStart(object sender, EventArgs e)
	{
		GameManager.Instance.GameCamera.VisionEffect.OnStart -= HandleVisionEffectOnStart;
		ShortcutExtensions.DOKill((Component)(object)m_DoorEntrance, false);
		((Component)m_DoorEntrance).gameObject.SetActive(false);
		m_DoorBlockage.SetActive(true);
		m_DoorPortal.open = false;
		GameManager.Instance.Player.SetSlowed(active: true);
		m_ArmsTemp.SetActive(true);
		for (int i = 0; i < m_Arms.Count; i++)
		{
			m_Arms[i].speed = Random.Range(0.85f, 1f);
			m_Arms[i].Play("Arm", 0, Random.Range(0f, 1f));
		}
	}

	private void HandleVisionEffectOnStop(object sender, EventArgs e)
	{
		GameManager.Instance.GameCamera.VisionEffect.OnStop -= HandleVisionEffectOnStop;
		m_InkMachineLoop.gameObject.SetActive(true);
		m_InkMachineLoop.Activate();
		m_ArmsTemp.SetActive(false);
		m_FunnelCollider.SetActive(false);
		GameManager.Instance.Player.SetSlowed(active: false);
	}

	private void HandleAliceDialogueTriggerOnEnter(object sender, EventArgs e)
	{
		m_AliceDialogueTrigger.OnEnter -= HandleAliceDialogueTriggerOnEnter;
		for (int i = 0; i < m_DialogueAliceSpiralStairClips.Length; i++)
		{
			AudioClip dialogue = m_DialogueAliceSpiralStairClips[i];
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(dialogue, SubtitleConstants.DIALOGUE_CH4_ALICE_SPIRAL_STAIRS[i], isTrimmed: true));
		}
		m_LostOneTrigger.OnEnter += HandleLostOneTriggerOnEnter;
		m_LostOneTrigger.SetActive(active: true);
	}

	private void HandleLostOneTriggerOnEnter(object sender, EventArgs e)
	{
		m_LostOneTrigger.OnEnter -= HandleLostOneTriggerOnEnter;
		m_CrazyLostOne.Activate();
	}

	private void HandleOnDoorOpen(object sender, EventArgs e)
	{
		m_DoorController.OnInteracted -= HandleOnDoorOpen;
		GameManager.Instance.AudioManager.Play(m_LostOneMusicClip, AudioObjectType.MUSIC);
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.LostOnesObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_DialogueAliceSpiralStairClips = null;
		m_LostOneMusicClip = null;
		base.OnDisposed();
	}
}
