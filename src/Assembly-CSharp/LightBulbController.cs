using System;
using TMG.Core;
using UnityEngine;

public class LightBulbController : TMGMonoBehaviour
{
	[SerializeField]
	private Light m_Light;

	[SerializeField]
	private MeshRenderer m_LightRenderer;

	[SerializeField]
	private int m_MaterialIndex = 1;

	public bool IsOn => ((Behaviour)m_Light).enabled;

	public event EventHandler OnTurnedOn;

	public event EventHandler OnTurnedOff;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
	}

	public void TurnOn()
	{
		((Renderer)m_LightRenderer).materials[m_MaterialIndex].SetInt("_LightOn", 1);
		((Component)m_Light).gameObject.SetActive(true);
		this.OnTurnedOn.Send(this);
	}

	public void TurnOff()
	{
		((Renderer)m_LightRenderer).materials[m_MaterialIndex].SetInt("_LightOn", 0);
		((Component)m_Light).gameObject.SetActive(false);
		this.OnTurnedOff.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnTurnedOn = null;
		this.OnTurnedOff = null;
		base.OnDisposed();
	}
}
