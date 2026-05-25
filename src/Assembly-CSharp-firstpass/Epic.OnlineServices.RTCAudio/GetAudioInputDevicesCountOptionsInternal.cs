using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct GetAudioInputDevicesCountOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	public void Set(GetAudioInputDevicesCountOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
		}
	}

	public void Set(object other)
	{
		Set(other as GetAudioInputDevicesCountOptions);
	}

	public void Dispose()
	{
	}
}
