using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct KickOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_RoomName;

	private IntPtr m_TargetUserId;

	public string RoomName
	{
		set
		{
			Helper.TryMarshalSet(ref m_RoomName, value);
		}
	}

	public ProductUserId TargetUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserId, value);
		}
	}

	public void Set(KickOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			RoomName = other.RoomName;
			TargetUserId = other.TargetUserId;
		}
	}

	public void Set(object other)
	{
		Set(other as KickOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_TargetUserId);
	}
}
