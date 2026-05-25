using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.CustomInvites;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct OnCustomInviteReceivedCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	private IntPtr m_TargetUserId;

	private IntPtr m_LocalUserId;

	private IntPtr m_CustomInviteId;

	private IntPtr m_Payload;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public ProductUserId TargetUserId
	{
		get
		{
			Helper.TryMarshalGet(m_TargetUserId, out ProductUserId target);
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

	public string CustomInviteId
	{
		get
		{
			Helper.TryMarshalGet(m_CustomInviteId, out string target);
			return target;
		}
	}

	public string Payload
	{
		get
		{
			Helper.TryMarshalGet(m_Payload, out string target);
			return target;
		}
	}
}
