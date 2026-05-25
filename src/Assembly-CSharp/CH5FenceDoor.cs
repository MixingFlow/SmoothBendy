using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5FenceDoor : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_LeftDoor;

	[SerializeField]
	private Transform m_RightDoor;

	[SerializeField]
	private GameObject m_Chains;

	[SerializeField]
	private Transform m_AttackLocation;

	public Transform AttackLocation => m_AttackLocation;

	public void Open()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0.5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftDoor, new Vector3(0f, 75f, 0f), 1f, (RotateMode)3), (Ease)6));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightDoor, new Vector3(0f, -97f, 0f), 1f, (RotateMode)3), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_Chains.SetActive(false);
		});
	}

	public void ForceOpen()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_LeftDoor.localEulerAngles = new Vector3(0f, 75f, 0f);
		m_RightDoor.localEulerAngles = new Vector3(0f, -97f, 0f);
		m_Chains.SetActive(false);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
