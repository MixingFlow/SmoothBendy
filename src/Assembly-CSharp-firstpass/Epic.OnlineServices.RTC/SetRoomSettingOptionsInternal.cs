using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetRoomSettingOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_LocalUserId;

	private IntPtr m_RoomName;

	private IntPtr m_SettingName;

	private IntPtr m_SettingValue;

	public ProductUserId LocalUserId
	{
		set
		{
			Helper.TryMarshalSet(ref m_LocalUserId, value);
		}
	}

	public string RoomName
	{
		set
		{
			Helper.TryMarshalSet(ref m_RoomName, value);
		}
	}

	public string SettingName
	{
		set
		{
			Helper.TryMarshalSet(ref m_SettingName, value);
		}
	}

	public string SettingValue
	{
		set
		{
			Helper.TryMarshalSet(ref m_SettingValue, value);
		}
	}

	public void Set(SetRoomSettingOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			LocalUserId = other.LocalUserId;
			RoomName = other.RoomName;
			SettingName = other.SettingName;
			SettingValue = other.SettingValue;
		}
	}

	public void Set(object other)
	{
		Set(other as SetRoomSettingOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_LocalUserId);
		Helper.TryMarshalDispose(ref m_RoomName);
		Helper.TryMarshalDispose(ref m_SettingName);
		Helper.TryMarshalDispose(ref m_SettingValue);
	}
}
