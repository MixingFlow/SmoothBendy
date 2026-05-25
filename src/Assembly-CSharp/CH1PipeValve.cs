using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH1PipeValve : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Valve;

	[SerializeField]
	private Interactable m_EmptyValve;

	public event EventHandler OnInteracted;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (Object.op_Implicit((Object)(object)m_EmptyValve))
		{
			m_Valve.gameObject.SetActive(false);
		}
	}

	public void Activate()
	{
		if (Object.op_Implicit((Object)(object)m_EmptyValve))
		{
			m_EmptyValve.gameObject.SetActive(false);
		}
		m_Valve.gameObject.SetActive(true);
		m_Valve.SetActive(active: true);
		m_Valve.OnInteracted += HandleValveOnInteracted;
	}

	public void ActivateEmpty()
	{
		m_Valve.gameObject.SetActive(false);
		m_EmptyValve.SetActive(active: true);
		m_EmptyValve.OnInteracted += HandleValveEmptyOnInteracted;
	}

	public void SetEmptyCollision(bool active)
	{
		((Component)m_EmptyValve).GetComponent<Collider>().enabled = active;
	}

	public void Disable()
	{
		m_Valve.SetActive(active: false);
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		m_Valve.OnInteracted -= HandleValveOnInteracted;
		m_Valve.SetActive(active: false);
		this.OnInteracted.Send(this);
	}

	private void HandleValveEmptyOnInteracted(object sender, EventArgs e)
	{
		m_EmptyValve.OnInteracted -= HandleValveEmptyOnInteracted;
		m_EmptyValve.SetActive(active: false);
		this.OnInteracted.Send(this);
	}

	public Tweener DORotate(float duration)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)m_Valve.transform, false);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Valve.transform, new Vector3(-180f, 0f, 0f), duration, (RotateMode)3), (Ease)7);
	}

	public Tweener DORotateReverse(float duration, Ease ease = (Ease)7)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)m_Valve.transform, false);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Valve.transform, new Vector3(180f, 0f, 0f), duration, (RotateMode)3), ease);
	}

	public void ForceComplete()
	{
		m_Valve.SetActive(active: false);
		m_Valve.ForceRemoveEffects();
		m_Valve.gameObject.SetActive(true);
		m_EmptyValve.gameObject.SetActive(false);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Valve.transform, false);
		m_Valve.OnInteracted -= HandleValveOnInteracted;
		this.OnInteracted = null;
		base.OnDisposed();
	}
}
