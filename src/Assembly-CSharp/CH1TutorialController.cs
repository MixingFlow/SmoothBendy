using System;
using TMG.Controls;
using UnityEngine;

public class CH1TutorialController : BaseController
{
	[SerializeField]
	private EventTrigger m_JumpTutorialTrigger;

	[SerializeField]
	private EventTrigger m_InteractTutorialTrigger;

	private bool m_IsShowingJump;

	private bool m_IsShowingInteract;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_JumpTutorialTrigger.SetActive(active: false);
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineRevealObjective.IsComplete)
		{
			Activate();
		}
	}

	public override void Activate()
	{
		m_JumpTutorialTrigger.OnEnter += HandleJumpTutorialTriggerOnEnter;
		m_JumpTutorialTrigger.OnExit += HandleJumpTutorialTriggerOnExit;
		m_JumpTutorialTrigger.SetActive(active: true);
		m_InteractTutorialTrigger.OnEnter += HandleInteractTutorialTriggerOnEnter;
		m_InteractTutorialTrigger.OnExit += HandleInteractTutorialTriggerOnExit;
		m_InteractTutorialTrigger.SetActive(active: true);
	}

	private void Update()
	{
		if (m_IsShowingJump && PlayerInput.Jump())
		{
			m_JumpTutorialTrigger.OnExit -= HandleJumpTutorialTriggerOnExit;
			m_JumpTutorialTrigger.OnEnter -= HandleJumpTutorialTriggerOnEnter;
			m_JumpTutorialTrigger.Dispose();
			GameManager.Instance.HideTutorial();
		}
		if (m_IsShowingInteract && PlayerInput.InteractOnPressed())
		{
			m_InteractTutorialTrigger.OnExit -= HandleInteractTutorialTriggerOnExit;
			m_InteractTutorialTrigger.OnEnter -= HandleInteractTutorialTriggerOnEnter;
			m_InteractTutorialTrigger.Dispose();
			GameManager.Instance.HideTutorial();
		}
	}

	private void HandleJumpTutorialTriggerOnExit(object sender, EventArgs e)
	{
		m_IsShowingJump = false;
		GameManager.Instance.HideTutorial();
	}

	private void HandleJumpTutorialTriggerOnEnter(object sender, EventArgs e)
	{
		m_IsShowingJump = true;
		GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_JUMP"));
	}

	private void HandleInteractTutorialTriggerOnExit(object sender, EventArgs e)
	{
		m_IsShowingInteract = false;
		GameManager.Instance.HideTutorial();
	}

	private void HandleInteractTutorialTriggerOnEnter(object sender, EventArgs e)
	{
		m_IsShowingInteract = true;
		GameManager.Instance.ShowTutorial(new TutorialDataVO("Tutorial/TUTORIAL_INTERACT"));
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_JumpTutorialTrigger))
		{
			m_JumpTutorialTrigger.OnExit -= HandleJumpTutorialTriggerOnExit;
			m_JumpTutorialTrigger.OnEnter -= HandleJumpTutorialTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_InteractTutorialTrigger))
		{
			m_InteractTutorialTrigger.OnExit -= HandleInteractTutorialTriggerOnExit;
			m_InteractTutorialTrigger.OnEnter -= HandleInteractTutorialTriggerOnEnter;
		}
		base.OnDisposed();
	}
}
