using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class InteractableSwitchBox : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Door;

	[SerializeField]
	private Interactable m_Switch;

	[SerializeField]
	private LightFlicker m_Light;

	private S13AudioSource m_doorSound;

	private bool m_IsDoorOpen;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Door.SetActive(active: false);
		m_Switch.SetActive(active: false);
		m_doorSound = ((Component)this).GetComponentInChildren<S13AudioSource>();
	}

	public void ActivateDoor()
	{
		if (!m_IsDoorOpen)
		{
			m_Door.OnInteracted += HandleDoorOnInteracted;
			m_Door.SetActive(active: true);
		}
		else
		{
			ActivateSwitch();
		}
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		m_Door.SetActive(active: false);
		m_IsDoorOpen = true;
		m_doorSound.Play();
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door.transform, new Vector3(0f, 0f, 180f), 0.5f, (RotateMode)3), (Ease)7), new TweenCallback(ActivateSwitch));
	}

	public void ActivateSwitch()
	{
		m_Switch.OnInteracted += HandleSwitchOnInteracted;
		m_Switch.SetActive(active: true);
	}

	private void HandleSwitchOnInteracted(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		m_Switch.OnInteracted -= HandleSwitchOnInteracted;
		m_Switch.SetActive(active: false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Switch.transform, new Vector3(0f, 20f, 0f), 0.1f, (RotateMode)3), (Ease)1), new TweenCallback(SendOnComplete));
	}

	public void ForceDoorOpen()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_Door.OnInteracted -= HandleDoorOnInteracted;
		m_Door.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		m_IsDoorOpen = true;
	}

	public void ForceSwitchOn()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_Switch.OnInteracted -= HandleSwitchOnInteracted;
		m_Switch.transform.localEulerAngles = new Vector3(0f, 10f, 0f);
	}

	private void SendOnComplete()
	{
		m_Light.TurnOff();
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
