using System;
using UnityEngine;

public class CH1SammysRoomController : BaseController
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_MusicPosition;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_LightBar;

	[SerializeField]
	private GameObject m_InnerLights;

	[Header("Door")]
	[SerializeField]
	private BaseDoorController m_Door;

	[Header("< Event Triggers >")]
	[SerializeField]
	private EventTrigger m_EventTrigger;

	private AudioObject m_RoomMusicAudioObject;

	private AudioClip m_LightClip;

	public override void InitOnComplete()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_RoomMusicAudioObject = GameManager.Instance.AudioManager.PlayAtPosition("Audio/MUS/CH1/MUS_Ode_To_Bendy_Loop_Through_Door_01", m_MusicPosition.position, AudioObjectType.SOUND_EFFECT, -1);
		m_Door.Lock();
		m_EventTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		m_EventTrigger.SetActive(active: true);
		m_EventTrigger.OnEnter += HandleEventTriggerOnEnter;
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_LightClip);
		UnlockRoom();
	}

	public void UnlockRoom()
	{
		if ((Object)(object)m_RoomMusicAudioObject != (Object)null)
		{
			m_RoomMusicAudioObject.Clear();
			m_RoomMusicAudioObject = null;
		}
		m_LightBar.SetActive(false);
		m_InnerLights.SetActive(true);
		m_Door.Unlock();
		Dispose();
	}

	public void ForceOpen()
	{
		if ((Object)(object)m_RoomMusicAudioObject != (Object)null)
		{
			m_RoomMusicAudioObject.Clear();
			m_RoomMusicAudioObject = null;
		}
		m_LightBar.SetActive(false);
		m_InnerLights.SetActive(true);
		m_Door.ForceOpen(145f);
		m_Door.Lock();
		Dispose();
	}

	public void ForceClose()
	{
		if ((Object)(object)m_EventTrigger != (Object)null)
		{
			m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		}
		if ((Object)(object)m_RoomMusicAudioObject != (Object)null)
		{
			m_RoomMusicAudioObject.Clear();
			m_RoomMusicAudioObject = null;
		}
		m_LightBar.SetActive(false);
		m_Door.Lock();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_RoomMusicAudioObject = null;
		m_LightClip = null;
		m_EventTrigger.OnEnter -= HandleEventTriggerOnEnter;
		base.OnDisposed();
	}
}
