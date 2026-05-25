using System;
using TMG.Core;
using UnityEngine;

public class LightFlicker : TMGMonoBehaviour
{
	private enum WaveType
	{
		SIN,
		TRI,
		SQR,
		SAW,
		INV,
		NOISE
	}

	[SerializeField]
	private WaveType m_WaveType;

	[SerializeField]
	private float m_Base;

	[SerializeField]
	private float m_Amplitude;

	[SerializeField]
	private float m_Phase;

	[SerializeField]
	private float m_Frequency;

	private Color m_OriginalColor;

	private float m_OrigintalIntensity;

	private Light m_Light;

	private bool m_IsOff;

	public Color OriginalColor => m_OriginalColor;

	public Light Light => m_Light;

	public override void Init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_Light = ((Component)this).GetComponent<Light>();
		m_OriginalColor = m_Light.color;
		m_OrigintalIntensity = m_Light.intensity;
	}

	public void FixedUpdate()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsOff && !GameManager.Instance.isPaused)
		{
			m_Light.color = m_OriginalColor * EvalWave();
		}
	}

	public void TurnOff()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_IsOff = true;
		m_Light.color = m_OriginalColor;
		m_Light.intensity = m_OrigintalIntensity;
	}

	public void TurnOn()
	{
		m_IsOff = false;
	}

	private float EvalWave()
	{
		float num = 0f;
		float num2 = (Time.time + m_Phase) * m_Frequency;
		num2 -= Mathf.Floor(num2);
		return m_WaveType switch
		{
			WaveType.SIN => Mathf.Sin(num2 * 2f * (float)Math.PI), 
			WaveType.TRI => (!(num2 < 0.5f)) ? (-4f * num2 + 3f) : (4f * num2 - 1f), 
			WaveType.SQR => (!(num2 < 0.5f)) ? (-1f) : 1f, 
			WaveType.SAW => num2, 
			WaveType.INV => 1f - num2, 
			WaveType.NOISE => 1f - Random.value * 2f, 
			_ => 1f, 
		} * (GameManager.Instance.PlayerSettings.flickerLights ? m_Amplitude : num) + m_Base;
	}

	protected override void OnDisposed()
	{
		m_Light = null;
		base.OnDisposed();
	}
}
