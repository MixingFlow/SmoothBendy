using UnityEngine;

namespace Ai;

public class FisherAi : ButcherGangAi
{
	[Header("<== PIPER OPTIONS ==>")]
	[SerializeField]
	private GameObject m_ThickInk;

	public void ThickInkSetActive(bool active)
	{
		m_ThickInk.SetActive(active);
	}

	public override void Activate()
	{
		base.Activate();
	}

	protected override void T_Inactive()
	{
		base.T_Inactive();
	}

	protected override void T_DistanceActivation()
	{
		base.T_DistanceActivation();
	}

	protected override void T_EnterActivate()
	{
		base.T_EnterActivate();
	}

	protected override void T_Activate()
	{
		base.T_Activate();
	}

	protected override void T_Idle()
	{
		base.T_Idle();
	}

	protected override void T_UseWaypoints()
	{
		base.T_UseWaypoints();
	}

	protected override void T_MoveToPoint()
	{
		base.T_MoveToPoint();
	}

	protected override void T_Hit()
	{
		base.T_Hit();
	}

	protected override void T_Retreat()
	{
		base.T_Retreat();
	}

	protected override void T_EnterDie()
	{
		base.T_EnterDie();
	}

	protected override void T_Die()
	{
		base.T_Die();
	}

	protected override void T_PlaySingleAnimation()
	{
		base.T_PlaySingleAnimation();
	}

	protected override void T_EnterAttack()
	{
		base.T_EnterAttack();
	}

	public override void AttackTarget()
	{
		base.AttackTarget();
	}

	protected override void T_Attack()
	{
		base.T_Attack();
	}

	protected override void T_Wait()
	{
		base.T_Wait();
	}

	protected override void T_Follow()
	{
		base.T_Follow();
	}
}
