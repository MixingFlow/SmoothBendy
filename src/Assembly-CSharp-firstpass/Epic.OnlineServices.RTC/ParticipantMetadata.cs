namespace Epic.OnlineServices.RTC;

public class ParticipantMetadata : ISettable
{
	public string Key { get; set; }

	public string Value { get; set; }

	internal void Set(ParticipantMetadataInternal? other)
	{
		if (other.HasValue)
		{
			Key = other.Value.Key;
			Value = other.Value.Value;
		}
	}

	public void Set(object other)
	{
		Set(other as ParticipantMetadataInternal?);
	}
}
