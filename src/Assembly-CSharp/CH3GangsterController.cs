using System;
using System.Collections.Generic;
using UnityEngine;

public class CH3GangsterController : BaseController
{
	[Header("Piper")]
	[SerializeField]
	private SpecialSpawnerNode m_PiperSpawner;

	[SerializeField]
	private WaypointList m_PiperWaypointList;

	[Header("Striker")]
	[SerializeField]
	private SpecialSpawnerNode m_StrikerSpawner;

	[SerializeField]
	private WaypointList m_StrikerWaypointList;

	[Header("Fisher")]
	[SerializeField]
	private SpecialSpawnerNode m_FisherSpawner;

	[SerializeField]
	private WaypointList m_FisherWaypointList;

	private ButcherGangAi m_Piper;

	private ButcherGangAi m_Striker;

	private ButcherGangAi m_Fisher;

	private List<ButcherGangAi> m_Gangsters = new List<ButcherGangAi>();

	public bool IsActive { get; private set; }

	public void SetActive(bool active)
	{
		IsActive = active;
		if (IsActive)
		{
			KillAllGangsters();
			ReSpawn();
		}
		else
		{
			KillAllGangsters();
		}
	}

	public void ReSpawn()
	{
		SpawnButcherGangAi(ref m_Piper, "GamePlay/Characters/Ai_Piper", m_PiperSpawner, m_PiperWaypointList);
		SpawnButcherGangAi(ref m_Striker, "GamePlay/Characters/Ai_Striker", m_StrikerSpawner, m_StrikerWaypointList);
		SpawnButcherGangAi(ref m_Fisher, "GamePlay/Characters/Ai_Fisher", m_FisherSpawner, m_FisherWaypointList);
	}

	private void SpawnButcherGangAi(ref ButcherGangAi butcherGangAi, string prefab, SpecialSpawnerNode spawnPoint, WaypointList waypointList)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Gangsters.Contains(butcherGangAi))
		{
			butcherGangAi = GameManager.Instance.AssetManager.CreateAsset<ButcherGangAi>(prefab);
			butcherGangAi.transform.position = spawnPoint.transform.position;
			butcherGangAi.UpdateWaypointList(waypointList.Waypoints);
			butcherGangAi.OnDeath += HandleGangsterOnDeath;
			m_Gangsters.Add(butcherGangAi);
		}
	}

	private void HandleGangsterOnDeath(object sender, EventArgs e)
	{
		ButcherGangAi butcherGangAi = (ButcherGangAi)sender;
		butcherGangAi.OnDeath -= HandleGangsterOnDeath;
		if (m_Gangsters.Contains(butcherGangAi))
		{
			m_Gangsters.Remove(butcherGangAi);
			if (!((object)butcherGangAi).Equals((object)m_Piper))
			{
				SpawnButcherGangAi(ref m_Piper, "GamePlay/Characters/Ai_Piper", m_PiperSpawner, m_PiperWaypointList);
			}
			if (!((object)butcherGangAi).Equals((object)m_Striker))
			{
				SpawnButcherGangAi(ref m_Striker, "GamePlay/Characters/Ai_Striker", m_StrikerSpawner, m_StrikerWaypointList);
			}
			if (!((object)butcherGangAi).Equals((object)m_Fisher))
			{
				SpawnButcherGangAi(ref m_Fisher, "GamePlay/Characters/Ai_Fisher", m_FisherSpawner, m_FisherWaypointList);
			}
		}
	}

	private void KillAllGangsters()
	{
		if ((Object)(object)m_Piper != (Object)null)
		{
			m_Piper.Dispose();
		}
		if ((Object)(object)m_Striker != (Object)null)
		{
			m_Striker.Dispose();
		}
		if ((Object)(object)m_Fisher != (Object)null)
		{
			m_Fisher.Dispose();
		}
	}

	protected override void OnDisposed()
	{
		if (m_Gangsters != null)
		{
			m_Gangsters.Clear();
			m_Gangsters = null;
		}
		base.OnDisposed();
	}
}
