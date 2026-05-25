using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct BeginSnapshotOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public void Set(BeginSnapshotOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
		}
	}

	public void Set(object other)
	{
		Set(other as BeginSnapshotOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
	}
}
