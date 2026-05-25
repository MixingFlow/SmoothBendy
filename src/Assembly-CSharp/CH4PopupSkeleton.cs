using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4PopupSkeleton : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Skeleton;

	public void Activate()
	{
		S13AudioSource componentInChildren = ((Component)this).GetComponentInChildren<S13AudioSource>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			componentInChildren.Play();
		}
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Skeleton, 3f, 0.2f, false)), (Ease)1);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Skeleton, false);
		base.OnDisposed();
	}
}
