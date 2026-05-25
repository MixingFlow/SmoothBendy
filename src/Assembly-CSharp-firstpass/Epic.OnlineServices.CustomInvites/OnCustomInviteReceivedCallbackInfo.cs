namespace Epic.OnlineServices.CustomInvites;

public class OnCustomInviteReceivedCallbackInfo : ICallbackInfo, ISettable
{
	public object ClientData { get; private set; }

	public ProductUserId TargetUserId { get; private set; }

	public ProductUserId LocalUserId { get; private set; }

	public string CustomInviteId { get; private set; }

	public string Payload { get; private set; }

	public Result? GetResultCode()
	{
		return null;
	}

	internal void Set(OnCustomInviteReceivedCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ClientData = other.Value.ClientData;
			TargetUserId = other.Value.TargetUserId;
			LocalUserId = other.Value.LocalUserId;
			CustomInviteId = other.Value.CustomInviteId;
			Payload = other.Value.Payload;
		}
	}

	public void Set(object other)
	{
		Set(other as OnCustomInviteReceivedCallbackInfoInternal?);
	}
}
