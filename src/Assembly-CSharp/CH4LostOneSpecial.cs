using S13Audio;
using TMG.Core;
using UnityEngine;

public class CH4LostOneSpecial : TMGMonoBehaviour
{
	[Header("Headbang")]
	[SerializeField]
	private ParticleSystem m_SplashParticles;

	private S13ObjectComplex m_S13Object;

	public override void Init()
	{
		base.Init();
		m_S13Object = ((Component)this).GetComponentInChildren<S13ObjectComplex>();
	}

	public void Splash()
	{
		m_SplashParticles.Emit(2);
		m_S13Object.Play();
		S13AudioManager.Instance.PlayAudio("sfx_headbang_echo_lfe");
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
