using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTC;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SetSettingOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_SettingName;

	private IntPtr m_SettingValue;

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

	public void Set(SetSettingOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			SettingName = other.SettingName;
			SettingValue = other.SettingValue;
		}
	}

	public void Set(object other)
	{
		Set(other as SetSettingOptions);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_SettingName);
		Helper.TryMarshalDispose(ref m_SettingValue);
	}
}
