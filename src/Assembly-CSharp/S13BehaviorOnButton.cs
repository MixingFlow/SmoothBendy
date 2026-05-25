using S13Audio;
using UnityEngine;

public class S13BehaviorOnButton : MonoBehaviour
{
	private S13AudioManager am;

	public void InvokeEvent(string eventName)
	{
		if ((Object)(object)am == (Object)null)
		{
			am = Object.FindObjectOfType<S13AudioManager>();
		}
		am.InvokeEvent(eventName);
	}

	public void PlayAudio(string objectName)
	{
		if ((Object)(object)am == (Object)null)
		{
			am = Object.FindObjectOfType<S13AudioManager>();
		}
		am.PlayAudio(objectName);
	}
}
