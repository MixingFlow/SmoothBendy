using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

public class S13VoicesImpl
{
	public delegate void PlayDelegate(int voiceIndex);

	private AudioSource[] _audioSources;

	private List<AudioSource> _activeSounds;

	private int _maxVoices;

	private int _clipVoices;

	public VoiceMode VoiceMode { get; set; }

	public S13VoicesImpl(VoiceMode voiceMode, AudioSource[] audioSources, int maxVoices)
	{
		_audioSources = audioSources;
		VoiceMode = voiceMode;
		_maxVoices = maxVoices;
		_clipVoices = Mathf.CeilToInt((float)maxVoices / (float)audioSources.Length);
		_activeSounds = new List<AudioSource>(maxVoices);
	}

	public void Play(int soundIndex, PlayDelegate play)
	{
		if (_audioSources == null || _audioSources.Length == 0)
		{
			Debug.LogWarning((object)"VoicesImpl: Reference to audio sources is null or empty; cannot play.");
			return;
		}
		switch (VoiceMode)
		{
		case VoiceMode.KillOld:
		{
			if (_activeSounds.Count >= _maxVoices)
			{
				AudioSource val = _activeSounds[0];
				_activeSounds.RemoveAt(0);
				val.Stop();
			}
			int num = soundIndex * _clipVoices + Mathf.FloorToInt((float)(_activeSounds.Count / _audioSources.Length));
			play(num);
			_activeSounds.Add(_audioSources[num]);
			break;
		}
		case VoiceMode.IgnoreNew:
			if (_activeSounds.Count < _maxVoices)
			{
				int num = soundIndex * _clipVoices + Mathf.FloorToInt((float)(_activeSounds.Count / _audioSources.Length));
				play(num);
				_activeSounds.Add(_audioSources[num]);
			}
			break;
		}
	}

	public void Update()
	{
		for (int i = 0; i < _activeSounds.Count; i++)
		{
			if (!_activeSounds[i].isPlaying)
			{
				_activeSounds.RemoveAt(i);
			}
		}
	}

	private void LogActiveSounds()
	{
		Debug.Log((object)"** Active sounds list:");
		foreach (AudioSource activeSound in _activeSounds)
		{
			Debug.Log((object)(((Object)activeSound).name + " at index " + _activeSounds.IndexOf(activeSound)));
		}
	}
}
