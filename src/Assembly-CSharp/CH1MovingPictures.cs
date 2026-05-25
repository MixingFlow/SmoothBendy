using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class CH1MovingPictures : TMGMonoBehaviour
{
	[SerializeField]
	private MeshRenderer m_Mesh;

	[Header("Textures")]
	[SerializeField]
	private List<Texture> m_Textures;

	private float m_ResetTimer;

	private float m_TimerMax = 2f;

	private bool m_IsImageChanged;

	private int m_CurrentIndex;

	private int m_NextIndex;

	public override void Init()
	{
		base.Init();
		m_NextIndex = GetNextIndex();
		((Renderer)m_Mesh).material.mainTexture = m_Textures[m_CurrentIndex];
	}

	public void Update()
	{
		if (!((Renderer)m_Mesh).isVisible)
		{
			if (m_IsImageChanged)
			{
				return;
			}
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer > m_TimerMax)
			{
				m_IsImageChanged = true;
				for (m_ResetTimer = 0f; m_NextIndex == m_CurrentIndex; m_NextIndex = GetNextIndex())
				{
				}
				m_CurrentIndex = m_NextIndex;
				((Renderer)m_Mesh).material.mainTexture = m_Textures[m_CurrentIndex];
			}
		}
		else if (m_IsImageChanged)
		{
			m_IsImageChanged = false;
			m_ResetTimer = 0f;
		}
	}

	private int GetNextIndex()
	{
		return Random.Range(0, m_Textures.Count - 1);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
