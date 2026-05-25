namespace Epic.OnlineServices.CustomInvites;

public class SendCustomInviteCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public ProductUserId[] TargetUserIds { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(SendCustomInviteCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			TargetUserIds = other.Value.TargetUserIds;
		}
	}

	public void Set(object other)
	{
		Set(other as SendCustomInviteCallbackInfoInternal?);
	}
}
