namespace Epic.OnlineServices.Metrics;

public class BeginPlayerSessionOptionsAccountId : ISettable
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

	public static implicit operator BeginPlayerSessionOptionsAccountId(EpicAccountId value)
	{
		BeginPlayerSessionOptionsAccountId beginPlayerSessionOptionsAccountId = new BeginPlayerSessionOptionsAccountId();
		beginPlayerSessionOptionsAccountId.Epic = value;
		return beginPlayerSessionOptionsAccountId;
	}

	public static implicit operator BeginPlayerSessionOptionsAccountId(string value)
	{
		BeginPlayerSessionOptionsAccountId beginPlayerSessionOptionsAccountId = new BeginPlayerSessionOptionsAccountId();
		beginPlayerSessionOptionsAccountId.External = value;
		return beginPlayerSessionOptionsAccountId;
	}

	internal void Set(BeginPlayerSessionOptionsAccountIdInternal? other)
	{
		if (other.HasValue)
		{
			Epic = other.Value.Epic;
			External = other.Value.External;
		}
	}

	public void Set(object other)
	{
		Set(other as BeginPlayerSessionOptionsAccountIdInternal?);
	}
}
