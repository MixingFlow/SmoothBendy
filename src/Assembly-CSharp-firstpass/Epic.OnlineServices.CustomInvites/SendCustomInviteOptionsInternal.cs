using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.CustomInvites;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SendCustomInviteOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_TargetUserIds;

	private uint m_TargetUserIdsCount;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public ProductUserId[] TargetUserIds
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserIds, value, out m_TargetUserIdsCount);
		}
	}

	public void Set(SendCustomInviteOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			TargetUserIds = other.TargetUserIds;
		}
	}

	public void Set(object other)
	{
		Set(other as SendCustomInviteOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_TargetUserIds);
	}
}
