using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct LocalRTCOptionsInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private uint m_Flags;

	private int m_UseManualAudioInput;

	private int m_UseManualAudioOutput;

	private int m_LocalAudioDeviceInputStartsMuted;

	public uint Flags
	{
		get
		{
			return m_Flags;
		}
		set
		{
			m_Flags = value;
		}
	}

	public bool UseManualAudioInput
	{
		get
		{
			Helper.TryMarshalGet(m_UseManualAudioInput, out var target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_UseManualAudioInput, value);
		}
	}

	public bool UseManualAudioOutput
	{
		get
		{
			Helper.TryMarshalGet(m_UseManualAudioOutput, out var target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_UseManualAudioOutput, value);
		}
	}

	public bool LocalAudioDeviceInputStartsMuted
	{
		get
		{
			Helper.TryMarshalGet(m_LocalAudioDeviceInputStartsMuted, out var target);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_LocalAudioDeviceInputStartsMuted, value);
		}
	}

	public void Set(LocalRTCOptions other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			Flags = other.Flags;
			UseManualAudioInput = other.UseManualAudioInput;
			UseManualAudioOutput = other.UseManualAudioOutput;
			LocalAudioDeviceInputStartsMuted = other.LocalAudioDeviceInputStartsMuted;
		}
	}

	public void Set(object other)
	{
		Set(other as LocalRTCOptions);
	}

	public void Dispose()
	{
	}
}
