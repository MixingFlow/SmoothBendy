using System.Collections;
using UnityEngine;

public class ParticleSystemPause : MonoBehaviour
{
	public ParticleSystem ps;

	private void OnValidate()
	{
		if (!Object.op_Implicit((Object)(object)ps))
		{
			ps = ((Component)this).GetComponent<ParticleSystem>();
		}
	}

	private void Awake()
	{
		if (!Object.op_Implicit((Object)(object)ps))
		{
			ps = ((Component)this).GetComponent<ParticleSystem>();
		}
	}

	private IEnumerator Start()
	{
		int maxCount = 100;
		SimpleCull sc = null;
		do
		{
			if (maxCount-- <= 0)
			{
				yield break;
			}
			sc = Object.FindObjectOfType<SimpleCull>();
			yield return null;
		}
		while ((Object)(object)sc == (Object)null);
		do
		{
			yield return null;
		}
		while (!Object.op_Implicit((Object)(object)sc.cullTransform));
		if (Vector3.Distance(((Component)this).transform.position, sc.cullTransform.position) > sc.cullingDistance)
		{
			OnBecameInvisible();
		}
	}

	private void OnBecameVisible()
	{
		if (Object.op_Implicit((Object)(object)ps) && ps.isPaused)
		{
			ps.Play();
		}
	}

	private void OnBecameInvisible()
	{
		if (Object.op_Implicit((Object)(object)ps) && ps.isPlaying)
		{
			ps.Pause();
		}
	}
}
