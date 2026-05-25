using System;
using System.Collections.Generic;
using System.Linq;
using TMG.Core;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : TMGMonoBehaviour
{
	private const string MASTER_MIXER_GROUP = "Master";

	private const string SFX_MIXER_GROUP = "SFX";

	private const string MUSIC_MIXER_GROUP = "Music";

	private const string DIALOGUE_MIXER_GROUP = "Dialogue";

	private List<AudioObject> m_AudioObjectPool = new List<AudioObject>();

	private List<AudioObject> m_UsedAudioObjects = new List<AudioObject>();

	private List<AudioObject> m_AudioObject2DPool = new List<AudioObject>();

	private List<AudioObject> m_UsedAudioObjects2D = new List<AudioObject>();

	private List<AudioObject> m_SoundEffectsQueue = new List<AudioObject>();

	private List<AudioObject> m_DialogueQueue = new List<AudioObject>();

	private AudioObject m_ActiveDialogue;

	private float m_DialogueTimer;

	private float m_DialogueLength;

	private float m_SoundEffectTimer;

	private float m_SoundEffectLength;

	private List<AudioSource> m_PausedAudioSources;

	public Transform AudioManagerParent { get; private set; }

	public Transform PooledAudioObjectsParent { get; private set; }

	public Transform Ambience { get; private set; }

	public Transform SoundEffects { get; private set; }

	public Transform Music { get; private set; }

	public Transform Dialogue { get; private set; }

	public AudioListener AudioListener { get; private set; }

	public AudioMixer AudioMixer { get; private set; }

	public static AudioManager Create()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		AudioManager audioManager = new GameObject("[Audio Manager]").AddComponent<AudioManager>();
		audioManager.AudioManagerParent = audioManager.transform;
		Object.DontDestroyOnLoad((Object)(object)audioManager.gameObject);
		audioManager.AudioListener = audioManager.gameObject.AddComponent<AudioListener>();
		audioManager.AudioMixer = GameManager.Instance.AssetManager.GetAsset<AudioMixer>("GamePlay/Audio/TMGAudioMixer");
		audioManager.PooledAudioObjectsParent = new GameObject("[Pooled Audio Objects]").transform;
		audioManager.PooledAudioObjectsParent.SetParent(audioManager.AudioManagerParent);
		audioManager.Ambience = new GameObject("[Ambience]").transform;
		audioManager.Ambience.SetParent(audioManager.AudioManagerParent);
		audioManager.SoundEffects = new GameObject("[Sound Effects]").transform;
		audioManager.SoundEffects.SetParent(audioManager.AudioManagerParent);
		audioManager.Music = new GameObject("[Music]").transform;
		audioManager.Music.SetParent(audioManager.AudioManagerParent);
		audioManager.Dialogue = new GameObject("[Dialogue]").transform;
		audioManager.Dialogue.SetParent(audioManager.AudioManagerParent);
		return audioManager;
	}

	private void FixedUpdate()
	{
		CheckDialogueQueue();
		CheckSoundEffectQueue();
		CheckForPooling();
	}

	public void ListenerSetActive(bool active)
	{
		((Behaviour)AudioListener).enabled = active;
	}

	private void CheckForPooling()
	{
		if (m_UsedAudioObjects.Count > 0)
		{
			for (int i = 0; i < m_UsedAudioObjects.Count; i++)
			{
				SendToPool(m_UsedAudioObjects[i]);
			}
			m_UsedAudioObjects.Clear();
		}
		if (m_UsedAudioObjects2D.Count > 0)
		{
			for (int j = 0; j < m_UsedAudioObjects2D.Count; j++)
			{
				SendToPool2D(m_UsedAudioObjects2D[j]);
			}
			m_UsedAudioObjects2D.Clear();
		}
	}

	private void CheckDialogueQueue()
	{
		if (m_DialogueQueue.Count > 0)
		{
			m_DialogueTimer += Time.deltaTime;
			if (m_DialogueTimer >= m_DialogueLength)
			{
				m_DialogueLength = 0f;
				m_DialogueTimer = 0f;
				m_ActiveDialogue = m_DialogueQueue[0];
				m_DialogueLength = m_ActiveDialogue.AudioClip.length - Time.fixedDeltaTime;
				m_DialogueQueue.RemoveAt(0);
				m_ActiveDialogue.IsQueued = false;
				m_ActiveDialogue.Play();
				m_ActiveDialogue.OnComplete += HandleActiveDialogueOnComplete;
			}
		}
	}

	private void HandleActiveDialogueOnComplete(object sender, EventArgs e)
	{
		AudioObject audioObject = (AudioObject)sender;
		if (!((Object)(object)audioObject == (Object)null))
		{
			audioObject.OnComplete -= HandleActiveDialogueOnComplete;
			if (m_DialogueQueue.Count <= 0)
			{
				m_ActiveDialogue = null;
				m_DialogueLength = 0f;
			}
		}
	}

	private void CheckSoundEffectQueue()
	{
		if (m_SoundEffectsQueue.Count > 0)
		{
			m_SoundEffectTimer += Time.deltaTime;
			if (m_SoundEffectTimer >= m_SoundEffectLength)
			{
				m_SoundEffectLength = 0f;
				m_SoundEffectTimer = 0f;
				AudioObject audioObject = m_SoundEffectsQueue[0];
				m_SoundEffectLength = ((m_SoundEffectsQueue.Count > 0) ? audioObject.AudioClip.length : 0f);
				m_SoundEffectsQueue.RemoveAt(0);
				audioObject.IsQueued = false;
				audioObject.Play();
			}
		}
	}

	public AudioObject PlayAtPosition(string _clipKey, Vector3 position, AudioObjectType audioType = AudioObjectType.SOUND_EFFECT, int _loops = 0, bool isQueued = false, Transform parent = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		AudioClip asset = GameManager.Instance.AssetManager.GetAsset<AudioClip>(_clipKey);
		return PlayAtPosition(asset, position, audioType, _loops, isQueued, parent);
	}

	public AudioObject PlayAtPosition(AudioClip _clip, Vector3 position, AudioObjectType audioType = AudioObjectType.SOUND_EFFECT, int _loops = 0, bool isQueued = false, Transform parent = null)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		AudioObject fromPool = GetFromPool();
		if (!Object.op_Implicit((Object)(object)fromPool.AudioSource))
		{
			fromPool.Dispose();
			return PlayAtPosition(_clip, position, audioType, _loops, isQueued, parent);
		}
		fromPool.AudioClip = _clip;
		fromPool.Loops = _loops;
		fromPool.AudioSource.loop = _loops == -1;
		fromPool.WorldPosition = position;
		fromPool.IsQueued = isQueued;
		fromPool.Parent = parent;
		fromPool.OnComplete += HandleAudioOnComplete;
		if ((Object)(object)parent == (Object)null)
		{
			SetEditorParent(fromPool, audioType);
		}
		CheckMixerGroup(fromPool, audioType);
		if (isQueued)
		{
			switch (audioType)
			{
			case AudioObjectType.SOUND_EFFECT:
				m_SoundEffectsQueue.Add(fromPool);
				break;
			case AudioObjectType.DIALOGUE:
				m_DialogueQueue.Add(fromPool);
				break;
			}
		}
		else
		{
			fromPool.Play();
		}
		return fromPool;
	}

	public AudioObject Play(string _clipKey, AudioObjectType audioType = AudioObjectType.SOUND_EFFECT, int _loops = 0, bool isQueued = false)
	{
		AudioClip asset = GameManager.Instance.AssetManager.GetAsset<AudioClip>(_clipKey);
		return Play(asset, audioType, _loops, isQueued);
	}

	public AudioObject Play(AudioClip _clip, AudioObjectType audioType = AudioObjectType.SOUND_EFFECT, int _loops = 0, bool isQueued = false)
	{
		AudioObject fromPool2D = GetFromPool2D();
		fromPool2D.AudioClip = _clip;
		fromPool2D.Loops = _loops;
		fromPool2D.AudioSource.loop = _loops == -1;
		fromPool2D.IsQueued = isQueued;
		if (audioType == AudioObjectType.SOUND_EFFECT)
		{
			fromPool2D.AudioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("SFX")[0];
		}
		fromPool2D.OnComplete += HandleAudioOnComplete;
		SetEditorParent(fromPool2D, audioType);
		CheckMixerGroup(fromPool2D, audioType);
		if (isQueued)
		{
			switch (audioType)
			{
			case AudioObjectType.SOUND_EFFECT:
				m_SoundEffectsQueue.Add(fromPool2D);
				break;
			case AudioObjectType.DIALOGUE:
				m_DialogueQueue.Add(fromPool2D);
				break;
			}
		}
		else
		{
			fromPool2D.Play();
		}
		return fromPool2D;
	}

	private void CheckMixerGroup(AudioObject audioObject, AudioObjectType audioType)
	{
		switch (audioType)
		{
		case AudioObjectType.SOUND_EFFECT:
			audioObject.AudioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("SFX")[0];
			break;
		case AudioObjectType.DIALOGUE:
			audioObject.AudioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Dialogue")[0];
			break;
		case AudioObjectType.MUSIC:
			audioObject.AudioSource.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Music")[0];
			break;
		}
	}

	public void PauseAll()
	{
		AudioSource[] array = Resources.FindObjectsOfTypeAll<AudioSource>();
		m_PausedAudioSources = new List<AudioSource>();
		AudioSource[] array2 = array;
		foreach (AudioSource val in array2)
		{
			if (val.isPlaying)
			{
				val.Pause();
				m_PausedAudioSources.Add(val);
			}
		}
	}

	public void ResumeAll()
	{
		if (m_PausedAudioSources != null)
		{
			for (int i = 0; i < m_PausedAudioSources.Count; i++)
			{
				m_PausedAudioSources[i].UnPause();
			}
			m_PausedAudioSources.Clear();
		}
		m_PausedAudioSources = null;
	}

	private void SetEditorParent(AudioObject audioObject, AudioObjectType audioType)
	{
	}

	private AudioObject GetFromPool()
	{
		if (m_AudioObjectPool.Count <= 0)
		{
			m_AudioObjectPool.Add(CreateAudioObject3D());
		}
		AudioObject audioObject = m_AudioObjectPool[0];
		m_AudioObjectPool.Remove(audioObject);
		return audioObject;
	}

	private AudioObject GetFromPool2D()
	{
		if (m_AudioObject2DPool.Count <= 0)
		{
			m_AudioObject2DPool.Add(CreateAudioObject2D());
		}
		AudioObject audioObject = m_AudioObject2DPool[0];
		m_AudioObject2DPool.Remove(audioObject);
		return audioObject;
	}

	private void SendToPool(AudioObject audioObject)
	{
		if ((Object)(object)audioObject != (Object)null)
		{
			audioObject.Clear();
			m_AudioObjectPool.Add(audioObject);
		}
	}

	private void SendToPool2D(AudioObject audioObject)
	{
		if ((Object)(object)audioObject != (Object)null)
		{
			audioObject.Clear();
			m_AudioObject2DPool.Add(audioObject);
		}
	}

	private void HandleAudioOnComplete(object sender, EventArgs e)
	{
		AudioObject audioObject = (AudioObject)sender;
		audioObject.OnComplete -= HandleAudioOnComplete;
		if (!base.IsDisposed)
		{
			if (m_UsedAudioObjects != null && !audioObject.Is2D)
			{
				m_UsedAudioObjects.Add(audioObject);
			}
			else if (m_UsedAudioObjects2D != null && audioObject.Is2D)
			{
				m_UsedAudioObjects2D.Add(audioObject);
			}
		}
	}

	private AudioObject CreateAudioObject2D()
	{
		return AudioObject.Create("GamePlay/Audio/AudioObject_2D", is2D: true);
	}

	private AudioObject CreateAudioObject3D()
	{
		return AudioObject.Create("GamePlay/Audio/AudioObject_3D");
	}

	public AudioClip Combine(List<AudioClip> clips)
	{
		int num = 48000;
		if (clips == null || clips.Count == 0)
		{
			return null;
		}
		int num2 = 0;
		for (int i = 0; i < clips.Count; i++)
		{
			if (!((Object)(object)clips[i] == (Object)null))
			{
				num2 += clips[i].samples;
				num = clips[i].frequency;
			}
		}
		float[] array = new float[num2];
		num2 = 0;
		for (int j = 0; j < clips.Count; j++)
		{
			if (!((Object)(object)clips[j] == (Object)null))
			{
				float[] array2 = new float[clips[j].samples];
				clips[j].GetData(array2, 0);
				array2.CopyTo(array, num2);
				num2 += array2.Length;
			}
		}
		if (num2 == 0)
		{
			return null;
		}
		AudioClip obj = AudioClip.Create("Combine", num2, 1, num, false);
		obj.SetData(array, 0);
		return obj;
	}

	public void ClearAll()
	{
		for (int i = 0; i < m_UsedAudioObjects.Count; i++)
		{
			SendToPool(m_UsedAudioObjects[i]);
		}
		m_UsedAudioObjects.Clear();
		for (int j = 0; j < m_SoundEffectsQueue.Count; j++)
		{
			SendToPool(m_SoundEffectsQueue[j]);
		}
		m_SoundEffectsQueue.Clear();
		for (int k = 0; k < m_DialogueQueue.Count; k++)
		{
			SendToPool(m_DialogueQueue[k]);
		}
		m_DialogueQueue.Clear();
	}

	public void ClearCurrentDialogueQueue()
	{
		m_DialogueLength = 0f;
		m_DialogueTimer = 0f;
		if ((Object)(object)m_ActiveDialogue != (Object)null)
		{
			m_ActiveDialogue.IsQueued = false;
			m_ActiveDialogue.Clear();
			m_ActiveDialogue = null;
		}
		for (int i = 0; i < m_DialogueQueue.Count; i++)
		{
			SendToPool(m_DialogueQueue[i]);
		}
		m_DialogueQueue.Clear();
	}

	private void DisposeAll()
	{
		if (m_AudioObjectPool != null)
		{
			for (int num = m_AudioObjectPool.Count - 1; num >= 0; num--)
			{
				m_AudioObjectPool[num].Dispose();
			}
			m_AudioObjectPool.Clear();
		}
		if (m_AudioObject2DPool != null)
		{
			for (int num2 = m_AudioObject2DPool.Count - 1; num2 >= 0; num2--)
			{
				m_AudioObject2DPool[num2].Dispose();
			}
			m_AudioObject2DPool.Clear();
		}
		if (m_UsedAudioObjects != null)
		{
			for (int num3 = m_UsedAudioObjects.Count - 1; num3 >= 0; num3--)
			{
				m_UsedAudioObjects[num3].Dispose();
			}
			m_UsedAudioObjects.Clear();
		}
		if (m_SoundEffectsQueue != null)
		{
			for (int num4 = m_SoundEffectsQueue.Count - 1; num4 >= 0; num4--)
			{
				m_SoundEffectsQueue[num4].Dispose();
			}
			m_SoundEffectsQueue.Clear();
		}
		if (m_DialogueQueue != null)
		{
			for (int num5 = m_DialogueQueue.Count - 1; num5 >= 0; num5--)
			{
				m_DialogueQueue[num5].Dispose();
			}
			m_DialogueQueue.Clear();
		}
		AudioObject[] array = Object.FindObjectsOfType<AudioObject>();
		for (int num6 = array.Length - 1; num6 >= 0; num6--)
		{
			array[num6].Dispose();
		}
	}

	protected override void OnDisposed()
	{
		DisposeAll();
		m_AudioObjectPool = null;
		m_AudioObject2DPool = null;
		m_UsedAudioObjects = null;
		m_SoundEffectsQueue = null;
		m_DialogueQueue = null;
		m_ActiveDialogue = null;
		base.OnDisposed();
	}

	public void RefreshAllAudioSources()
	{
		GameObject[] array = (from go in Object.FindObjectsOfType<GameObject>()
			where ((Object)go).name.Contains("AudioEventTriggers")
			select go).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			array[num].SetActive(false);
			array[num].SetActive(true);
			GameManager.Instance.AudioTypeChanged = false;
		}
	}
}
