using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13ObjectAmbient : S13AudioSource
{
	public AudioClip audioClip;

	public AudioSource rolloffTemplate;

	[AudioSlider("Fade-in time (s)", 0f, 60f)]
	public float fadeInTime = 1f;

	[AudioSlider("Fade-out time (s)", 0f, 60f)]
	public float fadeOutTime = 1f;

	public AnimationCurve fadeCurve;

	public bool enableRandomPitch = true;

	[AudioSlider("Random Pitch Min (semi)", -12, 12)]
	public int pitchMin;

	[AudioSlider("Random Pitch Max (semi)", -12, 12)]
	public int pitchMax;

	[AudioSlider("Replay Gate Time (ms)", 0, 1000)]
	public int retriggerTime;

	[AudioSlider("Chance To Play (%)", 0, 100)]
	public int chanceToPlay = 100;

	private AudioSource _audioSource;

	private S13FadeControl _fadeControl;

	private bool _canPlay = true;

	public override float Length => _audioSource.clip.length;

	public override int AudioSourceCount => 1;

	public override bool IsPlaying
	{
		get
		{
			if ((Object)(object)_fadeControl != (Object)null && _fadeControl.IsFadingOut)
			{
				return false;
			}
			if ((Object)(object)_audioSource == (Object)null)
			{
				return false;
			}
			return _audioSource.isPlaying;
		}
	}

	private void Awake()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_audioSource = new AudioSource();
		GameObject val;
		if ((Object)(object)rolloffTemplate != (Object)null)
		{
			val = _addAudioSourceChild("source " + ((Object)((Component)this).gameObject).name + ", " + ((Object)rolloffTemplate).name, ((Component)rolloffTemplate).gameObject);
			_audioSource = val.GetComponent<AudioSource>();
			_audioSource.outputAudioMixerGroup = rolloffTemplate.outputAudioMixerGroup;
		}
		else
		{
			val = _addAudioSourceChild("source " + ((Object)((Component)this).gameObject).name + ", default");
			_audioSource = val.AddComponent<AudioSource>();
		}
		_fadeControl = val.AddComponent<S13FadeControl>();
		_fadeControl.Curve = fadeCurve;
		UpdateParameters();
		if ((Object)(object)outputOverride != (Object)null)
		{
			_audioSource.outputAudioMixerGroup = outputOverride;
		}
		if ((Object)(object)audioClip != (Object)null)
		{
			_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)audioClip.frequency));
		}
	}

	private void OnEnable()
	{
		((Behaviour)_audioSource).enabled = true;
		if (playOnAwake)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		((Behaviour)_audioSource).enabled = false;
	}

	public override void Play()
	{
		if (IsPlaying)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)_audioSource))
		{
			Debug.LogWarning((object)"[S13Audio] - Yo, your audio source is null, you're about to hit some serious errors.");
			return;
		}
		if (chanceToPlay < 100)
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
		}
		if (_isPaused)
		{
			_isPaused = false;
		}
		if (fadeInTime > 0f)
		{
			_offsetImpl.SetPlayPosition(_audioSource, startRandom);
			_audioSource.Play();
			_fadeControl.FadeIn(_audioSource, _audioSource.volume, S13AudioUtil.dB2Lin(volume), fadeInTime, ignoreTimeScale);
		}
		else
		{
			_audioSource.volume = S13AudioUtil.dB2Lin(volume);
			_offsetImpl.SetPlayPosition(_audioSource, startRandom);
			if (timelineOffset == 0)
			{
				_audioSource.Play();
			}
			else
			{
				_audioSource.PlayScheduled(AudioSettings.dspTime + (double)((float)timelineOffset / 1000f));
			}
		}
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
		if (!IsPlaying)
		{
			return;
		}
		_isPaused = false;
		if (_audioSource.isPlaying)
		{
			if (fadeOutTime > 0f && !ignoreFade)
			{
				_fadeControl.FadeOut(_audioSource, _audioSource.volume, fadeOutTime, ignoreTimeScale, delegate
				{
					_audioSource.Stop();
				});
			}
			else
			{
				_audioSource.Stop();
				_audioSource.volume = 0f;
			}
		}
		_isPaused = false;
	}

	public override void Pause(bool ignoreFade)
	{
		if (!IsPlaying)
		{
			return;
		}
		if (fadeOutTime > 0f && !ignoreFade)
		{
			_fadeControl.FadeOut(_audioSource, _audioSource.volume, fadeOutTime, ignoreTimeScale, delegate
			{
				_audioSource.Pause();
			});
		}
		else
		{
			_audioSource.Pause();
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
		if (_isPaused)
		{
			_audioSource.UnPause();
			if (fadeInTime > 0f)
			{
				_fadeControl.FadeIn(_audioSource, 0f, S13AudioUtil.dB2Lin(volume), fadeInTime, ignoreTimeScale);
			}
			_isPaused = false;
		}
		if (audioEndedHandler != null)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
	}

	public override void SetClip(AudioClip clip, int slotIndex = 0)
	{
		if (_audioSource.isPlaying)
		{
			_audioSource.time = 0f;
		}
		_audioSource.clip = clip;
		_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)clip.frequency));
		audioClip = clip;
	}

	public override void SetMuting(bool muting)
	{
		if (muting != !mute)
		{
			mute = !muting;
			_audioSource.mute = mute;
		}
	}

	public override void UpdateParameters()
	{
		_audioSource.playOnAwake = false;
		_audioSource.clip = audioClip;
		_audioSource.loop = loop;
		_audioSource.volume = 0f;
		_audioSource.mute = mute;
		_audioSource.panStereo = pan;
		_audioSource.velocityUpdateMode = (AudioVelocityUpdateMode)0;
		if (enableRandomPitch)
		{
			_audioSource.pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
		}
		else
		{
			_audioSource.pitch = S13AudioUtil.SemitoneToPitch(pitch);
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
		if ((Object)(object)_audioSource == (Object)null || (Object)(object)_audioSource.clip == (Object)null)
		{
			yield return null;
		}
		do
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds(S13AudioSource.WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY));
		}
		while (_audioSource.timeSamples != 0 && _audioSource.timeSamples < _audioSource.clip.samples);
		if (audioEndedHandler != null)
		{
			audioEndedHandler(this);
		}
	}
}
