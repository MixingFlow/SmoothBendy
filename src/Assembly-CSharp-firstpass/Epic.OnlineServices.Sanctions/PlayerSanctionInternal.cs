using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sanctions;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct PlayerSanctionInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private long m_TimePlaced;

	private IntPtr m_Action;

	private long m_TimeExpires;

	private IntPtr m_ReferenceId;

	public long TimePlaced
	{
		get
		{
			return m_TimePlaced;
		}
		set
		{
			m_TimePlaced = value;
		}
	}

	public string Action
	{
		get
		{
			Helper.TryMarshalGet(m_Action, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_Action, value);
		}
	}

	public long TimeExpires
	{
		get
		{
			return m_TimeExpires;
		}
		set
		{
			m_TimeExpires = value;
		}
	}

	public string ReferenceId
	{
		get
		{
			Helper.TryMarshalGet(m_ReferenceId, out string target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_ReferenceId, value);
		}
	}

	public void Set(PlayerSanction other)
	{
		if (other != null)
		{
			m_ApiVersion = 2;
			TimePlaced = other.TimePlaced;
			Action = other.Action;
			TimeExpires = other.TimeExpires;
			ReferenceId = other.ReferenceId;
		}
	}

	public void Set(object other)
	{
		Set(other as PlayerSanction);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_Action);
		Helper.TryMarshalDispose(ref m_ReferenceId);
	}
}
