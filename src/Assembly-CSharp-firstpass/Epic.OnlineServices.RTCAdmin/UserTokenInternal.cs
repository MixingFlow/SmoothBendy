using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAdmin;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct UserTokenInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_ProductUserId;

	private IntPtr m_Token;

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

	public string Token
	{
		get
		{
			Helper.TryMarshalGet(m_Token, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_Token, value);
		}
	}

	public void Set(UserToken other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			ProductUserId = other.ProductUserId;
			Token = other.Token;
		}
	}

	public void Set(object other)
	{
		Set(other as UserToken);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_ProductUserId);
		Helper.TryMarshalDispose(ref m_Token);
	}
}
