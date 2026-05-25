namespace Epic.OnlineServices.Auth;

public class QueryIdTokenOptions
{
	public EpicAccountId LocalUserId { get; set; }

	public EpicAccountId TargetAccountId { get; set; }
}
