using UnityEngine;

namespace S13Audio;

[RequireComponent(typeof(Collider))]
public class S13BehaviorTriggerStrings : MonoBehaviour
{
	private enum EventType
	{
		InvokeEvent,
		MixerSnapshot,
		PlayAndStop
	}

	[SerializeField]
	private EventType eventType;

	[SerializeField]
	private string stringIDEnter;

	[AudioSlider("Delay/Transition Time", 0f, 1000f)]
	[SerializeField]
	private float enterTiming;

	[SerializeField]
	private string stringIDExit;

	[AudioSlider("Delay/Transition Time", 0f, 1000f)]
	[SerializeField]
	private float exitTiming;

	[Space]
	[SerializeField]
	private string mixerName;

	[Header("Trigger Conditions")]
	[SerializeField]
	private string triggerObjectTag;

	[SerializeField]
	private string triggerObjectName;

	[SerializeField]
	private string triggerObjectLayerName = "Audio";

	private S13AudioManager _audioManager;

	private void Awake()
	{
		_audioManager = Object.FindObjectOfType<S13AudioManager>();
		if ((Object)(object)_audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": No instance of AudioManager found in scene."), (Object)(object)((Component)this).gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			EnterHandler();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			EnterHandler();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			ExitHandler();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (MatchConditions(((Component)other).gameObject))
		{
			ExitHandler();
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

	private void EnterHandler()
	{
		if (!Object.op_Implicit((Object)(object)_audioManager) || stringIDEnter == string.Empty)
		{
			return;
		}
		switch (eventType)
		{
		case EventType.InvokeEvent:
			_audioManager.InvokeEvent(stringIDEnter, enterTiming);
			break;
		case EventType.MixerSnapshot:
			if (mixerName == string.Empty)
			{
				Debug.LogWarning((object)"Must enter a mixer name to change a snapshot", (Object)(object)((Component)this).gameObject);
			}
			else
			{
				_audioManager.ToSnapshot(mixerName, stringIDEnter, enterTiming);
			}
			break;
		case EventType.PlayAndStop:
			if (enterTiming > 0f)
			{
				_audioManager.PlayAudioDelayed(stringIDEnter, enterTiming);
			}
			else
			{
				_audioManager.PlayAudio(stringIDEnter);
			}
			break;
		}
	}

	private void ExitHandler()
	{
		if (!Object.op_Implicit((Object)(object)_audioManager) || stringIDExit == string.Empty)
		{
			return;
		}
		switch (eventType)
		{
		case EventType.InvokeEvent:
			_audioManager.InvokeEvent(stringIDExit, exitTiming);
			break;
		case EventType.MixerSnapshot:
			if (mixerName == string.Empty)
			{
				Debug.LogWarning((object)"Must enter a mixer name to change a snapshot", (Object)(object)((Component)this).gameObject);
			}
			else
			{
				_audioManager.ToSnapshot(mixerName, stringIDExit, exitTiming);
			}
			break;
		case EventType.PlayAndStop:
			if (exitTiming > 0f)
			{
				_audioManager.StopAudioDelayed(stringIDExit, exitTiming);
			}
			else
			{
				_audioManager.StopAudio(stringIDExit);
			}
			break;
		}
	}
}
