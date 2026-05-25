namespace Epic.OnlineServices.RTCAdmin;

public class SetParticipantHardMuteOptions
{
	public string RoomName { get; set; }

	public ProductUserId TargetUserId { get; set; }

	public bool Mute { get; set; }
}
