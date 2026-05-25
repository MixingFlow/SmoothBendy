namespace Epic.OnlineServices.Auth;

public class IdToken : ISettable
{
	public EpicAccountId AccountId { get; set; }

	public string JsonWebToken { get; set; }

	internal void Set(IdTokenInternal? other)
	{
		if (other.HasValue)
		{
			AccountId = other.Value.AccountId;
			JsonWebToken = other.Value.JsonWebToken;
		}
	}

	public void Set(object other)
	{
		Set(other as IdTokenInternal?);
	}
}
