using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct UnregisterPlatformAudioUserOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_UserId;

	public string UserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_UserId, value);
		}
	}

	public void Set(UnregisterPlatformAudioUserOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			UserId = other.UserId;
		}
	}

	public void Set(object other)
	{
		Set(other as UnregisterPlatformAudioUserOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_UserId);
	}
}
