using System;
using TMG.Core;
using UnityEngine;

public class LightController : TMGMonoBehaviour
{
	[SerializeField]
	private Light[] m_Lights;

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private int m_MaterialIndex = 1;

	public bool IsOn { get; private set; }

	public event EventHandler OnTurnedOn;

	public event EventHandler OnTurnedOff;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
	}

	public void TurnOn()
	{
		((Renderer)m_LightRenderer).materials[m_MaterialIndex].SetInt("_LightOn", 1);
		IsOn = true;
		for (int i = 0; i < m_Lights.Length; i++)
		{
			((Component)m_Lights[i]).gameObject.SetActive(true);
		}
		this.OnTurnedOn.Send(this);
	}

	public void TurnOff()
	{
		((Renderer)m_LightRenderer).materials[m_MaterialIndex].SetInt("_LightOn", 0);
		IsOn = false;
		for (int i = 0; i < m_Lights.Length; i++)
		{
			((Component)m_Lights[i]).gameObject.SetActive(false);
		}
		this.OnTurnedOff.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTurnedOn = null;
		this.OnTurnedOff = null;
		base.OnDisposed();
	}

	public new void Start()
	{
		QualitySettings.pixelLightCount = 20;
	}
}
