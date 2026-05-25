using System;
using System.Collections;
using UnityEngine;

public class RigidbodyOptimizer : MonoBehaviour
{
	[Tooltip("How many seconds to wait between full checks.")]
	public float checkInterval = 20f;

	[Tooltip("How many objects to process per frame to avoid lag spikes.")]
	public int objectsPerBatch = 25;

	[Tooltip("Max number of objects to scan. Increase if game has massive object counts.")]
	public int maxBuffer = 8000;

	[Tooltip("Radius of the world scan. Ensure this covers your whole map.")]
	public float worldRadius = 100000f;

	private Collider[] _colliderBuffer;

	private void Start()
	{
		_colliderBuffer = (Collider[])(object)new Collider[maxBuffer];
		((MonoBehaviour)this).StartCoroutine(CheckInterpolationRoutine());
	}

	private IEnumerator CheckInterpolationRoutine()
	{
		while (true)
		{
			int hitCount = Physics.OverlapSphereNonAlloc(Vector3.zero, worldRadius, _colliderBuffer);
			if (hitCount >= maxBuffer)
			{
				Debug.LogWarning((object)$"[Optimizer] Max Buffer hit ({maxBuffer}). Some objects might be missed. Increase Max Buffer.");
			}
			int processedCount = 0;
			for (int i = 0; i < hitCount; i++)
			{
				Collider val = _colliderBuffer[i];
				if ((Object)(object)val == (Object)null)
				{
					continue;
				}
				Rigidbody attachedRigidbody = val.attachedRigidbody;
				if (!((Object)(object)attachedRigidbody == (Object)null) && (int)attachedRigidbody.interpolation != 1)
				{
					string name = ((Object)((Component)attachedRigidbody).gameObject).name;
					if (!name.Contains("InkBlob") && !name.Contains("Bolt") && !name.Contains("Player"))
					{
						attachedRigidbody.interpolation = (RigidbodyInterpolation)1;
						processedCount++;
					}
					if (processedCount >= objectsPerBatch)
					{
						processedCount = 0;
						yield return null;
					}
				}
			}
			Array.Clear(_colliderBuffer, 0, hitCount);
			yield return (object)new WaitForSeconds(checkInterval);
		}
	}
}
