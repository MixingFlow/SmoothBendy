using UnityEngine;

namespace S13Audio;

public class S13BehaviorOnStart : MonoBehaviour
{
	public string onStart;

	public float delayStart;

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

	private void Start()
	{
		if (Object.op_Implicit((Object)(object)_audioManager) && !(onStart == string.Empty))
		{
			if (onStart.StartsWith("evt_"))
			{
				_audioManager.InvokeEvent(onStart, delayStart);
			}
			else if (onStart.StartsWith("mxs_"))
			{
				_audioManager.ToSnapshot(mixerName, onStart, delayStart);
			}
			else if (delayStart > 0f)
			{
				_audioManager.PlayAudioDelayed(onStart, delayStart);
			}
			else
			{
				_audioManager.PlayAudio(onStart);
			}
		}
	}
}
