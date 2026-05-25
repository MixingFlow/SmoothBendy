using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3ToyRack : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		DOSway(isPositive: true);
	}

	private void DOSway(bool isPositive)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Content, GetRandomRotation(isPositive), Random.Range(2f, 3.5f), (RotateMode)0), (Ease)7), (TweenCallback)delegate
		{
			DOSway(!isPositive);
		});
	}

	private Vector3 GetRandomRotation(bool isPositive)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float min = ((!isPositive) ? (-1f) : 0f);
		float max = ((!isPositive) ? 0f : 1f);
		return new Vector3(GetRandomValue(min, max), GetRandomValue(min, max), GetRandomValue(min, max));
	}

	private float GetRandomValue(float min, float max)
	{
		return Random.Range(min, max);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Content, false);
		base.OnDisposed();
	}
}
