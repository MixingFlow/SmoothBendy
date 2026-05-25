namespace Epic.OnlineServices.RTC;

public class BlockParticipantOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public ProductUserId ParticipantId { get; set; }

	public bool Blocked { get; set; }
}
