using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct ParticipantUpdatedCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_ParticipantId;

	private int m_Speaking;

	private RTCAudioStatus m_AudioStatus;

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

	public bool Speaking
	{
		get
		{
			Helper.TryMarshalGet(m_Speaking, out var target);
			return target;
		}
	}

	public RTCAudioStatus AudioStatus => m_AudioStatus;
}
