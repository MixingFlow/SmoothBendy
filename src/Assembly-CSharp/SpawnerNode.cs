using TMG.Core;
using UnityEngine;

public class SpawnerNode : TMGMonoBehaviour
{
	[SerializeField]
	private bool m_IsSearcherSpawner;

	[SerializeField]
	private bool m_IsPlayerSpawner;

	public void IsSearcherNode(bool isSearcherNode)
	{
		m_IsSearcherSpawner = isSearcherNode;
	}

	public void IsPlayerNode(bool isPlayerNode)
	{
		m_IsPlayerSpawner = true;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
