using System;
using TMG.Core;
using UnityEngine;

public class CH1GeneralController : TMGMonoBehaviour
{
	[Header("Interactable")]
	[SerializeField]
	private Interactable m_ProjectorInteract;

	[SerializeField]
	private ProjectorController m_Projector;

	[Header("Audio")]
	[SerializeField]
	private EventTrigger m_HenryOldDesk;

	[SerializeField]
	private EventTrigger m_HenryArtRoom;

	[SerializeField]
	private EventTrigger m_BorisRoom;

	private AudioClip m_HenryClip08;

	private AudioClip m_HenryClip11;

	private AudioClip m_HenryClip12;

	private bool m_IsProjectorOn;

	public override void InitOnComplete()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_HenryClip08 = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_08");
		m_HenryClip11 = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_11");
		m_HenryClip12 = GameManager.Instance.AssetManager.GetAsset<AudioClip>("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_12");
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Inkwell.IsStarted)
		{
			m_HenryOldDesk.OnEnter += HandleHenryOldDeskOnEnter;
			m_HenryArtRoom.OnEnter += HandleHenryArtRoomOnEnter;
		}
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH1Data.Wrench.IsStarted)
		{
			m_BorisRoom.OnEnter += HandleBorisRoomOnEnter;
		}
		m_ProjectorInteract.OnInteracted += HandleProjectorOnInteract;
		m_Projector.FilmAudioObject = GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/SFX_Projector_Run_With_Film_01", m_Projector.AudioPosition.position, AudioObjectType.SOUND_EFFECT, -1);
		m_Projector.MusicAudioObject = GameManager.Instance.AudioManager.PlayAtPosition("Audio/MUS/CH1/MUS_Hellfire_Follies", m_Projector.MusicPosition.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, m_Projector.MusicPosition);
		m_Projector.InitTurnOn();
		m_IsProjectorOn = true;
	}

	private void HandleHenryOldDeskOnEnter(object sender, EventArgs e)
	{
		m_HenryOldDesk.OnEnter -= HandleHenryOldDeskOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip11, "DIACH1/DIA_CH1_HENRY_11"));
	}

	private void HandleHenryArtRoomOnEnter(object sender, EventArgs e)
	{
		m_HenryArtRoom.OnEnter -= HandleHenryArtRoomOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip12, "DIACH1/DIA_CH1_HENRY_12"));
	}

	private void HandleBorisRoomOnEnter(object sender, EventArgs e)
	{
		m_BorisRoom.OnEnter -= HandleBorisRoomOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip08, "DIACH1/DIA_CH1_HENRY_08"));
	}

	private void HandleProjectorOnInteract(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsProjectorOn)
		{
			TurnOff();
			m_IsProjectorOn = false;
			return;
		}
		m_Projector.FilmAudioObject = GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/SFX_Projector_Run_With_Film_01", m_Projector.AudioPosition.position, AudioObjectType.SOUND_EFFECT, -1);
		m_Projector.MusicAudioObject = GameManager.Instance.AudioManager.PlayAtPosition("Audio/MUS/CH1/MUS_Hellfire_Follies", m_Projector.MusicPosition.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, m_Projector.MusicPosition);
		TurnOn();
		m_IsProjectorOn = true;
	}

	public void TurnOn()
	{
		m_Projector.TurnOn();
	}

	public void TurnOff()
	{
		m_Projector.TurnOff();
	}

	public void ShutDown()
	{
		TurnOff();
		m_ProjectorInteract.OnInteracted -= HandleProjectorOnInteract;
		m_ProjectorInteract.SetActive(active: false);
		Dispose();
	}

	protected override void OnDisposed()
	{
		m_ProjectorInteract.OnInteracted -= HandleProjectorOnInteract;
		if ((Object)(object)m_BorisRoom != (Object)null)
		{
			m_BorisRoom.OnEnter -= HandleBorisRoomOnEnter;
			m_BorisRoom.Dispose();
		}
		if ((Object)(object)m_HenryOldDesk != (Object)null)
		{
			m_HenryOldDesk.OnEnter -= HandleHenryOldDeskOnEnter;
			m_HenryOldDesk.Dispose();
		}
		if ((Object)(object)m_HenryArtRoom != (Object)null)
		{
			m_HenryArtRoom.OnEnter -= HandleHenryArtRoomOnEnter;
			m_HenryArtRoom.Dispose();
		}
		m_HenryClip08 = null;
		m_HenryClip11 = null;
		m_HenryClip12 = null;
		base.OnDisposed();
	}
}
