using UnityEngine;

namespace S13Audio;

public class EnvDetectPeak : IEnvelopeDetection
{
	private float[] _buffer;

	public float[] Buffer
	{
		get
		{
			return _buffer;
		}
		set
		{
			_buffer = value;
		}
	}

	public float this[int index] => Mathf.Abs(_buffer[index]);

	public void Reset()
	{
	}
}
