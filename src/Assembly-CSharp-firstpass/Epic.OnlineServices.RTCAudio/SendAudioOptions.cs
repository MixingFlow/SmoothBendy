namespace Epic.OnlineServices.RTCAudio;

public class SendAudioOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public AudioBuffer Buffer { get; set; }
}
