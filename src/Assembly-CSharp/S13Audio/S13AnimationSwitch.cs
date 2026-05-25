using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13AnimationSwitch : MonoBehaviour
{
	public S13AnimationAudioContainer[] audioContainers;

	private Animator animator;

	private Dictionary<string, S13AudioSource> sources = new Dictionary<string, S13AudioSource>();

	[SerializeField]
	private bool debug;

	private void Start()
	{
		animator = ((Component)this).GetComponentInParent<Animator>();
		if ((Object)(object)animator == (Object)null)
		{
			Log("parent object of S13AnimationSwitch does not contain an Animator", ((Component)this).gameObject, raiseWarning: true);
			((Component)this).gameObject.SetActive(false);
		}
		Refresh();
	}

	private void AddAnimationEvent(ref Animator animator, string clipName, ContainerFunction functionType, int frame, string id)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		if ((Object)(object)animator == (Object)null)
		{
			Log("Animator not found, AddAnimationEvent Cancelled", ((Component)this).gameObject);
		}
		if (clipName == string.Empty || id == string.Empty)
		{
			Log("Clip name and id cannot be blank, AddAnimationEvent Cancelled", ((Component)this).gameObject);
		}
		AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
		for (int i = 0; i < animationClips.Length; i++)
		{
			if (!((Object)(object)animationClips[i] != (Object)null) || !(((Object)animationClips[i]).name == clipName))
			{
				continue;
			}
			AnimationEvent val = new AnimationEvent();
			AnimationEvent[] events = animationClips[i].events;
			val.time = (float)frame / 30f;
			val.stringParameter = id;
			val.functionName = functionType.ToString();
			for (int j = 0; j < animationClips[i].events.Length; j++)
			{
				if (events[j].functionName.ToUpper() == functionType.ToString() && events[j].time == val.time)
				{
					Log("This event already exists on the clip.", ((Component)this).gameObject);
					return;
				}
			}
			animationClips[i].AddEvent(val);
			break;
		}
	}

	private void ClearAnimationEvents(string clipName)
	{
		AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
		for (int i = 0; i < animationClips.Length; i++)
		{
			if ((Object)(object)animationClips[i] != (Object)null && ((Object)animationClips[i]).name == clipName)
			{
				Log("Clearing events from " + clipName, ((Component)this).gameObject);
				animationClips[i].events = (AnimationEvent[])(object)new AnimationEvent[0];
				break;
			}
		}
	}

	private void Log(string message, GameObject gameObject, bool raiseWarning = false, bool raiseError = false)
	{
		if (debug)
		{
			if (raiseWarning)
			{
				Debug.LogWarning((object)message, (Object)(object)gameObject);
			}
			else if (raiseError)
			{
				Debug.LogError((object)message, (Object)(object)gameObject);
			}
			else
			{
				Debug.Log((object)message, (Object)(object)gameObject);
			}
		}
	}

	public bool Contains(string id)
	{
		if (sources == null)
		{
			return false;
		}
		return sources.ContainsKey(id);
	}

	public void Clear()
	{
		for (int i = 0; i < audioContainers.Length; i++)
		{
			ClearAnimationEvents(audioContainers[i].animationClipName);
		}
		sources.Clear();
	}

	public void Refresh()
	{
		S13AnimationAudioContainer[] array = audioContainers;
		for (int i = 0; i < array.Length; i++)
		{
			S13AnimationAudioContainer s13AnimationAudioContainer = array[i];
			if (s13AnimationAudioContainer.functionType == ContainerFunction.PlaySwitch || s13AnimationAudioContainer.functionType == ContainerFunction.StopSwitch)
			{
				if ((Object)(object)s13AnimationAudioContainer.audioSourceObject == (Object)null)
				{
					Log("Audio Container " + s13AnimationAudioContainer.id + " is missing an audioObject reference.", ((Component)this).gameObject, raiseWarning: true);
					continue;
				}
				S13AudioSource component = s13AnimationAudioContainer.audioSourceObject.GetComponent<S13AudioSource>();
				if ((Object)(object)component == (Object)null)
				{
					Log("object assigned to S13Switch does not contain a S13AudioSource", s13AnimationAudioContainer.audioSourceObject, raiseWarning: true);
					continue;
				}
				sources.Add(s13AnimationAudioContainer.id, component);
				Log("Adding " + s13AnimationAudioContainer.id, ((Component)component).gameObject);
			}
			for (int j = 0; j < s13AnimationAudioContainer.frame.Length; j++)
			{
				AddAnimationEvent(ref animator, s13AnimationAudioContainer.animationClipName, s13AnimationAudioContainer.functionType, s13AnimationAudioContainer.frame[j], s13AnimationAudioContainer.id);
			}
		}
	}

	public void PlaySwitch(string id)
	{
		if (sources != null)
		{
			if (sources.ContainsKey(id))
			{
				Log("ID FOUND, " + ((Object)((Component)this).gameObject).name + " is playing " + id, ((Component)this).gameObject);
				sources[id].Play();
			}
			else
			{
				Log("Audio Source with ID: " + id + ", not Found on " + ((Object)((Component)this).gameObject).name, ((Component)this).gameObject);
			}
		}
	}

	public void StopSwitch(string id)
	{
		if (sources != null && sources.ContainsKey(id))
		{
			sources[id].Stop();
		}
	}

	public void PlayLoadedSound(string id)
	{
		S13AudioManager.Instance.PlayAudio(id);
	}

	public void StopLoadedSound(string id)
	{
		S13AudioManager.Instance.StopAudio(id);
	}

	public void TriggerAudioEvent(string id)
	{
		S13AudioManager.Instance.InvokeEvent(id);
	}
}
