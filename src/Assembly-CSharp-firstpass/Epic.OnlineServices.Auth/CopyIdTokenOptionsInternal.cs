using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct CopyIdTokenOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_AccountId;

	public EpicAccountId AccountId
	{
		set
		{
			Helper.TryMarshalSet(ref m_AccountId, value);
		}
	}

	public void Set(CopyIdTokenOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			AccountId = other.AccountId;
		}
	}

	public void Set(object other)
	{
		Set(other as CopyIdTokenOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_AccountId);
	}
}
