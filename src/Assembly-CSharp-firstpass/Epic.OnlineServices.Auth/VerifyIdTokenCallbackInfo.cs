namespace Epic.OnlineServices.Auth;

public class VerifyIdTokenCallbackInfo : ICallbackInfo, ISettable
{
	public Result ResultCode { get; private set; }

	public object ClientData { get; private set; }

	public string ApplicationId { get; private set; }

	public string ClientId { get; private set; }

	public string ProductId { get; private set; }

	public string SandboxId { get; private set; }

	public string DeploymentId { get; private set; }

	public string DisplayName { get; private set; }

	public bool IsExternalAccountInfoPresent { get; private set; }

	public ExternalAccountType ExternalAccountIdType { get; private set; }

	public string ExternalAccountId { get; private set; }

	public string ExternalAccountDisplayName { get; private set; }

	public string Platform { get; private set; }

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
			ApplicationId = other.Value.ApplicationId;
			ClientId = other.Value.ClientId;
			ProductId = other.Value.ProductId;
			SandboxId = other.Value.SandboxId;
			DeploymentId = other.Value.DeploymentId;
			DisplayName = other.Value.DisplayName;
			IsExternalAccountInfoPresent = other.Value.IsExternalAccountInfoPresent;
			ExternalAccountIdType = other.Value.ExternalAccountIdType;
			ExternalAccountId = other.Value.ExternalAccountId;
			ExternalAccountDisplayName = other.Value.ExternalAccountDisplayName;
			Platform = other.Value.Platform;
		}
	}

	public void Set(object other)
	{
		Set(other as VerifyIdTokenCallbackInfoInternal?);
	}
}
