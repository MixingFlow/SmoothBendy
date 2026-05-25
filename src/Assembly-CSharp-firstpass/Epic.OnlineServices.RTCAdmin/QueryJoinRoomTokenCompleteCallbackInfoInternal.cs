using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct QueryJoinRoomTokenCompleteCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_RoomName;

	private IntPtr m_ClientBaseUrl;

	private uint m_QueryId;

	private uint m_TokenCount;

	public Result ResultCode => m_ResultCode;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public string RoomName
	{
		get
		{
			Helper.TryMarshalGet(m_RoomName, out string target);
			return target;
		}
	}

	public string ClientBaseUrl
	{
		get
		{
			Helper.TryMarshalGet(m_ClientBaseUrl, out string target);
			return target;
		}
	}

	public uint QueryId => m_QueryId;

	public uint TokenCount => m_TokenCount;
}
