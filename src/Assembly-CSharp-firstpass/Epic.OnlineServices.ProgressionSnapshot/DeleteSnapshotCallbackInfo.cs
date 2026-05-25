namespace Epic.OnlineServices.ProgressionSnapshot;

public class DeleteSnapshotCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public object ClientData { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(DeleteSnapshotCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			LocalUserId = other.Value.LocalUserId;
			ClientData = other.Value.ClientData;
		}
	}

	public void Set(object other)
	{
		Set(other as DeleteSnapshotCallbackInfoInternal?);
	}
}
