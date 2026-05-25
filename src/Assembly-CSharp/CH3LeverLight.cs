using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3LeverLight : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Lever;

	[SerializeField]
	private LightFlicker m_LightFlicker;

	[SerializeField]
	private Transform m_EndPosition;

	private Vector3 m_OriginalPosition;

	private bool m_IsActive;

	private bool m_IsEnabled;

	public bool IsComplete { get; private set; }

	public Interactable Interactable => m_Lever;

	public event EventHandler OnComplete;

	public event EventHandler OnInteracted;

	public override void InitOnComplete()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_OriginalPosition = m_Lever.transform.localPosition;
		((Component)m_LightFlicker.Light).gameObject.SetActive(false);
		((Behaviour)m_LightFlicker).enabled = false;
	}

	public void Activate(bool isAlreadyActive = false)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!isAlreadyActive)
		{
			if (!m_IsEnabled)
			{
				((Behaviour)m_LightFlicker).enabled = true;
				((Component)m_LightFlicker.Light).gameObject.SetActive(true);
				m_Lever.SetActive(active: true);
				m_Lever.OnInteracted += HandleLeverOnInteracted;
			}
		}
		else
		{
			((Behaviour)m_LightFlicker).enabled = true;
			((Component)m_LightFlicker.Light).gameObject.SetActive(true);
			m_LightFlicker.TurnOff();
			m_Lever.SetActive(active: false);
			m_Lever.transform.localPosition = m_EndPosition.localPosition;
			Complete();
		}
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.SetActive(active: false);
		if (!m_IsEnabled)
		{
			m_IsEnabled = true;
			GameManager.Instance.AudioManager.Play("Audio/SFX/CH3/SFX_CH3_LeverPull");
			this.OnInteracted.Send(this);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_EndPosition.localPosition, 0.5f, false), (Ease)28), new TweenCallback(LeverInteractOnComplete));
		}
	}

	private void LeverInteractOnComplete()
	{
		m_LightFlicker.TurnOff();
		Complete();
	}

	private void Complete()
	{
		IsComplete = true;
		this.OnComplete.Send(this);
	}

	public void DOReset()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_OriginalPosition, 0.5f, false), (Ease)28), new TweenCallback(Reset));
	}

	public void Reset()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_IsEnabled = false;
		((Behaviour)m_LightFlicker).enabled = true;
		((Component)m_LightFlicker.Light).gameObject.SetActive(true);
		m_LightFlicker.TurnOn();
		m_Lever.transform.localPosition = m_OriginalPosition;
		m_Lever.SetActive(active: true);
		m_Lever.OnInteracted += HandleLeverOnInteracted;
	}

	public void Disable()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		m_IsEnabled = false;
		m_LightFlicker.TurnOn();
		((Component)m_LightFlicker.Light).gameObject.SetActive(false);
		((Behaviour)m_LightFlicker).enabled = false;
		m_OriginalPosition = m_Lever.transform.localPosition;
		m_Lever.SetActive(active: false);
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
	}

	public void TurnOnLights(bool isFlickering = true)
	{
		((Behaviour)m_LightFlicker).enabled = true;
		((Component)m_LightFlicker.Light).gameObject.SetActive(true);
		if (!isFlickering)
		{
			m_LightFlicker.TurnOff();
		}
	}

	public void SetSingleInteraction(bool active)
	{
		m_Lever.SetSingleInteraction(active);
	}

	public void ForceComplete()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.SetActive(active: false);
		m_IsEnabled = true;
		m_Lever.transform.localPosition = m_EndPosition.localPosition;
		TurnOnLights(isFlickering: false);
		IsComplete = true;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
