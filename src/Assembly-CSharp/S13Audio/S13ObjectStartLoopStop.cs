using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13ObjectStartLoopStop : S13AudioSource
{
	public AudioClip startClip;

	public AudioClip loopClip;

	public AudioClip stopClip;

	public AudioSource rolloffTemplate;

	public AudioVelocityUpdateMode velocityUpdateMode;

	[AudioSlider("Fade-in time (s)", 0f, 60f)]
	public float fadeInTime;

	[AudioSlider("Fade-out time (s)", 0f, 60f)]
	public float fadeOutTime;

	[AudioSlider("Chance To Play (%)", 0, 100)]
	public int chanceToPlay = 100;

	private bool _isPlaying;

	private bool _canPlay = true;

	private int _pausedState;

	public override float Length => -1f;

	private void Awake()
	{
		_audioSources = (AudioSource[])(object)new AudioSource[3];
		AudioClip[] array = (AudioClip[])(object)new AudioClip[3] { startClip, loopClip, stopClip };
		int num = 0;
		AudioClip[] array2 = array;
		foreach (AudioClip clip in array2)
		{
			if ((Object)(object)rolloffTemplate != (Object)null)
			{
				GameObject val = _addAudioSourceChild($"source{num}", ((Component)rolloffTemplate).gameObject);
				_audioSources[num] = val.GetComponent<AudioSource>();
				_audioSources[num].outputAudioMixerGroup = rolloffTemplate.outputAudioMixerGroup;
			}
			else
			{
				GameObject val = _addAudioSourceChild($"source{num}");
				_audioSources[num] = val.AddComponent<AudioSource>();
			}
			_audioSources[num].clip = clip;
			_audioSources[num].loop = false;
			if ((Object)(object)outputOverride != (Object)null)
			{
				_audioSources[num].outputAudioMixerGroup = outputOverride;
			}
			num++;
		}
		_audioSources[1].loop = true;
		UpdateParameters();
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
		_isPlaying = true;
		if (!(fadeInTime > 0f))
		{
			_audioSources[0].Play();
			double num2 = AudioSettings.dspTime + (double)_audioSources[0].clip.length;
			_audioSources[1].PlayScheduled(num2);
			_audioSources[1].SetScheduledStartTime(num2);
		}
		if (_isPaused)
		{
			_isPaused = false;
		}
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void Stop(bool ignoreFade)
	{
		base.Stop(ignoreFade);
		_isPlaying = false;
		if (!(fadeOutTime > 0f) || ignoreFade)
		{
			if (_audioSources[0].isPlaying)
			{
				_audioSources[1].Stop();
			}
			else if (_audioSources[1].isPlaying)
			{
				_audioSources[1].Stop();
				_audioSources[2].Play();
			}
			else if (_audioSources[2].isPlaying)
			{
				_audioSources[2].Stop();
			}
		}
		if (_isPaused)
		{
			_isPaused = false;
		}
	}

	public override void Pause(bool ignoreFade)
	{
		if (!_isPlaying)
		{
			return;
		}
		_isPaused = true;
		int num = 0;
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			if (val.isPlaying)
			{
				val.Pause();
				_pausedState = num;
				break;
			}
			num++;
		}
	}

	public override void Resume()
	{
		if (!_isPaused)
		{
			Debug.LogWarning((object)(((Object)this).name + ": Sound is not paused; cannot resume."), (Object)(object)((Component)this).gameObject);
			return;
		}
		_isPaused = false;
		switch (_pausedState)
		{
		case 0:
		{
			double num = AudioSettings.dspTime + (double)_audioSources[0].clip.length - (double)_audioSources[0].time;
			_audioSources[0].UnPause();
			_audioSources[1].PlayScheduled(num);
			_audioSources[1].SetScheduledStartTime(num);
			break;
		}
		case 1:
			_audioSources[1].UnPause();
			break;
		case 2:
			_audioSources[2].UnPause();
			break;
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
		switch (slotIndex)
		{
		case 0:
			startClip = clip;
			break;
		case 1:
			loopClip = clip;
			break;
		case 2:
			stopClip = clip;
			break;
		}
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

	private IEnumerator WaitForAudioEnd()
	{
		int clipIndex = 0;
		AudioSource playingAudio = _audioSources[clipIndex];
		if ((Object)(object)playingAudio == (Object)null || (Object)(object)playingAudio.clip == (Object)null)
		{
			yield return null;
		}
		while (true)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds(S13AudioSource.WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY));
			if (playingAudio.timeSamples == 0 || playingAudio.timeSamples >= playingAudio.clip.samples)
			{
				if (clipIndex == _audioSources.Length - 1)
				{
					break;
				}
				AudioSource[] audioSources = _audioSources;
				int num;
				clipIndex = (num = clipIndex + 1);
				playingAudio = audioSources[num];
			}
		}
		if (audioEndedHandler != null)
		{
			audioEndedHandler(this);
		}
	}
}
