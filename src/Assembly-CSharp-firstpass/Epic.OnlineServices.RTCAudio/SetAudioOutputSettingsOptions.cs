namespace Epic.OnlineServices.RTCAudio;

public class SetAudioOutputSettingsOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string DeviceId { get; set; }

	public float Volume { get; set; }
}
