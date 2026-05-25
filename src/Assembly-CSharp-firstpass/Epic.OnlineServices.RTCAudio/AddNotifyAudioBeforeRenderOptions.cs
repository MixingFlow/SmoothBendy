namespace Epic.OnlineServices.RTCAudio;

public class AddNotifyAudioBeforeRenderOptions
{
	public ProductUserId LocalUserId { get; set; }

	public string RoomName { get; set; }

	public bool UnmixedAudio { get; set; }
}
