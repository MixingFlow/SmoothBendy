using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13SoundBank : MonoBehaviour
{
	public string soundBankId;

	public S13AudioSource[] sounds;

	private Dictionary<string, S13AudioSource> _data = new Dictionary<string, S13AudioSource>();

	public S13AudioSource this[string soundId]
	{
		get
		{
			S13AudioSource result = null;
			if (_data.ContainsKey(soundId))
			{
				result = _data[soundId];
			}
			return result;
		}
	}

	public Dictionary<string, S13AudioSource>.KeyCollection SoundIDs => _data.Keys;

	private void Awake()
	{
		int num = 0;
		num += RegisterSounds(sounds);
		num += RegisterSounds(((Component)this).GetComponentsInChildren<S13AudioSource>());
		Debug.Log((object)(((Object)this).name + ": Initialized sound bank with " + num + " sounds."));
	}

	public void RemoveSoundFromBank(string soundIdToRemove)
	{
		_data.Remove(soundIdToRemove);
	}

	private int RegisterSounds(S13AudioSource[] sounds)
	{
		int num = 0;
		foreach (S13AudioSource s13AudioSource in sounds)
		{
			if (!_data.ContainsKey(((Object)s13AudioSource).name))
			{
				_data.Add(((Object)s13AudioSource).name, s13AudioSource);
				num++;
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": A sound with the name '" + ((Object)s13AudioSource).name + "' already exists; sound was not added to the sound bank."));
			}
		}
		return num;
	}
}
