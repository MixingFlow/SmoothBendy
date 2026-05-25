using UnityEngine;

public class CH5BeastBendyInkPortal : MonoBehaviour
{
	public Transform m_StartPoint;

	public Transform m_EndPoint;

	[SerializeField]
	private ParticleSystem[] m_StartWallParticles;

	[SerializeField]
	private ParticleSystem[] m_EndWallParticles;

	public void ActivateStartParticles()
	{
		for (int i = 0; i < m_StartWallParticles.Length; i++)
		{
			m_StartWallParticles[i].Emit(10);
		}
	}

	public void ActivateEndParticles()
	{
		for (int i = 0; i < m_EndWallParticles.Length; i++)
		{
			m_EndWallParticles[i].Emit(10);
		}
	}
}
