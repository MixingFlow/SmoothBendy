using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4Cart : TMGMonoBehaviour
{
	private const float SWAY_MIN_MAX = 1f;

	[SerializeField]
	private Transform m_PlayerParent;

	[SerializeField]
	private Transform m_CartInternal;

	[SerializeField]
	private Transform m_CartStartPos;

	[SerializeField]
	private Transform m_CartEndPosPos;

	[SerializeField]
	private Transform m_CartBreakdownPos;

	[SerializeField]
	private GameObject m_ExternalCollision;

	[SerializeField]
	private Interactable m_CartInteraction;

	[SerializeField]
	private Transform m_CartInsidePos;

	private Sequence m_CartSequence;

	private S13Switch m_AudioSwitch;

	public event EventHandler OnComplete;

	public event EventHandler OnStart;

	public event EventHandler OnStop;

	public event EventHandler OnInteracted;

	public event EventHandler OnBreakdown;

	public event EventHandler OnSmash;

	public override void InitOnComplete()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_ExternalCollision.SetActive(false);
		base.transform.localPosition = m_CartEndPosPos.localPosition;
		m_CartInteraction.SetActive(active: false);
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
	}

	public void ForceComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.transform.localPosition = m_CartEndPosPos.localPosition;
	}

	public void BeginSway()
	{
		DOSway(isPositive: true);
	}

	public void CallCart(float delay = 0f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		ResetCartSequence();
		TweenSettingsExtensions.InsertCallback(m_CartSequence, delay, (TweenCallback)delegate
		{
			this.OnStart.Send(this);
		});
		TweenSettingsExtensions.Insert(m_CartSequence, delay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.transform, m_CartStartPos.localPosition, 20f, false), (Ease)4));
		TweenSettingsExtensions.OnComplete<Sequence>(m_CartSequence, new TweenCallback(CallCartOnComplete));
	}

	private void CallCartOnComplete()
	{
		m_CartInteraction.OnInteracted += HandleCartInteractionOnInteracted;
		m_CartInteraction.SetActive(active: true);
		this.OnStop.Send(this);
		SendCart(6f);
	}

	public void SendCart(float delay = 0f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		ResetCartSequence();
		TweenSettingsExtensions.InsertCallback(m_CartSequence, delay, (TweenCallback)delegate
		{
			this.OnStart.Send(this);
			m_CartInteraction.OnInteracted -= HandleCartInteractionOnInteracted;
			m_CartInteraction.SetActive(active: false);
		});
		TweenSettingsExtensions.Insert(m_CartSequence, delay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.transform, m_CartEndPosPos.localPosition, 20f, false), (Ease)4));
		TweenSettingsExtensions.OnComplete<Sequence>(m_CartSequence, new TweenCallback(SendCartOnComplete));
	}

	private void SendCartOnComplete()
	{
		this.OnStop.Send(this);
		CallCart(6f);
	}

	private void HandleCartInteractionOnInteracted(object sender, EventArgs e)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		m_CartInteraction.OnInteracted -= HandleCartInteractionOnInteracted;
		m_CartInteraction.SetActive(active: false);
		this.OnInteracted.Send(this);
		KillCartSequence();
		((Component)m_CartInteraction).GetComponent<Collider>().enabled = false;
		m_ExternalCollision.SetActive(true);
		Transform obj = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_CartInsidePos);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(obj, m_CartInsidePos.eulerAngles, 2f, (RotateMode)0), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(obj, GameManager.Instance.Player.HeadContainer.position.x, 2f, false), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(obj, GameManager.Instance.Player.HeadContainer.position.y, 2f, false), (Ease)28);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveZ(obj, GameManager.Instance.Player.HeadContainer.position.z, 2f, false), (Ease)7), (TweenCallback)delegate
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			GameManager.Instance.Player.transform.SetParent(m_PlayerParent);
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			GameManager.Instance.Player.isMoveLocked = true;
			TweenSettingsExtensions.OnComplete<Sequence>(DOAbyssCross(), new TweenCallback(SendOnComplete));
		});
	}

	private Sequence DOAbyssCross()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		Sequence obj = DOTween.Sequence();
		this.OnStart.Send(this);
		float num = 0f;
		TweenSettingsExtensions.Insert(obj, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.transform, m_CartBreakdownPos.localPosition, 10f, false), (Ease)5));
		num += 10f;
		TweenSettingsExtensions.Insert(obj, num, (Tween)(object)ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 2f, 0.7f, 10, 90f, false, true));
		TweenSettingsExtensions.InsertCallback(obj, num, (TweenCallback)delegate
		{
			this.OnBreakdown.Send(this);
			this.OnStop.Send(this);
		});
		num += 5f;
		TweenSettingsExtensions.Insert(obj, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(base.transform, m_CartEndPosPos.localPosition, 10f, false), (Ease)5));
		TweenSettingsExtensions.InsertCallback(obj, num, (TweenCallback)delegate
		{
			this.OnStart.Send(this);
		});
		num += 10f;
		TweenSettingsExtensions.Insert(obj, num, (Tween)(object)ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 2f, 0.7f, 10, 90f, false, true));
		TweenSettingsExtensions.InsertCallback(obj, num, (TweenCallback)delegate
		{
			this.OnSmash.Send(this);
		});
		return obj;
	}

	private void SendOnComplete()
	{
		this.OnComplete.Send(this);
	}

	private void DOSway(bool isPositive)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		m_AudioSwitch.Play("sway");
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_CartInternal, GetRandomRotation(isPositive), 3f, (RotateMode)0), (Ease)7), (UpdateType)2), (TweenCallback)delegate
		{
			DOSway(!isPositive);
		});
	}

	private Vector3 GetRandomRotation(bool isPositive)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((!isPositive) ? (-1f) : 1f, 0f, 0f);
	}

	private float GetRandomValue(float min, float max)
	{
		return Random.Range(min, max);
	}

	private void KillCartSequence()
	{
		if (m_CartSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_CartSequence, false);
		}
	}

	private void ResetCartSequence()
	{
		KillCartSequence();
		m_CartSequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_CartInternal, false);
		TweenExtensions.Kill((Tween)(object)m_CartSequence, false);
		base.OnDisposed();
	}
}
