namespace Epic.OnlineServices.RTCAudio;

public class AudioDevicesChangedCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(AudioDevicesChangedCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioDevicesChangedCallbackInfoInternal?);
	}
}
