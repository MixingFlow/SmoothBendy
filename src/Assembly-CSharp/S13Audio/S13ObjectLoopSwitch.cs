using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

public class S13ObjectLoopSwitch : S13AudioSource
{
	public AudioMixerGroup mixerOutput;

	public AudioClip[] audioSegments;

	[AudioSlider("Fade-in time (s)", 0f, 60f)]
	public float fadeInTime = 1f;

	[AudioSlider("Fade-out time (s)", 0f, 60f)]
	public float fadeOutTime = 1f;

	[AudioSlider("Transition time (s)", 0f, 60f)]
	public float transitionTime = 1f;

	public bool resumeOnPlay;

	private int _currentSegment;

	public override float Length => (!loop) ? _audioSources[_currentSegment].clip.length : (-1f);

	private void Awake()
	{
		_audioSources = (AudioSource[])(object)new AudioSource[audioSegments.Length];
		int num = 0;
		AudioClip[] array = audioSegments;
		foreach (AudioClip clip in array)
		{
			GameObject val = _addAudioSourceChild($"source{num}");
			_audioSources[num] = val.AddComponent<AudioSource>();
			_audioSources[num].clip = clip;
			_audioSources[num].loop = loop;
			_audioSources[num].outputAudioMixerGroup = mixerOutput;
			num++;
		}
		UpdateParameters();
		_resumeOnPlay = new S13ResumeImpl(audioSegments.Length);
		if ((Object)(object)audioSegments[0] != (Object)null)
		{
			_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)audioSegments[0].frequency));
		}
		_currentSegment = 0;
	}

	public override void Play()
	{
		if (resumeOnPlay)
		{
			_resumeOnPlay.ResumePosition(_audioSources[_currentSegment], _currentSegment);
		}
		else if (_offsetImpl != null)
		{
			_offsetImpl.SetPlayPosition(_audioSources[_currentSegment], startRandom);
		}
		if (fadeInTime > 0f)
		{
			_audioSources[_currentSegment].Play();
			((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(_audioSources[_currentSegment], fadeInTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
		}
		else if (timelineOffset == 0)
		{
			_audioSources[_currentSegment].Play();
		}
		else
		{
			_audioSources[_currentSegment].PlayScheduled(AudioSettings.dspTime + (double)((float)timelineOffset / 1000f));
		}
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void Stop(bool ignoreFade)
	{
		base.Stop(ignoreFade);
		if (resumeOnPlay)
		{
			_resumeOnPlay.SavePosition(_audioSources[_currentSegment], _currentSegment);
		}
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource source in audioSources)
		{
			if (!source.isPlaying)
			{
				continue;
			}
			if (fadeOutTime > 0f && !ignoreFade)
			{
				((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeOut(source, fadeOutTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale, delegate
				{
					if (resumeOnPlay)
					{
						source.Pause();
					}
					else
					{
						source.Stop();
					}
					source.volume = S13AudioUtil.dB2Lin(volume);
				}));
			}
			else if (resumeOnPlay)
			{
				source.Pause();
			}
			else
			{
				source.Stop();
			}
		}
	}

	public override void Pause(bool ignoreFadeOut)
	{
		AudioSource source = _audioSources[_currentSegment];
		if (source.isPlaying)
		{
			if (fadeOutTime > 0f && !ignoreFadeOut)
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
		}
		_isPaused = true;
	}

	public override void Resume()
	{
		if (!_isPaused)
		{
			Debug.LogWarning((object)(((Object)this).name + ": Sound is not paused; cannot resume."), (Object)(object)((Component)this).gameObject);
			return;
		}
		((MonoBehaviour)this).StopAllCoroutines();
		AudioSource val = _audioSources[_currentSegment];
		val.UnPause();
		if (fadeInTime > 0f)
		{
			((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(val, fadeInTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
		}
		_isPaused = false;
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void SetClip(AudioClip clip, int slotIndex = 0)
	{
		base.SetClip(clip, slotIndex);
		audioSegments[slotIndex] = clip;
	}

	public void TransitionToSegment(int toSegment)
	{
		if (_audioSources[_currentSegment].isPlaying && _currentSegment != toSegment)
		{
			if (resumeOnPlay)
			{
				_resumeOnPlay.ResumePosition(_audioSources[toSegment], toSegment);
				_resumeOnPlay.SavePosition(_audioSources[_currentSegment], _currentSegment);
			}
			((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(_audioSources[toSegment], transitionTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
			((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeOut(_audioSources[_currentSegment], transitionTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale, delegate
			{
				_currentSegment = toSegment;
			}));
		}
	}

	private IEnumerator WaitForAudioEnd()
	{
		AudioSource playingAudio = _audioSources[_currentSegment];
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
