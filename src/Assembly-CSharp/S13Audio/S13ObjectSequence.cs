using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13ObjectSequence : S13AudioSource
{
	public AudioClip[] audioClips;

	public float[] gapTimes;

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

	private double _startTime;

	private double _pausedTime;

	private bool[] _isSequenceClipPaused;

	public override float Length
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < _audioSources.Length; i++)
			{
				num += _audioSources[i].clip.length + ((i >= gapTimes.Length) ? 0f : gapTimes[i]);
			}
			return num;
		}
	}

	private void Awake()
	{
		_audioSources = (AudioSource[])(object)new AudioSource[audioClips.Length];
		int num = 0;
		AudioClip[] array = audioClips;
		foreach (AudioClip clip in array)
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
		UpdateParameters();
		_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)audioClips[0].frequency));
		_isSequenceClipPaused = new bool[audioClips.Length];
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
		if (!loop && !(fadeInTime > 0f))
		{
			_startTime = AudioSettings.dspTime;
			double num2 = _startTime;
			_audioSources[0].Play();
			for (int i = 1; i < _audioSources.Length; i++)
			{
				num2 += (double)_audioSources[i - 1].clip.length;
				if (i <= gapTimes.Length)
				{
					num2 += (double)gapTimes[i - 1];
				}
				_audioSources[i].PlayScheduled(num2);
			}
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
		if (!loop && (!(fadeOutTime > 0f) || ignoreFade))
		{
			AudioSource[] audioSources = _audioSources;
			foreach (AudioSource val in audioSources)
			{
				val.Stop();
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
		if (loop)
		{
			if ((!(fadeOutTime > 0f) || ignoreFade) && !ignoreFade)
			{
			}
			return;
		}
		_pausedTime = AudioSettings.dspTime - _startTime;
		int num = 0;
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			if (val.isPlaying)
			{
				val.Pause();
				_isSequenceClipPaused[num] = true;
			}
			else
			{
				_isSequenceClipPaused[num] = false;
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
		_startTime = AudioSettings.dspTime;
		int num = 0;
		float num2 = 0f;
		double num3 = 0.0;
		double dspTime = AudioSettings.dspTime;
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			if (_isSequenceClipPaused[num])
			{
				if (_pausedTime < num3)
				{
					val.Stop();
					val.PlayScheduled(dspTime + num3 - _pausedTime);
				}
				else
				{
					val.UnPause();
				}
				num2 = ((num >= gapTimes.Length) ? 0f : gapTimes[num]);
				num3 += (double)(val.clip.length + num2);
				_isSequenceClipPaused[num] = false;
			}
			num++;
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
		audioClips[slotIndex] = clip;
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
		int sequenceIndex = 0;
		AudioSource playingAudio = _audioSources[sequenceIndex];
		if ((Object)(object)playingAudio == (Object)null || (Object)(object)playingAudio.clip == (Object)null)
		{
			yield return null;
		}
		while (true)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds(S13AudioSource.WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY));
			if (playingAudio.timeSamples == 0 || playingAudio.timeSamples >= playingAudio.clip.samples)
			{
				if (sequenceIndex == _audioSources.Length - 1)
				{
					break;
				}
				AudioSource[] audioSources = _audioSources;
				int num;
				sequenceIndex = (num = sequenceIndex + 1);
				playingAudio = audioSources[num];
			}
		}
		if (audioEndedHandler != null)
		{
			audioEndedHandler(this);
		}
	}
}
