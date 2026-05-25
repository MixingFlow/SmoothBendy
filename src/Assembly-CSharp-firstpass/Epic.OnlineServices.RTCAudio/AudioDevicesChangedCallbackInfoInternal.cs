using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AudioDevicesChangedCallbackInfoInternal : ICallbackInfoInternal
{
	private IntPtr m_ClientData;

	public object ClientData
	{
		get
		{
			Helper.TryMarshalGet(m_ClientData, out object target);
			return target;
		}
	}

	public IntPtr ClientDataAddress => m_ClientData;
}
