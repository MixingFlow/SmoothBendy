namespace Epic.OnlineServices.CustomInvites;

public class FinalizeInviteOptions
{
	public ProductUserId TargetUserId { get; set; }

	public ProductUserId LocalUserId { get; set; }

	public string CustomInviteId { get; set; }

	public Result ProcessingResult { get; set; }
}
