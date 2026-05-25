namespace Epic.OnlineServices.Platform;

public class WindowsRTCOptionsPlatformSpecificOptions : ISettable
{
	public string XAudio29DllPath { get; set; }

	internal void Set(WindowsRTCOptionsPlatformSpecificOptionsInternal? other)
	{
		if (other.HasValue)
		{
			XAudio29DllPath = other.Value.XAudio29DllPath;
		}
	}

	public void Set(object other)
	{
		Set(other as WindowsRTCOptionsPlatformSpecificOptionsInternal?);
	}
}
