using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AudioInputDeviceInfoInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private int m_DefaultDevice;

	private IntPtr m_DeviceId;

	private IntPtr m_DeviceName;

	public bool DefaultDevice
	{
		get
		{
			Helper.TryMarshalGet(m_DefaultDevice, out var target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_DefaultDevice, value);
		}
	}

	public string DeviceId
	{
		get
		{
			Helper.TryMarshalGet(m_DeviceId, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_DeviceId, value);
		}
	}

	public string DeviceName
	{
		get
		{
			Helper.TryMarshalGet(m_DeviceName, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_DeviceName, value);
		}
	}

	public void Set(AudioInputDeviceInfo other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			DefaultDevice = other.DefaultDevice;
			DeviceId = other.DeviceId;
			DeviceName = other.DeviceName;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioInputDeviceInfo);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_DeviceId);
		Helper.TryMarshalDispose(ref m_DeviceName);
	}
}
