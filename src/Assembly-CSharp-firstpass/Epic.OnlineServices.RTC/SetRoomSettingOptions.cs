namespace Epic.OnlineServices.RTC;

public class SetRoomSettingOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public string SettingName { get; set; }

	public string SettingValue { get; set; }
}
