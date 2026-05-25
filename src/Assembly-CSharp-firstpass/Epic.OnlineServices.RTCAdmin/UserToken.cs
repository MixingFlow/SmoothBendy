namespace Epic.OnlineServices.RTCAdmin;

public class UserToken : ISettable
{
	public ProductUserId ProductUserId { get; set; }

	public string Token { get; set; }

	internal void Set(UserTokenInternal? other)
	{
		if (other.HasValue)
		{
			ProductUserId = other.Value.ProductUserId;
			Token = other.Value.Token;
		}
	}

	public void Set(object other)
	{
		Set(other as UserTokenInternal?);
	}
}
