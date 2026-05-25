using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5Safehouse : BaseController
{
	[SerializeField]
	private CH5Toilet m_Toilet;

	[SerializeField]
	private MeleeWeapon m_GentPipe;

	[SerializeField]
	private GameObject m_BreakablePlanks;

	[SerializeField]
	private EventTrigger m_ExitTrigger;

	[Header("Bathroom!")]
	[SerializeField]
	private Interactable m_Spoon;

	[SerializeField]
	private CH5SecretDoor m_SecretDoor;

	private AudioClip m_SeeingToolMusic;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_GentPipe.Interaction.SetActive(active: false);
		m_GentPipe.gameObject.SetActive(false);
		m_ExitTrigger.SetActive(active: false);
		m_Spoon.SetActive(active: false);
		m_SeeingToolMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_AnotherWorldRevealed");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.SafehouseObjective.IsComplete)
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
		GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_SEEING_TOOL"));
		GameManager.Instance.Player.OnSeeingToolActive += HandleSeeingToolOnActive;
		GameManager.Instance.Player.EnableSeeingTool(active: true);
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON_TIP", 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.SafehouseObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void ForceComplete()
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON", "OBJECTIVES/CH5_OBJECTIVE_ESCAPE_YOUR_PRISON_TIP"));
		m_SecretDoor.ForceOpen();
		m_Toilet.ForceComplete();
		m_BreakablePlanks.SetActive(false);
		GameManager.Instance.Player.WeaponGameObject = m_GentPipe.gameObject;
		GameManager.Instance.Player.EquipWeapon();
		if (Object.op_Implicit((Object)(object)m_GentPipe) && (Object)(object)m_GentPipe.Interaction != (Object)null)
		{
			m_GentPipe.gameObject.SetActive(true);
			m_GentPipe.Interaction.SetActive(active: false);
		}
		m_GentPipe.KillInteraction();
		m_GentPipe.Equip();
		m_GentPipe.transform.SetParent(GameManager.Instance.Player.WeaponParent);
		m_GentPipe.transform.localPosition = Vector3.zero;
		m_GentPipe.transform.localEulerAngles = Vector3.zero;
		GameManager.Instance.Player.EnableSeeingTool(active: true);
		SendOnComplete();
	}

	private void HandleSeeingToolOnActive(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnSeeingToolActive -= HandleSeeingToolOnActive;
		GameManager.Instance.HideTutorial();
		GameManager.Instance.AudioManager.Play(m_SeeingToolMusic, AudioObjectType.MUSIC);
		S13AudioManager.Instance.InvokeEvent("evt_ch5_seeing_tool_active");
		m_Spoon.OnInteracted += HandleSpoonOnInteracted;
		m_Spoon.SetActive(active: true);
	}

	private void HandleSpoonOnInteracted(object sender, EventArgs e)
	{
		m_Spoon.OnInteracted -= HandleSpoonOnInteracted;
		TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(m_Spoon.transform, -0.05f, 0.5f, false), (Ease)6));
		S13AudioManager.Instance.PlayAudio("sfx_spoon_lever");
		m_Spoon.ForceRemoveEffects();
		m_SecretDoor.Open();
		S13AudioManager.Instance.PlayAudio("sfx_hidden_door_slide");
		m_Toilet.OnInteracted += HandleToiletOnInteracted;
		m_Toilet.OnComplete += HandleToiletOnComplete;
		m_Toilet.Activate();
	}

	private void HandleToiletOnInteracted(object sender, EventArgs e)
	{
		m_Toilet.OnInteracted -= HandleToiletOnInteracted;
		S13AudioManager.Instance.PlayAudio("sfx_toilet_lid");
		m_GentPipe.gameObject.SetActive(true);
	}

	private void HandleToiletOnComplete(object sender, EventArgs e)
	{
		m_Toilet.OnComplete -= HandleToiletOnComplete;
		m_GentPipe.OnInteract += HandleGentPipeOnInteracted;
		m_GentPipe.Interaction.SetActive(active: true);
	}

	private void HandleGentPipeOnInteracted(object sender, EventArgs e)
	{
		m_GentPipe.OnInteract -= HandleGentPipeOnInteracted;
		m_ExitTrigger.OnEnter += HandleExitTriggerOnEnter;
		m_ExitTrigger.SetActive(active: true);
	}

	private void HandleExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.SafehouseObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Toilet))
		{
			m_Toilet.OnInteracted -= HandleToiletOnInteracted;
			m_Toilet.OnComplete -= HandleToiletOnComplete;
		}
		if (Object.op_Implicit((Object)(object)m_GentPipe))
		{
			m_GentPipe.OnInteract -= HandleGentPipeOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_ExitTrigger))
		{
			m_ExitTrigger.OnEnter -= HandleExitTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
