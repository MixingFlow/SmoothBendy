using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct DeleteSnapshotCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_LocalUserId;

	private IntPtr m_ClientData;

	public Result ResultCode => m_ResultCode;

	public ProductUserId LocalUserId
	{
		get
		{
			Helper.TryMarshalGet(m_LocalUserId, out ProductUserId target);
			return target;
		}
	}

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;
}
