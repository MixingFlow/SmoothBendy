namespace Epic.OnlineServices.Sanctions;

public class PlayerSanction : ISettable
{
	public long TimePlaced { get; set; }

	public string Action { get; set; }

	public long TimeExpires { get; set; }

	public string ReferenceId { get; set; }

	internal void Set(PlayerSanctionInternal? other)
	{
		if (other.HasValue)
		{
			TimePlaced = other.Value.TimePlaced;
			Action = other.Value.Action;
			TimeExpires = other.Value.TimeExpires;
			ReferenceId = other.Value.ReferenceId;
		}
	}

	public void Set(object other)
	{
		Set(other as PlayerSanctionInternal?);
	}
}
