using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class SwollenSearcherAi : TMGMonoBehaviour, IHittable
{
	private const float ANIMATION_TIME = 1.833f;

	private const string IS_HIDING = "IsHiding";

	private const string IS_ACTIVE = "IsActive";

	[SerializeField]
	protected LayerMask m_ObstructionLayers;

	[SerializeField]
	private CapsuleCollider m_Collider;

	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private AnimationClip m_IsHidingClip;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_EyeLocation;

	[Header("Particles")]
	[SerializeField]
	private GameObject m_InkExplosionEffects;

	[SerializeField]
	private ParticleSystem m_InkPuddle;

	[SerializeField]
	private ParticleSystem m_RainParticles;

	[Header("Death Options")]
	[SerializeField]
	private ParticleSystem m_InkExplosion;

	[SerializeField]
	private ParticleSystem m_InkDrops;

	[SerializeField]
	private Renderer m_Renderer;

	private Sequence m_ExplodeSequence;

	private AudioObject m_ActiveAudio;

	private AudioClip m_IdleClip;

	private float m_TargetVisibilityAngle = 60f;

	private bool m_IsActive;

	private bool m_IsHiding = true;

	private float m_TimerLimit;

	private float m_Timer;

	public event EventHandler OnDeath;

	public event EventHandler OnHide;

	public override void Init()
	{
		base.Init();
		m_IdleClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_IDLE");
		AnimationEventUtil.AddEventController(ref m_AnimationController).SetReference(this);
		AnimationEventUtil.AddAnimationEvent(ref m_AnimationController, ((Object)m_IsHidingClip).name, "SwollenSearcherHide", 0);
	}

	public void Update()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsHiding && m_IsActive)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerLimit)
			{
				m_Timer = 0f;
				m_IsHiding = true;
				Hide();
			}
			Vector3 position = GameManager.Instance.Player.transform.position;
			float num = Vector3.Angle(m_EyeLocation.forward, position - m_EyeLocation.position);
			if (num < m_TargetVisibilityAngle && Vector3.Distance(m_EyeLocation.position, position) < 15f && !Physics.Linecast(m_EyeLocation.position, GameManager.Instance.Player.transform.position, LayerMask.op_Implicit(m_ObstructionLayers)))
			{
				m_IsHiding = true;
				Hide();
			}
		}
	}

	public void Rise(float activeTime = 8f)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		m_TimerLimit = activeTime;
		m_IsHiding = false;
		((Collider)m_Collider).enabled = false;
		ClearAudio();
		m_ActiveAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_IdleClip, m_EyeLocation.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, base.transform);
		SetTriggerAnimation("IsActive");
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 7f / 30f, new TweenCallback(Explode));
		TweenSettingsExtensions.OnComplete<Sequence>(val, (TweenCallback)delegate
		{
			m_IsActive = true;
			((Collider)m_Collider).enabled = true;
		});
	}

	public void Hide()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		ClearAudio();
		SetBoolAnimation("IsHiding", value: true);
		KillSequence();
		m_ExplodeSequence = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(m_ExplodeSequence, 0.5f, (TweenCallback)delegate
		{
			Explode();
			((Collider)m_Collider).enabled = false;
		});
	}

	public void FinalHide()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		ClearAudio();
		SetBoolAnimation("IsHiding", value: true);
		((Collider)m_Collider).enabled = false;
		KillSequence();
		m_ExplodeSequence = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(m_ExplodeSequence, 0.5f, (TweenCallback)delegate
		{
			Explode();
			m_IsActive = false;
			Dispose();
		});
	}

	public void HideOnComplete()
	{
		if (!base.IsDisposed && m_IsActive)
		{
			SetBoolAnimation("IsHiding", value: false);
			m_IsActive = false;
			this.OnHide.Send(this);
		}
	}

	private void Explode()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsDisposed)
		{
			GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_DIE", m_EyeLocation.position);
			m_InkExplosion.Emit(15);
			m_InkDrops.Emit(10);
		}
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo != null)
		{
			GameManager.Instance.AudioManager.Play("Audio/SFX/CH3/SFX_CH3_swollensearcherpop");
			GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_HIT", m_EyeLocation.position);
			m_IsActive = false;
			ClearAudio();
			m_RainParticles.Stop();
			ShapeModule shape = m_InkExplosion.shape;
			Renderer renderer = m_Renderer;
			((ShapeModule)(ref shape)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer is SkinnedMeshRenderer) ? renderer : null);
			ShapeModule shape2 = m_InkDrops.shape;
			Renderer renderer2 = m_Renderer;
			((ShapeModule)(ref shape2)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer2 is SkinnedMeshRenderer) ? renderer2 : null);
			((Collider)m_Collider).enabled = false;
			m_Renderer.enabled = false;
			Explode();
			m_InkPuddle.Stop();
			((Component)m_InkPuddle).transform.SetParent((Transform)null);
			Object.Destroy((Object)(object)((Component)m_InkPuddle).gameObject, 5f);
			m_InkExplosionEffects.transform.SetParent((Transform)null);
			Object.Destroy((Object)(object)m_InkExplosionEffects, 5f);
			this.OnDeath.Send(this);
			Dispose();
		}
	}

	private void SetBoolAnimation(string name, bool value)
	{
		m_AnimationController.SetBool(name, value);
	}

	private void SetTriggerAnimation(string name)
	{
		m_AnimationController.SetTrigger("IsActive");
	}

	private void ClearAudio()
	{
		if ((Object)(object)m_ActiveAudio != (Object)null)
		{
			m_ActiveAudio.Clear();
			m_ActiveAudio = null;
		}
	}

	private void KillSequence()
	{
		if (m_ExplodeSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_ExplodeSequence, false);
			m_ExplodeSequence = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearAudio();
		KillSequence();
		base.OnDisposed();
	}
}
