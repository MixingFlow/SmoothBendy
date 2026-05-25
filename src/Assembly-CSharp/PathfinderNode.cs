using System;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

[Serializable]
public class PathfinderNode : TMGMonoBehaviour
{
	[Header("Manual Node Connection List")]
	[SerializeField]
	private List<PathfinderNode> m_ManualNodes = new List<PathfinderNode>();

	[SerializeField]
	private bool Unparent;

	public int NodeScore;

	public int ID { get; private set; }

	public Vector3 Position { get; private set; }

	[SerializeField]
	public List<PathfinderNode> ConnectedNodes { get; private set; }

	[SerializeField]
	public List<PathfinderNode> RejectNodes { get; private set; }

	public List<int> ConnectedNodeIDs { get; private set; }

	private LayerMask m_LayerMask => LayerMask.op_Implicit((1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("InvisibleCollider")));

	private Vector3 m_Offset => Vector3.up * 2f;

	public void Init(int id)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ID = id;
		Position = base.transform.position;
		GameObject val = GameObject.Find("Pathnodes");
		if (Unparent && ((Object)base.transform.parent).name != "Pathnodes" && Object.op_Implicit((Object)(object)val))
		{
			base.transform.parent = val.transform;
		}
		ConnectedNodes = new List<PathfinderNode>();
		RejectNodes = new List<PathfinderNode>();
		ConnectedNodeIDs = new List<int>();
	}

	public void RegisterNearbyNodes(List<PathfinderNode> NodeList)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		ResetConnectedNodes();
		for (int i = 0; i < NodeList.Count; i++)
		{
			PathfinderNode pathfinderNode = NodeList[i];
			if (ID == pathfinderNode.ID)
			{
				continue;
			}
			if (pathfinderNode.ConnectedNodes.Contains(this))
			{
				ConnectedNodes.Add(pathfinderNode);
				ConnectedNodeIDs.Add(pathfinderNode.ID);
			}
			else if (m_ManualNodes.Contains(pathfinderNode))
			{
				ConnectedNodes.Add(pathfinderNode);
				ConnectedNodeIDs.Add(pathfinderNode.ID);
				pathfinderNode.ConnectedNodes.Add(this);
				pathfinderNode.ConnectedNodeIDs.Add(ID);
			}
			else if (!RejectNodes.Contains(pathfinderNode) && Vector3.Distance(Position, pathfinderNode.Position) < GameManager.Instance.AiGlobalNetwork.NodeNearbyDistance)
			{
				if (!Physics.Linecast(Position + m_Offset, pathfinderNode.Position + m_Offset, LayerMask.op_Implicit(m_LayerMask), (QueryTriggerInteraction)1) && !Physics.Linecast(pathfinderNode.Position + m_Offset, Position + m_Offset, LayerMask.op_Implicit(m_LayerMask), (QueryTriggerInteraction)1))
				{
					ConnectedNodes.Add(pathfinderNode);
					ConnectedNodeIDs.Add(pathfinderNode.ID);
				}
				else
				{
					pathfinderNode.RemoveNearbyNode(this);
				}
			}
		}
	}

	public void RemoveNearbyNode(PathfinderNode node)
	{
		if (!RejectNodes.Contains(node))
		{
			RejectNodes.Add(node);
		}
	}

	private void ResetConnectedNodes()
	{
		if (ConnectedNodes != null)
		{
			ConnectedNodes.Clear();
			ConnectedNodeIDs.Clear();
		}
		else
		{
			ConnectedNodes = new List<PathfinderNode>();
			ConnectedNodeIDs = new List<int>();
		}
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.AiGlobalNetwork != null)
		{
			GameManager.Instance.AiGlobalNetwork.RemoveNode(this);
		}
		if (ConnectedNodes != null)
		{
			ConnectedNodes.Clear();
			ConnectedNodeIDs.Clear();
			ConnectedNodes = null;
		}
		if (m_ManualNodes != null)
		{
			m_ManualNodes.Clear();
			m_ManualNodes = null;
		}
		base.OnDisposed();
	}
}
