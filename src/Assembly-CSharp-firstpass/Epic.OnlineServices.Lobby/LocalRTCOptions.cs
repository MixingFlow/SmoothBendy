namespace Epic.OnlineServices.Lobby;

public class LocalRTCOptions : ISettable
{
	public uint Flags { get; set; }

	public bool UseManualAudioInput { get; set; }

	public bool UseManualAudioOutput { get; set; }

	public bool LocalAudioDeviceInputStartsMuted { get; set; }

	internal void Set(LocalRTCOptionsInternal? other)
	{
		if (other.HasValue)
		{
			Flags = other.Value.Flags;
			UseManualAudioInput = other.Value.UseManualAudioInput;
			UseManualAudioOutput = other.Value.UseManualAudioOutput;
			LocalAudioDeviceInputStartsMuted = other.Value.LocalAudioDeviceInputStartsMuted;
		}
	}

	public void Set(object other)
	{
		Set(other as LocalRTCOptionsInternal?);
	}
}
