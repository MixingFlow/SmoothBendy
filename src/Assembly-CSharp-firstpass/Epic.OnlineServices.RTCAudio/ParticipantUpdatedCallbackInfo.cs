namespace Epic.OnlineServices.RTCAudio;

public class ParticipantUpdatedCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public ProductUserId ParticipantId { get; private set; }

	public bool Speaking { get; private set; }

	public RTCAudioStatus AudioStatus { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(ParticipantUpdatedCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			ParticipantId = other.Value.ParticipantId;
			Speaking = other.Value.Speaking;
			AudioStatus = other.Value.AudioStatus;
		}
	}

	public void Set(object other)
	{
		Set(other as ParticipantUpdatedCallbackInfoInternal?);
	}
}
