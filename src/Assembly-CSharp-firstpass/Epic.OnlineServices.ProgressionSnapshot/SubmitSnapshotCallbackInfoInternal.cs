using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SubmitSnapshotCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private uint m_SnapshotId;

	private IntPtr m_ClientData;

	public Result ResultCode => m_ResultCode;

	public uint SnapshotId => m_SnapshotId;

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
