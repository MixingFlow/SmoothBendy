using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct JoinRoomCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

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

	public ProductUserId LocalUserId
	{
		get
		{
			Helper.TryMarshalGet(m_LocalUserId, out ProductUserId target);
			return target;
		}
	}

	public string RoomName
	{
		get
		{
			Helper.TryMarshalGet(m_RoomName, out string target);
			return target;
		}
	}
}
