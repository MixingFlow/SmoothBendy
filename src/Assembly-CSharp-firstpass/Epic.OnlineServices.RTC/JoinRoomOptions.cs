namespace Epic.OnlineServices.RTC;

public class JoinRoomOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public string ClientBaseUrl { get; set; }

	public string ParticipantToken { get; set; }

	public ProductUserId ParticipantId { get; set; }

	public JoinRoomFlags Flags { get; set; }

	public bool ManualAudioInputEnabled { get; set; }

	public bool ManualAudioOutputEnabled { get; set; }
}
