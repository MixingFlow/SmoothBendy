using System.Collections;
using TMG.Core;
using UnityEngine;

public class Broken_Mesh_Dissolve : TMGMonoBehaviour
{
	private MeshRenderer[] m_Renderers;

	private MaterialPropertyBlock m_MaterialProperties;

	private float m_FadeAmmount;

	public override void InitOnComplete()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		base.InitOnComplete();
		m_Renderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		m_MaterialProperties = new MaterialPropertyBlock();
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			if (i == 0)
			{
				m_MaterialProperties.SetTexture("_MainTex", ((Renderer)m_Renderers[i]).sharedMaterial.mainTexture);
			}
			((Renderer)m_Renderers[i]).sharedMaterial = GameManager.Instance.AssetManager.GetAsset<Material>("DissolveMaterial");
			((Renderer)m_Renderers[i]).SetPropertyBlock(m_MaterialProperties);
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		((MonoBehaviour)this).StartCoroutine(FadeOut());
	}

	private IEnumerator FadeOut()
	{
		yield return (object)new WaitForSeconds(15f);
		while (m_FadeAmmount < 1f)
		{
			if (!base.IsDisposed || !GameManager.Instance.isPaused)
			{
				m_MaterialProperties.SetFloat("_Dissolve", m_FadeAmmount += Time.deltaTime / 2f);
				for (int i = 0; i < m_Renderers.Length; i++)
				{
					((Renderer)m_Renderers[i]).SetPropertyBlock(m_MaterialProperties);
				}
			}
			yield return (object)new WaitForEndOfFrame();
		}
		Dispose();
	}
}
