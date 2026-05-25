using System;
using System.Collections;
using System.Collections.Generic;
using Ai;
using UnityEngine;

public class CH3SearcherController : BaseController
{
	private class SpawnPoints
	{
		public Vector3 Position;

		public SearcherAi Ai;

		public bool isInUse;
	}

	private const int MIN_SEARCHERS = 12;

	[SerializeField]
	private Transform m_SearcherSpawnersParent;

	private List<SpawnPoints> m_SearcherSpawnPoints = new List<SpawnPoints>();

	private List<SearcherAi> m_Searchers = new List<SearcherAi>();

	public bool IsActive { get; private set; }

	public override void InitOnComplete()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		foreach (Transform item in m_SearcherSpawnersParent)
		{
			Transform val = item;
			SpawnPoints spawnPoints = new SpawnPoints();
			Vector3 position = val.position;
			spawnPoints.Position = position;
			m_SearcherSpawnPoints.Add(spawnPoints);
			Transform val2 = GameManager.Instance.AssetManager.CreateAsset<Transform>("GamePlay/Particles/Searcher_SpawnPool");
			val2.position = spawnPoints.Position;
		}
	}

	public void SetActive(bool active)
	{
		IsActive = active;
		if (IsActive)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			KillAllSearchers();
			ClearAllSpawnPoints();
			m_SearcherSpawnPoints.Shuffle();
			int num = m_SearcherSpawnPoints.Count / 2;
			int count = ((num <= 12) ? 12 : num);
			CallSearcher(count);
		}
		else
		{
			((MonoBehaviour)this).StopAllCoroutines();
			KillAllSearchers();
		}
	}

	private void CallSearcher(int count = 1, SpawnPoints ignorePoint = null)
	{
		((MonoBehaviour)this).StartCoroutine(SpawnSearchers(count, ignorePoint));
	}

	private IEnumerator SpawnSearchers(int count = 1, SpawnPoints ignorePoint = null)
	{
		for (int i = 0; i < count; i++)
		{
			yield return (object)new WaitForSeconds(1f);
			yield return (object)new WaitForEndOfFrame();
			SearcherAi searcher = GameManager.Instance.AssetManager.CreateAsset<SearcherAi>("GamePlay/Characters/Ai_Searcher");
			m_SearcherSpawnPoints.Shuffle();
			foreach (SpawnPoints searcherSpawnPoint in m_SearcherSpawnPoints)
			{
				if (!searcherSpawnPoint.isInUse && !searcherSpawnPoint.Equals(ignorePoint))
				{
					searcherSpawnPoint.isInUse = true;
					searcherSpawnPoint.Ai = searcher;
					searcher.transform.position = searcherSpawnPoint.Position;
					searcher.OnRespawn += HandleSearcherOnRespawn;
					break;
				}
			}
			m_Searchers.Add(searcher);
		}
	}

	private void HandleSearcherOnRespawn(object sender, EventArgs e)
	{
		SearcherAi searcherAi = (SearcherAi)sender;
		searcherAi.OnRespawn -= HandleSearcherOnRespawn;
		if (m_Searchers.Contains(searcherAi))
		{
			m_Searchers.Remove(searcherAi);
		}
		m_SearcherSpawnPoints.Shuffle();
		foreach (SpawnPoints searcherSpawnPoint in m_SearcherSpawnPoints)
		{
			if ((Object)(object)searcherSpawnPoint.Ai != (Object)null && ((object)searcherSpawnPoint.Ai).Equals((object)searcherAi))
			{
				searcherSpawnPoint.Ai = null;
				searcherSpawnPoint.isInUse = false;
				CallSearcher(1, searcherSpawnPoint);
				break;
			}
		}
	}

	private void KillAllSearchers()
	{
		for (int i = 0; i < m_Searchers.Count; i++)
		{
			m_Searchers[i].Dispose();
		}
		m_Searchers.Clear();
	}

	private void ClearAllSpawnPoints()
	{
		for (int i = 0; i < m_SearcherSpawnPoints.Count; i++)
		{
			SpawnPoints spawnPoints = m_SearcherSpawnPoints[i];
			spawnPoints.isInUse = false;
			spawnPoints.Ai = null;
		}
	}

	protected override void OnDisposed()
	{
		if (m_Searchers != null)
		{
			m_Searchers.Clear();
			m_Searchers = null;
		}
		if (m_SearcherSpawnPoints != null)
		{
			m_SearcherSpawnPoints.Clear();
			m_SearcherSpawnPoints = null;
		}
		base.OnDisposed();
	}
}
