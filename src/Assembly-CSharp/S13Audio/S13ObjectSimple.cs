using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13ObjectSimple : S13AudioSource
{
	public AudioClip audioClip;

	[AudioField("Voice Max", 1, 50)]
	public int voices = 1;

	public VoiceMode voiceMode;

	public AudioSource rolloffTemplate;

	public AudioVelocityUpdateMode velocityUpdateMode;

	public bool usesExternalVolume;

	[AudioSlider("Fade-in time (s)", 0f, 60f)]
	public float fadeInTime = 1f;

	[AudioSlider("Fade-out time (s)", 0f, 60f)]
	public float fadeOutTime = 1f;

	public bool enableRandomPitch = true;

	[AudioSlider("Random Pitch Min (semi)", -12, 12)]
	public int pitchMin;

	[AudioSlider("Random Pitch Max (semi)", -12, 12)]
	public int pitchMax;

	[AudioSlider("Replay Gate Time (ms)", 0, 1000)]
	public int retriggerTime;

	[AudioSlider("Chance To Play (%)", 0, 100)]
	public int chanceToPlay = 100;

	private S13VoicesImpl _voicesImpl;

	private int _soundIndex;

	private bool _canPlay = true;

	private bool[] _isVoicePaused;

	public override float Length => (!loop) ? _audioSources[0].clip.length : (-1f);

	private void Awake()
	{
		_audioSources = (AudioSource[])(object)new AudioSource[voices];
		_isVoicePaused = new bool[voices];
		for (int i = 0; i < voices; i++)
		{
			if ((Object)(object)rolloffTemplate != (Object)null)
			{
				GameObject val = _addAudioSourceChild($"source{i}", ((Component)rolloffTemplate).gameObject);
				_audioSources[i] = val.GetComponent<AudioSource>();
				_audioSources[i].outputAudioMixerGroup = rolloffTemplate.outputAudioMixerGroup;
			}
			else
			{
				GameObject val = _addAudioSourceChild($"source{i}");
				_audioSources[i] = val.AddComponent<AudioSource>();
			}
			_audioSources[i].playOnAwake = false;
			_audioSources[i].clip = audioClip;
			_audioSources[i].loop = loop;
			if ((Object)(object)outputOverride != (Object)null)
			{
				_audioSources[i].outputAudioMixerGroup = outputOverride;
			}
			_isVoicePaused[i] = false;
		}
		UpdateParameters();
		if ((Object)(object)audioClip != (Object)null)
		{
			_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)audioClip.frequency));
		}
		_voicesImpl = new S13VoicesImpl(voiceMode, _audioSources, voices);
		_soundIndex = 0;
	}

	private void LateUpdate()
	{
		_voicesImpl.Update();
	}

	public override void Play()
	{
		float num = (float)chanceToPlay / 100f;
		if (!_canPlay || Random.value > num)
		{
			if (audioEndedHandler != null)
			{
				audioEndedHandler(this);
			}
			return;
		}
		if (_isPaused)
		{
			_isPaused = false;
			for (int i = 0; i < _audioSources.Length; i++)
			{
				_isVoicePaused[i] = false;
			}
		}
		if (fadeInTime > 0f)
		{
			_voicesImpl.Play(_soundIndex, delegate(int voiceIndex)
			{
				AudioSource val = _audioSources[voiceIndex];
				if (enableRandomPitch)
				{
					val.pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
				}
				_offsetImpl.SetPlayPosition(val, startRandom);
				val.Play();
				((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(val, fadeInTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
			});
		}
		else
		{
			if (_voicesImpl == null)
			{
				Debug.LogWarning((object)"Missing Audio Reference.", (Object)(object)((Component)this).gameObject);
				return;
			}
			_voicesImpl.Play(_soundIndex, delegate(int voiceIndex)
			{
				AudioSource val = _audioSources[voiceIndex];
				if (enableRandomPitch)
				{
					val.pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
				}
				if (!usesExternalVolume)
				{
					val.volume = S13AudioUtil.dB2Lin(volume);
				}
				_offsetImpl.SetPlayPosition(val, startRandom);
				if (timelineOffset == 0)
				{
					val.Play();
				}
				else
				{
					val.PlayScheduled(AudioSettings.dspTime + (double)((float)timelineOffset / 1000f));
				}
			});
		}
		_soundIndex = ((++_soundIndex < voices) ? _soundIndex : 0);
		if (retriggerTime > 0)
		{
			((MonoBehaviour)this).StartCoroutine(TriggerWait());
		}
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void Stop(bool ignoreFade)
	{
		base.Stop(ignoreFade);
		_isPaused = false;
		for (int i = 0; i < _audioSources.Length; i++)
		{
			AudioSource source = _audioSources[i];
			if (source.isPlaying)
			{
				if (fadeOutTime > 0f && !ignoreFade)
				{
					((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeOut(source, fadeOutTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale, delegate
					{
						source.Stop();
						source.volume = S13AudioUtil.dB2Lin(volume);
					}));
				}
				else
				{
					source.Stop();
				}
			}
			_isVoicePaused[i] = false;
		}
	}

	public override void Pause(bool ignoreFade)
	{
		for (int i = 0; i < _audioSources.Length; i++)
		{
			AudioSource source = _audioSources[i];
			if (!source.isPlaying)
			{
				continue;
			}
			if (fadeOutTime > 0f && !ignoreFade)
			{
				((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeOut(source, fadeOutTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale, delegate
				{
					source.Pause();
					source.volume = S13AudioUtil.dB2Lin(volume);
				}));
			}
			else
			{
				source.Pause();
			}
			_isVoicePaused[i] = true;
			_isPaused = true;
		}
	}

	public override void Resume()
	{
		if (!_isPaused)
		{
			Debug.LogWarning((object)(((Object)this).name + ": Sound is not paused; cannot resume."), (Object)(object)((Component)this).gameObject);
			return;
		}
		((MonoBehaviour)this).StopAllCoroutines();
		for (int i = 0; i < _audioSources.Length; i++)
		{
			AudioSource val = _audioSources[i];
			if (_isVoicePaused[i])
			{
				val.UnPause();
				if (fadeInTime > 0f)
				{
					((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(val, fadeInTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
				}
				_isPaused = false;
				_isVoicePaused[i] = false;
			}
		}
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void UpdateParameters()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateParameters();
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			val.velocityUpdateMode = velocityUpdateMode;
		}
	}

	public override void SetClip(AudioClip clip, int slotIndex = 0)
	{
		base.SetClip(clip, slotIndex);
		audioClip = clip;
	}

	public override void DrawGizmos()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rolloffTemplate != (Object)null)
		{
			Gizmos.color = S13AudioSource.DEFAULT_GIZMO_COLOR;
			Gizmos.DrawWireSphere(((Component)this).transform.position, rolloffTemplate.maxDistance);
		}
	}

	private IEnumerator TriggerWait()
	{
		_canPlay = false;
		if (ignoreTimeScale)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds((float)retriggerTime / 1000f));
		}
		else
		{
			yield return (object)new WaitForSeconds((float)retriggerTime / 1000f);
		}
		_canPlay = true;
	}

	private IEnumerator WaitForAudioEnd()
	{
		if (_audioSources.Length > 1)
		{
			Debug.LogError((object)(((Object)this).name + ": Assigning callback to audio source with more than one voice has undefined behavior."));
		}
		AudioSource playingAudio = _audioSources[0];
		if ((Object)(object)playingAudio == (Object)null || (Object)(object)playingAudio.clip == (Object)null)
		{
			yield return null;
		}
		do
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds(S13AudioSource.WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY));
		}
		while (playingAudio.timeSamples != 0 && playingAudio.timeSamples < playingAudio.clip.samples);
		if (audioEndedHandler != null)
		{
			audioEndedHandler(this);
		}
	}
}
