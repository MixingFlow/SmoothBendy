using System;
using DG.Tweening;
using UnityEngine;

public class InteractableMusicalInstrument : Interactable
{
	[Header("Instrument")]
	[SerializeField]
	private Transform m_Instrument;

	[SerializeField]
	private InstrumentType m_InstrumentType;

	private AudioClip[] m_InstrumentNotes;

	public InstrumentType InstrumentType => m_InstrumentType;

	public event EventHandler OnNotePlayed;

	public override void Init()
	{
		base.Init();
		if (m_InstrumentType == InstrumentType.BANJO)
		{
			m_InstrumentNotes = GetAudioClips("Audio/SFX/Instruments/Banjo/");
		}
		else if (m_InstrumentType == InstrumentType.PIANO)
		{
			m_InstrumentNotes = GetAudioClips("Audio/SFX/Instruments/Piano/");
		}
		else if (m_InstrumentType == InstrumentType.BASS_FIDDLE)
		{
			m_InstrumentNotes = GetAudioClips("Audio/SFX/Instruments/BassFiddle/");
		}
		else if (m_InstrumentType == InstrumentType.VIOLIN)
		{
			m_InstrumentNotes = GetAudioClips("Audio/SFX/Instruments/Violin/");
		}
		else if (m_InstrumentType == InstrumentType.DRUM)
		{
			m_InstrumentNotes = GetAudioClips("Audio/SFX/Instruments/Drum/");
		}
	}

	public override void OnInteract()
	{
		PlayMusicalNote();
		this.OnNotePlayed.Send(this);
	}

	private void PlayMusicalNote()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		m_Active = false;
		ShortcutExtensions.DOKill((Component)(object)m_Instrument, false);
		m_Instrument.localScale = Vector3.one;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(ShortcutExtensions.DOScale(m_Instrument, 1.025f, 0.05f), 2, (LoopType)1), (Ease)7), new TweenCallback(HandleInstrumentAudioObjectOnComplete));
		if (m_InstrumentNotes != null && m_InstrumentNotes.Length > 0)
		{
			int num = Random.Range(0, m_InstrumentNotes.Length);
			AudioClip val = m_InstrumentNotes[num];
			m_InstrumentNotes[num] = m_InstrumentNotes[0];
			m_InstrumentNotes[0] = val;
			GameManager.Instance.AudioManager.PlayAtPosition(val, base.transform.position);
		}
	}

	private void HandleInstrumentAudioObjectOnComplete()
	{
		m_Active = true;
	}

	private AudioClip[] GetAudioClips(string assetKey)
	{
		return GameManager.Instance.AssetManager.GetAssets<AudioClip>(assetKey);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Instrument, false);
		m_InstrumentNotes = null;
		base.OnDisposed();
	}
}
