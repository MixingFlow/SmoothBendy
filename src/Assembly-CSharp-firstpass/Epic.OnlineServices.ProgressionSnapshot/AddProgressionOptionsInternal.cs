using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AddProgressionOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private uint m_SnapshotId;

	private IntPtr m_Key;

	private IntPtr m_Value;

	public uint SnapshotId
	{
		set
		{
			m_SnapshotId = value;
		}
	}

	public string Key
	{
		set
		{
			Helper.TryMarshalSet(ref m_Key, value);
		}
	}

	public string Value
	{
		set
		{
			Helper.TryMarshalSet(ref m_Value, value);
		}
	}

	public void Set(AddProgressionOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			SnapshotId = other.SnapshotId;
			Key = other.Key;
			Value = other.Value;
		}
	}

	public void Set(object other)
	{
		Set(other as AddProgressionOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_Key);
		Helper.TryMarshalDispose(ref m_Value);
	}
}
