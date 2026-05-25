using UnityEngine;

namespace S13Audio;

public class S13AudioEventTrigger : MonoBehaviour
{
	public bool triggerOnEnter = true;

	public bool triggerOnExit = true;

	public string onEnterEventName = string.Empty;

	public string onExitEventName = string.Empty;

	[Space]
	public float onEnterDelayTime;

	public float onExitDelayTime;

	public bool useDelayTimes;

	private S13AudioManager am;

	private void Awake()
	{
		am = S13AudioManager.Instance;
		if (((object)am).Equals((object)null))
		{
			Debug.LogError((object)"S13 Audio Event Trigger cannot find instance of S13AudioManager in scene", (Object)(object)((Component)this).gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (triggerOnEnter)
		{
			am.InvokeEvent(onEnterEventName, onEnterDelayTime);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (triggerOnExit)
		{
			am.InvokeEvent(onExitEventName, onEnterDelayTime);
		}
	}
}
