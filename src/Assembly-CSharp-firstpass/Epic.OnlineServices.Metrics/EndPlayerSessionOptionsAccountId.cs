namespace Epic.OnlineServices.Metrics;

public class EndPlayerSessionOptionsAccountId : ISettable
{
	private MetricsAccountIdType m_AccountIdType;

	private EpicAccountId m_Epic;

	private string m_External;

	public MetricsAccountIdType AccountIdType
	{
		get
		{
			return m_AccountIdType;
		}
		private set
		{
			m_AccountIdType = value;
		}
	}

	public EpicAccountId Epic
	{
		get
		{
			Helper.TryMarshalGet(m_Epic, out var target, m_AccountIdType, MetricsAccountIdType.Epic);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_Epic, value, ref m_AccountIdType, MetricsAccountIdType.Epic);
		}
	}

	public string External
	{
		get
		{
			Helper.TryMarshalGet(m_External, out var target, m_AccountIdType, MetricsAccountIdType.External);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_External, value, ref m_AccountIdType, MetricsAccountIdType.External);
		}
	}

	public static implicit operator EndPlayerSessionOptionsAccountId(EpicAccountId value)
	{
		EndPlayerSessionOptionsAccountId endPlayerSessionOptionsAccountId = new EndPlayerSessionOptionsAccountId();
		endPlayerSessionOptionsAccountId.Epic = value;
		return endPlayerSessionOptionsAccountId;
	}

	public static implicit operator EndPlayerSessionOptionsAccountId(string value)
	{
		EndPlayerSessionOptionsAccountId endPlayerSessionOptionsAccountId = new EndPlayerSessionOptionsAccountId();
		endPlayerSessionOptionsAccountId.External = value;
		return endPlayerSessionOptionsAccountId;
	}

	internal void Set(EndPlayerSessionOptionsAccountIdInternal? other)
	{
		if (other.HasValue)
		{
			Epic = other.Value.Epic;
			External = other.Value.External;
		}
	}

	public void Set(object other)
	{
		Set(other as EndPlayerSessionOptionsAccountIdInternal?);
	}
}
