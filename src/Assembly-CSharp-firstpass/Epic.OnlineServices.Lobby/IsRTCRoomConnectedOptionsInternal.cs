using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct IsRTCRoomConnectedOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LobbyId;

	private IntPtr m_LocalUserId;

	public string LobbyId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LobbyId, value);
		}
	}

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public void Set(IsRTCRoomConnectedOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LobbyId = other.LobbyId;
			LocalUserId = other.LocalUserId;
		}
	}

	public void Set(object other)
	{
		Set(other as IsRTCRoomConnectedOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LobbyId);
		Helper.TryMarshalDispose(ref m_LocalUserId);
	}
}
