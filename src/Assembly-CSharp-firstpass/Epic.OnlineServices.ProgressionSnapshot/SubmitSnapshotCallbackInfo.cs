namespace Epic.OnlineServices.ProgressionSnapshot;

public class SubmitSnapshotCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public uint SnapshotId { get; private set; }

	public object ClientData { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(SubmitSnapshotCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			SnapshotId = other.Value.SnapshotId;
			ClientData = other.Value.ClientData;
		}
	}

	public void Set(object other)
	{
		Set(other as SubmitSnapshotCallbackInfoInternal?);
	}
}
