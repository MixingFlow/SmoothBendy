using System;
using UnityEngine;

public class CH4WarehousePowerStation : BaseController
{
	[SerializeField]
	private CH3LeverLight[] m_Levers;

	[SerializeField]
	private MeshRenderer[] m_PowerCables;

	private int m_ActiveLights;

	private int m_MaxLights = 4;

	private bool m_IsComplete;

	public int ActiveLights => m_ActiveLights;

	public event EventHandler OnPowerActivated;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_PowerCables.Length; i++)
		{
			((Renderer)m_PowerCables[i]).material.SetFloat("_Shimmer", 0f);
		}
		for (int j = 0; j < m_Levers.Length; j++)
		{
			m_Levers[j].Disable();
			m_Levers[j].TurnOnLights();
		}
	}

	public void ActivatePower()
	{
		if (!m_IsComplete)
		{
			((Renderer)m_PowerCables[m_ActiveLights]).material.SetFloat("_Shimmer", 1f);
			CH3LeverLight cH3LeverLight = m_Levers[m_ActiveLights];
			cH3LeverLight.OnComplete += HandleLeverOnComplete;
			cH3LeverLight.Activate();
		}
	}

	public void ForceActivatePower()
	{
		((Renderer)m_PowerCables[m_ActiveLights]).material.SetFloat("_Shimmer", 0f);
		m_Levers[m_ActiveLights].ForceComplete();
		m_ActiveLights++;
		CheckComplete();
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		m_Levers[m_ActiveLights].OnComplete -= HandleLeverOnComplete;
		((Renderer)m_PowerCables[m_ActiveLights]).material.SetFloat("_Shimmer", 0f);
		this.OnPowerActivated.Send(this);
		m_ActiveLights++;
		CheckComplete();
	}

	private void CheckComplete()
	{
		if (m_ActiveLights >= m_MaxLights)
		{
			m_IsComplete = true;
			Dispose();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
