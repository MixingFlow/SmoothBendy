using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct RegisterPlayersCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_RegisteredPlayers;

	private uint m_RegisteredPlayersCount;

	private IntPtr m_SanctionedPlayers;

	private uint m_SanctionedPlayersCount;

	public Result ResultCode => m_ResultCode;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;

	public ProductUserId[] RegisteredPlayers
	{
		get
		{
			Helper.TryMarshalGetHandle<ProductUserId>(m_RegisteredPlayers, out var target, m_RegisteredPlayersCount);
			return target;
		}
	}

	public ProductUserId[] SanctionedPlayers
	{
		get
		{
			Helper.TryMarshalGetHandle<ProductUserId>(m_SanctionedPlayers, out var target, m_SanctionedPlayersCount);
			return target;
		}
	}
}
