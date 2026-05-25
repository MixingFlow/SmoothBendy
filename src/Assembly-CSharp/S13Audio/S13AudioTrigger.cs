using UnityEngine;

namespace S13Audio;

[RequireComponent(typeof(Collider), typeof(S13AudioSource))]
public class S13AudioTrigger : MonoBehaviour
{
	public string triggerObjectTag = string.Empty;

	public string triggerObjectName = string.Empty;

	public string triggerObjectLayerName = "Audio";

	public bool triggerOnce;

	private S13AudioSource[] audioSources;

	private bool triggered;

	private void Awake()
	{
		audioSources = ((Component)this).GetComponents<S13AudioSource>();
		if (audioSources.Equals(null))
		{
			Debug.LogError((object)"The S13AudioTrigger Can't find any S13AudioSources.", (Object)(object)((Component)this).gameObject);
		}
		Collider component = ((Component)this).GetComponent<Collider>();
		if (((object)component).Equals((object)null))
		{
			Debug.LogError((object)"The S13AudioTrigger Can't find any S13AudioSources.", (Object)(object)((Component)this).gameObject);
		}
		component.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			PlayAudio();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			PlayAudio();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			StopAudio();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			StopAudio();
		}
	}

	private bool MatchConditions(GameObject obj)
	{
		if (triggerObjectTag != string.Empty && obj.CompareTag(triggerObjectTag))
		{
			return true;
		}
		if (triggerObjectName != string.Empty && triggerObjectName == ((Object)obj).name)
		{
			return true;
		}
		if (obj.layer == LayerMask.NameToLayer(triggerObjectLayerName))
		{
			return true;
		}
		return false;
	}

	private void PlayAudio()
	{
		if (triggered && triggerOnce)
		{
			return;
		}
		S13AudioSource[] array = audioSources;
		foreach (S13AudioSource s13AudioSource in array)
		{
			if ((Object)(object)s13AudioSource != (Object)null)
			{
				s13AudioSource.Play();
			}
		}
		triggered = true;
	}

	private void StopAudio()
	{
		if (!triggered)
		{
			return;
		}
		S13AudioSource[] array = audioSources;
		foreach (S13AudioSource s13AudioSource in array)
		{
			if ((Object)(object)s13AudioSource != (Object)null)
			{
				s13AudioSource.Stop();
			}
		}
		triggered = false;
	}
}
