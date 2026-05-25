using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4FairGameTarget : TMGMonoBehaviour
{
	[Header("Targets")]
	[SerializeField]
	private GameObject m_GoodTarget;

	[SerializeField]
	private GameObject m_BadTarget;

	private float m_HitPosition;

	private float m_HidePosition;

	public bool IsBad;

	public Collider HitCollider { get; private set; }

	public override void Init()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_HitPosition = base.transform.localPosition.y;
		m_HidePosition = m_HitPosition - 2.9f;
		HitCollider = ((Component)this).GetComponent<Collider>();
		bool flag = Random.value < 0.5f;
		m_GoodTarget.SetActive(flag);
		m_BadTarget.SetActive(!flag);
	}

	public Tweener Setup()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		DisableCollider();
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 0.25f, (RotateMode)0);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, m_HidePosition, 0.25f, false), (Ease)5);
	}

	public Tweener DOShowGood()
	{
		IsBad = false;
		m_GoodTarget.SetActive(true);
		m_BadTarget.SetActive(false);
		return DOShow();
	}

	public Tweener DOShowBad()
	{
		IsBad = true;
		m_GoodTarget.SetActive(false);
		m_BadTarget.SetActive(true);
		return DOShow();
	}

	public Tweener DOShow()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 0.12f, (RotateMode)0);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, m_HitPosition, 0.12f, false), (Ease)7);
	}

	public Tweener DOHide()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		ShortcutExtensions.DOLocalRotate(base.transform, Vector3.zero, 0.13f, (RotateMode)0);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, m_HidePosition, 0.13f, false), (Ease)7);
	}

	public void Hit()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DisableCollider();
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(base.transform, new Vector3(0f, 0f, 90f), 0.2f, (RotateMode)0), (Ease)30);
		TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, m_HidePosition, 0.2f, false), 0.3f), (Ease)5);
	}

	public void EnableCollider()
	{
		HitCollider.enabled = true;
	}

	public void DisableCollider()
	{
		HitCollider.enabled = false;
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)base.transform, false);
		base.OnDisposed();
	}
}
