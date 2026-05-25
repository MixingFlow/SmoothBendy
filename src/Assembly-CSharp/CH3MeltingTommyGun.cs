using DG.Tweening;
using UnityEngine;

public class CH3MeltingTommyGun : Interactable
{
	[SerializeField]
	private ParticleSystem m_Explosion;

	[SerializeField]
	private ParticleSystem m_Drips;

	[SerializeField]
	private Renderer[] m_RenderObjects;

	private AudioClip m_GunMeltClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_RenderObjects[0].sharedMaterial.SetFloat("_MeltPercentage", 0f);
		m_GunMeltClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_tommygunmelt");
	}

	public override void OnInteract()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		m_Drips.Play();
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOFloat(m_RenderObjects[0].sharedMaterial, 1f, "_MeltPercentage", 1.5f), (Ease)7), new TweenCallback(MeltOnComplete));
	}

	private void MeltOnComplete()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		m_Drips.Stop();
		Object.Destroy((Object)(object)((Component)m_Drips).gameObject, 5f);
		((Component)m_Explosion).transform.SetParent((Transform)null);
		m_Explosion.Emit(20);
		Object.Destroy((Object)(object)((Component)m_Explosion).gameObject, 5f);
		GameManager.Instance.AudioManager.PlayAtPosition(m_GunMeltClip, base.transform.position);
		for (int i = 0; i < m_RenderObjects.Length; i++)
		{
			m_RenderObjects[i].enabled = false;
		}
	}
}
