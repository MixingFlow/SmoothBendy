using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class LightFixtureController : TMGMonoBehaviour
{
	[Header("Start")]
	[SerializeField]
	private bool m_StartOn = true;

	[Header("Lights")]
	[SerializeField]
	private List<Light> m_Lights;

	[Header("Renderer")]
	[SerializeField]
	private Renderer m_LightRenderer;

	public override void Init()
	{
		if (!m_StartOn)
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		m_LightRenderer.material.SetFloat("_LightOn", 1f);
		for (int i = 0; i < m_Lights.Count; i++)
		{
			((Component)m_Lights[i]).gameObject.SetActive(true);
		}
	}

	public void TurnOff()
	{
		m_LightRenderer.material.SetFloat("_LightOn", 0f);
		for (int i = 0; i < m_Lights.Count; i++)
		{
			if ((Object)(object)m_Lights[i] != (Object)null)
			{
				((Component)m_Lights[i]).gameObject.SetActive(false);
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
