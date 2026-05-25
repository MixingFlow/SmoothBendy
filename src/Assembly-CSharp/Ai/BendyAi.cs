using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Ai;

public class BendyAi : BaseAiController
{
	[Header("<== BENDY OPTIONS ==>")]
	[SerializeField]
	private GameObject m_InkEffect;

	[SerializeField]
	private ParticleSystem m_InkExplosion;

	[SerializeField]
	private ParticleSystem m_InkDrops;

	[Header("AudioSources")]
	[SerializeField]
	private List<AudioSource> m_AudioSources;

	public event EventHandler OnTrackingLost;

	protected override void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (GameManager.Instance.InkEffectManager != null)
		{
			GameManager.Instance.InkEffectManager.SetPosition(base.transform.position);
		}
		if ((!Object.op_Implicit((Object)(object)GameManager.Instance.Player) || GameManager.Instance.Player.CurrentStatus != CombatStatus.Hiding) && Object.op_Implicit((Object)(object)GameManager.Instance.Player) && Vector3.Distance(m_KneeLocation.position, GameManager.Instance.Player.transform.position) < 5f)
		{
			for (int i = 0; i < 6; i++)
			{
				GameManager.Instance.ShowHurtBorder();
			}
		}
	}

	public override void Activate()
	{
		base.Activate();
		GameManager.Instance.InkEffectManager = new GlobalInkEffectManager();
		GameManager.Instance.InkEffectManager.GetEffects();
		GameManager.Instance.InkEffectManager.SetActive(active: true);
		GameManager.Instance.CharacterManager.Bendy = this;
	}

	protected override void T_EnterInactive()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		base.T_EnterInactive();
		SetAnimatiorMovement(0);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
		{
			SetThought(AiThought.UseWaypoints);
		});
	}

	protected override void T_EnterUseWaypoints()
	{
		m_WalkMode = 1;
		base.T_EnterUseWaypoints();
	}

	protected override void T_UseWaypoints()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player) && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) < 30f && GameManager.Instance.Player.CurrentStatus != CombatStatus.Hiding && !m_PassiveAi && !Physics.Linecast(m_KneeLocation.position, GameManager.Instance.Player.transform.position, ref val, LayerMask.op_Implicit(m_ObstructionLayers)))
		{
			m_WalkMode = 2;
			SetTarget(GameManager.Instance.Player.transform);
			DoWait(m_SpottedAnimationWaitTime, AiThought.Follow);
			base.CurrentTarget = GameManager.Instance.Player.transform;
			m_TrackObjectPosition = base.CurrentTarget.position;
			base.transform.rotation = FaceDirection(isSmooth: false);
			SetAnimationTrigger("EnemySpotted");
			SendOnSpotted();
		}
		else
		{
			base.T_UseWaypoints();
		}
	}

	protected override void T_Follow()
	{
		if (GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			SetThought(AiThought.UseWaypoints);
		}
		else
		{
			base.T_Follow();
		}
	}

	protected override bool TrackingLostEvents(AiThought _repalcementThought)
	{
		if (base.TrackingLostEvents(_repalcementThought))
		{
			this.OnTrackingLost.Send(this);
			return true;
		}
		return false;
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SetThought(AiThought.UseWaypoints);
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList, bool _isActive = false)
	{
		m_CurrentWaypointList.Clear();
		m_WaypointIndex = 0;
		m_CurrentWaypointList = new List<WaypointNode>(waypointList);
		m_StartingThought = AiThought.UseWaypoints;
		if (_isActive)
		{
			SetThought(AiThought.UseWaypoints);
		}
		else
		{
			SetThought(AiThought.Inactive);
		}
	}

	public void ForceSetAnimationTrigger(string animation)
	{
		SetAnimationTrigger(animation);
	}

	public void ForceKill()
	{
		m_InkEffect.transform.SetParent((Transform)null);
		m_InkExplosion.Emit(30);
		m_InkDrops.Emit(20);
		Object.Destroy((Object)(object)m_InkEffect, 5f);
		Dispose();
	}

	private void ClearAudioSources()
	{
		for (int i = 0; i < m_AudioSources.Count; i++)
		{
			AudioSource val = m_AudioSources[i];
			((Component)val).transform.SetParent((Transform)null);
			val.DOFade(0f, 2f);
			Object.Destroy((Object)(object)((Component)val).gameObject, 2.05f);
		}
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.CharacterManager.Bendy = null;
		GameManager.Instance.InkEffectManager.SetActive(active: false);
		ClearAudioSources();
		base.OnDisposed();
	}
}
