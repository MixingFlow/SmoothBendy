using UnityEngine;

namespace S13Audio;

public class GUITestButton : MonoBehaviour
{
	public GameObject audioSourceOrEvent;

	public Rect buttonRect = new Rect(40f, 40f, 200f, 40f);

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

	private void OnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(buttonRect, ((Object)audioSourceOrEvent).name))
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

	private void OnDrawGizmosSelected()
	{
	}
}
