using UnityEngine;

public class BeastBendy_AnimationEvents : MonoBehaviour
{
	public BeastBendy_Ai BeastBendy;

	public Ch5BeastBendyChargeController ChargeBendy;

	public void Stomp()
	{
		if (Object.op_Implicit((Object)(object)BeastBendy))
		{
			BeastBendy.ApplyShake(1f);
		}
	}

	public void DoAttack()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)BeastBendy))
		{
			BeastBendy.AttackTarget(BeastBendy.transform.forward * 5f);
		}
	}

	public void ChargeStomp()
	{
		if (Object.op_Implicit((Object)(object)ChargeBendy))
		{
			ChargeBendy.ApplyShake(1f);
		}
	}
}
