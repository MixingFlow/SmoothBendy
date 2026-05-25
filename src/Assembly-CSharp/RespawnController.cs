using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RespawnController
{
	[SerializeField]
	private Transform m_SpawnerParent;

	[SerializeField]
	private List<PlayerSpawnNode> m_IgnoreSpawners;

	private List<PlayerSpawnNode> m_SpawnPoints = new List<PlayerSpawnNode>();

	public event EventHandler OnSpawned;

	public void Activate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		foreach (Transform item in m_SpawnerParent)
		{
			Transform val = item;
			PlayerSpawnNode component = ((Component)val).GetComponent<PlayerSpawnNode>();
			if ((Object)(object)component != (Object)null && !m_IgnoreSpawners.Contains(component))
			{
				m_SpawnPoints.Add(component);
			}
		}
	}

	public void Respawn()
	{
		PlayerSpawnNode closestSpawnPoint = GetClosestSpawnPoint();
		GameManager.Instance.Player.GoToAndLookAt(closestSpawnPoint.transform);
		this.OnSpawned.Send(this);
	}

	private PlayerSpawnNode GetClosestSpawnPoint()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		PlayerSpawnNode result = null;
		float num = float.PositiveInfinity;
		Vector3 position = GameManager.Instance.Player.transform.position;
		foreach (PlayerSpawnNode spawnPoint in m_SpawnPoints)
		{
			Vector3 position2 = spawnPoint.transform.position;
			float num2 = Vector3.Distance(position, position2) + Mathf.Abs(position.y - position2.y);
			if (num2 < num)
			{
				num = num2;
				result = spawnPoint;
			}
		}
		return result;
	}

	public void AddSpawner(PlayerSpawnNode spawner)
	{
		if (!m_SpawnPoints.Contains(spawner))
		{
			m_SpawnPoints.Add(spawner);
		}
	}

	public void RemoveSpawner(PlayerSpawnNode spawner)
	{
		if (m_SpawnPoints.Contains(spawner))
		{
			m_SpawnPoints.Remove(spawner);
		}
	}
}
