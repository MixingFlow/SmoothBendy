using System.Collections;
using UnityEngine;

namespace S13Audio;

public class S13ObjectComplex : S13AudioSource
{
	private delegate void FadeCompletionHandler();

	public AudioClip[] audioClips;

	[AudioField("Voice Max", 1, 50)]
	public int voices = 1;

	public VoiceMode voiceMode;

	public AudioSource rolloffTemplate;

	public AudioVelocityUpdateMode velocityUpdateMode;

	public bool usesExternalVolume;

	[AudioSlider("Spawn Time Min (s)", 0f, 30f)]
	public float delayMin;

	[AudioSlider("Spawn Time Max (s)", 0f, 30f)]
	public float delayMax = 1f;

	public bool enableRandomPitch = true;

	[AudioSlider("Random Pitch Min (semi)", -12, 12)]
	public int pitchMin;

	[AudioSlider("Random Pitch Max (semi)", -12, 12)]
	public int pitchMax;

	[AudioSlider("Random 3D Position (units)", 0f, 100f)]
	public float distanceVariance = 1f;

	[AudioSlider("Fade-in time (s)", 0f, 60f)]
	public float fadeInTime;

	[AudioSlider("Fade-out time (s)", 0f, 60f)]
	public float fadeOutTime;

	[AudioSlider("Replay Gate Time (ms)", 0, 1000)]
	public int retriggerTime;

	[AudioSlider("Chance To Play (%)", 0, 100)]
	public int chanceToPlay = 100;

	private bool _isPlaying;

	private bool _isFadingOut;

	private bool _isLooping;

	private bool _canPlay = true;

	private bool _waitForAudioEndLock;

	private bool[] _isVoicePaused;

	private S13VoicesImpl _voicesImpl;

	private S13ShuffleImpl _shuffleImpl;

	private static Color DISTANCE_VARIANCE_GIZMO_COLOR = new Color(50f / 51f, 0.99607843f, 0.49803922f, 0.62f);

	public override float Length => -1f;

	public override bool IsPlaying => base.IsPlaying || (_isPlaying && _isLooping) || _isFadingOut;

	private void Awake()
	{
		int num = voices / audioClips.Length + 1;
		_audioSources = (AudioSource[])(object)new AudioSource[audioClips.Length * num];
		_isVoicePaused = new bool[_audioSources.Length];
		int num2 = 0;
		AudioClip[] array = audioClips;
		foreach (AudioClip clip in array)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = num2 * num + j;
				if ((Object)(object)rolloffTemplate != (Object)null)
				{
					GameObject val = _addAudioSourceChild($"source{num2}-{j}", ((Component)rolloffTemplate).gameObject);
					_audioSources[num3] = val.GetComponent<AudioSource>();
					_audioSources[num3].outputAudioMixerGroup = rolloffTemplate.outputAudioMixerGroup;
				}
				else
				{
					GameObject val = _addAudioSourceChild($"source{num2}-{j}");
					_audioSources[num3] = val.AddComponent<AudioSource>();
				}
				_audioSources[num3].clip = clip;
				_audioSources[num3].loop = false;
				if ((Object)(object)outputOverride != (Object)null)
				{
					_audioSources[num3].outputAudioMixerGroup = outputOverride;
				}
				_isVoicePaused[num3] = false;
			}
			num2++;
		}
		UpdateParameters();
		if ((Object)(object)audioClips[0] != (Object)null)
		{
			_offsetImpl = new S13OffsetImpl((int)(offsetTime * (float)audioClips[0].frequency));
		}
		_voicesImpl = new S13VoicesImpl(voiceMode, _audioSources, voices);
		_shuffleImpl = new S13ShuffleImpl(new S13Range(0, audioClips.Length - 1));
	}

	private void LateUpdate()
	{
		_voicesImpl.Update();
	}

	public override void Play()
	{
		if (!_canPlay)
		{
			if (audioEndedHandler != null)
			{
				audioEndedHandler(this);
			}
			return;
		}
		_isPlaying = true;
		if (loop)
		{
			if (_isFadingOut)
			{
				((MonoBehaviour)this).StopAllCoroutines();
			}
			if (Object.op_Implicit((Object)(object)((Component)((Component)this).transform.parent).gameObject) && !((Component)((Component)this).transform.parent).gameObject.activeSelf)
			{
				((Component)((Component)this).transform.parent).gameObject.SetActive(true);
			}
			((MonoBehaviour)this).StartCoroutine(RandomLoop());
			if (fadeInTime > 0f)
			{
				((MonoBehaviour)this).StartCoroutine(FadeAudioSources(fadeInTime, 0f, S13AudioUtil.dB2Lin(volume), null));
			}
		}
		else
		{
			float num = (float)chanceToPlay / 100f;
			if (Random.value > num)
			{
				if (audioEndedHandler != null)
				{
					audioEndedHandler(this);
				}
				return;
			}
			int soundIndex = _shuffleImpl.Next();
			if (fadeInTime > 0f)
			{
				_voicesImpl.Play(soundIndex, delegate(int voiceIndex)
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
				_voicesImpl.Play(soundIndex, delegate(int voiceIndex)
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
					_offsetImpl.SetPlayPosition(_audioSources[voiceIndex], startRandom);
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
		}
		if (retriggerTime > 0)
		{
			((MonoBehaviour)this).StartCoroutine(TriggerWait());
		}
		if (audioEndedHandler != null && !_waitForAudioEndLock)
		{
			((MonoBehaviour)this).StartCoroutine(WaitForAudioEnd());
		}
		if (_isPaused)
		{
			_isPaused = false;
			for (int num2 = 0; num2 < _audioSources.Length; num2++)
			{
				_isVoicePaused[num2] = false;
			}
		}
	}

	public override void Stop(bool ignoreFade)
	{
		base.Stop(ignoreFade);
		_isPlaying = false;
		if (loop)
		{
			if (fadeOutTime > 0f && !ignoreFade)
			{
				_isFadingOut = true;
				((MonoBehaviour)this).StartCoroutine(FadeAudioSources(fadeOutTime, S13AudioUtil.dB2Lin(volume), 0f, delegate
				{
					_isFadingOut = false;
					AudioSource[] audioSources4 = _audioSources;
					foreach (AudioSource val3 in audioSources4)
					{
						val3.volume = S13AudioUtil.dB2Lin(volume);
					}
				}));
			}
			else
			{
				((MonoBehaviour)this).StopCoroutine(RandomLoop());
				_isLooping = false;
				if (ignoreFade)
				{
					AudioSource[] audioSources = _audioSources;
					foreach (AudioSource val in audioSources)
					{
						val.Stop();
					}
				}
			}
		}
		else if (fadeOutTime > 0f && !ignoreFade)
		{
			AudioSource[] audioSources2 = _audioSources;
			foreach (AudioSource source in audioSources2)
			{
				if (source.isPlaying)
				{
					((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeOut(source, fadeOutTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale, delegate
					{
						source.Stop();
						source.volume = S13AudioUtil.dB2Lin(volume);
					}));
				}
			}
		}
		else
		{
			AudioSource[] audioSources3 = _audioSources;
			foreach (AudioSource val2 in audioSources3)
			{
				val2.Stop();
			}
		}
		if (_isPaused)
		{
			_isPaused = false;
			for (int num4 = 0; num4 < _audioSources.Length; num4++)
			{
				_isVoicePaused[num4] = false;
			}
		}
	}

	public override void Pause(bool ignoreFade)
	{
		if (!_isPlaying)
		{
			return;
		}
		_isPaused = true;
		_waitForAudioEndLock = false;
		if (loop)
		{
			if (fadeOutTime > 0f && !ignoreFade)
			{
				_isFadingOut = true;
				((MonoBehaviour)this).StartCoroutine(FadeAudioSources(fadeOutTime, S13AudioUtil.dB2Lin(volume), 0f, delegate
				{
					_isFadingOut = false;
					AudioSource[] audioSources = _audioSources;
					foreach (AudioSource val2 in audioSources)
					{
						val2.volume = S13AudioUtil.dB2Lin(volume);
					}
				}));
			}
			else
			{
				if (!ignoreFade)
				{
					return;
				}
				for (int num = 0; num < _audioSources.Length; num++)
				{
					AudioSource val = _audioSources[num];
					if (val.isPlaying)
					{
						val.Pause();
						_isVoicePaused[num] = true;
					}
				}
			}
			return;
		}
		for (int num2 = 0; num2 < _audioSources.Length; num2++)
		{
			AudioSource source = _audioSources[num2];
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
			_isVoicePaused[num2] = true;
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
		if (loop)
		{
			if (_isFadingOut)
			{
				((MonoBehaviour)this).StopAllCoroutines();
				((MonoBehaviour)this).StartCoroutine(RandomLoop());
			}
			if (fadeInTime > 0f)
			{
				((MonoBehaviour)this).StartCoroutine(FadeAudioSources(fadeInTime, 0f, S13AudioUtil.dB2Lin(volume), null));
			}
		}
		for (int i = 0; i < _audioSources.Length; i++)
		{
			AudioSource val = _audioSources[i];
			if (_isVoicePaused[i])
			{
				val.UnPause();
				if (fadeInTime > 0f && !loop)
				{
					((MonoBehaviour)this).StartCoroutine(S13AudioUtil.FadeIn(val, fadeInTime, S13AudioUtil.dB2Lin(volume), ignoreTimeScale));
				}
				_isVoicePaused[i] = false;
			}
		}
		if (audioEndedHandler != null && !_waitForAudioEndLock)
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
		audioClips[slotIndex] = clip;
	}

	private IEnumerator RandomLoop()
	{
		float timer = 0f;
		float lastTime = Time.realtimeSinceStartup;
		float timeDelay = Random.Range(delayMin, delayMax);
		float chance = (float)chanceToPlay / 100f;
		_isLooping = true;
		Vector3 soundPos = Vector3.zero;
		while (_isPlaying || _isFadingOut)
		{
			if (timer >= timeDelay && !_isPaused)
			{
				timer = 0f;
				timeDelay = Random.Range(delayMin, delayMax);
				if (Random.value < chance)
				{
					soundPos.x = Random.Range(0f - distanceVariance, distanceVariance);
					soundPos.y = Random.Range(0f - distanceVariance, distanceVariance);
					soundPos.z = Random.Range(0f - distanceVariance, distanceVariance);
					int soundIndex = _shuffleImpl.Next();
					_voicesImpl.Play(soundIndex, delegate(int voiceIndex)
					{
						//IL_0018: Unknown result type (might be due to invalid IL or missing references)
						//IL_002d: Unknown result type (might be due to invalid IL or missing references)
						((Component)_audioSources[voiceIndex]).transform.localPosition = Vector3.ClampMagnitude(soundPos, distanceVariance);
						_audioSources[voiceIndex].pitch = S13AudioUtil.SemitoneToPitch(Random.Range(pitchMin, pitchMax));
						_audioSources[voiceIndex].Play();
					});
				}
			}
			if (ignoreTimeScale)
			{
				timer += Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
			}
			else
			{
				timer += Time.deltaTime;
			}
			yield return null;
		}
		_isLooping = false;
	}

	private IEnumerator FadeAudioSources(float fadeTime, float from, float to, FadeCompletionHandler handler)
	{
		float timeElapsed = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (timeElapsed < fadeTime)
		{
			float t = timeElapsed / fadeTime;
			AudioSource[] audioSources = _audioSources;
			foreach (AudioSource val in audioSources)
			{
				val.volume = Mathf.Lerp(from, to, t);
			}
			if (ignoreTimeScale)
			{
				timeElapsed += Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
			}
			else
			{
				timeElapsed += Time.deltaTime;
			}
			yield return null;
		}
		handler?.Invoke();
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
		_waitForAudioEndLock = true;
		bool allStopped;
		do
		{
			yield return ((MonoBehaviour)this).StartCoroutine(S13AudioUtil.RealTimeWaitForSeconds(S13AudioSource.WAIT_FOR_AUDIO_END_UPDATE_FREQUENCY));
			allStopped = true;
			AudioSource[] audioSources = _audioSources;
			foreach (AudioSource val in audioSources)
			{
				if (val.timeSamples > 0 && val.timeSamples < val.clip.samples)
				{
					allStopped = false;
					break;
				}
			}
		}
		while (!allStopped || _isLooping);
		_waitForAudioEndLock = false;
		if (audioEndedHandler != null)
		{
			audioEndedHandler(this);
		}
	}

	public override void DrawGizmos()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rolloffTemplate != (Object)null)
		{
			Gizmos.color = S13AudioSource.DEFAULT_GIZMO_COLOR;
			Gizmos.DrawWireSphere(((Component)this).transform.position, rolloffTemplate.maxDistance);
		}
		Gizmos.color = DISTANCE_VARIANCE_GIZMO_COLOR;
		Gizmos.DrawWireSphere(((Component)this).transform.position, distanceVariance);
	}
}
