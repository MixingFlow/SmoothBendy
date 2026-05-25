using System;
using DG.Tweening;
using TMG.Controls;
using UnityEngine;

public class IntroController : BaseController
{
	[SerializeField]
	private Transform m_Camera;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_StartLocation;

	[SerializeField]
	private Transform m_LogoLocation;

	[SerializeField]
	private Transform m_EndLocation;

	[SerializeField]
	private CameraMovements m_CameraMovement;

	private bool m_IsSkipped;

	private AudioObject m_AudioObject;

	private Sequence m_Sequence;

	public event EventHandler OnCompleteEvent;

	public override void InitOnComplete()
	{
		SequenceOnComplete();
	}

	private void Update()
	{
		if (!m_IsSkipped && PlayerInput.Any())
		{
			m_IsSkipped = true;
			if (m_Sequence != null)
			{
				TweenExtensions.Kill((Tween)(object)m_Sequence, false);
				m_Sequence = null;
			}
			if ((Object)(object)m_AudioObject != (Object)null)
			{
				m_AudioObject.Stop();
			}
			GameManager.Instance.ShowScreenBlocker(0f);
			SequenceOnComplete();
		}
	}

	private void FixedUpdate()
	{
		m_CameraMovement.Sway(m_Camera);
	}

	private Sequence DOSequence()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			m_AudioObject = GameManager.Instance.AudioManager.Play("Audio/MUS/MUS_Logo_Intro");
		});
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.HideScreenBlocker(1f);
		});
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Camera, m_LogoLocation.position, 2.75f, false), (Ease)3));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_Camera, m_LogoLocation.eulerAngles, 2.75f, (RotateMode)0), (Ease)3));
		num += 4f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Camera, m_LogoLocation.position + -m_LogoLocation.forward * 5f, 3f, false), (Ease)2));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowScreenBlocker(2f);
		});
		return val;
	}

	private void SequenceOnComplete()
	{
		int currentAudioType = GameManager.Instance.PlayerSettings.currentAudioType;
		if (currentAudioType == 0)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)1;
		}
		if (currentAudioType == 1)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)2;
		}
		if (currentAudioType == 2)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)3;
		}
		if (currentAudioType == 3)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)4;
		}
		if (currentAudioType == 4)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)5;
		}
		if (currentAudioType == 5)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)6;
		}
		this.OnCompleteEvent.Send(this);
	}

	private void InitializeAudio()
	{
		float volume = GameManager.Instance.PlayerSettings.Volume;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Master", LinearToDecibel(volume));
		float musicVolume = GameManager.Instance.PlayerSettings.MusicVolume;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Music", LinearToDecibel(musicVolume));
		float sFXVolume = GameManager.Instance.PlayerSettings.SFXVolume;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Effects", LinearToDecibel(sFXVolume));
		float dialogueVolume = GameManager.Instance.PlayerSettings.DialogueVolume;
		GameManager.Instance.AudioManager.AudioMixer.SetFloat("Dialogue", LinearToDecibel(dialogueVolume));
		int currentAudioType = GameManager.Instance.PlayerSettings.currentAudioType;
		if (currentAudioType == 0)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)1;
		}
		if (currentAudioType == 1)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)2;
		}
		if (currentAudioType == 2)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)3;
		}
		if (currentAudioType == 3)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)4;
		}
		if (currentAudioType == 4)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)5;
		}
		if (currentAudioType == 5)
		{
			AudioSettings.speakerMode = (AudioSpeakerMode)6;
		}
	}

	private float LinearToDecibel(float linearVal)
	{
		return (linearVal == 0f) ? (-80f) : (20f * Mathf.Log10(linearVal));
	}

	protected override void OnDisposed()
	{
		this.OnCompleteEvent = null;
		m_AudioObject = null;
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
		base.OnDisposed();
	}
}
