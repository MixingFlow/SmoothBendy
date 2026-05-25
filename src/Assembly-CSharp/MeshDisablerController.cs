using TMG.Core;
using UnityEngine;

public class MeshDisablerController : TMGMonoBehaviour
{
	private Renderer m_renderer;

	public void SetMeshEnabled(bool isEnabled)
	{
		if ((Object)(object)m_renderer == (Object)null)
		{
			m_renderer = (Renderer)(object)((Component)this).GetComponent<MeshRenderer>();
			if ((Object)(object)m_renderer == (Object)null)
			{
				m_renderer = (Renderer)(object)((Component)this).GetComponentInChildren<MeshRenderer>();
			}
			if ((Object)(object)m_renderer != (Object)null)
			{
				m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			m_renderer.enabled = isEnabled;
		}
	}

	public void SetSkinnedMeshEnabled(bool isEnabled)
	{
		if ((Object)(object)m_renderer == (Object)null)
		{
			m_renderer = (Renderer)(object)((Component)this).GetComponent<SkinnedMeshRenderer>();
			if ((Object)(object)m_renderer == (Object)null)
			{
				m_renderer = (Renderer)(object)((Component)this).GetComponentInChildren<SkinnedMeshRenderer>();
			}
			if ((Object)(object)m_renderer != (Object)null)
			{
				m_renderer.enabled = isEnabled;
			}
		}
		else
		{
			m_renderer.enabled = isEnabled;
		}
	}
}
