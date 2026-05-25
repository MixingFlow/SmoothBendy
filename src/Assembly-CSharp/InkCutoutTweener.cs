using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class InkCutoutTweener : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_OnEnable = true;

	[SerializeField]
	private Transform m_MeshRendererParent;

	private MeshRenderer[] m_MeshRenderers;

	public override void Init()
	{
		base.Init();
		m_MeshRenderers = ((Component)m_MeshRendererParent).GetComponentsInChildren<MeshRenderer>();
	}

	public override void OnEnable()
	{
		if (!m_OnEnable)
		{
			return;
		}
		for (int i = 0; i < m_MeshRenderers.Length; i++)
		{
			MeshRenderer val = m_MeshRenderers[i];
			if (((Renderer)val).material.HasProperty("_Cutout"))
			{
				((Renderer)val).material.SetFloat("_Cutout", 0.05f);
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(((Renderer)val).material, 0.5f, "_Cutout", 0.75f), (Ease)1);
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < m_MeshRenderers.Length; i++)
		{
			ShortcutExtensions.DOKill(((Renderer)m_MeshRenderers[i]).material, false);
		}
		base.OnDisposed();
	}
}
