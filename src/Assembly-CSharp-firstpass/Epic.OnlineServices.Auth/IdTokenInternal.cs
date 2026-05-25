using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Auth;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct IdTokenInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_AccountId;

	private IntPtr m_JsonWebToken;

	public EpicAccountId AccountId
	{
		get
		{
			Helper.TryMarshalGet(m_AccountId, out EpicAccountId target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_AccountId, value);
		}
	}

	public string JsonWebToken
	{
		get
		{
			Helper.TryMarshalGet(m_JsonWebToken, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_JsonWebToken, value);
		}
	}

	public void Set(IdToken other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			AccountId = other.AccountId;
			JsonWebToken = other.JsonWebToken;
		}
	}

	public void Set(object other)
	{
		Set(other as IdToken);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_AccountId);
		Helper.TryMarshalDispose(ref m_JsonWebToken);
	}
}
