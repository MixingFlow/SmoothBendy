namespace Epic.OnlineServices.Platform;

public class WindowsRTCOptions : ISettable
{
	public WindowsRTCOptionsPlatformSpecificOptions PlatformSpecificOptions { get; set; }

	internal void Set(WindowsRTCOptionsInternal? other)
	{
		if (other.HasValue)
		{
			PlatformSpecificOptions = other.Value.PlatformSpecificOptions;
		}
	}

	public void Set(object other)
	{
		Set(other as WindowsRTCOptionsInternal?);
	}
}
