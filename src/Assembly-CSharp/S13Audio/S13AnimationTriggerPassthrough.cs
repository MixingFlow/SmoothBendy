using UnityEngine;

namespace S13Audio;

public class S13AnimationTriggerPassthrough : MonoBehaviour
{
	private S13AnimationSwitch animationSwitch;

	private void Start()
	{
		animationSwitch = ((Component)this).GetComponentInChildren<S13AnimationSwitch>();
		if ((Object)(object)animationSwitch == (Object)null)
		{
			Debug.LogError((object)"S13AnimationTriggerPassthrough cannot find an S13AnimationSwitch in child objects.");
		}
	}

	public void PlaySwitch(string id)
	{
		if (!((Object)(object)animationSwitch == (Object)null))
		{
			animationSwitch.PlaySwitch(id);
		}
	}

	public void StopSwitch(string id)
	{
		if (!((Object)(object)animationSwitch == (Object)null))
		{
			animationSwitch.StopSwitch(id);
		}
	}

	public void PlayLoadedSound(string id)
	{
		if (!((Object)(object)animationSwitch == (Object)null))
		{
			animationSwitch.PlayLoadedSound(id);
		}
	}

	public void StopLoadedSound(string id)
	{
		if (!((Object)(object)animationSwitch == (Object)null))
		{
			animationSwitch.StopLoadedSound(id);
		}
	}

	public void TriggerAudioEvent(string id)
	{
		if (!((Object)(object)animationSwitch == (Object)null))
		{
			animationSwitch.TriggerAudioEvent(id);
		}
	}
}
