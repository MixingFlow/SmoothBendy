using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetParticipantHardMuteOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_RoomName;

	private IntPtr m_TargetUserId;

	private int m_Mute;

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

	public bool Mute
	{
		set
		{
			Helper.TryMarshalSet(ref m_Mute, value);
		}
	}

	public void Set(SetParticipantHardMuteOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			RoomName = other.RoomName;
			TargetUserId = other.TargetUserId;
			Mute = other.Mute;
		}
	}

	public void Set(object other)
	{
		Set(other as SetParticipantHardMuteOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_TargetUserId);
	}
}
