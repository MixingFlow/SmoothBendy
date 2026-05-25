using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.RTCAudio;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct AudioBufferInternal : ISettable, IDisposable
{
	private int m_ApiVersion;

	private IntPtr m_Frames;

	private uint m_FramesCount;

	private uint m_SampleRate;

	private uint m_Channels;

	public short[] Frames
	{
		get
		{
			Helper.TryMarshalGet(m_Frames, out short[] target, m_FramesCount);
			return target;
		}
		set
		{
			Helper.TryMarshalSet(ref m_Frames, value, out m_FramesCount);
		}
	}

	public uint SampleRate
	{
		get
		{
			return m_SampleRate;
		}
		set
		{
			m_SampleRate = value;
		}
	}

	public uint Channels
	{
		get
		{
			return m_Channels;
		}
		set
		{
			m_Channels = value;
		}
	}

	public void Set(AudioBuffer other)
	{
		if (other != null)
		{
			m_ApiVersion = 1;
			Frames = other.Frames;
			SampleRate = other.SampleRate;
			Channels = other.Channels;
		}
	}

	public void Set(object other)
	{
		Set(other as AudioBuffer);
	}

	public void Dispose()
	{
		Helper.TryMarshalDispose(ref m_Frames);
	}
}
