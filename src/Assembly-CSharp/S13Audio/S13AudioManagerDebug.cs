using UnityEngine;

namespace S13Audio;

public class S13AudioManagerDebug : MonoBehaviour
{
	private S13AudioManager audioManager;

	private string soundId = string.Empty;

	private Rect controlsArea;

	private void Awake()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		controlsArea = new Rect(20f, 20f, 400f, 280f);
	}

	private void Start()
	{
		audioManager = ((Component)this).GetComponentInParent<S13AudioManager>();
		if ((Object)(object)audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": Could not find AudioManager component on parent game object."));
		}
	}

	private void OnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginArea(controlsArea, "Audio Manager Debug");
		GUILayout.Space(40f);
		GUILayout.Label("Sound ID or Event name:", (GUILayoutOption[])(object)new GUILayoutOption[0]);
		soundId = GUILayout.TextField(soundId, 128, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Play Audio", (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			audioManager.PlayAudio(soundId);
		}
		if (GUILayout.Button("Stop Audio", (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			audioManager.StopAudio(soundId);
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("Invoke Event", (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			audioManager.InvokeEvent(soundId);
		}
		if (GUILayout.Button("Stop All Audio", (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			audioManager.StopAllAudio();
		}
		GUILayout.EndArea();
	}
}
