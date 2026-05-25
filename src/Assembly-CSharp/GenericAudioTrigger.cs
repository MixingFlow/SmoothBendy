using UnityEngine;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public class GenericAudioTrigger : MonoBehaviour
{
	public string triggerObjectTag = string.Empty;

	public string triggerObjectName = string.Empty;

	public string triggerObjectLayerName = "Audio";

	public bool triggerOnce;

	private AudioSource[] audioSources;

	private bool triggered;

	private void Awake()
	{
		audioSources = ((Component)this).GetComponents<AudioSource>();
		if (audioSources.Equals(null))
		{
			Debug.LogError((object)"The GenericAudioTrigger Can't find any AudioSources.", (Object)(object)((Component)this).gameObject);
		}
		Collider component = ((Component)this).GetComponent<Collider>();
		if (((object)component).Equals((object)null))
		{
			Debug.LogError((object)"The GenericAudioTrigger Can't find any AudioSources.", (Object)(object)((Component)this).gameObject);
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

	private void OnTriggerExit(Collider other)
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
		AudioSource[] array = audioSources;
		foreach (AudioSource val in array)
		{
			if ((Object)(object)val != (Object)null)
			{
				val.Play();
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
		AudioSource[] array = audioSources;
		foreach (AudioSource val in array)
		{
			if ((Object)(object)val != (Object)null)
			{
				val.Stop();
			}
		}
		triggered = false;
	}
}
