using System;
using System.Collections.Generic;
using System.Linq;
using TMG.Core;
using UnityEngine;

[Serializable]
public class PathfinderManager : TMGAbstractDisposable
{
	public float NodeNearbyDistance = 10f;

	public List<PathfinderNode> GlobalPathNodeList { get; private set; }

	public PathfinderManager(float nodeNearbyDistance = 12f)
	{
		NodeNearbyDistance = 10f;
	}

	public void BuildPaths()
	{
		ResetNodeList();
		GlobalPathNodeList = Object.FindObjectsOfType<PathfinderNode>().ToList();
		bool flag = false;
		for (int i = 0; i < GlobalPathNodeList.Count; i++)
		{
			if (!flag)
			{
				GlobalPathNodeList[i].Init(i);
				if (i >= GlobalPathNodeList.Count - 1)
				{
					i = -1;
					flag = true;
				}
			}
			else
			{
				GlobalPathNodeList[i].RegisterNearbyNodes(GlobalPathNodeList);
			}
		}
	}

	private void ResetNodeList()
	{
		if (GlobalPathNodeList != null)
		{
			GlobalPathNodeList.Clear();
		}
		else
		{
			GlobalPathNodeList = new List<PathfinderNode>();
		}
	}

	protected override void OnDisposed()
	{
		if (GlobalPathNodeList != null)
		{
			GlobalPathNodeList.Clear();
			GlobalPathNodeList = null;
		}
		base.OnDisposed();
	}
}
