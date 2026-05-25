using System;

namespace Epic.OnlineServices.Platform;

public class RTCOptions : ISettable
{
	public IntPtr PlatformSpecificOptions { get; set; }

	internal void Set(RTCOptionsInternal? other)
	{
		if (other.HasValue)
		{
			PlatformSpecificOptions = other.Value.PlatformSpecificOptions;
		}
	}

	public void Set(object other)
	{
		Set(other as RTCOptionsInternal?);
	}
}
