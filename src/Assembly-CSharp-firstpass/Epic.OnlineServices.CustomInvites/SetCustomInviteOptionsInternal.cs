using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.CustomInvites;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetCustomInviteOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_Payload;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string Payload
	{
		set
		{
			Helper.TryMarshalSet(ref m_Payload, value);
		}
	}

	public void Set(SetCustomInviteOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			Payload = other.Payload;
		}
	}

	public void Set(object other)
	{
		Set(other as SetCustomInviteOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_Payload);
	}
}
