namespace Epic.OnlineServices.RTCAudio;

public class AudioOutputDeviceInfo : ISettable
{
	public bool DefaultDevice { get; set; }

	public string DeviceId { get; set; }

	public string DeviceName { get; set; }

	internal void Set(AudioOutputDeviceInfoInternal? other)
	{
		if (other.HasValue)
		{
			DefaultDevice = other.Value.DefaultDevice;
			DeviceId = other.Value.DeviceId;
			DeviceName = other.Value.DeviceName;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioOutputDeviceInfoInternal?);
	}
}
