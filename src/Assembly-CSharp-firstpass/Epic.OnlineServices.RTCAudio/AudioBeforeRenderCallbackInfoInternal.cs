using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AudioBeforeRenderCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_Buffer;

	private IntPtr m_ParticipantId;

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

	public AudioBuffer Buffer
	{
		get
		{
			Helper.TryMarshalGet<AudioBufferInternal, AudioBuffer>(m_Buffer, out var target);
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
}
