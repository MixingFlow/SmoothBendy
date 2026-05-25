using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct RTCOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_PlatformSpecificOptions;

	public IntPtr PlatformSpecificOptions
	{
		get
		{
			return m_PlatformSpecificOptions;
		}
		set
		{
			m_PlatformSpecificOptions = value;
		}
	}

	public void Set(RTCOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			PlatformSpecificOptions = other.PlatformSpecificOptions;
		}
	}

	public void Set(object other)
	{
		Set(other as RTCOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_PlatformSpecificOptions);
	}
}
