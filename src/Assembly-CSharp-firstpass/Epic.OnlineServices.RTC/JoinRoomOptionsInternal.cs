using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct JoinRoomOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_ClientBaseUrl;

	private IntPtr m_ParticipantToken;

	private IntPtr m_ParticipantId;

	private JoinRoomFlags m_Flags;

	private int m_ManualAudioInputEnabled;

	private int m_ManualAudioOutputEnabled;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string RoomName
	{
		set
		{
			Helper.TryMarshalSet(ref m_RoomName, value);
		}
	}

	public string ClientBaseUrl
	{
		set
		{
			Helper.TryMarshalSet(ref m_ClientBaseUrl, value);
		}
	}

	public string ParticipantToken
	{
		set
		{
			Helper.TryMarshalSet(ref m_ParticipantToken, value);
		}
	}

	public ProductUserId ParticipantId
	{
		set
		{
			Helper.TryMarshalSet(ref m_ParticipantId, value);
		}
	}

	public JoinRoomFlags Flags
	{
		set
		{
			m_Flags = value;
		}
	}

	public bool ManualAudioInputEnabled
	{
		set
		{
			Helper.TryMarshalSet(ref m_ManualAudioInputEnabled, value);
		}
	}

	public bool ManualAudioOutputEnabled
	{
		set
		{
			Helper.TryMarshalSet(ref m_ManualAudioOutputEnabled, value);
		}
	}

	public void Set(JoinRoomOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			ClientBaseUrl = other.ClientBaseUrl;
			ParticipantToken = other.ParticipantToken;
			ParticipantId = other.ParticipantId;
			Flags = other.Flags;
			ManualAudioInputEnabled = other.ManualAudioInputEnabled;
			ManualAudioOutputEnabled = other.ManualAudioOutputEnabled;
		}
	}

	public void Set(object other)
	{
		Set(other as JoinRoomOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_ClientBaseUrl);
		Helper.TryMarshalDispose(ref m_ParticipantToken);
		Helper.TryMarshalDispose(ref m_ParticipantId);
	}
}
