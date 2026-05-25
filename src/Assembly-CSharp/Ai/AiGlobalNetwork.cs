using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

namespace Ai;

public class AiGlobalNetwork : TMGAbstractDisposable
{
	private bool m_IsBuilt;

	public float NodeNearbyDistance => 10f;

	public List<BaseAiController> ConnectedAi { get; private set; }

	public List<PathfinderNode> GlobalPathNodeList { get; private set; }

	public List<Ch4_SoundBasedAI> ListeningAi { get; private set; }

	public List<Transform> AllyTargets { get; private set; }

	public Vector3 SoundLocation { get; private set; }

	public static AiGlobalNetwork Create()
	{
		AiGlobalNetwork aiGlobalNetwork = new AiGlobalNetwork();
		aiGlobalNetwork.ResetConnectedAi();
		aiGlobalNetwork.ResetNodeList();
		aiGlobalNetwork.ResetSoundAi();
		aiGlobalNetwork.ResetAllyTargets();
		return aiGlobalNetwork;
	}

	public void RebuildPathing()
	{
		m_IsBuilt = false;
		BuildPathing();
	}

	public void BuildPathing()
	{
		if (m_IsBuilt)
		{
			return;
		}
		ResetNodeList();
		PathfinderNode[] array = Resources.FindObjectsOfTypeAll<PathfinderNode>();
		foreach (PathfinderNode item in array)
		{
			GlobalPathNodeList.Add(item);
		}
		bool flag = false;
		for (int j = 0; j < GlobalPathNodeList.Count; j++)
		{
			PathfinderNode pathfinderNode = GlobalPathNodeList[j];
			if (!flag)
			{
				pathfinderNode.Init(j);
				if (j >= GlobalPathNodeList.Count - 1)
				{
					j = -1;
					flag = true;
				}
			}
			else
			{
				pathfinderNode.RegisterNearbyNodes(GlobalPathNodeList);
			}
		}
		m_IsBuilt = true;
	}

	public List<PathfinderNode> SolvePath(Vector3 currentPosition, Vector3 targetPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		PathfinderNode pathfinderNode = FindStartNode(currentPosition);
		PathfinderNode pathfinderNode2 = FindEndNode(targetPosition);
		if (!Object.op_Implicit((Object)(object)pathfinderNode))
		{
			Debug.LogWarning((object)"[UNABLE TO SOLVE PATH] - Pathfinding needs to be rebuilt!");
			return null;
		}
		if (!Object.op_Implicit((Object)(object)pathfinderNode2))
		{
			return null;
		}
		List<PathfinderNode> list = new List<PathfinderNode>();
		List<PathfinderNode> list2 = new List<PathfinderNode>();
		List<PathfinderNode> list3 = new List<PathfinderNode>();
		List<int> list4 = new List<int>();
		list2.Add(pathfinderNode);
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		while (!flag)
		{
			if (list2.Count > 0)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					PathfinderNode pathfinderNode3 = list2[i];
					pathfinderNode3.NodeScore = num;
					list3.Add(pathfinderNode3);
					list4.Add(pathfinderNode3.ID);
					if (pathfinderNode3.ID == pathfinderNode2.ID)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					num++;
					if (num > 1000)
					{
						flag = true;
						flag2 = true;
					}
					list2 = FindClosestNearNodes(list2, list4);
				}
			}
			else
			{
				flag = true;
				flag2 = true;
			}
		}
		if (!flag2)
		{
			bool flag3 = false;
			PathfinderNode pathfinderNode4 = pathfinderNode2;
			list.Add(pathfinderNode2);
			while (!flag3)
			{
				if (list3.Count == 0)
				{
					flag3 = true;
					continue;
				}
				bool flag4 = false;
				if ((Object)(object)pathfinderNode2 == (Object)(object)pathfinderNode)
				{
					flag4 = true;
					flag3 = true;
				}
				else
				{
					for (int j = 0; j < list3.Count; j++)
					{
						PathfinderNode pathfinderNode5 = list3[j];
						if (pathfinderNode4.ConnectedNodeIDs.Contains(pathfinderNode5.ID) && pathfinderNode5.NodeScore == pathfinderNode4.NodeScore - 1)
						{
							list.Add(pathfinderNode5);
							pathfinderNode4 = pathfinderNode5;
							flag4 = true;
							if ((Object)(object)pathfinderNode5 == (Object)(object)pathfinderNode)
							{
								flag3 = true;
							}
						}
					}
				}
				if (!flag4 && !flag3)
				{
					Debug.LogWarning((object)("[UNABLE TO SOLVE PATH] - Path found, but somehow un-solveable? Node: " + ((Object)pathfinderNode2).name + "|" + ((Object)pathfinderNode).name + "C:" + flag4 + "S:" + flag3));
					flag3 = true;
				}
			}
			list.Reverse();
		}
		else
		{
			Debug.LogWarning((object)("[UNABLE TO SOLVE PATH] - No path to end node " + ((Object)pathfinderNode2).name));
		}
		return list;
	}

	private PathfinderNode FindStartNode(Vector3 currentPosition)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		PathfinderNode result = null;
		float num = 0f;
		float num2 = -1f;
		for (int i = 0; i < GlobalPathNodeList.Count; i++)
		{
			PathfinderNode pathfinderNode = GlobalPathNodeList[i];
			if (Object.op_Implicit((Object)(object)pathfinderNode) && pathfinderNode.ConnectedNodes.Count != 0)
			{
				num = Vector3.Distance(pathfinderNode.Position, currentPosition);
				if (num < num2 || num2 < 0f)
				{
					result = pathfinderNode;
					num2 = num;
				}
			}
		}
		return result;
	}

	public PathfinderNode FindEndNode(Vector3 targetPosition)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		PathfinderNode result = null;
		float num = 0f;
		float num2 = -1f;
		for (int i = 0; i < GlobalPathNodeList.Count; i++)
		{
			PathfinderNode pathfinderNode = GlobalPathNodeList[i];
			if (Object.op_Implicit((Object)(object)pathfinderNode) && pathfinderNode.ConnectedNodes.Count != 0)
			{
				num = Vector3.Distance(pathfinderNode.Position, targetPosition);
				Vector3 val = targetPosition + Vector3.up * 2f;
				Vector3 val2 = pathfinderNode.Position + Vector3.up * 2f;
				if (!Physics.Linecast(val, val2, LayerMask.GetMask(new string[1] { "Default" }), (QueryTriggerInteraction)1) && (num < num2 || num2 < 0f))
				{
					result = pathfinderNode;
					num2 = num;
				}
			}
		}
		return result;
	}

	private List<PathfinderNode> FindClosestNearNodes(List<PathfinderNode> OpenNodes, List<int> ScoredNodes)
	{
		List<PathfinderNode> list = new List<PathfinderNode>();
		for (int i = 0; i < OpenNodes.Count; i++)
		{
			for (int j = 0; j < OpenNodes[i].ConnectedNodes.Count; j++)
			{
				if (!list.Contains(OpenNodes[i].ConnectedNodes[j]) && !ScoredNodes.Contains(OpenNodes[i].ConnectedNodes[j].ID))
				{
					list.Add(OpenNodes[i].ConnectedNodes[j]);
				}
			}
		}
		return list;
	}

	public void CheckCombatStatus()
	{
		CombatStatus combatStatus = CombatStatus.Idle;
		if (GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			combatStatus = CombatStatus.Hiding;
		}
		for (int i = 0; i < ConnectedAi.Count; i++)
		{
			BaseAiController baseAiController = ConnectedAi[i];
			if (Object.op_Implicit((Object)(object)baseAiController.CurrentTarget) && GameManager.Instance.Player.CurrentStatus != CombatStatus.Hiding)
			{
				combatStatus = CombatStatus.InCombat;
			}
		}
		GameManager.Instance.Player.SetCombatStatus(combatStatus);
	}

	public void RemoveNode(PathfinderNode node)
	{
		for (int i = 0; i < GlobalPathNodeList.Count; i++)
		{
			PathfinderNode pathfinderNode = GlobalPathNodeList[i];
			for (int num = pathfinderNode.ConnectedNodes.Count - 1; num > -1; num--)
			{
				PathfinderNode pathfinderNode2 = pathfinderNode.ConnectedNodes[num];
				if (((object)pathfinderNode2).Equals((object)node))
				{
					pathfinderNode.ConnectedNodes.Remove(pathfinderNode2);
				}
			}
		}
		if (GlobalPathNodeList.Contains(node))
		{
			GlobalPathNodeList.Remove(node);
		}
	}

	public void SetNoiseLocation(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		SoundLocation = position;
		if (ListeningAi != null)
		{
			for (int i = 0; i < ListeningAi.Count; i++)
			{
				ListeningAi[i].SetNoiseLocation(SoundLocation);
			}
		}
	}

	public void AddSoundAi(Ch4_SoundBasedAI ai)
	{
		if (ListeningAi != null && !ListeningAi.Contains(ai))
		{
			ListeningAi.Add(ai);
		}
	}

	public void RemoveSoundAi(Ch4_SoundBasedAI ai)
	{
		if (ListeningAi != null && ListeningAi.Contains(ai))
		{
			ListeningAi.Remove(ai);
		}
	}

	public void ClearSoundAi()
	{
		if (ListeningAi != null)
		{
			ListeningAi.Clear();
			ListeningAi = null;
		}
	}

	public void ClearAndDisposeSoundAi()
	{
		if (ListeningAi == null)
		{
			return;
		}
		for (int i = 0; i < ListeningAi.Count; i++)
		{
			Ch4_SoundBasedAI ch4_SoundBasedAI = ListeningAi[i];
			if (Object.op_Implicit((Object)(object)ch4_SoundBasedAI))
			{
				ch4_SoundBasedAI.Dispose();
			}
		}
		ListeningAi.Clear();
		ListeningAi = null;
	}

	private void ResetSoundAi()
	{
		ClearSoundAi();
		ListeningAi = new List<Ch4_SoundBasedAI>();
	}

	public void AddAi(BaseAiController ai)
	{
		if (!ConnectedAi.Contains(ai))
		{
			ConnectedAi.Add(ai);
		}
	}

	public void RemoveAi(BaseAiController ai)
	{
		if (ConnectedAi.Contains(ai))
		{
			ConnectedAi.Remove(ai);
		}
	}

	public void ClearAllAi()
	{
		List<BaseAiController> list = new List<BaseAiController>(ConnectedAi);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Dispose();
		}
	}

	private void ResetConnectedAi()
	{
		ClearConnectedAi();
		ConnectedAi = new List<BaseAiController>();
	}

	private void ClearConnectedAi()
	{
		if (ConnectedAi != null)
		{
			ConnectedAi.Clear();
			ConnectedAi = null;
		}
	}

	private void ResetNodeList()
	{
		ClearNodeList();
		GlobalPathNodeList = new List<PathfinderNode>();
	}

	private void ClearNodeList()
	{
		if (GlobalPathNodeList != null)
		{
			GlobalPathNodeList.Clear();
			GlobalPathNodeList = null;
		}
	}

	public void AddAllyTarget(Transform target)
	{
		if (AllyTargets == null)
		{
			ResetAllyTargets();
		}
		if (!AllyTargets.Contains(target))
		{
			AllyTargets.Add(target);
		}
	}

	public void RemoveAllyTarget(Transform target)
	{
		if (AllyTargets.Contains(target))
		{
			AllyTargets.Remove(target);
		}
	}

	public bool CheckAllyTarget(Transform target)
	{
		if (AllyTargets != null && AllyTargets.Contains(target))
		{
			return true;
		}
		return false;
	}

	public void CleanAllyTargets()
	{
		if (AllyTargets == null)
		{
			return;
		}
		for (int num = AllyTargets.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)AllyTargets[num] == (Object)null)
			{
				AllyTargets.RemoveAt(num);
			}
		}
	}

	private void ResetAllyTargets()
	{
		AllyTargets = new List<Transform>();
	}

	protected override void OnDisposed()
	{
		ClearConnectedAi();
		ClearNodeList();
		base.OnDisposed();
	}
}
