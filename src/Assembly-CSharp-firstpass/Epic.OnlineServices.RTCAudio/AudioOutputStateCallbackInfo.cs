namespace Epic.OnlineServices.RTCAudio;

public class AudioOutputStateCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public RTCAudioOutputStatus Status { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(AudioOutputStateCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			Status = other.Value.Status;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioOutputStateCallbackInfoInternal?);
	}
}
