using System;
using DG.Tweening;
using TMG.Controls;
using TMG.Core;
using UnityEngine;

public class CH5DemonWorthy : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	[SerializeField]
	private Interactable m_Lever;

	[SerializeField]
	private Transform m_LeverOn;

	[SerializeField]
	private GameObject m_LeverLight;

	[SerializeField]
	private BuoyantObject m_Byoyancy;

	[SerializeField]
	private GameObject m_ExteriorCollider;

	[SerializeField]
	private GameObject m_InteriorCollider;

	[SerializeField]
	private Transform m_ForwardLocation;

	[SerializeField]
	private Transform m_BehindLocation;

	[SerializeField]
	private Transform m_HandLocation;

	[SerializeField]
	private Transform m_HandStartLocation;

	[SerializeField]
	private CH5BoatClogger[] m_InkCloggers;

	[Header("Power Button")]
	[SerializeField]
	private Interactable m_EngineButton;

	[SerializeField]
	private Transform m_EnginePushLocation;

	[SerializeField]
	private GameObject m_EngineLight;

	private bool m_IsActive;

	private Vector3 m_EngineOffPosition;

	private Vector3 m_LeverOffPosition;

	private Vector3 m_LeverOnPosition;

	private Sequence m_BoatSequence;

	private BoatAudioControl m_BoatAudioControl;

	private AudioClip m_ThickInkPickup;

	private bool m_IsInitialClog;

	private bool m_IsClogged;

	private int m_ClogCount;

	public Transform Content => m_Content;

	public Transform HandLocation => m_HandLocation;

	public Transform HandStartLocation => m_HandStartLocation;

	public event EventHandler OnThrottled;

	public event EventHandler OnReleased;

	public override void InitOnComplete()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_EngineOffPosition = m_EngineButton.transform.localPosition;
		m_LeverOnPosition = m_LeverOn.localPosition;
		m_LeverOffPosition = m_Lever.transform.localPosition;
		m_EngineButton.SetActive(active: false);
		m_EngineLight.SetActive(false);
		m_Lever.SetActive(active: false);
		m_LeverLight.SetActive(false);
		m_InteriorCollider.SetActive(false);
		m_ThickInkPickup = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_swollensearcherpop");
		m_BoatAudioControl = ((Component)m_Content).GetComponentInChildren<BoatAudioControl>();
	}

	public void Activate()
	{
		m_Byoyancy.Buoyancy(isPositive: true);
		m_ExteriorCollider.SetActive(false);
		ActivateEngine();
	}

	public void Update()
	{
		if (m_IsActive && PlayerInput.InteractOnReleased())
		{
			Release();
		}
	}

	private void ActivateEngine()
	{
		m_EngineButton.OnInteracted += HandleEngineOnInteracted;
		m_EngineButton.SetActive(active: true);
	}

	private void HandleEngineOnInteracted(object sender, EventArgs e)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		m_EngineButton.OnInteracted -= HandleEngineOnInteracted;
		m_BoatAudioControl.EngineOn();
		m_EngineLight.SetActive(true);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_EngineButton.transform, m_EnginePushLocation.localPosition, 0.75f, false), (Ease)6), new TweenCallback(ActivateLever));
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_FOLLOW_THE_INK_RIVER", "OBJECTIVES/CH5_OBJECTIVE_FOLLOW_THE_INK_RIVER_TIP", 4f));
	}

	private void ActivateLever()
	{
		m_IsActive = false;
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.SetActive(active: true);
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsClogged)
		{
			m_Lever.OnInteracted -= HandleLeverOnInteracted;
			m_Lever.AllowShimmer(_active: false);
			m_Lever.ForceRemoveEffects();
			m_InteriorCollider.SetActive(true);
			m_IsActive = true;
			ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_LeverOnPosition, 0.25f, false), (Ease)7);
			m_LeverLight.SetActive(true);
			GameManager.Instance.Player.transform.SetParent(m_Content);
			GameManager.Instance.Player.SetLockedMovement(active: true);
			GameManager.Instance.Player.LockRotation(40f, 35f);
			ThrottleSequence();
			m_BoatAudioControl.ThrottleOn();
			this.OnThrottled.Send(this);
		}
	}

	private Sequence ThrottleSequence()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(m_BoatSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Content, m_BehindLocation.localPosition, 6f, false), (Ease)7));
		num += 2f;
		TweenSettingsExtensions.Insert(m_BoatSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Content, Vector3.zero, 18f, false), (Ease)7));
		return m_BoatSequence;
	}

	public void Release()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		this.OnReleased.Send(this);
		m_BoatAudioControl.ThrottleOff();
		if (!m_IsActive)
		{
			ResetBoat();
			ActivateLever();
		}
		else
		{
			m_IsActive = false;
			ResetBoat();
			TweenSettingsExtensions.OnComplete<Sequence>(ReleaseSequence(), new TweenCallback(ActivateLever));
		}
	}

	private void ResetBoat()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_LeverOffPosition, 0.25f, false), (Ease)7);
		m_LeverLight.SetActive(false);
		GameManager.Instance.Player.SetLockedMovement(active: false);
		GameManager.Instance.Player.UnlockRotation();
		m_Lever.AllowShimmer(_active: false);
		m_Lever.ForceRemoveEffects();
	}

	public Sequence ReleaseSequence()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(m_BoatSequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Content, m_ForwardLocation.localPosition, 3f, false), (Ease)6));
		return m_BoatSequence;
	}

	public void ForceFinalPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		m_Content.localPosition = m_ForwardLocation.localPosition;
	}

	public void ForceMidPosition()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		m_IsActive = false;
		Release();
		KillSequence();
		ShortcutExtensions.DOKill((Component)(object)m_Content, false);
		m_Content.localPosition = Vector3.zero;
	}

	public void UnlockBoat()
	{
		m_ExteriorCollider.SetActive(false);
		m_InteriorCollider.SetActive(false);
	}

	public void FinalBoatPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		val.transform.position = GameManager.Instance.Player.transform.position;
		val.transform.eulerAngles = GameManager.Instance.Player.transform.eulerAngles;
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		GameManager.Instance.Player.SetLockedMovement(active: false);
		GameManager.Instance.Player.UnlockRotation();
		GameManager.Instance.Player.GoToAndLookAt(val.transform);
		Vector3 zero = Vector3.zero;
		zero.x = GameManager.Instance.Player.HeadContainer.eulerAngles.x;
		Vector3 zero2 = Vector3.zero;
		zero2.y = val.transform.eulerAngles.y;
		GameManager.Instance.Player.LookRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
	}

	public void Clog()
	{
		Release();
		ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 1f, 0.25f, 15, 90f, false, false);
		m_IsClogged = true;
		m_BoatAudioControl.EngineOff();
		CH5BoatClogger cH5BoatClogger = m_InkCloggers[Random.Range(0, 3)];
		cH5BoatClogger.OnHit += HandleCloggerOnHit;
		cH5BoatClogger.Activate();
		m_ClogCount++;
		CH5BoatClogger cH5BoatClogger2 = m_InkCloggers[Random.Range(3, 6)];
		cH5BoatClogger2.OnHit += HandleCloggerOnHit;
		cH5BoatClogger2.Activate();
		m_ClogCount++;
		CH5BoatClogger cH5BoatClogger3 = m_InkCloggers[Random.Range(6, 9)];
		cH5BoatClogger3.OnHit += HandleCloggerOnHit;
		cH5BoatClogger3.Activate();
		m_ClogCount++;
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.SetActive(active: false);
		if (!m_IsInitialClog)
		{
			m_IsInitialClog = true;
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_UNCLOG_THE_PADDLEWHEEL", "OBJECTIVES/CH5_OBJECTIVE_UNCLOG_THE_PADDLEWHEEL_TIP", 4f));
		}
	}

	public void ForceUnclog()
	{
		for (int i = 0; i < m_InkCloggers.Length; i++)
		{
			m_InkCloggers[i].Reset();
			m_InkCloggers[i].OnHit -= HandleCloggerOnHit;
		}
		m_ClogCount = 0;
		m_IsClogged = false;
		m_BoatAudioControl.EngineOn();
		m_Lever.OnInteracted += HandleLeverOnInteracted;
		m_Lever.SetActive(active: true);
	}

	private void HandleCloggerOnHit(object sender, EventArgs e)
	{
		(sender as CH5BoatClogger).OnHit -= HandleCloggerOnHit;
		GameManager.Instance.AudioManager.Play(m_ThickInkPickup);
		m_ClogCount--;
		if (m_ClogCount <= 0)
		{
			m_IsClogged = false;
			m_BoatAudioControl.EngineOn();
			m_Lever.OnInteracted += HandleLeverOnInteracted;
			m_Lever.SetActive(active: true);
		}
	}

	public void Disable()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		m_IsActive = false;
		m_Lever.OnInteracted -= HandleLeverOnInteracted;
		m_Lever.AllowShimmer(_active: false);
		m_Lever.ForceRemoveEffects();
		m_Lever.SetActive(active: false);
		m_LeverLight.SetActive(false);
		ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_LeverOffPosition, 0.5f, false), (Ease)7), new TweenCallback(DisableOnComplete));
	}

	private void DisableOnComplete()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		m_BoatAudioControl.EngineOff();
		m_EngineLight.SetActive(false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_EngineButton.transform, m_EngineOffPosition, 0.25f, false), (Ease)6);
	}

	public void RemoveColliders()
	{
		m_ExteriorCollider.SetActive(false);
		m_InteriorCollider.SetActive(false);
	}

	private void KillSequence()
	{
		if (m_BoatSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_BoatSequence, false);
			m_BoatSequence = null;
		}
	}

	private void ResetSequence()
	{
		KillSequence();
		m_BoatSequence = DOTween.Sequence();
	}

	protected override void OnDisposed()
	{
		this.OnThrottled = null;
		this.OnReleased = null;
		KillSequence();
		base.OnDisposed();
	}
}
