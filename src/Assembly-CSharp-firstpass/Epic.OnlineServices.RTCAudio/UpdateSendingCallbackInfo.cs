namespace Epic.OnlineServices.RTCAudio;

public class UpdateSendingCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string RoomName { get; private set; }

	public RTCAudioStatus AudioStatus { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(UpdateSendingCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			RoomName = other.Value.RoomName;
			AudioStatus = other.Value.AudioStatus;
		}
	}

	public void Set(object other)
	{
		Set(other as UpdateSendingCallbackInfoInternal?);
	}
}
