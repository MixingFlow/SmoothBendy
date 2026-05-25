namespace Epic.OnlineServices.RTCAudio;

public class AudioBeforeSendCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public AudioBuffer Buffer { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(AudioBeforeSendCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			Buffer = other.Value.Buffer;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioBeforeSendCallbackInfoInternal?);
	}
}
