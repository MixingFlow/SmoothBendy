using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void OnDisconnectedCallbackInternal(IntPtr data);
