using DG.Tweening;
using TMG.Controls;
using UnityEngine;

public class Minigame_Balloon : WinnableMiniGameBaseController
{
	[Header("Balloon Game Props")]
	[SerializeField]
	private Transform m_TargetTransform;

	[SerializeField]
	private Transform m_ShootTransform;

	[SerializeField]
	private Transform m_BalloonPivotTransform;

	[SerializeField]
	private float m_IncreaseValue;

	[SerializeField]
	private Collider m_TargetCollider;

	[SerializeField]
	private ParticleSystem m_InkParticles;

	[SerializeField]
	private float m_WinScale = 10f;

	private float m_CurrentBalloonScale;

	private RaycastHit m_hit;

	private EmissionModule m_ParticleModule;

	private bool m_GameWon;

	public override void Init()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_ParticleModule = m_InkParticles.emission;
	}

	public override void HandleGameStartupSequence()
	{
		base.HandleGameStartupSequence();
		HandleOnPrepHeldObject();
	}

	public override void BeginGameLoops()
	{
		base.BeginGameLoops();
		Sequence_DoTargetMove();
	}

	public override void Update()
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (base.CurrentState == MiniGameState.ACTIVE)
		{
			if (PlayerInput.AttackHold())
			{
				if (Physics.Raycast(m_ShootTransform.position, m_ShootTransform.forward, ref m_hit))
				{
					Debug.Log((object)((Object)((RaycastHit)(ref m_hit)).transform).name);
					((EmissionModule)(ref m_ParticleModule)).rateOverTime = MinMaxCurve.op_Implicit(20f);
					if ((Object)(object)((RaycastHit)(ref m_hit)).collider == (Object)(object)m_TargetCollider)
					{
						m_CurrentBalloonScale += m_IncreaseValue * 1.5f * Time.deltaTime;
						Debug.Log((object)("HIT " + m_CurrentBalloonScale));
						if (!(m_CurrentBalloonScale >= m_WinScale))
						{
						}
					}
				}
			}
			else
			{
				((EmissionModule)(ref m_ParticleModule)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			}
		}
		else
		{
			((EmissionModule)(ref m_ParticleModule)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			if (m_CurrentBalloonScale > 1f)
			{
				m_CurrentBalloonScale -= m_IncreaseValue * 2f * Time.deltaTime;
			}
		}
		m_CurrentBalloonScale -= m_IncreaseValue * Time.deltaTime;
		m_CurrentBalloonScale = Mathf.Clamp(m_CurrentBalloonScale, 1f, m_WinScale);
		m_BalloonPivotTransform.localScale = Vector3.one * m_CurrentBalloonScale;
	}

	public void Sequence_DoTargetMove()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		if (base.CurrentState == MiniGameState.ACTIVE)
		{
			Sequence val = DOTween.Sequence();
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(m_TargetTransform.localPosition.x, Random.Range(-1f, 1f), Random.Range(-2f, 2f));
			TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_TargetTransform, val2, 1f, false), (Ease)7));
			TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(Sequence_DoTargetMove));
		}
		else
		{
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_TargetTransform, new Vector3(m_TargetTransform.localPosition.x, 0f, 0f), 0.5f, false), (Ease)7);
		}
	}
}
