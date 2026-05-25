using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class PositionRandomizer : TMGMonoBehaviour
{
	[SerializeField]
	private Transform movableObject;

	[SerializeField]
	private List<Transform> positions = new List<Transform>();

	public override void InitOnComplete()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		if (!((Object)(object)movableObject == (Object)null) && positions.Count > 0)
		{
			positions.Shuffle();
			int index = Random.Range(0, positions.Count);
			Transform val = positions[index];
			movableObject.position = val.position;
			movableObject.rotation = val.rotation;
			Dispose();
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
