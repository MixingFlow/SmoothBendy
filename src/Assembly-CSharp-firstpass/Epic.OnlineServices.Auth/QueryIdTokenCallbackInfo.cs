namespace Epic.OnlineServices.Auth;

public class QueryIdTokenCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public EpicAccountId LocalUserId { get; private set; }

	public EpicAccountId TargetAccountId { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(QueryIdTokenCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			LocalUserId = other.Value.LocalUserId;
			TargetAccountId = other.Value.TargetAccountId;
		}
	}

	public void Set(object other)
	{
		Set(other as QueryIdTokenCallbackInfoInternal?);
	}
}
