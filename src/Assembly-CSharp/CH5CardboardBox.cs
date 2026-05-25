using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5CardboardBox : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private Transform m_LidLeft;

	[SerializeField]
	private Transform m_LidRight;

	[SerializeField]
	private bool m_IsActive;

	private AudioClip m_BoxOpenClip;

	public event EventHandler OnInteracted;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BoxOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/sfx_boxopen");
		if (m_IsActive)
		{
			Activate();
		}
	}

	public void Activate()
	{
		m_Interactable.OnInteracted += HandleInteractableOnInteracted;
		m_Interactable.SetActive(active: true);
	}

	private void HandleInteractableOnInteracted(object sender, EventArgs e)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		m_Interactable.Dispose();
		this.OnInteracted.Send(this);
		GameManager.Instance.AudioManager.PlayAtPosition(m_BoxOpenClip, base.transform.position);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LidLeft, new Vector3(-125f, 0f, 0f), 0.5f, (RotateMode)0), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LidRight, new Vector3(125f, 0f, 0f), 0.5f, (RotateMode)0), (Ease)7);
	}

	public void ForceOpen()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		m_Interactable.Dispose();
		m_LidLeft.localEulerAngles = new Vector3(-125f, 0f, 0f);
		m_LidRight.localEulerAngles = new Vector3(125f, 0f, 0f);
	}

	protected override void OnDisposed()
	{
		this.OnInteracted = null;
		if (Object.op_Implicit((Object)(object)m_Interactable))
		{
			m_Interactable.OnInteracted -= HandleInteractableOnInteracted;
		}
		base.OnDisposed();
	}
}
