using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S13Audio;

public class SoundTest : MonoBehaviour
{
	public Dropdown eventDropDown;

	public Dropdown soundDropDown;

	public InputField eventIDInputField;

	public Button startEventButton;

	public Button stopEventButton;

	public Button stopAllAudioButton;

	public Toggle ignoreFadeToggle;

	[Space]
	[Header("Make sure to drag in the S13AudioManager before use.")]
	public S13AudioManager audioManager;

	private void Awake()
	{
		try
		{
			audioManager = S13AudioManager.Instance;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("No instance of S13AudioManager found in scene: " + ex), (Object)(object)((Component)this).gameObject);
			throw;
		}
		if ((Object)(object)audioManager == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": Could not find S13AudioManager component attached to Sound Test."), (Object)(object)((Component)this).gameObject);
		}
		eventDropDown.ClearOptions();
		soundDropDown.ClearOptions();
	}

	private void Start()
	{
		eventDropDown.AddOptions(S13EventList.GetList());
		S13AudioSource[] componentsInChildren = ((Component)audioManager).GetComponentsInChildren<S13AudioSource>();
		List<string> list = new List<string>(componentsInChildren.Length);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			list.Add(((Object)((Component)componentsInChildren[i]).gameObject).name);
		}
		soundDropDown.AddOptions(list);
	}

	public void InvokeEvent()
	{
		if (eventIDInputField.text != string.Empty)
		{
			audioManager.InvokeEvent(eventIDInputField.text);
		}
		else
		{
			audioManager.InvokeEvent(eventDropDown.captionText.text);
		}
	}

	public void PlayAudio()
	{
		audioManager.PlayAudio(soundDropDown.captionText.text);
	}

	public void StopAudio()
	{
		audioManager.StopAudio(soundDropDown.captionText.text, Object.op_Implicit((Object)(object)ignoreFadeToggle));
	}

	public void StopAllAudio()
	{
		Debug.Log((object)"Stopping all audio playback");
		audioManager.StopAllAudio(Object.op_Implicit((Object)(object)ignoreFadeToggle));
	}
}
