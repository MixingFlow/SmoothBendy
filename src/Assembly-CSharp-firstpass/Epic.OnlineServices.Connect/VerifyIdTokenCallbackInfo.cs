namespace Epic.OnlineServices.Connect;

public class VerifyIdTokenCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public ProductUserId ProductUserId { get; private set; }

	public bool IsAccountInfoPresent { get; private set; }

	public ExternalAccountType AccountIdType { get; private set; }

	public string AccountId { get; private set; }

	public string Platform { get; private set; }

	public string DeviceType { get; private set; }

	public string ClientId { get; private set; }

	public string ProductId { get; private set; }

	public string SandboxId { get; private set; }

	public string DeploymentId { get; private set; }

	public Result? GetResultCode()
	{
		return ResultCode;
	}

	internal void Set(VerifyIdTokenCallbackInfoInternal? other)
	{
		if (other.HasValue)
		{
			ResultCode = other.Value.ResultCode;
			ClientData = other.Value.ClientData;
			ProductUserId = other.Value.ProductUserId;
			IsAccountInfoPresent = other.Value.IsAccountInfoPresent;
			AccountIdType = other.Value.AccountIdType;
			AccountId = other.Value.AccountId;
			Platform = other.Value.Platform;
			DeviceType = other.Value.DeviceType;
			ClientId = other.Value.ClientId;
			ProductId = other.Value.ProductId;
			SandboxId = other.Value.SandboxId;
			DeploymentId = other.Value.DeploymentId;
		}
	}

	public void Set(object other)
	{
		Set(other as VerifyIdTokenCallbackInfoInternal?);
	}
}
