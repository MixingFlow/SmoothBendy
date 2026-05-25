using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3BorisJumpscareController : BaseController
{
	[Header("Boris")]
	[SerializeField]
	private BorisAi m_Boris;

	[SerializeField]
	private MeleeWeapon m_Pipe;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_EnableGameObjects;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_Cutout;

	[SerializeField]
	private Transform m_StartCutoutPosition;

	[SerializeField]
	private Transform m_ScareCutoutPosition;

	[SerializeField]
	private Transform m_EndCutoutPosition;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_JumpscareTrigger;

	[SerializeField]
	private EventTrigger m_AudioTrigger;

	[Header("Doors")]
	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private BaseDoorController m_PreviousDoor;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private MeleeWeapon m_InstancedPipe;

	private AudioClip m_JumpscareClip;

	private AudioClip m_Henry24Clip;

	private AudioClip[] m_HenryBorisScareClip;

	public override void InitOnComplete()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Jumpscare_01");
		m_Henry24Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_24_thiswilldo");
		m_HenryBorisScareClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/BorisScare/");
		m_Door.Lock();
		m_Boris = GameManager.Instance.CharacterManager.Boris;
		m_Cutout.position = m_StartCutoutPosition.position;
		((Component)m_Cutout).gameObject.SetActive(false);
		m_JumpscareTrigger.SetActive(active: false);
		m_AudioTrigger.SetActive(active: false);
		m_EnableGameObjects.SetActive(false);
	}

	public override void Activate()
	{
		m_InstancedPipe = Object.Instantiate<MeleeWeapon>(m_Pipe);
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.BorisJumpscareObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		m_InstancedPipe.gameObject.SetActive(true);
		m_InstancedPipe.transform.SetParent(m_Boris.PipeHand);
		m_InstancedPipe.transform.localPosition = Vector3.zero;
		m_InstancedPipe.transform.localEulerAngles = Vector3.zero;
		m_InstancedPipe.Interaction.SetActive(active: false);
		m_JumpscareTrigger.OnEnter += HandleJumpscareTriggerOnEnter;
		m_JumpscareTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_06");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		m_Door.ForceOpen(145f);
		m_Door.Lock();
		m_PreviousDoor.ForceOpen(145f);
		m_PreviousDoor.Lock();
		GameManager.Instance.Player.WeaponGameObject = m_InstancedPipe.gameObject;
		m_InstancedPipe.gameObject.SetActive(true);
		m_InstancedPipe.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		m_InstancedPipe.transform.localPosition = Vector3.zero;
		m_InstancedPipe.transform.localEulerAngles = Vector3.zero;
		m_InstancedPipe.Equip();
		m_InstancedPipe.Interaction.SetActive(active: false);
		m_InstancedPipe.Interaction.Dispose();
		m_EnableGameObjects.SetActive(true);
		((Component)m_Cutout).gameObject.SetActive(true);
		m_Cutout.position = m_EndCutoutPosition.position;
		m_Cutout.eulerAngles = m_EndCutoutPosition.eulerAngles;
		SendOnComplete();
	}

	private void HandleJumpscareTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		m_JumpscareTrigger.OnEnter -= HandleJumpscareTriggerOnEnter;
		m_EnableGameObjects.SetActive(true);
		((Component)m_Cutout).gameObject.SetActive(true);
		GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0.225f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Cutout, m_ScareCutoutPosition.position, num2, false), (Ease)27));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Cutout, m_ScareCutoutPosition.eulerAngles, num2, (RotateMode)0), (Ease)27));
		num += 0.825f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Cutout, m_StartCutoutPosition.position, num2, false), (Ease)26));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Cutout, m_StartCutoutPosition.eulerAngles, num2, (RotateMode)0), (Ease)26));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ScareOnComplete));
	}

	private void ScareOnComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Cutout.position = m_EndCutoutPosition.position;
		m_Cutout.eulerAngles = m_EndCutoutPosition.eulerAngles;
		m_AudioTrigger.SetActive(active: true);
		m_AudioTrigger.OnEnter += HandleAudioTriggerOnEnter;
	}

	private void HandleAudioTriggerOnEnter(object sender, EventArgs e)
	{
		m_AudioTrigger.OnEnter -= HandleAudioTriggerOnEnter;
		for (int i = 0; i < m_HenryBorisScareClip.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryBorisScareClip[i], SubtitleConstants.DIA_CH3_HENRY_BORIS_SCARE[i]));
			if (i == 1)
			{
				audioObject.OnComplete += HandleBorisScareOnComplete;
			}
		}
	}

	private void HandleBorisScareOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleBorisScareOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_BORIS_PIPE", "OBJECTIVES/CH3_OBJECTIVE_BORIS_PIPE_TIP", 4f));
		m_InstancedPipe.Interaction.SetActive(active: true);
		m_InstancedPipe.OnEquipped += HandlePipeOnEquipped;
	}

	private void HandlePipeOnEquipped(object sender, EventArgs e)
	{
		m_InstancedPipe.OnEquipped -= HandlePipeOnEquipped;
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry24Clip, "DIACH3/DIA_CH3_HENRY_20"));
		m_Door.OnOpen += HandleDoorOnOpened;
		m_Door.Unlock();
	}

	private void HandleDoorOnOpened(object sender, EventArgs e)
	{
		m_Door.OnOpen -= HandleDoorOnOpened;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.BorisJumpscareObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_JumpscareClip = null;
		m_Henry24Clip = null;
		m_HenryBorisScareClip = null;
		base.OnDisposed();
	}
}
