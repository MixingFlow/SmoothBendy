using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AddNotifyAudioDevicesChangedOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	public void Set(AddNotifyAudioDevicesChangedOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
		}
	}

	public void Set(object other)
	{
		Set(other as AddNotifyAudioDevicesChangedOptions);
	}

	public void Dispose()
	{
	}
}
