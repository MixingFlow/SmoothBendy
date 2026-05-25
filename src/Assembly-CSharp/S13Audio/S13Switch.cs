using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13Switch : MonoBehaviour
{
	[Tooltip("Assign Default Sound Source to 1st slot")]
	public S13AudioContainer[] audioContainers;

	private string defaultID = string.Empty;

	private Dictionary<string, S13AudioSource> sources = new Dictionary<string, S13AudioSource>();

	private void Awake()
	{
		S13AudioContainer[] array = audioContainers;
		for (int i = 0; i < array.Length; i++)
		{
			S13AudioContainer s13AudioContainer = array[i];
			S13AudioSource component = s13AudioContainer.obj.GetComponent<S13AudioSource>();
			if ((Object)(object)component == (Object)null)
			{
				Debug.LogError((object)"object assigned to S13Switch does not contain a S13AudioSource", (Object)(object)s13AudioContainer.obj);
			}
			else
			{
				sources.Add(s13AudioContainer.id, component);
			}
		}
		defaultID = audioContainers[0].id;
	}

	public void Play(string id)
	{
		if (sources.ContainsKey(id))
		{
			sources[id].Play();
		}
		else
		{
			Debug.Log((object)("Audio Source with ID: " + id + ", not Found on " + ((Object)((Component)this).gameObject).name), (Object)(object)((Component)this).gameObject);
		}
	}

	public void Play()
	{
		Play(defaultID);
	}

	public void Stop(string id)
	{
		if (sources.ContainsKey(id))
		{
			sources[id].Stop();
		}
	}

	public void Stop()
	{
		Stop(defaultID);
	}

	public bool Contains(string id)
	{
		return sources.ContainsKey(id);
	}
}
