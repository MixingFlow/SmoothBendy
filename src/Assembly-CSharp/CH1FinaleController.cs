using System;
using System.Collections.Generic;
using S13Audio;
using TMG.Controls;
using UnityEngine;

public class CH1FinaleController : BaseController
{
	[Header("Axe")]
	[SerializeField]
	private MeleeWeapon m_AxeWeapon;

	[Header("Objective: Enter The Ritual Room")]
	[SerializeField]
	private GameObject m_CollapsedVisuals;

	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private List<Breakable> m_Breakables;

	[Header("Ceiling Plank")]
	[SerializeField]
	private Breakable m_CeilingPlank;

	[SerializeField]
	private GameObject m_CeilingSpill;

	private AudioClip m_HenryClip07;

	private AudioClip m_BoardsBreakClip;

	private bool m_ShowAttackTutorial;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip07 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_07");
		m_BoardsBreakClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Floor_Boards_Break_01.L");
		m_CollapsedVisuals.SetActive(false);
		m_Door.Lock();
		m_CeilingSpill.SetActive(false);
		m_AxeWeapon.Interaction.SetActive(active: false);
	}

	public override void Activate()
	{
		m_AxeWeapon.OnEquipped += HandleAxeOnEquipped;
		m_AxeWeapon.Interaction.SetActive(active: true);
		m_CeilingPlank.OnBroken += HandleCeilingPlankOnBroken;
	}

	private void HandleCeilingPlankOnBroken(object sender, EventArgs e)
	{
		m_CeilingPlank.OnBroken -= HandleCeilingPlankOnBroken;
		m_CeilingSpill.SetActive(true);
	}

	private void HandleAxeOnEquipped(object sender, EventArgs e)
	{
		GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_ATTACK"));
		m_ShowAttackTutorial = true;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip07, "DIACH1/DIA_CH1_HENRY_07"));
		for (int i = 0; i < m_Breakables.Count; i++)
		{
			m_Breakables[i].OnBroken += HandleOnBroken;
		}
	}

	private void Update()
	{
		if (m_ShowAttackTutorial && PlayerInput.Attack())
		{
			GameManager.Instance.HideTutorial();
			m_ShowAttackTutorial = false;
		}
	}

	private void HandleOnBroken(object sender, EventArgs e)
	{
		for (int i = 0; i < m_Breakables.Count; i++)
		{
			m_Breakables[i].OnBroken -= HandleOnBroken;
		}
		m_Door.Unlock();
		m_Door.OnInteracted += HandleDoorOnOpened;
	}

	private void HandleDoorOnOpened(object sender, EventArgs e)
	{
		m_Door.OnInteracted -= HandleDoorOnOpened;
		m_CollapsedVisuals.SetActive(true);
		GameManager.Instance.AudioManager.Play(m_BoardsBreakClip);
		S13AudioManager.Instance.PlayAudioDelayed("sfx_collapse_ink", 1f);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_HenryClip07 = null;
		m_BoardsBreakClip = null;
		base.OnDisposed();
	}
}
