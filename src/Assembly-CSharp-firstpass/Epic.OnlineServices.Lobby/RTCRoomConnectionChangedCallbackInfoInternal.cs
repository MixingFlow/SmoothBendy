using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct RTCRoomConnectionChangedCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	private IntPtr m_LobbyId;

	private IntPtr m_LocalUserId;

	private int m_IsConnected;

	private Result m_DisconnectReason;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public string LobbyId
	{
		get
		{
			Helper.TryMarshalGet(m_LobbyId, out string target);
			return target;
		}
	}

	public ProductUserId LocalUserId
	{
		get
		{
			Helper.TryMarshalGet(m_LocalUserId, out ProductUserId target);
			return target;
		}
	}

	public bool IsConnected
	{
		get
		{
			Helper.TryMarshalGet(m_IsConnected, out var target);
			return target;
		}
	}

	public Result DisconnectReason => m_DisconnectReason;
}
