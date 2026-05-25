using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetAudioOutputSettingsOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_DeviceId;

	private float m_Volume;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string DeviceId
	{
		set
		{
			Helper.TryMarshalSet(ref m_DeviceId, value);
		}
	}

	public float Volume
	{
		set
		{
			m_Volume = value;
		}
	}

	public void Set(SetAudioOutputSettingsOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			DeviceId = other.DeviceId;
			Volume = other.Volume;
		}
	}

	public void Set(object other)
	{
		Set(other as SetAudioOutputSettingsOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_DeviceId);
	}
}
