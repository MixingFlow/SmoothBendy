using UnityEngine;

namespace S13Audio;

public class EnvDetectRms : IEnvelopeDetection
{
	private int _iter;

	private float _lastTotal;

	private float[] _buffer;

	private S13RingBuffer<float> _rmsWindow;

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

	public float this[int index]
	{
		get
		{
			float num = _buffer[index] * _buffer[index];
			float num2 = 0f;
			float result;
			if (_iter < _rmsWindow.Length - 1)
			{
				num2 = _lastTotal + num;
				result = Mathf.Sqrt(1f / (float)(index + 1) * num2);
			}
			else
			{
				num2 = _lastTotal + num - _rmsWindow.Read();
				result = Mathf.Sqrt(1f / (float)_rmsWindow.Length * num2);
			}
			_rmsWindow.Write(num);
			_lastTotal = num2;
			_iter++;
			return result;
		}
	}

	public EnvDetectRms()
	{
		_iter = 0;
		_lastTotal = 0f;
		_rmsWindow = new S13RingBuffer<float>(128);
	}

	public void Reset()
	{
		_iter = 0;
		_lastTotal = 0f;
		_rmsWindow.Clear(0f);
	}
}
