using TMG.Core;

namespace Ai;

public abstract class AiThoughtMachineBehaviour : TMGMonoBehaviour
{
	public virtual void Activate()
	{
	}

	protected virtual void T_EnterInactive()
	{
	}

	protected virtual void T_Inactive()
	{
	}

	protected virtual void T_EnterDistanceActivation()
	{
	}

	protected virtual void T_DistanceActivation()
	{
	}

	protected virtual void T_EnterActivate()
	{
	}

	protected virtual void T_Activate()
	{
	}

	protected virtual void T_EnterIdle()
	{
	}

	protected virtual void T_Idle()
	{
	}

	protected virtual void T_EnterUseWaypoints()
	{
	}

	protected virtual void T_UseWaypoints()
	{
	}

	protected virtual void T_EnterMoveToPoint()
	{
	}

	protected virtual void T_MoveToPoint()
	{
	}

	protected virtual void T_EnterHit()
	{
	}

	protected virtual void T_Hit()
	{
	}

	protected virtual void T_EnterRetreat()
	{
	}

	protected virtual void T_Retreat()
	{
	}

	protected virtual void T_EnterDie()
	{
	}

	protected virtual void T_Die()
	{
	}

	protected virtual void T_EnterPlaySingleAnimation()
	{
	}

	protected virtual void T_PlaySingleAnimation()
	{
	}

	protected virtual void T_EnterAttack()
	{
	}

	protected virtual void T_Attack()
	{
	}

	protected virtual void T_EnterWait()
	{
	}

	protected virtual void T_Wait()
	{
	}

	protected virtual void T_EnterFollow()
	{
	}

	protected virtual void T_Follow()
	{
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
