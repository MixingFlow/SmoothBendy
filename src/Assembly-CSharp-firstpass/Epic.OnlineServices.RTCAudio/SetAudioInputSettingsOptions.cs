namespace Epic.OnlineServices.RTCAudio;

public class SetAudioInputSettingsOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string DeviceId { get; set; }

	public float Volume { get; set; }

	public bool PlatformAEC { get; set; }
}
