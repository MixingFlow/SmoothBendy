using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.ProgressionSnapshot;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void OnSubmitSnapshotCallbackInternal(IntPtr data);
