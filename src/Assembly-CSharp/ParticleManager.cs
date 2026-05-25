using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class ParticleManager : TMGAbstractDisposable
{
	private class ParticleData
	{
		public ParticleSystem ParticleSystem;

		public int MaxParticles;
	}

	private const int MAX_PARTICLES_MEDIUM = 5;

	private ParticleSystem[] m_SceneParticles;

	private List<ParticleData> m_Particles = new List<ParticleData>();

	public void Initialize()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_Particles.Clear();
		m_SceneParticles = null;
		m_SceneParticles = Object.FindObjectsOfType<ParticleSystem>();
		for (int i = 0; i < m_SceneParticles.Length; i++)
		{
			ParticleSystem val = m_SceneParticles[i];
			List<ParticleData> particles = m_Particles;
			ParticleData particleData = new ParticleData
			{
				ParticleSystem = val
			};
			MainModule main = val.main;
			particleData.MaxParticles = ((MainModule)(ref main)).maxParticles;
			particles.Add(particleData);
		}
		UpdateQuality(GameManager.Instance.PlayerSettings.currentQuality);
	}

	public void UpdateQuality(int level)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Particles.Count; i++)
		{
			ParticleData particleData = m_Particles[i];
			if (particleData != null && !((Object)(object)particleData.ParticleSystem == (Object)null))
			{
				MainModule main = particleData.ParticleSystem.main;
				SubEmittersModule subEmitters = particleData.ParticleSystem.subEmitters;
				CollisionModule collision = particleData.ParticleSystem.collision;
				if (level > 2)
				{
					((SubEmittersModule)(ref subEmitters)).enabled = true;
					((CollisionModule)(ref collision)).enabled = true;
					((MainModule)(ref main)).maxParticles = particleData.MaxParticles;
				}
				else if (level > 1)
				{
					((SubEmittersModule)(ref subEmitters)).enabled = true;
					((CollisionModule)(ref collision)).enabled = true;
					((MainModule)(ref main)).maxParticles = 5;
				}
				else if (level > 0)
				{
					((SubEmittersModule)(ref subEmitters)).enabled = true;
					((CollisionModule)(ref collision)).enabled = true;
					((MainModule)(ref main)).maxParticles = 2;
				}
				else
				{
					((SubEmittersModule)(ref subEmitters)).enabled = false;
					((CollisionModule)(ref collision)).enabled = false;
					((MainModule)(ref main)).maxParticles = 0;
				}
			}
		}
	}
}
