using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13ParticleTrigger : MonoBehaviour
{
	[SerializeField]
	private S13AudioSource m_AudioSource;

	private ParticleSystem m_Particles;

	private List<Particle> enter = new List<Particle>();

	private void Start()
	{
		m_Particles = ((Component)this).GetComponent<ParticleSystem>();
	}

	private void OnParticleTrigger()
	{
		if (Object.op_Implicit((Object)(object)m_Particles) && Object.op_Implicit((Object)(object)m_AudioSource))
		{
			int triggerParticles = ParticlePhysicsExtensions.GetTriggerParticles(m_Particles, (ParticleSystemTriggerEventType)2, enter);
			if (triggerParticles > 0)
			{
				m_AudioSource.Play();
			}
		}
	}
}
