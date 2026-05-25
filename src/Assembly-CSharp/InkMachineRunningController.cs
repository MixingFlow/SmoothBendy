using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class InkMachineRunningController : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Ink;

	[SerializeField]
	private Transform[] m_RotatorsNormal;

	[SerializeField]
	private Transform[] m_RotatorsFast;

	[SerializeField]
	private Transform m_TopLever;

	[SerializeField]
	private Transform m_TopPump;

	public void Activate()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		m_Ink.SetActive(true);
		for (int i = 0; i < m_RotatorsNormal.Length; i++)
		{
			Rotate(m_RotatorsNormal[i], 1.8f);
		}
		for (int j = 0; j < m_RotatorsFast.Length; j++)
		{
			Rotate(m_RotatorsFast[j], 1f, -1f);
		}
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_TopLever, new Vector3(10f, 0f, 0f), 0.75f, (RotateMode)3), (Ease)7), -1, (LoopType)1);
		TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_TopPump, -0.02f, 0.75f, false), (Ease)7), -1, (LoopType)1));
	}

	private void Rotate(Transform trans, float speed, float direction = 1f)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(trans, new Vector3(360f * direction, 0f, 0f), speed * 3f, (RotateMode)3), (Ease)1), -1);
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_RotatorsNormal.Length; i++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_RotatorsNormal[i], false);
		}
		for (int j = 0; j < m_RotatorsFast.Length; j++)
		{
			ShortcutExtensions.DOKill((Component)(object)m_RotatorsFast[j], false);
		}
		base.OnDisposed();
	}
}
