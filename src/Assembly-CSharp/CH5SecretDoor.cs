using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5SecretDoor : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Door;

	[SerializeField]
	private Transform m_PushLocation;

	[SerializeField]
	private Transform m_SlideLocation;

	private OcclusionPortal m_Portal;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Portal = ((Component)this).GetComponent<OcclusionPortal>();
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = false;
		}
	}

	public void Open()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Door, m_PushLocation.localPosition, 1f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, 1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Door, m_SlideLocation.localPosition, 2f, false), (Ease)7));
	}

	public void ForceOpen()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		m_Door.localPosition = m_SlideLocation.localPosition;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
