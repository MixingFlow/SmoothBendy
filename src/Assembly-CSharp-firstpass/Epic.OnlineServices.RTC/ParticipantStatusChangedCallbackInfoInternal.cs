using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ParticipantStatusChangedCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_ParticipantId;

	private RTCParticipantStatus m_ParticipantStatus;

	private uint m_ParticipantMetadataCount;

	private IntPtr m_ParticipantMetadata;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public ProductUserId LocalUserId
	{
		get
		{
			Helper.TryMarshalGet(m_LocalUserId, out ProductUserId target);
			return target;
		}
	}

	public string RoomName
	{
		get
		{
			Helper.TryMarshalGet(m_RoomName, out string target);
			return target;
		}
	}

	public ProductUserId ParticipantId
	{
		get
		{
			Helper.TryMarshalGet(m_ParticipantId, out ProductUserId target);
			return target;
		}
	}

	public RTCParticipantStatus ParticipantStatus => m_ParticipantStatus;

	public ParticipantMetadata[] ParticipantMetadata
	{
		get
		{
			Helper.TryMarshalGet<ParticipantMetadataInternal, ParticipantMetadata>(m_ParticipantMetadata, out var target, m_ParticipantMetadataCount);
			return target;
		}
	}
}
