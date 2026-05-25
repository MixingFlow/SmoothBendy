namespace Epic.OnlineServices.CustomInvites;

public class SendCustomInviteOptions
{
	public ProductUserId LocalUserId { get; set; }

	public ProductUserId[] TargetUserIds { get; set; }
}
