using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class SwayProp : TMGMonoBehaviour
{
	[SerializeField]
	private float m_MinMax = 1f;

	[Header("Duration")]
	[SerializeField]
	private float m_DurationMin = 2f;

	[SerializeField]
	private float m_DurationMax = 3.5f;

	[Header("Options")]
	[SerializeField]
	private bool m_OnStart = true;

	private float m_InitialMinMax;

	public override void Init()
	{
		base.Init();
		m_InitialMinMax = m_MinMax;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (m_OnStart)
		{
			DOSway(isPositive: true);
		}
	}

	private void DOSway(bool isPositive)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, GetRandomRotation(isPositive), Random.Range(m_DurationMin, m_DurationMax), (RotateMode)0), (Ease)7), (TweenCallback)delegate
		{
			DOSway(!isPositive);
		});
	}

	private void DOHeavySway(bool isPositive)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, GetRandomRotation(isPositive), Random.Range(m_DurationMin / 4f, m_DurationMax / 4f), (RotateMode)0), (Ease)7), (TweenCallback)delegate
		{
			DOSway(!isPositive);
		});
	}

	private Vector3 GetRandomRotation(bool isPositive)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		float min = ((!isPositive) ? (0f - m_MinMax) : 0f);
		float max = ((!isPositive) ? 0f : m_MinMax);
		return new Vector3(GetRandomValue(min, max), GetRandomValue(min, max), GetRandomValue(min, max));
	}

	private float GetRandomValue(float min, float max)
	{
		return Random.Range(min, max);
	}

	public void Sway()
	{
		m_MinMax = m_InitialMinMax;
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		DOSway(isPositive: true);
	}

	public void HeavySway(float intensity)
	{
		m_MinMax = m_InitialMinMax * intensity;
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		DOHeavySway(isPositive: true);
	}

	public void ResetSway()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		base.transform.localEulerAngles = Vector3.zero;
		Sway();
	}

	public void Stop()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 1f, (RotateMode)0), (TweenCallback)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			base.transform.localEulerAngles = Vector3.zero;
		});
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		base.OnDisposed();
	}
}
