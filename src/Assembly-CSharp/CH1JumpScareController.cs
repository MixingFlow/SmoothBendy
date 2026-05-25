using System;
using DG.Tweening;
using UnityEngine;

public class CH1JumpScareController : BaseController
{
	[Header("Scare: Falling Plank")]
	[SerializeField]
	private Transform m_Plank;

	[SerializeField]
	private Transform m_EndPosition;

	[SerializeField]
	private EventTrigger m_PlankEventTrigger;

	[SerializeField]
	private EventTrigger m_PlankExitTrigger;

	[Header("Scare: Bendy Cutout")]
	[SerializeField]
	private GameObject m_Cutout;

	[SerializeField]
	private EventTrigger m_JumpScareTrigger;

	[SerializeField]
	private EventTrigger m_DestroyTrigger;

	[SerializeField]
	private EventTrigger m_DialogueTrigger;

	[Header("Scare: Behind The Door")]
	[SerializeField]
	private Transform m_BendyDoorCutout;

	[SerializeField]
	private Transform m_BendyDoorCutoutEndPosition;

	[SerializeField]
	private CustomDoorController m_BendyDoor;

	private AudioClip m_PlankFallClip;

	private AudioClip m_JumpscareClip;

	private AudioClip m_HenryClip10;

	public override void Init()
	{
		base.Init();
		m_BendyDoor.Lock();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsStarted)
		{
			((Component)m_Plank).gameObject.SetActive(false);
			return;
		}
		m_PlankFallClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_WoodPlankFall");
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Jumpscare_01");
		m_HenryClip10 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_10");
		m_PlankEventTrigger.SetActive(active: false);
		m_PlankExitTrigger.SetActive(active: false);
		m_Cutout.SetActive(false);
		m_JumpScareTrigger.SetActive(active: false);
		m_DestroyTrigger.SetActive(active: false);
		m_DialogueTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete)
		{
			ActivateBendyDoor();
		}
		else
		{
			ActivatePlankScare();
		}
	}

	private void ActivatePlankScare()
	{
		m_PlankEventTrigger.SetActive(active: true);
		m_PlankEventTrigger.OnEnter += HandleEventTriggerOnEnter;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_PlankEventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		TweenSettingsExtensions.OnComplete<Sequence>(DOPlankScare(), new TweenCallback(PlankScareOnComplete));
	}

	private Sequence DOPlankScare()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Plank, m_EndPosition.position, 1f, false), (Ease)30, 0.025f));
		num += 0.4f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_PlankFallClip);
		});
		return val;
	}

	private void PlankScareOnComplete()
	{
		m_PlankExitTrigger.SetActive(active: true);
		m_PlankExitTrigger.OnEnter += HandlePlankExitTriggerOnEnter;
	}

	private void HandlePlankExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_PlankExitTrigger.OnEnter -= HandlePlankExitTriggerOnEnter;
		ActivateCutoutScare();
	}

	private void ActivateCutoutScare()
	{
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete)
		{
			((Component)m_Plank).gameObject.SetActive(false);
			m_Cutout.SetActive(true);
			m_JumpScareTrigger.SetActive(active: true);
			m_JumpScareTrigger.OnEnter += HandleJumpScareTriggerOnEnter;
			m_DialogueTrigger.SetActive(active: true);
			m_DialogueTrigger.OnEnter += HandleDialogueTriggerOnEnter;
		}
	}

	private void HandleJumpScareTriggerOnEnter(object sender, EventArgs e)
	{
		m_JumpScareTrigger.OnEnter -= HandleJumpScareTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		m_DestroyTrigger.SetActive(active: true);
		m_DestroyTrigger.OnEnter += HandleDestroyTriggerOnEnter;
	}

	private void HandleDialogueTriggerOnEnter(object sender, EventArgs e)
	{
		m_DialogueTrigger.OnEnter -= HandleDialogueTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip10, "DIACH1/DIA_CH1_HENRY_10"));
	}

	private void HandleDestroyTriggerOnEnter(object sender, EventArgs e)
	{
		m_DestroyTrigger.OnEnter -= HandleDestroyTriggerOnEnter;
		m_Cutout.SetActive(false);
		ActivateBendyDoor();
	}

	private void ActivateBendyDoor()
	{
		m_BendyDoor.Unlock();
		m_BendyDoor.DoorSpeed = 0.35f;
		m_BendyDoor.OnInteract += HandleBendyDoorOnInteract;
	}

	private void HandleBendyDoorOnInteract(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_BendyDoor.OnInteract -= HandleBendyDoorOnInteract;
		TweenSettingsExtensions.OnComplete<Sequence>(DOBendyDoor(), new TweenCallback(base.SendOnComplete));
	}

	private Sequence DOBendyDoor()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0.15f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_BendyDoorCutout, m_BendyDoorCutoutEndPosition.localPosition, num2, false), (Ease)1));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_BendyDoorCutout, m_BendyDoorCutoutEndPosition.localEulerAngles, num2, (RotateMode)0), (Ease)1));
		num += num2 / 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		});
		TweenSettingsExtensions.InsertCallback(val, num + 0.25f, (TweenCallback)delegate
		{
		});
		return val;
	}

	protected override void OnDisposed()
	{
		m_PlankFallClip = null;
		m_BendyDoor.OnInteract -= HandleBendyDoorOnInteract;
		if ((Object)(object)m_PlankExitTrigger != (Object)null)
		{
			m_PlankExitTrigger.OnEnter -= HandlePlankExitTriggerOnEnter;
		}
		if ((Object)(object)m_PlankEventTrigger != (Object)null)
		{
			m_PlankEventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		if ((Object)(object)m_JumpScareTrigger != (Object)null)
		{
			m_JumpScareTrigger.OnEnter += HandleJumpScareTriggerOnEnter;
		}
		if ((Object)(object)m_DialogueTrigger != (Object)null)
		{
			m_DialogueTrigger.OnEnter += HandleDialogueTriggerOnEnter;
		}
		if ((Object)(object)m_DestroyTrigger != (Object)null)
		{
			m_DestroyTrigger.OnEnter -= HandleDestroyTriggerOnEnter;
		}
		m_PlankFallClip = null;
		m_JumpscareClip = null;
		m_HenryClip10 = null;
		base.OnDisposed();
	}
}
