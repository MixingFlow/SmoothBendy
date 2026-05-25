using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct UnregisterPlayersCallbackInfoInternal : ICallbackInfoInternal
{
	private Result m_ResultCode;

	private IntPtr m_ClientData;

	private IntPtr m_UnregisteredPlayers;

	private uint m_UnregisteredPlayersCount;

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

	public ProductUserId[] UnregisteredPlayers
	{
		get
		{
			Helper.TryMarshalGetHandle<ProductUserId>(m_UnregisteredPlayers, out var target, m_UnregisteredPlayersCount);
			return target;
		}
	}
}
