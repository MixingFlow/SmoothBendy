using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.CustomInvites;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct FinalizeInviteOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_TargetUserId;

	private IntPtr m_LocalUserId;

	private IntPtr m_CustomInviteId;

	private Result m_ProcessingResult;

	public ProductUserId TargetUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserId, value);
		}
	}

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string CustomInviteId
	{
		set
		{
			Helper.TryMarshalSet(ref m_CustomInviteId, value);
		}
	}

	public Result ProcessingResult
	{
		set
		{
			m_ProcessingResult = value;
		}
	}

	public void Set(FinalizeInviteOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			TargetUserId = other.TargetUserId;
			LocalUserId = other.LocalUserId;
			CustomInviteId = other.CustomInviteId;
			ProcessingResult = other.ProcessingResult;
		}
	}

	public void Set(object other)
	{
		Set(other as FinalizeInviteOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_TargetUserId);
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_CustomInviteId);
	}
}
