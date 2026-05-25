using UnityEngine;

namespace S13Audio;

[RequireComponent(typeof(Collider))]
public class ENVTestTrigger : MonoBehaviour
{
	public GameObject audioSourceOrEvent;

	private S13AudioManager _audioManager;

	private S13AudioSource _audioSource;

	private void Start()
	{
		_audioManager = Object.FindObjectOfType<S13AudioManager>();
		if ((Object)(object)_audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": AudioManager not found."));
		}
		_audioSource = audioSourceOrEvent.GetComponent<S13AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)_audioSource != (Object)null)
		{
			_audioManager.PlayAudio(((Object)_audioSource).name);
		}
		else
		{
			_audioManager.InvokeEvent(((Object)audioSourceOrEvent).name);
		}
	}
}
