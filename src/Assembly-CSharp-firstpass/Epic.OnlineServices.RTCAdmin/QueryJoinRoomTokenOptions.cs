namespace Epic.OnlineServices.RTCAdmin;

public class QueryJoinRoomTokenOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public ProductUserId[] TargetUserIds { get; set; }

	public string TargetUserIpAddresses { get; set; }
}
