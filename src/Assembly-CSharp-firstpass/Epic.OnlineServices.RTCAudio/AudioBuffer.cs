namespace Epic.OnlineServices.RTCAudio;

public class AudioBuffer : ISettable
{
	public short[] Frames { get; set; }

	public uint SampleRate { get; set; }

	public uint Channels { get; set; }

	internal void Set(AudioBufferInternal? other)
	{
		if (other.HasValue)
		{
			Frames = other.Value.Frames;
			SampleRate = other.Value.SampleRate;
			Channels = other.Value.Channels;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioBufferInternal?);
	}
}
