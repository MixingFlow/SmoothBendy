using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct VerifyIdTokenOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_IdToken;

	public IdToken IdToken
	{
		set
		{
			Helper.TryMarshalSet<IdTokenInternal, IdToken>(ref m_IdToken, value);
		}
	}

	public void Set(VerifyIdTokenOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			IdToken = other.IdToken;
		}
	}

	public void Set(object other)
	{
		Set(other as VerifyIdTokenOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_IdToken);
	}
}
