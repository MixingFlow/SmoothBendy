using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class VisionEffectController : TMGMonoBehaviour
{
	private const string VISION_EFFECT_MODE = "_EffectMode";

	[SerializeField]
	private AmplifyPostProcess PostProcessScript;

	[SerializeField]
	private Material VisionMaterial;

	private float EffectValue;

	private bool Dev_Toggle;

	private Sequence m_Sequence;

	public event EventHandler OnStart;

	public event EventHandler OnStop;

	public override void Init()
	{
		base.Init();
		PostProcessScript = ((Component)this).GetComponent<AmplifyPostProcess>();
		DisableVisionEffect();
	}

	public void BeginEffect(bool isDeath = false)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(EnableVisionEffect));
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(SendOnStart));
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(LoopVisionEffect));
		S13AudioManager.Instance.InvokeEvent((!isDeath) ? "evt_horror_vision_start" : "evt_deathtunnel_start");
	}

	public void EndEffect(bool isDeath = false)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		ResetSequence();
		float num = 0.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(EnableVisionEffect));
		num += 0.5f;
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(SendOnStop));
		TweenSettingsExtensions.InsertCallback(m_Sequence, num, new TweenCallback(DisableVisionEffect));
		S13AudioManager.Instance.InvokeEvent((!isDeath) ? "evt_horror_vision_stop" : "evt_deathtunnel_stop");
	}

	private void LoopVisionEffect()
	{
		SetVisionEffect(1);
	}

	private void EnableVisionEffect()
	{
		SetVisionEffect(2);
	}

	private void DisableVisionEffect()
	{
		SetVisionEffect(0);
	}

	private void SetVisionEffect(int visionMode)
	{
		VisionMaterial.SetFloat("_EffectMode", (float)visionMode);
		if (visionMode == 0)
		{
			((Behaviour)PostProcessScript).enabled = false;
		}
		else
		{
			((Behaviour)PostProcessScript).enabled = true;
		}
	}

	private void Update()
	{
	}

	private void SendOnStart()
	{
		this.OnStart.Send(this);
	}

	private void SendOnStop()
	{
		this.OnStop.Send(this);
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		this.OnStart = null;
		this.OnStop = null;
		base.OnDisposed();
	}
}
