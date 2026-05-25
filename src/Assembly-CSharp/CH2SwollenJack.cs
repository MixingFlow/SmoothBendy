using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH2SwollenJack : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_EyeLocation;

	[SerializeField]
	private Transform m_Hat;

	[SerializeField]
	private Interactable m_Valve;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private Renderer m_Renderer;

	[Header("Particles")]
	[SerializeField]
	private GameObject m_InkExplosionEffects;

	[SerializeField]
	private ParticleSystem m_InkRain;

	[SerializeField]
	private ParticleSystem m_InkPuddle;

	[SerializeField]
	private ParticleSystem m_InkExplosion;

	[SerializeField]
	private ParticleSystem m_InkDrops;

	[Header("Decal")]
	[SerializeField]
	private GameObject m_SplatDecal;

	private AudioObject m_ActiveAudio;

	private AudioClip m_IdleClip;

	private S13Switch m_AudioSwitch;

	private AudioClip m_Hit;

	public Transform Hat => m_Hat;

	public Interactable Valve => m_Valve;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_AudioSwitch = ((Component)this).GetComponentInChildren<S13Switch>();
		m_Hit = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_HIT");
		m_IdleClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_IDLE");
		base.transform.SetParent((Transform)null);
		m_SplatDecal.SetActive(false);
	}

	public void PlayAudio()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		m_ActiveAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_IdleClip, m_EyeLocation.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, base.transform);
	}

	public void Kill(bool isSilent = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		ShapeModule shape = m_InkExplosion.shape;
		Renderer renderer = m_Renderer;
		((ShapeModule)(ref shape)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer is SkinnedMeshRenderer) ? renderer : null);
		ShapeModule shape2 = m_InkDrops.shape;
		Renderer renderer2 = m_Renderer;
		((ShapeModule)(ref shape2)).skinnedMeshRenderer = (SkinnedMeshRenderer)(object)((renderer2 is SkinnedMeshRenderer) ? renderer2 : null);
		m_Renderer.enabled = false;
		Explode();
		m_InkRain.Stop();
		m_InkPuddle.Stop();
		((Component)m_InkPuddle).transform.SetParent((Transform)null);
		Object.Destroy((Object)(object)((Component)m_InkPuddle).gameObject, 5f);
		m_InkExplosionEffects.transform.SetParent((Transform)null);
		Object.Destroy((Object)(object)m_InkExplosionEffects, 5f);
		m_SplatDecal.SetActive(true);
		m_SplatDecal.transform.SetParent((Transform)null);
		Dispose();
	}

	private void Explode(bool isSilent = false)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!isSilent)
		{
			m_AudioSwitch.Play("death");
			GameManager.Instance.AudioManager.PlayAtPosition(m_Hit, m_EyeLocation.position);
			m_InkExplosion.Emit(15);
			m_InkDrops.Emit(10);
		}
	}

	public void Show()
	{
		m_AudioSwitch.Play("appear");
		m_Animator.SetBool("IsHiding", false);
	}

	public void Hide()
	{
		m_AudioSwitch.Play("hide");
		m_Animator.SetBool("IsHiding", true);
	}

	private void ClearAudio()
	{
		if ((Object)(object)m_ActiveAudio != (Object)null)
		{
			m_ActiveAudio.Clear();
			m_ActiveAudio = null;
		}
	}

	protected override void OnDisposed()
	{
		ClearAudio();
		m_Hit = null;
		m_AudioSwitch = null;
		base.OnDisposed();
	}
}
