using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4BertrumDoor : TMGMonoBehaviour
{
	private const float DOOR_OPEN = 100f;

	[SerializeField]
	private Transform m_LeftDoor;

	[SerializeField]
	private Transform m_RightDoor;

	private Vector3 m_OriginRotation;

	private Sequence m_Sequence;

	public override void InitOnComplete()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_OriginRotation = m_LeftDoor.localEulerAngles;
	}

	public void Open()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		ResetSequence();
		Vector3 originRotation = m_OriginRotation;
		originRotation.z += 100f;
		Vector3 originRotation2 = m_OriginRotation;
		originRotation2.z -= 100f;
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftDoor, originRotation, 0.5f, (RotateMode)0), (Ease)30));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightDoor, originRotation2, 0.5f, (RotateMode)0), (Ease)30));
	}

	public void Close(float speed = 0.5f, Ease ease = (Ease)30)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftDoor, m_OriginRotation, speed, (RotateMode)0), ease));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightDoor, m_OriginRotation, speed, (RotateMode)0), ease));
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
