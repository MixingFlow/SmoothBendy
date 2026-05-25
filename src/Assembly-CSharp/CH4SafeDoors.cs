using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4SafeDoors : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Door;

	private Sequence m_Sequence;

	private Vector3 m_StartDoorRotaiton;

	public override void InitOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_Door.localEulerAngles = new Vector3(0f, Random.Range(0f, -40f), 0f);
		m_StartDoorRotaiton = m_Door.localEulerAngles;
	}

	public void OpenAndShut()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		KillSequence();
		m_Sequence = DOTween.Sequence();
		float num = 0f;
		float num2 = Random.Range(0.1f, 0.2f);
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(((Component)m_Door).transform, new Vector3(0f, Random.Range(-25f, -35f), 0f), num2, (RotateMode)0), (Ease)1));
		num += num2;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(((Component)m_Door).transform, Vector3.zero, num2 / 2f, (RotateMode)0), (Ease)1));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(OpenAndShut));
	}

	public void ResetDoor()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		KillSequence();
		m_Door.localEulerAngles = m_StartDoorRotaiton;
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
