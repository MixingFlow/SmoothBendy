using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct QueryIdTokenOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_TargetAccountId;

	public EpicAccountId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public EpicAccountId TargetAccountId
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetAccountId, value);
		}
	}

	public void Set(QueryIdTokenOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			TargetAccountId = other.TargetAccountId;
		}
	}

	public void Set(object other)
	{
		Set(other as QueryIdTokenOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_TargetAccountId);
	}
}
