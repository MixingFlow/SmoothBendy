namespace Epic.OnlineServices.RTCAudio;

public class UpdateSendingOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public RTCAudioStatus AudioStatus { get; set; }
}
