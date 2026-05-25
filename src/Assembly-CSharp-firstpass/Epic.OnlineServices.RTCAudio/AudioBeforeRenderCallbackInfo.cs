namespace Epic.OnlineServices.RTCAudio;

public class AudioBeforeRenderCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public AudioBuffer Buffer { get; private set; }

	public ProductUserId ParticipantId { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(AudioBeforeRenderCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			Buffer = other.Value.Buffer;
			ParticipantId = other.Value.ParticipantId;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioBeforeRenderCallbackInfoInternal?);
	}
}
