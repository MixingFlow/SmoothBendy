namespace S13Audio;

public class S13RingBuffer<T> where T : struct
{
	private T[] _data;

	private int _length;

	private int _maxLength;

	private int _writeCursor;

	private int _readCursor;

	public int Length
	{
		get
		{
			return _length;
		}
		set
		{
			if (value < 0)
			{
				_length = 0;
			}
			else if (value <= MaxLength)
			{
				_length = value;
			}
			else
			{
				_length = MaxLength;
			}
		}
	}

	public int MaxLength => _maxLength;

	public S13RingBuffer(int length)
	{
		_length = length;
		_maxLength = length;
		_data = new T[_maxLength];
		_writeCursor = (_readCursor = 0);
	}

	public void Write(T data)
	{
		_data[_writeCursor] = data;
		_writeCursor = ((++_writeCursor < _length) ? _writeCursor : 0);
	}

	public T Read()
	{
		T result = _data[_readCursor];
		_readCursor = ((++_readCursor < _length) ? _readCursor : 0);
		return result;
	}

	public void Clear(T clearVal)
	{
		for (int i = 0; i < _length; i++)
		{
			_data[i] = clearVal;
		}
	}
}
