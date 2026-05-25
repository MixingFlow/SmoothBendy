using DG.Tweening;
using UnityEngine;

public class InteractableDrawer : Interactable
{
	private AudioClip m_DrawerClip;

	private Vector3 m_OriginPosition;

	private bool m_IsOpen;

	public override void InitOnComplete()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_OriginPosition = base.transform.localPosition;
		m_DrawerClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Drawer");
	}

	public override void OnInteract()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		if (m_IsOpen)
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(base.transform, m_OriginPosition.x, 1f, false), (Ease)7);
		}
		else
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(base.transform, -0.2f, 1f, false), (Ease)7);
		}
		m_IsOpen = !m_IsOpen;
		GameManager.Instance.AudioManager.PlayAtPosition(m_DrawerClip, base.transform.position);
	}

	protected override void OnDisposed()
	{
		m_DrawerClip = null;
		base.OnDisposed();
	}
}
