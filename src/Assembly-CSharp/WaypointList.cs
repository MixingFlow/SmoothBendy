using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class WaypointList : TMGMonoBehaviour
{
	[SerializeField]
	private List<WaypointNode> m_Waypoints;

	public List<WaypointNode> Waypoints => m_Waypoints;

	public override void InitOnComplete()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		base.InitOnComplete();
		if (m_Waypoints.Count > 0)
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			Transform val = item;
			WaypointNode component = ((Component)val).GetComponent<WaypointNode>();
			if ((Object)(object)component != (Object)null)
			{
				m_Waypoints.Add(component);
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
