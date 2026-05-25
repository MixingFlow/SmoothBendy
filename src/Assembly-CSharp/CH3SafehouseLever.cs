using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3SafehouseLever : TMGMonoBehaviour
{
	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_Lever;

	[SerializeField]
	private GameObject m_Innards;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_OffPosition;

	[SerializeField]
	private Transform m_OnPosition;

	[Header("Interactables")]
	[SerializeField]
	private Interactable m_Panel;

	[SerializeField]
	private Interactable m_Handle;

	[Header("Lights")]
	[SerializeField]
	private Light m_Light;

	private AudioClip m_HandlePickupClip;

	private AudioClip m_HandlePlaceClip;

	private AudioClip m_LeverPullClip;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HandlePickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_safehousehandlepickup");
		m_HandlePlaceClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_safehousehandleplace");
		m_LeverPullClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_LeverPull");
		((Behaviour)m_Light).enabled = false;
		m_Panel.SetActive(active: false);
		m_Lever.SetActive(false);
		m_Innards.SetActive(false);
		m_Handle.SetActive(active: false);
		m_Handle.gameObject.SetActive(false);
	}

	public void Activate()
	{
		m_Innards.SetActive(true);
		m_Handle.gameObject.SetActive(true);
		m_Handle.SetActive(active: true);
		m_Handle.OnInteracted += HandleHandleOnInteracted;
		m_Handle.SetActive(active: true);
	}

	public void ForceComplete()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		m_Innards.SetActive(true);
		m_Lever.SetActive(true);
		m_Lever.transform.localPosition = m_OnPosition.localPosition;
		((Behaviour)m_Light).enabled = true;
	}

	private void HandleHandleOnInteracted(object sender, EventArgs e)
	{
		m_Handle.OnInteracted -= HandleHandleOnInteracted;
		m_Handle.Dispose();
		GameManager.Instance.AudioManager.Play(m_HandlePickupClip);
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_05", "OBJECTIVES/CH3_OBJECTIVE_05_TIP", 4f));
		m_Panel.OnInteracted += HandlePanelOnInteracted;
		m_Panel.SetActive(active: true);
	}

	private void HandlePanelOnInteracted(object sender, EventArgs e)
	{
		m_Panel.OnInteracted -= HandlePanelOnInteracted;
		m_Panel.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_HandlePlaceClip);
		m_Lever.SetActive(true);
		m_Panel.OnInteracted += HandleLeverOnInteracted;
		m_Panel.SetActive(active: true);
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		m_Panel.OnInteracted -= HandleLeverOnInteracted;
		m_Panel.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_LeverPullClip);
		ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Lever.transform, m_OnPosition.localPosition, 0.5f, false), (Ease)28), new TweenCallback(LeverOnComplete));
	}

	private void LeverOnComplete()
	{
		((Behaviour)m_Light).enabled = true;
		this.OnComplete.Send(this);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Lever.transform, false);
		this.OnComplete = null;
		m_Panel.OnInteracted -= HandlePanelOnInteracted;
		m_Handle.OnInteracted -= HandleHandleOnInteracted;
		m_HandlePickupClip = null;
		m_HandlePlaceClip = null;
		m_LeverPullClip = null;
		base.OnDisposed();
	}
}
