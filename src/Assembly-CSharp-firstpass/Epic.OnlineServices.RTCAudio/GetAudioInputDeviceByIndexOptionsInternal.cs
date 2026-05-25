using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct GetAudioInputDeviceByIndexOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private uint m_DeviceInfoIndex;

	public uint DeviceInfoIndex
	{
		set
		{
			m_DeviceInfoIndex = value;
		}
	}

	public void Set(GetAudioInputDeviceByIndexOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			DeviceInfoIndex = other.DeviceInfoIndex;
		}
	}

	public void Set(object other)
	{
		Set(other as GetAudioInputDeviceByIndexOptions);
	}

	public void Dispose()
	{
	}
}
