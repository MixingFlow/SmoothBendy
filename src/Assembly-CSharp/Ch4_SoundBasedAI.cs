using Ai;
using UnityEngine;

public class Ch4_SoundBasedAI : ButcherGangAi
{
	private float m_InvestigateTimer;

	private bool m_IsInvestigating;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		GameManager.Instance.AiGlobalNetwork.AddSoundAi(this);
	}

	public void SetNoiseLocation(Vector3 location)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)base.CurrentTarget))
		{
			m_CurrentPath = null;
			if (SolvePathMovement(location) && m_CurrentPath != null && m_CurrentPath.Count > 0)
			{
				SetVectorPoint(m_CurrentPath[m_CurrentPath.Count - 1].Position);
				SetThought(AiThought.MoveToPoint);
				m_InvestigateTimer = 0f;
				m_IsInvestigating = false;
			}
			else
			{
				m_WaypointIndex = 0;
				SetThought(AiThought.UseWaypoints);
				m_IsInvestigating = false;
			}
		}
	}

	protected override void OnMoveToPointReached()
	{
		SetThought(AiThought.Idle);
		m_InvestigateTimer = 5f;
		m_IsInvestigating = true;
	}

	protected override void Update()
	{
		base.Update();
		if (m_InvestigateTimer > 0f)
		{
			m_InvestigateTimer -= Time.deltaTime;
		}
		else if (m_IsInvestigating || (base.CurrentThought == AiThought.Idle && m_WaypointIndex == 0 && (Object)(object)base.CurrentTarget == (Object)null))
		{
			m_WaypointIndex = 0;
			SetThought(AiThought.UseWaypoints);
			m_IsInvestigating = false;
		}
		if (!Object.op_Implicit((Object)(object)base.CurrentTarget))
		{
			TestTargetVisibility();
		}
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.AiGlobalNetwork.RemoveSoundAi(this);
		base.OnDisposed();
	}
}
