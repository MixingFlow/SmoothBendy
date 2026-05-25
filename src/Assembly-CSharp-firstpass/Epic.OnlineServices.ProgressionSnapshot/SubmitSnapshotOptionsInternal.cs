using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SubmitSnapshotOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private uint m_SnapshotId;

	public uint SnapshotId
	{
		set
		{
			m_SnapshotId = value;
		}
	}

	public void Set(SubmitSnapshotOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			SnapshotId = other.SnapshotId;
		}
	}

	public void Set(object other)
	{
		Set(other as SubmitSnapshotOptions);
	}

	public void Dispose()
	{
	}
}
