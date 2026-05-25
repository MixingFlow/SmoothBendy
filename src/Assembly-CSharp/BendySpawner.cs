using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BendySpawner : TMGMonoBehaviour
{
	[SerializeField]
	private List<WaypointNode> m_Waypoints;

	[SerializeField]
	private ParticleSystem m_Particles;

	private CH3BendyController m_BendyController;

	private CH5Administration m_CH5BendyController;

	public List<WaypointNode> Watpoints => m_Waypoints;

	public BendySpawnerList BendySpawnerList { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_BendyController = Object.FindObjectOfType<CH3BendyController>();
		m_CH5BendyController = Object.FindObjectOfType<CH5Administration>();
	}

	public void Initialize(BendySpawnerList list = null)
	{
		BendySpawnerList = list;
	}

	public void Update()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_BendyController != (Object)null && Object.op_Implicit((Object)(object)m_BendyController.Bendy))
		{
			if (Vector3.Distance(m_BendyController.Bendy.transform.position, base.transform.position) < 10f)
			{
				if (!m_Particles.isPlaying)
				{
					m_Particles.Play();
				}
			}
			else if (m_Particles.isPlaying)
			{
				m_Particles.Stop();
			}
		}
		else if ((Object)(object)m_CH5BendyController != (Object)null && Object.op_Implicit((Object)(object)m_CH5BendyController.Bendy))
		{
			if (Vector3.Distance(m_CH5BendyController.Bendy.transform.position, base.transform.position) < 10f)
			{
				if (!m_Particles.isPlaying)
				{
					m_Particles.Play();
				}
			}
			else if (m_Particles.isPlaying)
			{
				m_Particles.Stop();
			}
		}
		else
		{
			Disable();
		}
	}

	public void Disable()
	{
		if (m_Particles.isPlaying)
		{
			m_Particles.Stop();
		}
	}

	protected override void OnDisposed()
	{
		m_BendyController = null;
		m_CH5BendyController = null;
		BendySpawnerList = null;
		base.OnDisposed();
	}
}
