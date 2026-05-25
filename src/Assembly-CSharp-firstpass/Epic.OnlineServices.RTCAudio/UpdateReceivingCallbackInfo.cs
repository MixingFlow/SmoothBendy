namespace Epic.OnlineServices.RTCAudio;

public class UpdateReceivingCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public ProductUserId ParticipantId { get; private set; }

	public bool AudioEnabled { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(UpdateReceivingCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			ParticipantId = other.Value.ParticipantId;
			AudioEnabled = other.Value.AudioEnabled;
		}
	}

	public void Set(object other)
	{
		Set(other as UpdateReceivingCallbackInfoInternal?);
	}
}
