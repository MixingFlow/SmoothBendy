namespace Epic.OnlineServices.Connect;

public class IdToken : ISettable
{
	public ProductUserId ProductUserId { get; set; }

	public string JsonWebToken { get; set; }

	internal void Set(IdTokenInternal? other)
	{
		if (other.HasValue)
		{
			ProductUserId = other.Value.ProductUserId;
			JsonWebToken = other.Value.JsonWebToken;
		}
	}

	public void Set(object other)
	{
		Set(other as IdTokenInternal?);
	}
}
