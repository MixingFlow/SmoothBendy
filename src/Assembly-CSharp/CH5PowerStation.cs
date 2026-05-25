using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5PowerStation : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_Valve;

	[SerializeField]
	private LightFlicker[] m_Lights;

	private AudioClip m_ValveClip;

	private int m_LightCount;

	public event EventHandler OnStartUpComplete;

	public event EventHandler OnPowerShutDown;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Valve.SetActive(active: false);
		for (int i = 0; i < m_Lights.Length; i++)
		{
			m_Lights[i].TurnOff();
			m_Lights[i].gameObject.SetActive(false);
		}
		m_ValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
	}

	public void Activate()
	{
		m_Valve.OnInteracted += HandleValveOnInteracted;
		m_Valve.SetActive(active: true);
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		m_Valve.OnInteracted -= HandleValveOnInteracted;
		m_Valve.AllowShimmer(_active: false);
		m_Valve.ForceRemoveEffects();
		GameManager.Instance.Player.SetLockedMovement(active: true);
		GameManager.Instance.AudioManager.Play(m_ValveClip);
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOLocalRotate(m_Valve.transform, new Vector3(0f, 0f, 360f), 1.5f, (RotateMode)3), new TweenCallback(ValveOnComplete));
	}

	private void ValveOnComplete()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		Sequence val2 = DOTween.Sequence();
		for (int i = 0; i < m_Lights.Length; i++)
		{
			LightFlicker light = m_Lights[i];
			TweenSettingsExtensions.InsertCallback(val2, 0.5f + 0.25f * (float)i, (TweenCallback)delegate
			{
				light.gameObject.SetActive(true);
			});
		}
		TweenSettingsExtensions.Insert(val2, 0.1f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLookAt(val, m_Lights[1].transform.position, 1f, (AxisConstraint)0, (Vector3?)null), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(StartUpOnComplete));
	}

	private void StartUpOnComplete()
	{
		this.OnStartUpComplete.Send(this);
	}

	public void PowerDown()
	{
		m_Lights[m_LightCount].TurnOn();
		m_LightCount++;
		if (m_LightCount >= m_Lights.Length)
		{
			this.OnPowerShutDown.Send(this);
		}
	}

	protected override void OnDisposed()
	{
		this.OnStartUpComplete = null;
		base.OnDisposed();
	}
}
