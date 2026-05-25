using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class ProjectionistAi : BaseAiController
{
	[SerializeField]
	private List<Light> m_LightSource;

	[SerializeField]
	private AudioSource m_Audio;

	[Header("Additional Projectionist Options")]
	[SerializeField]
	private bool m_IsInWater;

	private bool m_IsHit;

	private AudioClip[] m_FootstepsInk;

	private AudioClip[] m_FootstepsDefault;

	private AudioClip m_ScreamClip;

	private AudioClip m_DeathClip;

	private AudioObject m_ScreamAudio;

	private float m_FootstepTimer;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_FootstepsInk = GameManager.Instance.GetAudioClips("Audio/SFX/Footsteps/ProjectionistWater/");
		m_FootstepsDefault = GameManager.Instance.GetAudioClips("Audio/SFX/Footsteps/ProjectionistDry/");
		m_ScreamClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_projectionist_scream");
		m_DeathClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_projectionist_death");
	}

	protected override void Update()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (IsInThought(AiThought.Die))
		{
			return;
		}
		Vector3 velocity = m_CharacterController.velocity;
		if (((Vector3)(ref velocity)).magnitude < 0.1f)
		{
			m_FootstepTimer = 0f;
			return;
		}
		m_FootstepTimer += Time.deltaTime;
		float num = ((m_CurrentMoveSpeed != m_WalkSpeed) ? 0.3f : 0.8f);
		if (!(m_FootstepTimer > num))
		{
			return;
		}
		m_FootstepTimer = 0f;
		Vector3 position = base.transform.position;
		float y = position.y;
		Bounds bounds = ((Collider)m_CharacterController).bounds;
		position.y = y + (((Bounds)(ref bounds)).extents.y - 0.1f);
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(position, Vector3.down, ref val, m_CharacterController.height + 1f, ~(1 << LayerMask.NameToLayer("Ai"))))
		{
			if (((Component)((RaycastHit)(ref val)).collider).CompareTag("DeepInk"))
			{
				PlayAudio(ref m_FootstepsInk);
			}
			else
			{
				PlayAudio(ref m_FootstepsDefault);
			}
		}
	}

	protected override void T_EnterFollow()
	{
		if ((Object)(object)m_ScreamAudio == (Object)null)
		{
			m_ScreamAudio = GameManager.Instance.AudioManager.Play(m_ScreamClip);
			m_ScreamAudio.OnComplete += delegate
			{
				m_ScreamAudio.Clear();
				m_ScreamAudio = null;
			};
		}
		SendOnSpotted();
		base.T_EnterFollow();
	}

	protected override void T_Follow()
	{
		if (GameManager.Instance.Player.CurrentStatus == CombatStatus.Hiding)
		{
			SetUseRunForWaypoints(useRun: false);
			SetThought(AiThought.UseWaypoints);
			SendOnRetreat();
		}
		else
		{
			base.T_Follow();
		}
	}

	protected override void OnMoveToPointReached()
	{
		SetUseRunForWaypoints(useRun: false);
		base.OnMoveToPointReached();
	}

	protected override void T_EnterUseWaypoints()
	{
		m_IsHit = false;
		SendOnRetreat();
		base.T_EnterUseWaypoints();
	}

	protected override void T_EnterDie()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		SetTarget(null);
		((Component)this).tag = "Dead";
		SetMoveDirection(Vector3.zero);
		((Collider)m_CharacterController).enabled = false;
		CapsuleCollider component = ((Component)this).GetComponent<CapsuleCollider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			((Collider)component).enabled = false;
		}
		for (int i = 0; i < m_LightSource.Count; i++)
		{
			Light light = m_LightSource[i];
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(ShortcutExtensions.DOIntensity(light, 0f, 0.25f), 5, (LoopType)0), (TweenCallback)delegate
			{
				((Behaviour)light).enabled = false;
			});
		}
		TweenSettingsExtensions.OnComplete<Tweener>(m_Audio.DOFade(0f, 1.25f), (TweenCallback)delegate
		{
			m_Audio.Stop();
		});
		GameManager.Instance.AudioManager.Play(m_DeathClip);
		SendOnDeath();
		SetAnimationTrigger("Dead");
	}

	protected override void T_EnterRetreat()
	{
		base.T_EnterRetreat();
		SendOnWaypointComplete();
		SetUseRunForWaypoints(useRun: false);
		SetThought(AiThought.UseWaypoints);
	}

	public override void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo != null)
		{
			base.Hit(hit, weaponInfo);
			if (!m_IsHit)
			{
				m_IsHit = true;
				SetTarget(GameManager.Instance.Player.transform);
				SetThought(AiThought.Follow);
			}
		}
	}

	public void UpdateWaypointList(List<WaypointNode> waypointList, bool SetThoughtToWaypoint = true)
	{
		if (waypointList != null && waypointList.Count > 0)
		{
			m_CurrentWaypointList.Clear();
			m_WaypointIndex = 0;
			m_CurrentWaypointList = new List<WaypointNode>(waypointList);
		}
		m_StartingThought = AiThought.UseWaypoints;
		if (SetThoughtToWaypoint)
		{
			SetThought(AiThought.UseWaypoints);
		}
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject result = GameManager.Instance.AudioManager.PlayAtPosition(val, m_EyeLocation.position);
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return result;
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_ScreamAudio != (Object)null)
		{
			m_ScreamAudio.Clear();
			m_ScreamAudio = null;
		}
		m_FootstepsInk = null;
		m_FootstepsDefault = null;
		m_ScreamClip = null;
		m_DeathClip = null;
		base.OnDisposed();
	}
}
