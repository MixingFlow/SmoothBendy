using System.Collections;
using UnityEngine;

namespace Ai;

public class SearcherAi : BaseAiController
{
	[Header("<== Searcher OPTIONS ==>")]
	[SerializeField]
	private bool m_HasInkPool;

	[SerializeField]
	private bool m_IgnoreSpawnInkExplosion;

	[SerializeField]
	private GameObject m_DripEffect;

	[SerializeField]
	private ParticleSystem m_InkPuddle;

	[SerializeField]
	private Transform m_DraggerFoot;

	[SerializeField]
	private Renderer m_ModelRenderer;

	private AudioClip[] m_AppearClips;

	private AudioClip[] m_IdleClips;

	private AudioClip[] m_HitClips;

	private AudioClip[] m_AttackClips;

	private AudioClip[] m_DeathClips;

	private AudioObject m_IdleAudio;

	private bool m_HasIdleAudio;

	private float m_IdleAudioTimer;

	private float m_IdleAudioTimerLimit = 10f;

	public override void Init()
	{
		base.Init();
		m_DripEffect.SetActive(false);
	}

	public override void InitOnComplete()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_AppearClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/Searchers/Appear/");
		m_IdleClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/Searchers/Idle/");
		m_AttackClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/Searchers/Attack/");
		m_DeathClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/Searchers/Death/");
		m_HitClips = GameManager.Instance.AssetManager.GetAssets<AudioClip>("Audio/SFX/Characters/Searchers/Hit/");
		m_ModelRenderer.enabled = false;
		((Collider)m_CharacterController).enabled = false;
		if (m_HasInkPool)
		{
			Transform val = GameManager.Instance.AssetManager.CreateAsset<Transform>("GamePlay/Particles/Searcher_SpawnPool");
			val.position = base.transform.position;
			val.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
		}
	}

	public override void Activate()
	{
		base.Activate();
	}

	private void InkSplash(bool isInitial)
	{
		if (m_InkDeathEffect == null || m_InkDeathEffect.Count <= 0)
		{
			return;
		}
		m_InkDeathEffect[0].InkExplosion.OnExplode += HandleDeathOnComplete;
		for (int i = 0; i < m_InkDeathEffect.Count; i++)
		{
			InkDeathEffect inkDeathEffect = m_InkDeathEffect[i];
			if (isInitial)
			{
				if (!m_IgnoreSpawnInkExplosion)
				{
					inkDeathEffect.InkExplosion.Birth(inkDeathEffect.Renderer);
				}
				else
				{
					inkDeathEffect.InkExplosion.ExplodeOnly();
				}
			}
			else
			{
				inkDeathEffect.InkExplosion.Activate(inkDeathEffect.Renderer, 0f, 0.6f);
			}
		}
	}

	protected override void Update()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!base.IsDisposed && !GameManager.Instance.isPaused)
		{
			m_IdleAudioTimer += Time.deltaTime;
			if (m_HasIdleAudio && m_IdleAudioTimer > m_IdleAudioTimerLimit && ((Object)(object)m_IdleAudio == (Object)null || !m_IdleAudio.AudioSource.isPlaying))
			{
				m_IdleAudioTimer = 0f;
				m_IdleAudio = PlayAudio(ref m_IdleClips);
				m_IdleAudioTimerLimit = m_IdleAudio.AudioClip.length + Random.Range(0.5f, 1f);
			}
			Vector3 position = m_DraggerFoot.position;
			position.y = ((Component)m_InkPuddle).transform.position.y;
			position += base.transform.forward;
			((Component)m_InkPuddle).transform.position = position;
		}
	}

	protected override void T_EnterDistanceActivation()
	{
		base.T_EnterDistanceActivation();
		m_StartingThought = AiThought.Retreat;
		m_ModelRenderer.enabled = false;
		((Collider)m_CharacterController).enabled = false;
	}

	protected override void T_EnterActivate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.T_EnterActivate();
		SendOnActive();
		PlayAudio(ref m_AppearClips);
		m_TrackObjectPosition = GameManager.Instance.Player.transform.position;
		base.transform.rotation = FaceDirection(isSmooth: false);
		m_ModelRenderer.enabled = true;
		m_HasIdleAudio = true;
		m_IdleAudio = PlayAudio(ref m_IdleClips);
		m_IdleAudioTimerLimit = m_IdleAudio.AudioClip.length + 0.5f;
		m_InkPuddle.Play();
	}

	protected override void T_Activate()
	{
		base.T_Activate();
		m_DripEffect.SetActive(true);
		InkSplash(isInitial: true);
	}

	protected override void T_EnterIdle()
	{
		base.T_EnterIdle();
		if (!((Collider)m_CharacterController).enabled)
		{
			((Collider)m_CharacterController).enabled = true;
		}
	}

	protected override void T_EnterRetreat()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.T_EnterRetreat();
		PlayAudio(ref m_AppearClips);
		((Collider)m_CharacterController).enabled = false;
		SetTarget(null);
		SetAnimatiorMovement(0);
		SetAnimatiorAttack(0);
		SetMoveDirection(Vector3.zero);
		SetAnimationTrigger("Hide");
		((MonoBehaviour)this).StartCoroutine(HideDelay());
	}

	private IEnumerator HideDelay()
	{
		if (base.IsDisposed)
		{
			yield return null;
		}
		float halfTime = m_AwakeAnimationWaitTime / 2f;
		yield return (object)new WaitForSeconds(halfTime);
		yield return (object)new WaitForEndOfFrame();
		if (base.IsDisposed)
		{
			yield return null;
		}
		InkSplash(isInitial: false);
		if (m_InkDeathEffect != null && m_InkDeathEffect.Count > 0)
		{
			for (int i = 0; i < m_InkDeathEffect.Count; i++)
			{
				InkDeathEffect inkDeathEffect = m_InkDeathEffect[i];
				inkDeathEffect.InkExplosion.transform.SetParent((Transform)null);
				Object.Destroy((Object)(object)inkDeathEffect.InkExplosion.gameObject, 5f);
			}
		}
		yield return (object)new WaitForSeconds(halfTime);
		yield return (object)new WaitForEndOfFrame();
		if (base.IsDisposed)
		{
			yield return null;
		}
		m_InkPuddle.Stop();
		((Component)m_InkPuddle).transform.SetParent((Transform)null);
		Object.Destroy((Object)(object)((Component)m_InkPuddle).gameObject, 5f);
		SendOnHide();
		SendOnRespawn();
		Dispose();
	}

	public override void AttackTarget()
	{
		base.AttackTarget();
		PlayAudio(ref m_AttackClips);
	}

	public override void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo != null)
		{
			PlayAudio(ref m_HitClips);
			base.Hit(hit, weaponInfo);
		}
	}

	protected override void T_EnterDie()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		m_HasIdleAudio = false;
		if ((Object)(object)m_IdleAudio != (Object)null)
		{
			m_IdleAudio.Clear();
			m_IdleAudio = null;
		}
		PlayAudio(ref m_DeathClips);
		SetTarget(null);
		((Component)this).tag = "Dead";
		SetMoveDirection(Vector3.zero);
		((Collider)m_CharacterController).enabled = false;
		SetAnimationTrigger("Dead");
		InkSplash(isInitial: false);
		m_InkPuddle.Stop();
		((Component)m_InkPuddle).transform.SetParent((Transform)null);
		Object.Destroy((Object)(object)((Component)m_InkPuddle).gameObject, 5f);
		m_DripEffect.SetActive(false);
		SendOnDeath();
		SendOnRespawn();
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips, bool is2D = false)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject audioObject = null;
		audioObject = ((!is2D) ? GameManager.Instance.AudioManager.PlayAtPosition(val, m_EyeLocation.position) : GameManager.Instance.AudioManager.Play(val));
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return audioObject;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
