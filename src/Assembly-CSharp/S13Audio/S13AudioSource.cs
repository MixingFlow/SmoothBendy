using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace S13Audio;

public abstract class S13AudioSource : MonoBehaviour, IAudioSource
{
	public delegate void AudioEndedHandler(S13AudioSource audioSource);

	public S13AudioGroup group;

	public AudioMixerGroup outputOverride;

	public bool mute;

	public bool loop;

	public bool playOnAwake;

	public bool gamePauseEnabled = true;

	public bool ignoreTimeScale = true;

	[AudioSlider("Volume (dB)", -60f, 0f)]
	public float volume;

	[AudioSlider("Pitch (semitones)", -12f, 12f)]
	public float pitch;

	[AudioSlider("Stereo Pan", -1f, 1f)]
	public float pan;

	[AudioField("Start Position (s)", 0f, 3600f)]
	public float offsetTime;

	public bool startRandom;

	[AudioSlider("Start Delay Time (ms)", 0, 5000)]
	public int timelineOffset;

	protected AudioSource[] _audioSources;

	protected S13OffsetImpl _offsetImpl;

	protected S13ResumeImpl _resumeOnPlay;

	protected bool _isPaused;

	private IEnumerator _delayCoroutine;

	public AudioEndedHandler audioEndedHandler;

	protected static float WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY = 0.25f;

	protected static Color DEFAULT_GIZMO_COLOR = new Color(0.49803922f, 0.69803923f, 0.99607843f, 0.62f);

	public abstract float Length { get; }

	public virtual int AudioSourceCount => _audioSources.Length;

	public virtual bool IsPlaying
	{
		get
		{
			AudioSource[] audioSources = _audioSources;
			foreach (AudioSource val in audioSources)
			{
				if (val.isPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void OnEnable()
	{
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			((Behaviour)val).enabled = true;
		}
		if (playOnAwake)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			((Behaviour)val).enabled = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		DrawGizmos();
	}

	public abstract void Play();

	public abstract void Pause(bool ignoreFade = false);

	public abstract void Resume();

	public virtual void UpdateParameters()
	{
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource val in audioSources)
		{
			val.mute = mute;
			val.volume = S13AudioUtil.dB2Lin(volume);
			val.pitch = S13AudioUtil.SemitoneToPitch(pitch);
			val.panStereo = pan;
		}
	}

	public virtual void Play(float duration)
	{
		Play();
		((MonoBehaviour)this).StartCoroutine(S13AudioUtil.WaitForDuration(duration, ignoreTimeScale, delegate
		{
			Stop();
		}));
	}

	public virtual void PlayDelayed(float delayTime)
	{
		_delayCoroutine = S13AudioUtil.WaitForDuration(delayTime, ignoreTimeScale, delegate
		{
			Play();
		});
		((MonoBehaviour)this).StartCoroutine(_delayCoroutine);
	}

	public virtual void Stop(bool ignoreFade = false)
	{
		if (_delayCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(_delayCoroutine);
			_delayCoroutine = null;
		}
	}

	public virtual void StopDelayed(float delayTime, bool ignoreFade = false)
	{
		((MonoBehaviour)this).StartCoroutine(S13AudioUtil.WaitForDuration(delayTime, ignoreTimeScale, delegate
		{
			Stop(ignoreFade);
		}));
	}

	public virtual void SetMuting(bool muting)
	{
		if (muting != !mute)
		{
			mute = !muting;
			AudioSource[] audioSources = _audioSources;
			foreach (AudioSource val in audioSources)
			{
				val.mute = mute;
			}
		}
	}

	public virtual void SetClip(AudioClip clip, int slotIndex = 0)
	{
		if (slotIndex >= _audioSources.Length)
		{
			Debug.LogError((object)(((Object)this).name + ": Slot index exceeds the number of AudioSources."));
			return;
		}
		if (_audioSources[slotIndex].isPlaying)
		{
			_audioSources[slotIndex].time = 0f;
		}
		_audioSources[slotIndex].clip = clip;
		_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)clip.frequency));
	}

	public AudioSource UnitySource(int index)
	{
		if (index >= 0 && index < _audioSources.Length)
		{
			return _audioSources[index];
		}
		return null;
	}

	public virtual void DrawGizmos()
	{
	}

	protected GameObject _addAudioSourceChild(string name, GameObject template = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		GameObject val;
		if ((Object)(object)template != (Object)null)
		{
			val = Object.Instantiate<GameObject>(template);
			((Object)val).name = name;
		}
		else
		{
			val = new GameObject(name);
		}
		val.transform.parent = ((Component)this).transform;
		val.transform.position = ((Component)this).transform.position;
		val.transform.rotation = ((Component)this).transform.rotation;
		return val;
	}
}
