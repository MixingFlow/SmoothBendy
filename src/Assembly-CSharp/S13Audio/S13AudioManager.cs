using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace S13Audio;

public class S13AudioManager : MonoBehaviour
{
	private static S13AudioManager _instance;

	public S13SoundBank masterSoundBank;

	public S13AudioEvents audioEvents;

	public bool debugMode;

	public AudioMixer[] mixerAssets;

	public bool debugScreenMode;

	private Dictionary<string, S13SoundBank> _loadedSoundBanks = new Dictionary<string, S13SoundBank>();

	private Dictionary<string, AudioMixer> _mixers = new Dictionary<string, AudioMixer>();

	private List<string> _modifiedMixerProperties = new List<string>();

	private List<string> m_keysList = new List<string>();

	public static S13AudioManager Instance => _instance;

	private void Awake()
	{
		if ((Object)(object)_instance != (Object)null)
		{
			Debug.LogWarning((object)(((Object)this).name + ": Another singleton instance found in scene. Destroying..."));
			Object.Destroy((Object)(object)((Component)this).gameObject);
			Object.Destroy((Object)(object)this);
			((Behaviour)this).enabled = false;
			((Component)this).gameObject.SetActive(false);
			return;
		}
		_instance = this;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		LoadSoundBanksInScene();
		audioEvents = ((Component)this).GetComponent<S13AudioEvents>();
		if ((Object)(object)audioEvents == (Object)null)
		{
			Debug.LogWarning((object)(((Object)this).name + ": Reference to AudioEvents not found."));
		}
		if (mixerAssets.Length < 1)
		{
			Debug.LogWarning((object)"S13AudioManager missing explicit references to mixer assets, resource load methods will increase load times significantly");
			mixerAssets = Resources.LoadAll(string.Empty, typeof(AudioMixer)) as AudioMixer[];
		}
		string text = string.Empty;
		AudioMixer[] array = mixerAssets;
		foreach (AudioMixer val in array)
		{
			if ((Object)(object)val != (Object)null)
			{
				_mixers.Add(((Object)val).name, val);
				text = text + ((Object)val).name + ", ";
			}
		}
		Debug.Log((object)(((Object)this).name + ": Loaded " + _mixers.Count + " mixers: " + text));
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	private void Start()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (debugMode && debugScreenMode)
		{
			GameObject val = new GameObject("debug");
			val.AddComponent<S13AudioManagerDebug>();
			val.transform.parent = ((Component)this).transform;
		}
	}

	public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		LoadSoundBanksInScene();
	}

	public void SetSoundBank(string bankName)
	{
	}

	public void LoadSoundBank(string bankName)
	{
		if (!_loadedSoundBanks.ContainsKey(bankName))
		{
			GameObject val = Resources.Load<GameObject>(bankName);
			if ((Object)(object)val != (Object)null)
			{
				GameObject val2 = Object.Instantiate<GameObject>(val);
				((Object)val2).name = bankName;
				_loadedSoundBanks.Add(bankName, val2.GetComponent<S13SoundBank>());
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": Error loading sound bank '" + bankName + "'. Could not locate in Resources."));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Cannot load sound bank '" + bankName + "' because it is already loaded."));
		}
	}

	public void UnloadSoundBank(string bankName)
	{
		if (_loadedSoundBanks.ContainsKey(bankName))
		{
			GameObject val = GameObject.Find(bankName);
			if ((Object)(object)val != (Object)null)
			{
				_loadedSoundBanks.Remove(bankName);
				Object.Destroy((Object)(object)val);
				Resources.UnloadUnusedAssets();
			}
			else
			{
				_loadedSoundBanks.Remove(bankName);
				Debug.Log((object)(((Object)this).name + ": Removed sound bank '" + bankName + "' from loaded sound banks, but its GameObject has already been destroyed."));
			}
		}
		else
		{
			Debug.LogWarning((object)(((Object)this).name + ": Cannot unload sound bank '" + bankName + "' because it is not loaded."));
		}
	}

	public void UnloadAllSoundBanks()
	{
		if (_loadedSoundBanks.Count < 1)
		{
			Debug.Log((object)"UnloadAllSoundBanks() cancelled. There are no sound banks to unload", (Object)(object)((Component)this).gameObject);
			return;
		}
		List<string> list = new List<string>(_loadedSoundBanks.Keys);
		foreach (string item in list)
		{
			if (_loadedSoundBanks.ContainsKey(item))
			{
				UnloadSoundBank(item);
			}
		}
	}

	public void PruneIDFromSoundBank(string soundIdToRemove)
	{
		foreach (KeyValuePair<string, S13SoundBank> loadedSoundBank in _loadedSoundBanks)
		{
			m_keysList.Clear();
			m_keysList.AddRange(loadedSoundBank.Value.SoundIDs);
			foreach (string keys in m_keysList)
			{
				if (keys == soundIdToRemove)
				{
					loadedSoundBank.Value.RemoveSoundFromBank(soundIdToRemove);
				}
			}
		}
	}

	public void PlayAudio(string soundId)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null)
		{
			audioInSoundBanks.Play();
		}
	}

	public void PlayAudio(string soundId, float duration)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null)
		{
			audioInSoundBanks.Play(duration);
		}
	}

	public void PlayAudioDelayed(string soundId, float delayTime)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null)
		{
			audioInSoundBanks.PlayDelayed(delayTime);
		}
	}

	public void StopAudio(string soundId, bool ignoreFade = false)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null)
		{
			audioInSoundBanks.Stop(ignoreFade);
		}
	}

	public void StopAudioDelayed(string soundId, float delayTime, bool ignoreFade = false)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null)
		{
			audioInSoundBanks.StopDelayed(delayTime, ignoreFade);
		}
	}

	public void StopAllAudio(bool ignoreFade = false)
	{
		foreach (string soundID in masterSoundBank.SoundIDs)
		{
			try
			{
				masterSoundBank[soundID].Stop(ignoreFade);
			}
			catch (Exception ex)
			{
				Debug.Log((object)("Trying to stop " + soundID + " threw an error: " + ex.ToString()));
			}
		}
		foreach (KeyValuePair<string, S13SoundBank> loadedSoundBank in _loadedSoundBanks)
		{
			foreach (string soundID2 in loadedSoundBank.Value.SoundIDs)
			{
				try
				{
					loadedSoundBank.Value[soundID2].Stop(ignoreFade);
				}
				catch (Exception ex2)
				{
					Debug.Log((object)("Trying to stop " + soundID2 + " threw an error: " + ex2.ToString()));
				}
			}
		}
	}

	public void PauseAudio(string soundId, bool ignoreFade = false)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null && audioInSoundBanks.gamePauseEnabled)
		{
			audioInSoundBanks.Pause(ignoreFade);
		}
	}

	public void ResumeAudio(string soundId)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		if ((Object)(object)audioInSoundBanks != (Object)null && audioInSoundBanks.gamePauseEnabled)
		{
			audioInSoundBanks.Resume();
		}
	}

	public bool IsAudioPlaying(string soundId)
	{
		S13AudioSource audioInSoundBanks = GetAudioInSoundBanks(soundId);
		return audioInSoundBanks.IsPlaying;
	}

	public void InvokeEvent(string eventName, float delayTime = 0f)
	{
		if ((Object)(object)audioEvents != (Object)null)
		{
			((MonoBehaviour)audioEvents).Invoke(eventName, delayTime);
			if (debugMode)
			{
				Debug.Log((object)(((Object)this).name + ": InvokeEvent '" + eventName + "' with delay " + delayTime));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": No reference to AudioEvents; could not invoke event '" + eventName + "'."));
		}
	}

	public void SetMutingForAudioGroup(bool muting, S13AudioGroup group)
	{
		foreach (string soundID in masterSoundBank.SoundIDs)
		{
			S13AudioSource s13AudioSource = masterSoundBank[soundID];
			if (s13AudioSource.group == group)
			{
				s13AudioSource.SetMuting(muting);
			}
		}
		foreach (KeyValuePair<string, S13SoundBank> loadedSoundBank in _loadedSoundBanks)
		{
			foreach (string soundID2 in loadedSoundBank.Value.SoundIDs)
			{
				S13AudioSource s13AudioSource2 = loadedSoundBank.Value[soundID2];
				if (s13AudioSource2.group == group)
				{
					s13AudioSource2.SetMuting(muting);
				}
			}
		}
	}

	public void ToSnapshot(string mixerName, string snapshotName, float time)
	{
		AudioMixer val = _mixers[mixerName];
		if ((Object)(object)val != (Object)null)
		{
			AudioMixerSnapshot val2 = val.FindSnapshot(snapshotName);
			if ((Object)(object)val2 != (Object)null)
			{
				if (time > 0f && Time.timeScale == 0f)
				{
					Debug.LogWarning((object)(((Object)this).name + ": Transition time to snapshot " + snapshotName + " is > 0 with a time scale of 0."));
				}
				val2.TransitionTo(time);
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": Cannot transition to snapshot. No snapshot named '" + snapshotName + "' in master mixer."));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Error transitioning to snapshot '" + snapshotName + "' because the mixer '" + mixerName + "' was not found."));
		}
	}

	public void BlendSnapshots(string mixerName, string[] snapshotNames, float[] weights, float time)
	{
		if (snapshotNames.Length != weights.Length)
		{
			Debug.LogError((object)(((Object)this).name + ": Each snapshot in blending must have a corresponding weight value. Length of arrays not equal."));
			return;
		}
		AudioMixer val = _mixers[mixerName];
		if ((Object)(object)val != (Object)null)
		{
			AudioMixerSnapshot[] array = (AudioMixerSnapshot[])(object)new AudioMixerSnapshot[snapshotNames.Length];
			int num = 0;
			foreach (string text in snapshotNames)
			{
				AudioMixerSnapshot val2 = val.FindSnapshot(text);
				if ((Object)(object)val2 != (Object)null)
				{
					array[num++] = val2;
					continue;
				}
				Debug.LogError((object)(text + ": Snapshot named '" + text + "' not found in mixer '" + mixerName + "'."));
			}
			val.TransitionToSnapshots(array, weights, time);
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Mixer '" + mixerName + "' was not found."));
		}
	}

	public void SetMixerProperty(string mixerName, string propName, float value)
	{
		AudioMixer val = _mixers[mixerName];
		if ((Object)(object)val != (Object)null)
		{
			if (val.SetFloat(propName, value))
			{
				if (!_modifiedMixerProperties.Contains(propName))
				{
					_modifiedMixerProperties.Add(propName);
				}
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": Failed to set mixer property '" + propName + "' because it is either not exposed, or snapshots are editing."));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Error setting mixer property '" + propName + "' because the mixer '" + mixerName + "' was not found."));
		}
	}

	public void LerpMixerProperty(string mixerName, string propName, float targetValue, float time, bool ignoreTimeScale = false)
	{
		AudioMixer mixer = _mixers[mixerName];
		if ((Object)(object)mixer != (Object)null)
		{
			float fromValue = default(float);
			if (mixer.GetFloat(propName, ref fromValue))
			{
				((MonoBehaviour)this).StartCoroutine(LerpMixerValue(mixer, propName, fromValue, targetValue, time, ignoreTimeScale, delegate
				{
					mixer.SetFloat(propName, targetValue);
				}));
				if (!_modifiedMixerProperties.Contains(propName))
				{
					_modifiedMixerProperties.Add(propName);
				}
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": Failed to lerp mixer property '" + propName + "' because it is either not exposed, or snapshots are editing."));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Error lerping mixer property '" + propName + "' because the mixer '" + mixerName + "' was not found."));
		}
	}

	public void ClearMixerProperty(string mixerName, string propName)
	{
		AudioMixer val = _mixers[mixerName];
		if ((Object)(object)val != (Object)null)
		{
			if (val.ClearFloat(propName))
			{
				if (_modifiedMixerProperties.Contains(propName))
				{
					_modifiedMixerProperties.Remove(propName);
				}
			}
			else
			{
				Debug.LogError((object)(((Object)this).name + ": Failed to clear mixer property '" + propName + "' because it is either not exposed, or snapshots are editing."));
			}
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Error clearing mixer property '" + propName + "' because the mixer '" + mixerName + "' was not found."));
		}
	}

	public void ClearAllMixerProperties(string mixerName)
	{
		AudioMixer val = _mixers[mixerName];
		if ((Object)(object)val != (Object)null)
		{
			foreach (string modifiedMixerProperty in _modifiedMixerProperties)
			{
				if (!val.ClearFloat(modifiedMixerProperty))
				{
					Debug.LogError((object)(((Object)this).name + ": Failed to clear mixer property '" + modifiedMixerProperty + "' because it is either not exposed, or snapshots are editing."));
				}
			}
			_modifiedMixerProperties.Clear();
		}
		else
		{
			Debug.LogError((object)(((Object)this).name + ": Error clearing all mixer properties because the mixer '" + mixerName + "' was not found."));
		}
	}

	public static void VOPlay(AudioClip clip, S13AudioSource.AudioEndedHandler handler = null)
	{
	}

	public static void VOStop()
	{
	}

	public static void VOStopAll()
	{
	}

	private void LoadSoundBanksInScene()
	{
		S13SoundBank[] array = Object.FindObjectsOfType<S13SoundBank>();
		int num = 0;
		S13SoundBank[] array2 = array;
		foreach (S13SoundBank s13SoundBank in array2)
		{
			if ((Object)(object)s13SoundBank != (Object)(object)masterSoundBank)
			{
				if (_loadedSoundBanks.ContainsKey(((Object)((Component)s13SoundBank).gameObject).name))
				{
					_loadedSoundBanks[((Object)((Component)s13SoundBank).gameObject).name] = s13SoundBank;
				}
				else
				{
					_loadedSoundBanks.Add(((Object)((Component)s13SoundBank).gameObject).name, s13SoundBank);
				}
				num++;
				Debug.Log((object)(((Object)this).name + ": Loaded sound bank '" + ((Object)((Component)s13SoundBank).gameObject).name + "'."));
			}
		}
		Debug.Log((object)(((Object)this).name + ": Loaded " + num + " sound banks. Total sound banks loaded: " + _loadedSoundBanks.Count));
	}

	private S13AudioSource GetAudioInSoundBanks(string soundId)
	{
		S13AudioSource s13AudioSource = null;
		foreach (KeyValuePair<string, S13SoundBank> loadedSoundBank in _loadedSoundBanks)
		{
			s13AudioSource = loadedSoundBank.Value[soundId];
			if (Object.op_Implicit((Object)(object)s13AudioSource))
			{
				break;
			}
		}
		if ((Object)(object)s13AudioSource == (Object)null)
		{
			s13AudioSource = masterSoundBank[soundId];
		}
		if ((Object)(object)s13AudioSource == (Object)null)
		{
			Debug.LogError((object)(((Object)this).name + ": Could not find audio with name '" + soundId + "' in either the current sound bank or the master sound bank."), (Object)(object)((Component)this).gameObject);
		}
		return s13AudioSource;
	}

	private IEnumerator LerpMixerValue(AudioMixer mixer, string propName, float fromValue, float toValue, float time, bool ignoreTimeScale, S13AudioUtil.FadeInOutDelegate completionHandler = null)
	{
		float timeElapsed = 0f;
		float lastTime = Time.realtimeSinceStartup;
		while (timeElapsed < time)
		{
			float t = timeElapsed / time;
			mixer.SetFloat(propName, Mathf.Lerp(fromValue, toValue, t));
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
		completionHandler?.Invoke();
	}

	private static void OnVOAudioComplete(S13AudioSource audioSource)
	{
	}
}
