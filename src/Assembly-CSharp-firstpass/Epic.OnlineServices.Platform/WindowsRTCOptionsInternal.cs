using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct WindowsRTCOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_PlatformSpecificOptions;

	public WindowsRTCOptionsPlatformSpecificOptions PlatformSpecificOptions
	{
		get
		{
			Helper.TryMarshalGet<WindowsRTCOptionsPlatformSpecificOptionsInternal, WindowsRTCOptionsPlatformSpecificOptions>(m_PlatformSpecificOptions, out var target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet<WindowsRTCOptionsPlatformSpecificOptionsInternal, WindowsRTCOptionsPlatformSpecificOptions>(ref m_PlatformSpecificOptions, value);
		}
	}

	public void Set(WindowsRTCOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			PlatformSpecificOptions = other.PlatformSpecificOptions;
		}
	}

	public void Set(object other)
	{
		Set(other as WindowsRTCOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_PlatformSpecificOptions);
	}
}
