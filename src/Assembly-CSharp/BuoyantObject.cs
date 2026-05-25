using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class BuoyantObject : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_OnStart;

	public override void OnEnable()
	{
		if (m_OnStart)
		{
			Buoyancy(isPositive: true);
		}
	}

	public void Buoyancy(bool isPositive)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, GetRandomRotation(isPositive), 2f, (RotateMode)0), (Ease)7), (TweenCallback)delegate
		{
			Buoyancy(!isPositive);
		});
	}

	private Vector3 GetRandomRotation(bool isPositive)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float min = ((!isPositive) ? (-1f) : 0f);
		float max = ((!isPositive) ? 0f : 1f);
		return new Vector3(GetRandomValue(min, max), 0f, GetRandomValue(-1f, 1f));
	}

	private float GetRandomValue(float min, float max)
	{
		return Random.Range(min, max);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		base.OnDisposed();
	}
}
