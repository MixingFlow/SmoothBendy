using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4PopupGhost : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Ghost;

	[SerializeField]
	private Transform m_LeftArm;

	[SerializeField]
	private Transform m_RightArm;

	public void Activate()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		S13AudioSource componentInChildren = ((Component)this).GetComponentInChildren<S13AudioSource>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			componentInChildren.Play();
		}
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Ghost, 3f, 0.2f, false)), (Ease)1);
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftArm, new Vector3(0f, -100f, 0f), 0.15f, (RotateMode)3), 0.2f), (Ease)30);
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightArm, new Vector3(0f, 100f, 0f), 0.15f, (RotateMode)3), 0.2f), (Ease)30);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Ghost, false);
		ShortcutExtensions.DOKill((Component)(object)m_LeftArm, false);
		ShortcutExtensions.DOKill((Component)(object)m_RightArm, false);
		base.OnDisposed();
	}
}
