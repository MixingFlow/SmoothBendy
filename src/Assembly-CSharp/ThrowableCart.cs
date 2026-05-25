using UnityEngine;

public class ThrowableCart : ThrowableObject
{
	[SerializeField]
	private BruteBorisAi m_Boris;

	public override void DoHitStuff(Vector3 hitPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.DoHitStuff(hitPosition);
		m_Boris.AttackTarget(base.transform.position, 8f);
	}
}
