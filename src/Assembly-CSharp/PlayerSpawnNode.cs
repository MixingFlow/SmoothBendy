using TMG.Core;
using UnityEngine;

public class PlayerSpawnNode : TMGMonoBehaviour
{
	public override void Init()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		Transform obj = base.transform;
		obj.position += new Vector3(0f, 2.955f, 0f);
	}
}
