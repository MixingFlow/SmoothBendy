using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct QueryJoinRoomTokenOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_TargetUserIds;

	private uint m_TargetUserIdsCount;

	private IntPtr m_TargetUserIpAddresses;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string RoomName
	{
		set
		{
			Helper.TryMarshalSet(ref m_RoomName, value);
		}
	}

	public ProductUserId[] TargetUserIds
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserIds, value, out m_TargetUserIdsCount);
		}
	}

	public string TargetUserIpAddresses
	{
		set
		{
			Helper.TryMarshalSet(ref m_TargetUserIpAddresses, value);
		}
	}

	public void Set(QueryJoinRoomTokenOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 2;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			TargetUserIds = other.TargetUserIds;
			TargetUserIpAddresses = other.TargetUserIpAddresses;
		}
	}

	public void Set(object other)
	{
		Set(other as QueryJoinRoomTokenOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_TargetUserIds);
		Helper.TryMarshalDispose(ref m_TargetUserIpAddresses);
	}
}
