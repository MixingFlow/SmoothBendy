using System;
using TMG.Core;
using UnityEngine;

public class CH3LiftButton : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Button;

	[SerializeField]
	private CH3LiftContainer m_Floor;

	public CH3LiftContainer Floor => m_Floor;

	public event EventHandler OnPressed;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Button.SetActive(active: false);
	}

	public void Enable()
	{
		m_Button.SetActive(active: true);
		m_Button.OnInteracted -= HandleButtonOnInteracted;
		m_Button.OnInteracted += HandleButtonOnInteracted;
	}

	private void HandleButtonOnInteracted(object sender, EventArgs e)
	{
		DebugLog("[LIFT BUTTON] - (" + ((Object)base.gameObject).name + ") OnPressed Sent");
		this.OnPressed.Send(this);
	}

	public void Disable()
	{
		m_Button.SetActive(active: false);
		m_Button.OnInteracted -= HandleButtonOnInteracted;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
