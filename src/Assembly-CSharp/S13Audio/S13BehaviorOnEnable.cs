using UnityEngine;

namespace S13Audio;

public class S13BehaviorOnEnable : MonoBehaviour
{
	[Tooltip("Sends message when this script, object or parent is Enabled or Added to the Scene")]
	public string onEnable;

	[Tooltip("Waits for 'X' seconds to send the message")]
	public float delayEnable;

	[Tooltip("Sends message when this script, object or parent is Disabled or Removed from the Scene")]
	public string onDisable;

	[Tooltip("Waits for 'X' seconds to send the message")]
	public float delayDisable;

	[Tooltip("Only one common mixer name possible")]
	public string mixerName;

	private S13AudioManager _audioManager;

	private void Awake()
	{
		_audioManager = Object.FindObjectOfType<S13AudioManager>();
		if ((Object)(object)_audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": No instance of AudioManager found in scene."));
		}
	}

	private void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)_audioManager) && !(onEnable == string.Empty))
		{
			if (onEnable.StartsWith("evt_"))
			{
				_audioManager.InvokeEvent(onEnable, delayEnable);
			}
			else if (onEnable.StartsWith("mxs_"))
			{
				_audioManager.ToSnapshot(mixerName, onEnable, delayEnable);
			}
			else if (delayEnable > 0f)
			{
				_audioManager.PlayAudioDelayed(onEnable, delayEnable);
			}
			else
			{
				_audioManager.PlayAudio(onEnable);
			}
		}
	}

	private void OnDisable()
	{
		if (Object.op_Implicit((Object)(object)_audioManager) && !(onDisable == string.Empty))
		{
			if (onDisable.StartsWith("evt_"))
			{
				_audioManager.InvokeEvent(onDisable, delayDisable);
			}
			else if (onDisable.StartsWith("mxs_"))
			{
				_audioManager.ToSnapshot(mixerName, onDisable, delayDisable);
			}
			else if (delayDisable > 0f)
			{
				_audioManager.StopAudioDelayed(onDisable, delayDisable);
			}
			else
			{
				_audioManager.StopAudio(onDisable);
			}
		}
	}
}
