using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Platform;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct WindowsRTCOptionsPlatformSpecificOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_XAudio29DllPath;

	public string XAudio29DllPath
	{
		get
		{
			Helper.TryMarshalGet(m_XAudio29DllPath, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_XAudio29DllPath, value);
		}
	}

	public void Set(WindowsRTCOptionsPlatformSpecificOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			XAudio29DllPath = other.XAudio29DllPath;
		}
	}

	public void Set(object other)
	{
		Set(other as WindowsRTCOptionsPlatformSpecificOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_XAudio29DllPath);
	}
}
