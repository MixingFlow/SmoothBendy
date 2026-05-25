namespace Epic.OnlineServices.RTCAudio;

public class UpdateReceivingOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public ProductUserId ParticipantId { get; set; }

	public bool AudioEnabled { get; set; }
}
