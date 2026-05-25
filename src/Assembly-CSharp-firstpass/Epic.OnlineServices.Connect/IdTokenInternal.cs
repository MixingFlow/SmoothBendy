using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Connect;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct IdTokenInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_ProductUserId;

	private IntPtr m_JsonWebToken;

	public ProductUserId ProductUserId
	{
		get
		{
			Helper.TryMarshalGet(m_ProductUserId, out ProductUserId target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_ProductUserId, value);
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
			ProductUserId = other.ProductUserId;
			JsonWebToken = other.JsonWebToken;
		}
	}

	public void Set(object other)
	{
		Set(other as IdToken);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_ProductUserId);
		Helper.TryMarshalDispose(ref m_JsonWebToken);
	}
}
