using System.Collections.Generic;
using Ai;
using UnityEngine;

public class ButcherGangAi : BaseAiController
{
	public enum ButcherGang
	{
		PIPER,
		FISHER,
		STRIKER
	}

	[Header("Butcher Gang Options")]
	[SerializeField]
	private ButcherGang m_ButcherGangType;

	[SerializeField]
	private bool m_EnableIdleAudio = true;

	[SerializeField]
	private bool m_EnableIdleAudioOnDistanceActive;

	private AudioClip[] m_IdleClips;

	private AudioClip[] m_HitClips;

	private AudioClip[] m_AttackClips;

	private AudioClip[] m_DeathClips;

	private AudioClip m_HitMeclip;

	private AudioObject m_IdleAudio;

	private bool m_HasIdleAudio;

	private float m_IdleAudioTimer;

	private float m_IdleAudioTimerLimit = 10f;

	private BendyAi m_Bendy;

	public ButcherGang ButcherGangTyoe => m_ButcherGangType;

	public override void Init()
	{
		base.Init();
		m_IdleClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/ButcherGang/Idle/");
		m_AttackClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/ButcherGang/Attack/");
		m_DeathClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/ButcherGang/Death/");
		m_HitClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/ButcherGang/Hit/");
		m_HitMeclip = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/SFX/Characters/ButcherGang/CH3_BUTCHER_GANG_HITME");
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if (m_EnableIdleAudio && !m_EnableIdleAudioOnDistanceActive)
		{
			m_HasIdleAudio = true;
			m_IdleAudio = PlayAudio(ref m_IdleClips);
			m_IdleAudioTimerLimit = m_IdleAudio.AudioClip.length + 0.5f;
		}
	}

	protected override void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (CheckForBendy())
		{
			Vector3 val = m_Bendy.transform.position - base.transform.position;
			Vector3 position = base.transform.position + ((Vector3)(ref val)).normalized * 5f;
			ForceDeath(position);
		}
		else if (!base.IsDisposed && !GameManager.Instance.isPaused)
		{
			m_IdleAudioTimer += Time.deltaTime;
			if (m_HasIdleAudio && m_IdleAudioTimer > m_IdleAudioTimerLimit && ((Object)(object)m_IdleAudio == (Object)null || !m_IdleAudio.AudioSource.isPlaying))
			{
				m_IdleAudioTimer = 0f;
				m_IdleAudio = PlayAudio(ref m_IdleClips);
				m_IdleAudioTimerLimit = m_IdleAudio.AudioClip.length + Random.Range(0.5f, 1f);
			}
		}
	}

	private bool CheckForBendy()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (!Object.op_Implicit((Object)(object)m_Bendy))
		{
			m_Bendy = GameManager.Instance.CharacterManager.Bendy;
		}
		if (Object.op_Implicit((Object)(object)m_Bendy))
		{
			result = Vector3.Distance(base.transform.position, m_Bendy.transform.position) < 20f;
		}
		return result;
	}

	protected override void T_EnterActivate()
	{
		base.T_EnterActivate();
		SendOnActive();
		if (m_EnableIdleAudio)
		{
			m_HasIdleAudio = true;
			m_IdleAudio = PlayAudio(ref m_IdleClips);
			m_IdleAudioTimerLimit = m_IdleAudio.AudioClip.length + 0.5f;
		}
	}

	protected override void T_EnterDistanceActivation()
	{
		base.T_EnterDistanceActivation();
		SendOnDistanceActivate();
	}

	protected override void T_EnterFollow()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.T_EnterFollow();
		if (Random.value < 0.02f)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_HitMeclip, m_EyeLocation.position);
		}
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SetThought(AiThought.UseWaypoints);
	}

	public override void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo != null)
		{
			PlayAudio(ref m_HitClips, is2D: true);
			base.Hit(hit, weaponInfo);
		}
	}

	public override void AttackTarget()
	{
		base.AttackTarget();
		PlayAudio(ref m_AttackClips);
	}

	protected override void T_EnterDie()
	{
		base.T_EnterDie();
		m_HasIdleAudio = false;
		if ((Object)(object)m_IdleAudio != (Object)null)
		{
			m_IdleAudio.Clear();
			m_IdleAudio = null;
		}
		PlayAudio(ref m_DeathClips);
		SendOnDeath();
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList, bool isRunning = false)
	{
		m_CurrentWaypointList.Clear();
		m_UseRunForWaypoints = isRunning;
		m_WaypointIndex = 0;
		m_CurrentWaypointList = new List<WaypointNode>(waypointList);
		m_StartingThought = AiThought.UseWaypoints;
		SetThought(AiThought.UseWaypoints);
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips, bool is2D = false)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (audioClips == null || audioClips.Length == 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject result = ((!is2D) ? GameManager.Instance.AudioManager.PlayAtPosition(val, m_EyeLocation.position, AudioObjectType.SOUND_EFFECT, 0, isQueued: false, m_EyeLocation) : GameManager.Instance.AudioManager.Play(val));
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return result;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
