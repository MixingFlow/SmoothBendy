using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioTrigger : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem m_Particles;

	[SerializeField]
	private AudioSource m_AudioSource;

	private List<Particle> enter = new List<Particle>();

	private void OnParticleTrigger()
	{
		int triggerParticles = ParticlePhysicsExtensions.GetTriggerParticles(m_Particles, (ParticleSystemTriggerEventType)2, enter);
		for (int i = 0; i < triggerParticles; i++)
		{
			m_AudioSource.PlayOneShot(m_AudioSource.clip);
		}
	}
}
