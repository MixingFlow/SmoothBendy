using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AddNotifyAudioBeforeRenderOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private int m_UnmixedAudio;

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

	public bool UnmixedAudio
	{
		set
		{
			Helper.TryMarshalSet(ref m_UnmixedAudio, value);
		}
	}

	public void Set(AddNotifyAudioBeforeRenderOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			UnmixedAudio = other.UnmixedAudio;
		}
	}

	public void Set(object other)
	{
		Set(other as AddNotifyAudioBeforeRenderOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_RoomName);
	}
}
