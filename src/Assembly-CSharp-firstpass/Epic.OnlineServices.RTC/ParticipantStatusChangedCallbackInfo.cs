namespace Epic.OnlineServices.RTC;

public class ParticipantStatusChangedCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public ProductUserId ParticipantId { get; private set; }

	public RTCParticipantStatus ParticipantStatus { get; private set; }

	public ParticipantMetadata[] ParticipantMetadata { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(ParticipantStatusChangedCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			ParticipantId = other.Value.ParticipantId;
			ParticipantStatus = other.Value.ParticipantStatus;
			ParticipantMetadata = other.Value.ParticipantMetadata;
		}
	}

	public void Set(object other)
	{
		Set(other as ParticipantStatusChangedCallbackInfoInternal?);
	}
}
