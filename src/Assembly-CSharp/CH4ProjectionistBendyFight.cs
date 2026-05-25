using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4ProjectionistBendyFight : TMGMonoBehaviour
{
	[SerializeField]
	private GameObject m_Battle;

	[SerializeField]
	private Transform m_AudioProxy;

	[Header("<Projectionist head>")]
	[SerializeField]
	private GameObject m_SeveredHead;

	[SerializeField]
	private GameObject m_RealHead;

	[SerializeField]
	private Light m_FaceLight;

	[SerializeField]
	private Transform m_BendyHand;

	[SerializeField]
	private Transform m_headThrowPosition;

	[SerializeField]
	private Transform m_headThrowPosition2;

	[SerializeField]
	private AudioSource m_ProjectorAudioSource;

	[Header("<Ink Fx>")]
	[SerializeField]
	private InkExplosionEffect m_NeckExplosion;

	[SerializeField]
	private GameObject m_DeathInkTrail;

	[Header("Animations")]
	[SerializeField]
	private Animator m_BendyAnimator;

	[SerializeField]
	private AnimationClip m_BendyAnimationClip;

	private bool m_IsBendyActive;

	private S13ObjectSimple m_AudioSimple;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Battle.SetActive(false);
		m_AudioSimple = ((Component)m_AudioProxy).GetComponentInChildren<S13ObjectSimple>();
		AnimationEventUtil.AddAnimationEvent(ref m_BendyAnimator, ((Object)m_BendyAnimationClip).name, "EventOnComplete", 986);
	}

	private void Update()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsBendyActive)
		{
			GameManager.Instance.InkEffectManager.SetPosition(m_BendyHand.position);
		}
	}

	public void Activate()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		m_Battle.SetActive(true);
		m_AudioSimple.Play();
		Sequence val = DOTween.Sequence();
		float num = 7f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_IsBendyActive = true;
			GameManager.Instance.InkEffectManager.SetActive(m_IsBendyActive);
			GameManager.Instance.GameCamera.VisionEffect.BeginEffect();
		});
		num = 15f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			((Component)m_FaceLight).gameObject.SetActive(false);
			m_ProjectorAudioSource.Stop();
		});
		num = 16.3f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			m_SeveredHead.SetActive(true);
			m_SeveredHead.transform.SetParent(m_BendyHand);
			m_RealHead.transform.localScale = Vector3.zero;
			GameManager.Instance.GameCamera.VisionEffect.EndEffect();
		});
		num = 17.5f;
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_NeckExplosion.ExplodeOnly));
		num = 20f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_SeveredHead.transform.SetParent(base.transform);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_SeveredHead.transform, m_headThrowPosition.position, 0.4f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_SeveredHead.transform, m_headThrowPosition.eulerAngles, 0.35f, (RotateMode)0), (Ease)7));
		num += 0.3f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_SeveredHead.transform, m_headThrowPosition2.position, 0.4f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_SeveredHead.transform, m_headThrowPosition2.eulerAngles, 0.4f, (RotateMode)0), (Ease)7));
	}

	public void EventOnComplete()
	{
		m_IsBendyActive = false;
		m_Battle.SetActive(false);
		m_DeathInkTrail.SetActive(true);
		GameManager.Instance.InkEffectManager.SetActive(m_IsBendyActive);
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		m_IsBendyActive = false;
		m_DeathInkTrail.SetActive(true);
		m_SeveredHead.transform.SetParent(base.transform);
		m_SeveredHead.SetActive(true);
		m_SeveredHead.transform.position = m_headThrowPosition2.position;
		m_SeveredHead.transform.eulerAngles = m_headThrowPosition2.eulerAngles;
	}

	protected override void OnDisposed()
	{
		m_AudioSimple = null;
		base.OnDisposed();
	}
}
