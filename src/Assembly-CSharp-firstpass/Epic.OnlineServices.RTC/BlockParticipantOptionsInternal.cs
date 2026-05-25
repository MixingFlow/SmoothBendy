using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct BlockParticipantOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_ParticipantId;

	private int m_Blocked;

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

	public ProductUserId ParticipantId
	{
		set
		{
			Helper.TryMarshalSet(ref m_ParticipantId, value);
		}
	}

	public bool Blocked
	{
		set
		{
			Helper.TryMarshalSet(ref m_Blocked, value);
		}
	}

	public void Set(BlockParticipantOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			ParticipantId = other.ParticipantId;
			Blocked = other.Blocked;
		}
	}

	public void Set(object other)
	{
		Set(other as BlockParticipantOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_ParticipantId);
	}
}
