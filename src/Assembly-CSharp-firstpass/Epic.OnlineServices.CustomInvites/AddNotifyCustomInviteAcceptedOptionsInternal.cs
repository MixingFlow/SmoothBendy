using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.CustomInvites;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AddNotifyCustomInviteAcceptedOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	public void Set(AddNotifyCustomInviteAcceptedOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
		}
	}

	public void Set(object other)
	{
		Set(other as AddNotifyCustomInviteAcceptedOptions);
	}

	public void Dispose()
	{
	}
}
