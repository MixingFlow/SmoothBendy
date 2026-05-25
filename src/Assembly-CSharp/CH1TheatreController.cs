using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH1TheatreController : BaseController
{
	private const string CUTOUT = "_Cutout";

	[Header("Jumpscare: Bendy Cutout")]
	[SerializeField]
	private Transform m_Cutout;

	[SerializeField]
	private Transform m_CutoutStartPosition;

	[SerializeField]
	private Transform m_CutoutScarePosition;

	[SerializeField]
	private Transform m_CutoutEndPosition;

	[SerializeField]
	private List<GameObject> m_EnableGameObjects;

	[SerializeField]
	private EventTrigger m_TheatreEnterEvent;

	[SerializeField]
	private EventTrigger m_TheatreExitEvent;

	[Header("Jumpscare: Projector")]
	[SerializeField]
	private ProjectorController m_ProjectorController;

	[SerializeField]
	private EventTrigger m_TheatreMainEvent;

	[Header("Objective: Ink Flow Button")]
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private List<InkPipeController> m_InkPipes;

	[SerializeField]
	private List<GameObject> m_InkEnableGameObjects;

	[SerializeField]
	private Transform m_RaisingInk;

	[SerializeField]
	private MeshRenderer m_InkRenderer;

	[SerializeField]
	private BasicAnimationController[] m_AnimationControllers;

	private AudioClip m_HenryClip06;

	private AudioClip m_BendyCartoonMusicClip;

	private AudioClip m_TheatreProjectorClip;

	private AudioClip m_FlowValveClip;

	private AudioClip m_DuctCrawlingClip;

	private AudioClip m_JumpscareClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_TheatreMainEvent.SetActive(active: false);
		m_TheatreEnterEvent.SetActive(active: false);
		m_TheatreExitEvent.SetActive(active: false);
		m_Interactable.SetActive(active: false);
		SetInkEnableGameObjectsActive(active: false);
		SetEnableGameObjectsActive(active: false);
		m_HenryClip06 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_06");
		m_BendyCartoonMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH1/MUS_BendyCartoonMusic");
		m_TheatreProjectorClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Projector_Run_With_Film_01");
		m_DuctCrawlingClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Duct_Scare_01");
		m_FlowValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Jumpscare_01");
		((Renderer)m_InkRenderer).material.SetFloat("_Cutout", 0f);
		for (int i = 0; i < m_AnimationControllers.Length; i++)
		{
			m_AnimationControllers[i].Stop();
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.TheatreObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.TheatreObjective.IsStarted)
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
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip06, "DIACH1/DIA_CH1_HENRY_06"));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_04", "OBJECTIVES/CH1_OBJ_04_TIP", 4f));
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.CollectablesObjective.IsComplete = true;
			GameManager.Instance.GameData.CurrentSaveFile.CH1Data.TheatreObjective.IsStarted = true;
			GameManager.Instance.GameDataManager.Save();
		};
		m_TheatreEnterEvent.OnEnter += HandleTheatreEnterOnEnter;
		m_TheatreEnterEvent.SetActive(active: true);
	}

	private void ForceStart()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_04", "OBJECTIVES/CH1_OBJ_04_TIP"));
		m_TheatreEnterEvent.OnEnter += HandleTheatreEnterOnEnter;
		m_TheatreEnterEvent.SetActive(active: true);
	}

	private void ForceComplete()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH1_save_point_03");
		((Component)m_RaisingInk).tag = "DeepInk";
		Vector3 localPosition = m_RaisingInk.localPosition;
		localPosition.y += 1f;
		m_RaisingInk.localPosition = localPosition;
		((Renderer)m_InkRenderer).material.SetFloat("_Cutout", 1f);
		for (int i = 0; i < m_AnimationControllers.Length; i++)
		{
			m_AnimationControllers[i].Play();
		}
		for (int j = 0; j < m_InkPipes.Count; j++)
		{
			m_InkPipes[j].TurnOn();
		}
		SetInkEnableGameObjectsActive(active: true);
		SetEnableGameObjectsActive(active: true);
		m_Cutout.localPosition = m_CutoutEndPosition.localPosition;
		m_Cutout.localRotation = m_CutoutEndPosition.localRotation;
		Vector3 localPosition2 = m_Interactable.transform.localPosition;
		localPosition2.z -= 0.15f;
		m_Interactable.transform.localPosition = localPosition2;
		m_ProjectorController.TurnOn(isSilent: true);
		m_ProjectorController.FilmAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_TheatreProjectorClip, m_ProjectorController.AudioPosition.position, AudioObjectType.MUSIC, -1);
		SendOnComplete();
	}

	private void SetEnableGameObjectsActive(bool active)
	{
		for (int i = 0; i < m_EnableGameObjects.Count; i++)
		{
			m_EnableGameObjects[i].SetActive(active);
		}
	}

	private void SetInkEnableGameObjectsActive(bool active)
	{
		for (int i = 0; i < m_InkEnableGameObjects.Count; i++)
		{
			m_InkEnableGameObjects[i].SetActive(active);
		}
	}

	private void HandleTheatreEnterOnEnter(object sender, EventArgs e)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		m_TheatreEnterEvent.OnEnter -= HandleTheatreEnterOnEnter;
		GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		SetEnableGameObjectsActive(active: true);
		ShortcutExtensions.DOShakePosition(((Component)Camera.main).transform, 0.3f, 0.1f, 15, 90f, false, true);
		m_Cutout.localPosition = m_CutoutStartPosition.localPosition;
		m_Cutout.localRotation = m_CutoutStartPosition.localRotation;
		TweenSettingsExtensions.OnComplete<Sequence>(DOCutoutSequence(), new TweenCallback(HandleCutoutSequenceOnComplete));
	}

	private Sequence DOCutoutSequence()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 0.25f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Cutout, m_CutoutScarePosition.localPosition, num2, false), (Ease)27));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(m_Cutout, m_CutoutScarePosition.localRotation, num2), (Ease)27));
		num += 0.85f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Cutout, m_CutoutStartPosition.localPosition, num2, false), (Ease)26));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(m_Cutout, m_CutoutStartPosition.localRotation, num2), (Ease)26));
		return val;
	}

	private void HandleCutoutSequenceOnComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Cutout.localPosition = m_CutoutEndPosition.localPosition;
		m_Cutout.localRotation = m_CutoutEndPosition.localRotation;
		m_TheatreMainEvent.SetActive(active: true);
		m_TheatreMainEvent.OnEnter += HandleTheatreMainOnEnter;
	}

	private void HandleTheatreMainOnEnter(object sender, EventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		m_TheatreMainEvent.OnEnter -= HandleTheatreMainOnEnter;
		m_ProjectorController.TurnOn();
		m_ProjectorController.MusicAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_BendyCartoonMusicClip, m_ProjectorController.MusicPosition.position, AudioObjectType.MUSIC);
		m_ProjectorController.FilmAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_TheatreProjectorClip, m_ProjectorController.AudioPosition.position, AudioObjectType.MUSIC, -1);
		m_Interactable.OnInteracted += HandleInteractableOnInteracted;
		m_Interactable.SetActive(active: true);
	}

	private void HandleInteractableOnInteracted(object sender, EventArgs e)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		m_Interactable.SetActive(active: false);
		GameManager.Instance.AudioManager.PlayAtPosition(m_FlowValveClip, m_Interactable.transform.position);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Interactable.transform, new Vector3(-360f, 0f, 0f), 1.5f, (RotateMode)3), (Ease)7), new TweenCallback(BeginInkFlow));
	}

	private void BeginInkFlow()
	{
		S13AudioManager.Instance.InvokeEvent("evt_ink_pressure_restored");
		for (int i = 0; i < m_AnimationControllers.Length; i++)
		{
			m_AnimationControllers[i].Play();
		}
		for (int j = 0; j < m_InkPipes.Count; j++)
		{
			m_InkPipes[j].TurnOn();
		}
		SetInkEnableGameObjectsActive(active: true);
		DOFlooding();
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.TheatreObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_TheatreExitEvent.SetActive(active: true);
		m_TheatreExitEvent.OnEnter += HandleTheatreExitOnEnter;
	}

	private Sequence DOFlooding()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		float num2 = 4f;
		((Component)m_RaisingInk).tag = "Ink";
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(((Renderer)m_InkRenderer).material, 1f, "_Cutout", num2), (Ease)5));
		num += num2;
		num2 += num2 / 2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_RaisingInk, m_RaisingInk.localPosition.y + 1f, num2, false), (Ease)7));
		TweenSettingsExtensions.InsertCallback(val, num + num2 / 8f, (TweenCallback)delegate
		{
			((Component)m_RaisingInk).tag = "DeepInk";
		});
		return val;
	}

	private void FloodingOnComplete()
	{
		m_TheatreExitEvent.SetActive(active: true);
		m_TheatreExitEvent.OnEnter += HandleTheatreExitOnEnter;
	}

	private void HandleTheatreExitOnEnter(object sender, EventArgs e)
	{
		m_TheatreExitEvent.OnEnter -= HandleTheatreExitOnEnter;
		GameManager.Instance.AudioManager.Play(m_DuctCrawlingClip);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_TheatreMainEvent))
		{
			m_TheatreMainEvent.OnEnter -= HandleTheatreMainOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_TheatreExitEvent))
		{
			m_TheatreExitEvent.OnEnter -= HandleTheatreExitOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_TheatreEnterEvent))
		{
			m_TheatreEnterEvent.OnEnter -= HandleTheatreEnterOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_Interactable))
		{
			m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		}
		m_HenryClip06 = null;
		m_FlowValveClip = null;
		m_DuctCrawlingClip = null;
		m_BendyCartoonMusicClip = null;
		m_TheatreProjectorClip = null;
		m_JumpscareClip = null;
		base.OnDisposed();
	}
}
