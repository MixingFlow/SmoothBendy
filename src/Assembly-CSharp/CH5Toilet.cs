using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5Toilet : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Lid;

	public event EventHandler OnInteracted;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		m_Lid.SetActive(active: false);
	}

	public void Activate()
	{
		m_Lid.OnInteracted += HandleLidOnInteracted;
		m_Lid.SetActive(active: true);
	}

	public void ForceComplete()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		m_Lid.gameObject.layer = LayerMask.NameToLayer("InvisibleCollider");
		Transform obj = m_Lid.transform;
		obj.localPosition += new Vector3(0.7f, 0f, 0f);
		m_Lid.transform.localEulerAngles = new Vector3(0f, 20f, 0f);
	}

	private void HandleLidOnInteracted(object sender, EventArgs e)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		m_Lid.OnInteracted -= HandleLidOnInteracted;
		this.OnInteracted.Send(this);
		m_Lid.gameObject.layer = LayerMask.NameToLayer("InvisibleCollider");
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(m_Lid.transform, 0.7f, 1f, false), (Ease)7);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOLocalRotate(m_Lid.transform, new Vector3(0f, 20f, 0f), 1f, (RotateMode)0), new TweenCallback(SendOnComplete));
	}

	private void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnInteracted = null;
		this.OnComplete = null;
		base.OnDisposed();
	}
}
