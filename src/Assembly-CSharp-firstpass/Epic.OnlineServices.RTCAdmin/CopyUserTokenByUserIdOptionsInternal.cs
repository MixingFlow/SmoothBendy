using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct CopyUserTokenByUserIdOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_TargetUserId;

	private uint m_QueryId;

	public ProductUserId TargetUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserId, value);
		}
	}

	public uint QueryId
	{
		set
		{
			m_QueryId = value;
		}
	}

	public void Set(CopyUserTokenByUserIdOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 2;
			TargetUserId = other.TargetUserId;
			QueryId = other.QueryId;
		}
	}

	public void Set(object other)
	{
		Set(other as CopyUserTokenByUserIdOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_TargetUserId);
	}
}
