using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PAExclusionZone : MonoBehaviour
{
	public static List<PAExclusionZone> exclusionZones;

	public LayerMask affectsLayers = LayerMask.op_Implicit(-1);

	public Vector3 edgeThreshold = new Vector3(0.9f, 0.9f, 0.9f);

	public bool important;

	private Bounds bounds
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Max(((Component)this).transform.TransformPoint(Vector3.right * 0.5f), ((Component)this).transform.TransformPoint(Vector3.left * 0.5f));
			val = Vector3.Max(((Component)this).transform.TransformPoint(Vector3.up * 0.5f), val);
			val = Vector3.Max(((Component)this).transform.TransformPoint(Vector3.down * 0.5f), val);
			val = Vector3.Max(((Component)this).transform.TransformPoint(Vector3.forward * 0.5f), val);
			val = Vector3.Max(((Component)this).transform.TransformPoint(Vector3.back * 0.5f), val);
			return new Bounds(((Component)this).transform.position, (val - ((Component)this).transform.position) * 2f);
		}
	}

	public static void RegisterZone(PAExclusionZone zone)
	{
		if (exclusionZones == null)
		{
			exclusionZones = new List<PAExclusionZone>();
		}
		if (!exclusionZones.Contains(zone))
		{
			exclusionZones.Add(zone);
		}
	}

	public static void UnregisterZone(PAExclusionZone zone)
	{
		if (exclusionZones != null && exclusionZones.Contains(zone))
		{
			exclusionZones.Remove(zone);
		}
		exclusionZones.RemoveAll((PAExclusionZone obj) => (Object)(object)obj == (Object)null);
	}

	private void OnEnable()
	{
		RegisterZone(this);
	}

	private void OnDisable()
	{
		UnregisterZone(this);
	}

	private void OnDrawGizmos()
	{
	}

	private static Vector3 ClosestPointOnBounds(Bounds bounds, Vector3 point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ((Bounds)(ref bounds)).ClosestPoint(point);
	}

	public static bool GetExclusionZones(ref PAExclusionZone[] zones, Vector3 position, Bounds checkBounds, int layer)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		for (int i = 0; i < zones.Length; i++)
		{
			zones[i] = null;
		}
		if (exclusionZones == null)
		{
			return result;
		}
		for (int j = 0; j < exclusionZones.Count; j++)
		{
			PAExclusionZone pAExclusionZone = exclusionZones[j];
			if (!((Bounds)(ref checkBounds)).Intersects(pAExclusionZone.bounds) || ((1 << layer) & LayerMask.op_Implicit(pAExclusionZone.affectsLayers)) == 0)
			{
				continue;
			}
			result = true;
			for (int k = 0; k < zones.Length; k++)
			{
				if ((Object)(object)zones[k] == (Object)null)
				{
					zones[k] = pAExclusionZone;
					break;
				}
				float num = Vector3.SqrMagnitude(ClosestPointOnBounds(pAExclusionZone.bounds, position) - position);
				float num2 = Vector3.SqrMagnitude(ClosestPointOnBounds(zones[k].bounds, position) - position);
				if ((pAExclusionZone.important && !zones[k].important) || (pAExclusionZone.important == zones[k].important && num < num2))
				{
					for (int num3 = zones.Length - 1; num3 > k; num3--)
					{
						zones[num3] = zones[num3 - 1];
					}
					zones[k] = pAExclusionZone;
					break;
				}
			}
		}
		return result;
	}

	public static PAExclusionZone Create(string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new GameObject(name).AddComponent<PAExclusionZone>();
	}
}
