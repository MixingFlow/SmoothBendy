using System;
using TMG.Core;
using UnityEngine;

public class AudioObject : TMGMonoBehaviour
{
	public const string DEFAULT_NAME = "[POOLED AUDIO OBJECT]";

	public AudioClip AudioClip;

	public int Loops;

	public bool IsQueued;

	private int m_TimesPlayed;

	private float m_PauseTime;

	private bool m_Stopped;

	public AudioSource AudioSource { get; private set; }

	public bool Is2D { get; private set; }

	public Vector3 WorldPosition
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return base.transform.position;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			base.transform.position = value;
		}
	}

	public Vector3 LocalPosition
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return base.transform.localPosition;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			base.transform.localPosition = value;
		}
	}

	public Transform Parent
	{
		get
		{
			return base.transform.parent;
		}
		set
		{
			base.transform.SetParent(value);
		}
	}

	private AudioManager m_AudioManager => GameManager.Instance.AudioManager;

	public event EventHandler OnComplete;

	public static AudioObject Create(string objectType, bool is2D = false)
	{
		AudioObject audioObject = GameManager.Instance.AssetManager.CreateAsset<AudioObject>(objectType);
		Object.DontDestroyOnLoad((Object)(object)audioObject.gameObject);
		audioObject.Is2D = is2D;
		audioObject.AudioSource = ((Component)audioObject).GetComponent<AudioSource>();
		audioObject.AudioSource.playOnAwake = false;
		audioObject.AudioSource.mute = false;
		return audioObject;
	}

	private void FixedUpdate()
	{
		if (!AudioSource.isPlaying && !IsQueued && !m_Stopped)
		{
			m_TimesPlayed++;
			if (Loops == -1 || (Loops > 0 && m_TimesPlayed < Loops))
			{
				m_Stopped = false;
				AudioSource.time = 0f;
				AudioSource.Play();
			}
			else
			{
				Stop();
			}
		}
	}

	public void Play()
	{
		m_Stopped = false;
		AudioSource.clip = AudioClip;
		m_TimesPlayed = 0;
		AudioSource.time = 0f;
		AudioSource.Play();
	}

	public void Pause()
	{
		m_PauseTime = AudioSource.time;
		AudioSource.Pause();
	}

	public void Resume()
	{
		AudioSource.time = m_PauseTime;
		AudioSource.Play();
	}

	public void Stop()
	{
		m_Stopped = true;
		if ((Object)(object)AudioSource != (Object)null)
		{
			AudioSource.Stop();
			this.OnComplete.Send(this);
		}
	}

	public void Clear()
	{
		this.OnComplete = null;
		if (Object.op_Implicit((Object)(object)AudioSource))
		{
			AudioSource.clip = null;
			AudioSource.volume = 1f;
		}
		AudioClip = null;
		Parent = null;
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		AudioClip = null;
		if ((Object)(object)AudioSource != (Object)null)
		{
			AudioSource.Stop();
			AudioSource.clip = null;
			AudioSource = null;
		}
		base.OnDisposed();
	}
}
