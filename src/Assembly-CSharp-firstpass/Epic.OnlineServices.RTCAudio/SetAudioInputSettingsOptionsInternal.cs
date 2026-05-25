using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetAudioInputSettingsOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_DeviceId;

	private float m_Volume;

	private int m_PlatformAEC;

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

	public bool PlatformAEC
	{
		set
		{
			Helper.TryMarshalSet(ref m_PlatformAEC, value);
		}
	}

	public void Set(SetAudioInputSettingsOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			DeviceId = other.DeviceId;
			Volume = other.Volume;
			PlatformAEC = other.PlatformAEC;
		}
	}

	public void Set(object other)
	{
		Set(other as SetAudioInputSettingsOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_DeviceId);
	}
}
