using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BendySpawnerList : TMGMonoBehaviour
{
	[SerializeField]
	private List<BendySpawner> m_BendySpawners;

	public List<BendySpawner> BendySpawners => m_BendySpawners;

	public bool InUse { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		for (int i = 0; i < m_BendySpawners.Count; i++)
		{
			m_BendySpawners[i].Initialize(this);
		}
	}

	public void Use()
	{
		InUse = true;
	}

	public void Reset()
	{
		InUse = false;
		for (int i = 0; i < m_BendySpawners.Count; i++)
		{
			m_BendySpawners[i].Disable();
		}
	}

	protected override void OnDisposed()
	{
		if (m_BendySpawners != null)
		{
			m_BendySpawners.Clear();
			m_BendySpawners = null;
		}
		base.OnDisposed();
	}
}
